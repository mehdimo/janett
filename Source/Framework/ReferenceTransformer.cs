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
				if (typeDeclaration != null && typeDeclaration.BaseTypes.Count > 0)
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
							if (baseTypeDeclaration != null && DefinedInFieldsClass(baseTypeDeclaration, identifierExpression.Identifier))
							{
								TypeReferenceExpression id = new TypeReferenceExpression(referedBaseType);
								FieldReferenceExpression replaced = new FieldReferenceExpression(id, identifierExpression.Identifier);
								replaced.Parent = identifierExpression.Parent;
								ReplaceCurrentNode(replaced);
							}
						}
					}
				}
			}
			return null;
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
							}
							else
							{
								Expression setValue = (Expression) invocationExpression.Arguments[0];
								AssignmentExpression assignment = new AssignmentExpression(replaced, AssignmentOperatorType.Assign, setValue);
								assignment.Parent = invocationExpression.Parent;
								ReplaceCurrentNode(assignment);
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
			if (CodeBase.References.Contains(key))
				return (string) CodeBase.References[key];
			else
			{
				IList usings = AstUtil.GetChildrenWithType(ns, typeof(UsingDeclaration));
				foreach (UsingDeclaration usingDeclaration in usings)
				{
					Using usi = (Using) usingDeclaration.Usings[0];
					key = usi.Name + "." + identifier.Identifier;
					if (usi.IsAlias && usi.Alias.Type.EndsWith("." + identifier.Identifier))
						key = usi.Alias.Type;
					if (CodeBase.References.Contains(key))
						return (string) CodeBase.References[key];
				}
			}
			return null;
		}
	}
}