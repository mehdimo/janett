namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class ReferenceTransformer : Transformer
	{
		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			string identifier = ContainsIdentifier(identifierExpression);

			if (identifier != null)
			{
				TypeReferenceExpression replacedIdentifier = new TypeReferenceExpression(identifier);
				replacedIdentifier.Parent = identifierExpression.Parent;

				ReplaceCurrentNode(replacedIdentifier);
			}
			else
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(identifierExpression, typeof(TypeDeclaration));
				if (typeDeclaration != null)
					CheckThroughParents(typeDeclaration, identifierExpression);
			}
			return null;
		}

		private void CheckThroughParents(TypeDeclaration typeDeclaration, IdentifierExpression identifierExpression)
		{
			if (typeDeclaration.Parent is TypeDeclaration)
			{
				TypeDeclaration parent = (TypeDeclaration) typeDeclaration.Parent;
				CheckThroughParents(parent, identifierExpression);
			}
			if (typeDeclaration.BaseTypes.Count > 0)
			{
				foreach (TypeReference baseType in typeDeclaration.BaseTypes)
				{
					string fullName = GetFullName(baseType);
					if (CodeBase.References.Contains(fullName))
					{
						string referedBaseType = (string) CodeBase.References[fullName];
						TypeReference newBaseType = AstUtil.GetTypeReference(referedBaseType, baseType.Parent);
						string referenceBaseType = GetFullName(newBaseType);
						TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[referenceBaseType];
						if (baseTypeDeclaration != null)
						{
							if (DefinedInFieldsClass(baseTypeDeclaration, identifierExpression.Identifier))
							{
								TypeReferenceExpression id = new TypeReferenceExpression(referedBaseType);
								FieldReferenceExpression replaced = new FieldReferenceExpression(id, identifierExpression.Identifier);
								replaced.Parent = identifierExpression.Parent;
								ReplaceCurrentNode(replaced);
							}
							else
							{
								TypeDeclaration type = (TypeDeclaration) CodeBase.Types[fullName];
								CheckThroughParents(type, identifierExpression);
							}
						}
					}
					else if (CodeBase.Types.Contains(fullName))
					{
						TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[fullName];
						CheckThroughParents(baseTypeDeclaration, identifierExpression);
					}
				}
			}
		}

		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			Expression targetObject = invocationExpression.TargetObject;
			string methodName = null;
			if (targetObject is IdentifierExpression)
				methodName = ((IdentifierExpression) targetObject).Identifier;
			else if (targetObject is FieldReferenceExpression)
				methodName = ((FieldReferenceExpression) targetObject).FieldName;

			if (methodName.StartsWith("set") || methodName.StartsWith("get"))
			{
				if (targetObject is IdentifierExpression)
				{
					TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(invocationExpression, typeof(TypeDeclaration));
					string key;

					if (ContainsReferences(typeDeclaration, methodName, out key))
					{
						IdentifierExpression identifierExpression = new IdentifierExpression((string) CodeBase.References[key]);
						if (methodName.StartsWith("get"))
						{
							identifierExpression.Parent = invocationExpression.Parent;
							ReplaceCurrentNode(identifierExpression);
						}
						else if (methodName.StartsWith("set"))
						{
							Expression setValue = (Expression) invocationExpression.Arguments[0];
							AssignmentExpression assignment = new AssignmentExpression(identifierExpression, AssignmentOperatorType.Assign, setValue);
							assignment.Parent = invocationExpression.Parent;
							ReplaceCurrentNode(assignment);
							assignment.AcceptVisitor(this, data);
						}
					}
				}
				else if (targetObject is FieldReferenceExpression)
				{
					Expression target = ((FieldReferenceExpression) targetObject).TargetObject;
					TypeReference invokerType = GetExpressionType(target);
					if (invokerType != null)
					{
						string fullName = GetFullName(invokerType);

						string key = fullName + "." + methodName;
						if (CodeBase.References.Contains(key))
						{
							string property = (string) CodeBase.References[key];
							FieldReferenceExpression replaced = new FieldReferenceExpression(target, property);
							if (methodName.StartsWith("get"))
							{
								replaced.Parent = invocationExpression.Parent;
								ReplaceCurrentNode(replaced);
								replaced.AcceptVisitor(this, data);
							}
							else
							{
								Expression setValue = (Expression) invocationExpression.Arguments[0];
								AssignmentExpression assignment = new AssignmentExpression(replaced, AssignmentOperatorType.Assign, setValue);
								assignment.Parent = invocationExpression.Parent;
								ReplaceCurrentNode(assignment);
								assignment.AcceptVisitor(this, data);
							}
						}
					}
				}
			}
			return base.TrackedVisitInvocationExpression(invocationExpression, data);
		}

		public override object TrackedVisitTypeReference(TypeReference typeReference, object data)
		{
			if (CodeBase.References.Contains(typeReference.Type))
			{
				string reference = (string) CodeBase.References[typeReference.Type];
				if (!reference.EndsWith("_Fields"))
					typeReference.Type = reference;
			}
			return base.TrackedVisitTypeReference(typeReference, data);
		}

		public override object TrackedVisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			string fullName = GetFullName(objectCreateExpression.CreateType);
			if (CodeBase.References.Contains("Cons:" + fullName))
			{
				objectCreateExpression.Parameters.Add(new ThisReferenceExpression());
			}
			return base.TrackedVisitObjectCreateExpression(objectCreateExpression, data);
		}

		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			IList usings = AstUtil.GetChildrenWithType(namespaceDeclaration, typeof(UsingDeclaration));
			foreach (UsingDeclaration usi in usings)
			{
				AddUsing(namespaceDeclaration, usings, usi);
			}
			return base.TrackedVisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		public void AddUsing(NamespaceDeclaration namespaceDeclaration, IList usings, UsingDeclaration usingDeclaration)
		{
			Using usin = (Using) usingDeclaration.Usings[0];
			string alias = null;
			string name = usin.Name;

			if (!usin.IsAlias)
				name = name.Substring(name.LastIndexOf('.') + 1);
			else
				alias = usin.Alias.Type;

			if (CodeBase.References.Contains(name))
			{
				string reference = (string) CodeBase.References[name];
				alias = alias.Replace("." + name, "." + reference);
				TypeReference typeReference = AstUtil.GetTypeReference(alias, namespaceDeclaration);
				UsingDeclaration addingUsingDeclaration = new UsingDeclaration(reference, typeReference);
				if (!Contains(usings, addingUsingDeclaration))
				{
					namespaceDeclaration.Children.Insert(0, addingUsingDeclaration);
				}
			}
		}

		private bool Contains(IList list, UsingDeclaration us)
		{
			foreach (UsingDeclaration usingDeclaration in list)
			{
				Using usi = (Using) usingDeclaration.Usings[0];
				Using uss = (Using) us.Usings[0];
				if (usi.Name == uss.Name)
					return true;
			}
			return false;
		}

		private bool DefinedInFieldsClass(TypeDeclaration tyedeDeclaration, string identifier)
		{
			IList fields = AstUtil.GetChildrenWithType(tyedeDeclaration, typeof(FieldDeclaration));
			foreach (FieldDeclaration field in fields)
			{
				if (identifier == ((VariableDeclaration) field.Fields[0]).Name)
					return true;
			}
			return false;
		}

		private bool ContainsReferences(TypeDeclaration typeDeclaration, string methodName, out string res)
		{
			bool flag = false;
			string fullName = GetFullName(typeDeclaration);
			string key = fullName + "." + methodName;
			res = key;
			if (CodeBase.References.Contains(key))
				return true;
			foreach (TypeReference baseType in typeDeclaration.BaseTypes)
			{
				if (!flag)
				{
					fullName = GetFullName(baseType);
					key = fullName + "." + methodName;
					if (CodeBase.References.Contains(key))
					{
						res = key;
						return true;
					}
					else if (CodeBase.Types.Contains(key))
					{
						TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[key];
						flag = ContainsReferences(baseTypeDeclaration, methodName, out res);
						if (flag)
							break;
					}
				}
			}
			return flag;
		}

		private string ContainsIdentifier(IdentifierExpression identifier)
		{
			NamespaceDeclaration ns = (NamespaceDeclaration) AstUtil.GetParentOfType(identifier, typeof(NamespaceDeclaration));
			string key = ns.Name + "." + identifier.Identifier;
			INode idParent = identifier.Parent;

			if (CodeBase.References.Contains(key))
			{
				if (identifier.Parent is FieldReferenceExpression)
				{
					FieldReferenceExpression fieldReference = (FieldReferenceExpression) identifier.Parent;
					string st = GetInterfaceFieldsClass(key, fieldReference.FieldName);
					if (st != null)
						return st;
				}
			}
			else if (CodeBase.Types.Contains(key))
			{
				return GetProperIdentifier(idParent, key);
			}
			else
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(identifier, typeof(TypeDeclaration));
				key = ns.Name + "." + typeDeclaration.Name;
				if (key.EndsWith("_Fields"))
					key = key.Replace("_Fields", "");
				if (CodeBase.References.Contains(key))
				{
					TypeDeclaration baseInterface = (TypeDeclaration) CodeBase.Types[key];
					if (baseInterface.BaseTypes.Count > 0)
					{
						foreach (TypeReference baseType in baseInterface.BaseTypes)
						{
							string fullName = GetFullName(baseType);
							string interfaceFieldsClass = GetInterfaceFieldsClass(fullName, identifier.Identifier);
							if (interfaceFieldsClass != null)
								return interfaceFieldsClass + "." + identifier.Identifier;
						}
					}
				}

				IList usings = AstUtil.GetChildrenWithType(ns, typeof(UsingDeclaration));
				foreach (UsingDeclaration usingDeclaration in usings)
				{
					Using usi = (Using) usingDeclaration.Usings[0];
					key = usi.Name + "." + identifier.Identifier;
					if (usi.IsAlias && usi.Alias.Type.EndsWith("." + identifier.Identifier))
						key = usi.Alias.Type;
					if (CodeBase.References.Contains(key))
						return (string) CodeBase.References[key];
					else if (CodeBase.Types.Contains(key))
					{
						return GetProperIdentifier(idParent, key);
					}
				}
			}
			return null;
		}

		private string GetProperIdentifier(INode idParent, string key)
		{
			IList values = SearchForParents(key);
			if (values.Count > 0 && idParent is FieldReferenceExpression)
			{
				FieldReferenceExpression fieldReference = (FieldReferenceExpression) idParent;
				foreach (string value in values)
				{
					string interfaceFieldsClass = GetInterfaceFieldsClass(value, fieldReference.FieldName);
					if (interfaceFieldsClass != null)
						return interfaceFieldsClass;
				}
			}
			return null;
		}

		private string GetInterfaceFieldsClass(string interfaceName, string interfaceField)
		{
			if (CodeBase.References.Contains(interfaceName))
			{
				string fieldClass = (string) CodeBase.References[interfaceName];
				TypeDeclaration type = (TypeDeclaration) CodeBase.Types[fieldClass];
				IList members = GetFieldsName(type);
				if (members.Contains(interfaceField))
					return fieldClass;
			}
			return null;
		}

		private IList SearchForParents(string typeName)
		{
			IList result = new ArrayList();
			if (CodeBase.Types.Contains(typeName))
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) CodeBase.Types[typeName];
				if (typeDeclaration.BaseTypes.Count > 0)
				{
					foreach (TypeReference baseType in typeDeclaration.BaseTypes)
					{
						string fullName = GetFullName(baseType);
						if (CodeBase.References.Contains(fullName))
							result.Add(fullName);
						else
							SearchForParents(fullName);
					}
				}
			}
			return result;
		}

		private IList GetFieldsName(TypeDeclaration typeDeclaration)
		{
			IList fields = new ArrayList();

			ArrayList fieldDeclarations = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
			foreach (FieldDeclaration fieldDeclaration in fieldDeclarations)
				fields.Add(((VariableDeclaration) fieldDeclaration.Fields[0]).Name);
			return fields;
		}
	}
}