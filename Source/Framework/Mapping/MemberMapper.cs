namespace Janett.Framework
{
	using System.Collections;
	using System.IO;

	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;

	public class MemberMapper : Transformer
	{
		public override object TrackedVisitExpressionStatement(ExpressionStatement ExpressionStatement, object data)
		{
			IList removeStatement = new ArrayList();
			base.TrackedVisitExpressionStatement(ExpressionStatement, removeStatement);
			if (removeStatement.Count > 0)
				RemoveCurrentNode();
			return null;
		}

		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			Expression invocationTarget = invocationExpression.TargetObject;

			if (invocationTarget is FieldReferenceExpression)
			{
				FieldReferenceExpression methodTargetObject = (FieldReferenceExpression) invocationTarget;

				Expression invoker = methodTargetObject.TargetObject;
				TypeReference invokerType = GetExpressionType(invoker);

				if (invokerType != null)
				{
					ReplaceMember(invocationExpression, data, invokerType);
				}
			}
			else if (invocationTarget is IdentifierExpression)
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(invocationExpression, typeof(TypeDeclaration));
				VerifyDerivedMethod(typeDeclaration, invocationExpression, data);
			}

			if (invocationExpression.TypeArguments.Count == 0)
				return base.TrackedVisitInvocationExpression(invocationExpression, data);
			else
				return null;
		}

		public override object TrackedVisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			string objectType = GetFullName(objectCreateExpression.CreateType);

			TypeMapping mapping = CodeBase.Mappings[objectType];
			if (mapping != null)
			{
				string key;
				if (ContainsMapping(mapping, objectCreateExpression, out key))
				{
					string cSharpMethodMap = mapping.Members[key];
					Expression replacedExpression = GetReplacedExpression(objectCreateExpression, cSharpMethodMap);
					ReplaceCurrentNode(replacedExpression);
					replacedExpression.AcceptVisitor(this, data);
				}
			}
			return base.TrackedVisitObjectCreateExpression(objectCreateExpression, data);
		}

		public override object TrackedVisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			Expression invoker = fieldReferenceExpression.TargetObject;
			TypeReference invokerType = GetExpressionType(invoker);

			if (invokerType != null)
			{
				string returnType = GetFullName(invokerType);
				if (invokerType.RankSpecifier != null && invokerType.RankSpecifier.Length > 0)
				{
					returnType = "Array";
				}

				TypeMapping mapping = CodeBase.Mappings[returnType];
				string key;
				if (ContainsMapping(mapping, fieldReferenceExpression, out key))
				{
					string cSharpMethodMap = mapping.Members[key];
					Expression replacedExpression = GetReplacedExpression(fieldReferenceExpression, cSharpMethodMap);
					ReplaceCurrentNode(replacedExpression);
					replacedExpression.AcceptVisitor(this, data);
				}
			}
			return base.TrackedVisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		private void VerifyDerivedMethod(TypeDeclaration typeDeclaration, InvocationExpression invocationExpression, object data)
		{
			if (typeDeclaration.BaseTypes.Count > 0)
			{
				TypeReference baseType = (TypeReference) typeDeclaration.BaseTypes[0];
				string fullName = GetFullName(baseType);
				if (CodeBase.Mappings.Contains(fullName))
				{
					TypeMapping mapping = CodeBase.Mappings[fullName];
					if (mapping != null)
					{
						ReplaceMember(invocationExpression, data, baseType);
					}
					else
						CheckAtDefaultType(invocationExpression, data);
				}
				else if (CodeBase.Types.Contains(fullName))
				{
					TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[fullName];
					if (baseTypeDeclaration.Type == ClassType.Class)
					{
						VerifyDerivedMethod(baseTypeDeclaration, invocationExpression, data);
					}
				}
			}
			else
			{
				CheckAtDefaultType(invocationExpression, data);
			}
		}

		private void CheckAtDefaultType(InvocationExpression invocationExpression, object data)
		{
			TypeReference defaultType = AstUtil.GetTypeReference("java.lang.Object", invocationExpression.Parent);
			defaultType.Parent = invocationExpression.Parent;

			if (CodeBase.Mappings.Contains(defaultType.Type))
			{
				TypeMapping defaultMapping = CodeBase.Mappings[defaultType.Type];
				string methodKey;
				if (ContainsMapping(defaultMapping, invocationExpression, out methodKey))
				{
					ThisReferenceExpression thisReference = new ThisReferenceExpression();
					string methodName = ((IdentifierExpression) invocationExpression.TargetObject).Identifier;
					FieldReferenceExpression fieldReference = new FieldReferenceExpression(thisReference, methodName);
					InvocationExpression newInvocation = new InvocationExpression(fieldReference, invocationExpression.Arguments);
					newInvocation.Parent = invocationExpression.Parent;
					ReplaceMember(newInvocation, data, defaultType);
				}
			}
		}

		private void ReplaceMember(InvocationExpression invocationExpression, object data, TypeReference invokerType)
		{
			Expression replacedExpression;
			string returnType = GetFullName(invokerType);
			if (invokerType.RankSpecifier != null && invokerType.RankSpecifier.Length > 0)
				returnType = "Array";

			TypeMapping mapping = CodeBase.Mappings[returnType];
			string methodKey;

			if (ContainsMapping(mapping, invocationExpression, out methodKey))
			{
				replacedExpression = GetReplacedExpression(invocationExpression, methodKey, mapping.Members);
				if (replacedExpression is NullExpression)
				{
					RemoveCurrentNode();
					((IList) data).Add(invocationExpression);
				}
				else
				{
					invocationExpression.TypeArguments.Add(null);
					ReplaceCurrentNode(replacedExpression);
					replacedExpression.AcceptVisitor(this, data);
				}
			}
			else if (invocationExpression.TargetObject is FieldReferenceExpression)
			{
				Expression invocationTarget = invocationExpression.TargetObject;
				FieldReferenceExpression methodTargetObject = (FieldReferenceExpression) invocationTarget;

				string fullName = GetFullName(invokerType);
				TypeReference retType = GetInstanceType(fullName, methodTargetObject.FieldName);
				if (retType != null)
				{
					string fullReturnType = GetFullName(retType);
					mapping = CodeBase.Mappings[fullReturnType];
				}

				if (ContainsMapping(mapping, invocationExpression, out methodKey))
				{
					replacedExpression = GetReplacedExpression(invocationExpression, methodKey, mapping.Members);
					ReplaceCurrentNode(replacedExpression);
					replacedExpression.AcceptVisitor(this, data);
				}
			}
		}

		private Expression GetReplacedExpression(Expression expression, string methodKey, IDictionary classMap)
		{
			Expression replacedExpression;
			string cSharpMethodMap = (string) classMap[methodKey];
			replacedExpression = GetReplacedExpression(expression, cSharpMethodMap);

			return replacedExpression;
		}

		protected Expression GetReplacedExpression(Expression expression, string mapKey)
		{
			bool removeId = false;
			if (mapKey.StartsWith("!"))
			{
				removeId = true;
				mapKey = mapKey.Substring(1);
			}
			AssignmentExpression mappedExpression = (AssignmentExpression) GetMapExpression(mapKey);

			Substitution substitution = new Substitution();
			ArrayList parameters = GetParameters(expression);
			Expression targetId = GetTargetObject(expression);
			substitution.Identifier = targetId;
			if (!removeId && expression is InvocationExpression)
			{
				InvocationExpression invocationExpression = (InvocationExpression) expression;
				if (invocationExpression.TargetObject is FieldReferenceExpression)
				{
					FieldReferenceExpression invocationTarget = (FieldReferenceExpression) invocationExpression.TargetObject;
					if (mappedExpression.Right is InvocationExpression)
					{
						IdentifierExpression identifierExpression = GetIdentifierExpression((InvocationExpression) mappedExpression.Right);
						if (identifierExpression != null)
						{
							FieldReferenceExpression referenceExpression = new FieldReferenceExpression(invocationTarget.TargetObject, identifierExpression.Identifier);
							Expression identifierParent = (Expression) identifierExpression.Parent;
							if (identifierParent is InvocationExpression)
								((InvocationExpression) identifierParent).TargetObject = referenceExpression;
						}
					}
				}
			}

			substitution.Substitute(mappedExpression, parameters);
			mappedExpression.Right.Parent = expression.Parent;
			return mappedExpression.Right;
		}

		private Expression GetTargetObject(Expression expression)
		{
			if (expression is FieldReferenceExpression)
				return ((FieldReferenceExpression) expression).TargetObject;
			else if (expression is InvocationExpression)
				return GetTargetObject(((InvocationExpression) expression).TargetObject);
			return null;
		}

		private ArrayList GetParameters(Expression expression)
		{
			if (expression is InvocationExpression)
				return ((InvocationExpression) expression).Arguments;
			else if (expression is ObjectCreateExpression)
				return ((ObjectCreateExpression) expression).Parameters;
			return null;
		}

		private IdentifierExpression GetIdentifierExpression(InvocationExpression invocationExpression)
		{
			Expression target = invocationExpression.TargetObject;
			while (target is FieldReferenceExpression || target is InvocationExpression)
			{
				if (target is FieldReferenceExpression)
					target = ((FieldReferenceExpression) target).TargetObject;
				else
					target = ((InvocationExpression) target).TargetObject;
			}
			if (target is IdentifierExpression)
				return (IdentifierExpression) target;

			return null;
		}

		private Expression GetMapExpression(string mapKey)
		{
			string program = "namespace Test { public class A { public void Method() { result = " + mapKey + "; } }}";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(program));
			parser.ParseMethodBodies = true;
			parser.Parse();

			CompilationUnit cu = parser.CompilationUnit;

			ParentVisitor parentVisitor = new ParentVisitor();
			parentVisitor.VisitCompilationUnit(cu, null);

			TypeReferenceCorrector typeReferenceCorrector = new TypeReferenceCorrector();
			typeReferenceCorrector.VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];
			ExpressionStatement st = (ExpressionStatement) md.Body.Children[0];
			AssignmentExpression ase = (AssignmentExpression) st.Expression;
			return ase;
		}

		private TypeReference GetMethodType(string name, IList baseTypes)
		{
			TypeReference declaringType = null;
			foreach (TypeReference baseType in baseTypes)
			{
				string fullName = GetFullName(baseType);
				declaringType = GetInstanceType(fullName, name);
				if (declaringType != null)
					break;
			}
			return declaringType;
		}

		private TypeReference GetInstanceType(string type, string method)
		{
			TypeReference declaringType = null;
			if (CodeBase.Types.Contains(type))
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) CodeBase.Types[type];
				declaringType = GetDeclaringType(typeDeclaration, method);
				if (declaringType == null && typeDeclaration.BaseTypes.Count > 0)
				{
					declaringType = GetMethodType(method, typeDeclaration.BaseTypes);
				}
				if (declaringType == null)
				{
					string obj = "java.lang.Object";
					if (CodeBase.Types.Contains(obj))
						typeDeclaration = (TypeDeclaration) CodeBase.Types[obj];
					declaringType = GetDeclaringType(typeDeclaration, method);
				}
			}
			return declaringType;
		}

		private TypeReference GetDeclaringType(TypeDeclaration typeDeclaration, string name)
		{
			TypeReference declaringType = null;
			IList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
			foreach (MethodDeclaration method in methods)
			{
				if (name == method.Name)
				{
					return AstUtil.GetTypeReference(typeDeclaration.Name, method.Parent);
				}
			}
			return declaringType;
		}
	}
}