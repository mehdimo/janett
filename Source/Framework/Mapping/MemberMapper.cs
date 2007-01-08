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
			string program = GetCode(expression, mapKey);

			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(program));
			parser.ParseMethodBodies = true;
			parser.Parse();

			ParentVisitor parentVisitor = new ParentVisitor();
			parentVisitor.VisitCompilationUnit(parser.CompilationUnit, null);
			CompilationUnit cu = parser.CompilationUnit;

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];
			ExpressionStatement st = (ExpressionStatement) md.Body.Children[0];
			AssignmentExpression ase = (AssignmentExpression) st.Expression;
			Expression exp = ase.Right;
			exp.Parent = expression.Parent;

			return exp;
		}

		private string GetCode(Expression expression, string mapKey)
		{
			string result = mapKey;

			if (expression is InvocationExpression)
			{
				InvocationExpression ivExpression = (InvocationExpression) expression;
				string invocationTargetString = null;
				if (ivExpression.TargetObject is FieldReferenceExpression)
				{
					Expression invocationTarget = ((FieldReferenceExpression) ivExpression.TargetObject).TargetObject;
					invocationTargetString = GetCode(invocationTarget);
				}

				result = ReplaceArguments(ivExpression.Arguments, result);

				if (result.IndexOf("#id") != -1)
				{
					result = result.Replace("#id", invocationTargetString);
				}
				else if (result.StartsWith("!"))
				{
					result = result.Substring(1);
				}
				else if (invocationTargetString != null)
					result = invocationTargetString + "." + result;
			}

			else if (expression is FieldReferenceExpression)
			{
				if (result.IndexOf("#id") != -1)
				{
					Expression target = ((FieldReferenceExpression) expression).TargetObject;
					string targetString = GetCode(target);
					result = result.Replace("#id", targetString);
				}
			}

			else if (expression is ObjectCreateExpression)
			{
				ObjectCreateExpression obj = (ObjectCreateExpression) expression;
				result = ReplaceArguments(obj.Parameters, result);
			}

			return @"namespace A { public class B { public void Method() { a = " + result + @";} }}";
		}

		private string ReplaceArguments(ArrayList arguments, string result)
		{
			char argChar = 'a';
			foreach (Expression expr  in arguments)
			{
				string argStr = GetCode(expr);
				result = result.Replace("#" + argChar, argStr);
				argChar++;
			}
			return result;
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