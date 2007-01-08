namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.NRefactory.PrettyPrinter;
	using ICSharpCode.NRefactory.Visitors;

	using Janett.Commons;

	public class Transformer : NodeTrackingAstTransformer
	{
		private CodeBase codeBase;

		protected AstUtil AstUtil;
		private TypeResolver TypeResolver;
		private ExpressionTypeResolver ExpressionTypeResolver;

		public CodeBase CodeBase
		{
			get { return codeBase; }
			set
			{
				codeBase = value;
				ExpressionTypeResolver.CodeBase = value;
				TypeResolver.CodeBase = value;
			}
		}

		public string Mode;

		public Transformer()
		{
			TypeReference.Java = true;
			codeBase = new CodeBase(SupportedLanguage.Java);
			AstUtil = new AstUtil();

			TypeResolver = new TypeResolver();
			TypeResolver.AstUtil = AstUtil;
			TypeResolver.CodeBase = CodeBase;

			ExpressionTypeResolver = new ExpressionTypeResolver();
			ExpressionTypeResolver.AstUtil = AstUtil;
			ExpressionTypeResolver.CodeBase = CodeBase;

			ExpressionTypeResolver.TypeResolver = TypeResolver;
		}

		protected override void BeginVisit(INode node)
		{
			if (node is MethodDeclaration)
				Diagnostics.Set("Method", ((MethodDeclaration) node).Name);
			else if (node is TypeDeclaration)
				Diagnostics.Set("Type", ((TypeDeclaration) node).Name);
			else if (node is NamespaceDeclaration)
				Diagnostics.Set("Namesapce", ((NamespaceDeclaration) node).Name);
			else if (node is CompilationUnit)
				Diagnostics.Set("Transformer", GetType().Name);
			base.BeginVisit(node);
		}

		protected override void EndVisit(INode node)
		{
			if (node is MethodDeclaration)
				Diagnostics.Remove("Method");
			else if (node is TypeDeclaration)
				Diagnostics.Remove("Type");
			else if (node is NamespaceDeclaration)
				Diagnostics.Remove("Namesapce");
			else if (node is CompilationUnit)
				Diagnostics.Remove("Transformer");
			base.EndVisit(node);
		}

		protected TypeReference GetExpressionType(Expression invoker)
		{
			return ExpressionTypeResolver.GetType(invoker);
		}

		protected TypeReference GetExpressionType(Expression ex, INode parent)
		{
			return ExpressionTypeResolver.GetType(ex, parent);
		}

		protected string GetFullName(TypeReference typeReference)
		{
			return TypeResolver.GetFullName(typeReference);
		}

		protected string GetFullName(TypeDeclaration typeDeclaration)
		{
			return TypeResolver.GetFullName(typeDeclaration);
		}

		protected string GetStaticFullName(string identifier, INode parent)
		{
			return TypeResolver.GetStaticFullName(identifier, parent);
		}

		protected TypeDeclaration RemoveBaseTypeFrom(TypeDeclaration typeDeclaration, TypeReference removableBaseType)
		{
			if (typeDeclaration.BaseTypes.Count > 0)
			{
				TypeDeclaration replacedTypeDeclaration = typeDeclaration;
				string removableFullName = GetFullName(removableBaseType);
				int index = GetBaseTypeIndex(typeDeclaration, removableFullName);
				if (index != -1)
				{
					replacedTypeDeclaration.BaseTypes.RemoveAt(index);
					return replacedTypeDeclaration;
				}
			}
			return typeDeclaration;
		}

		protected int GetBaseTypeIndex(TypeDeclaration type, string parentName)
		{
			int index = 0;
			foreach (TypeReference baseType in type.BaseTypes)
			{
				string fullType = GetFullName(baseType);
				if (fullType == parentName)
					return index;
				index++;
			}
			return -1;
		}

		protected TypeDeclaration GetEnclosingTypeDeclaration(InvocationExpression invocationExpression)
		{
			TypeDeclaration typeDeclaration;

			if (invocationExpression.TargetObject is IdentifierExpression)
			{
				typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(invocationExpression, typeof(TypeDeclaration));
				return GetTypeDeclarationOf(typeDeclaration, invocationExpression);
			}
			else if (invocationExpression.TargetObject is FieldReferenceExpression)
			{
				FieldReferenceExpression fieldReference = (FieldReferenceExpression) invocationExpression.TargetObject;
				TypeReference targetType = GetExpressionType(fieldReference.TargetObject);
				if (targetType != null)
				{
					string fullName = GetFullName(targetType);
					if (CodeBase.Types.Contains(fullName))
					{
						TypeDeclaration targetTypeDeclaration = (TypeDeclaration) CodeBase.Types[fullName];
						return GetTypeDeclarationOf(targetTypeDeclaration, invocationExpression);
					}
				}
			}
			return null;
		}

		protected TypeDeclaration GetTypeDeclarationOf(TypeDeclaration typeDeclaration, InvocationExpression invocationExpression)
		{
			MethodDeclaration methodFound = GetMethodDeclarationOf(typeDeclaration, invocationExpression);
			if (methodFound != null)
				return (TypeDeclaration) methodFound.Parent;
			return null;
		}

		protected MethodDeclaration GetMethodDeclarationOf(TypeDeclaration typeDeclaration, InvocationExpression invocationExpression)
		{
			MethodDeclaration methodDeclaration = FindMethod(typeDeclaration, invocationExpression);
			if (methodDeclaration == null && typeDeclaration.BaseTypes.Count != 0)
			{
				TypeDeclaration parentClass = GetParentClass(typeDeclaration);
				if (parentClass != null)
					methodDeclaration = GetMethodDeclarationOf(parentClass, invocationExpression);
			}
			return methodDeclaration;
		}

		protected MethodDeclaration FindMethod(TypeDeclaration typeDeclaration, InvocationExpression invocationExpression)
		{
			foreach (MethodDeclaration methodDeclaration in AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration)))
			{
				if (IsInvocationForMethod(methodDeclaration, invocationExpression))
					return methodDeclaration;
			}
			return null;
		}

		protected TypeDeclaration GetParentClass(TypeDeclaration typeDeclaration)
		{
			if (typeDeclaration.BaseTypes.Count != 0)
			{
				TypeReference baseType = typeDeclaration.BaseTypes[0] as TypeReference;
				string fullName = GetFullName(baseType);
				if (CodeBase.Types.Contains(fullName))
				{
					TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[fullName];
					if (baseTypeDeclaration.Type == ClassType.Class)
						return baseTypeDeclaration;
				}
			}
			return null;
		}

		protected bool IsInvocationForMethod(MethodDeclaration methodDeclaration, InvocationExpression invocationExpression)
		{
			string identifier = null;
			if (invocationExpression.TargetObject is IdentifierExpression)
				identifier = ((IdentifierExpression) invocationExpression.TargetObject).Identifier;
			else if (invocationExpression.TargetObject is FieldReferenceExpression)
				identifier = ((FieldReferenceExpression) invocationExpression.TargetObject).FieldName;
			if (identifier == methodDeclaration.Name &&
			    invocationExpression.Arguments.Count == methodDeclaration.Parameters.Count)
			{
				int i = 0;
				foreach (ParameterDeclarationExpression parameter in methodDeclaration.Parameters)
				{
					Expression argument = (Expression) invocationExpression.Arguments[i];
					TypeReference objectType = GetExpressionType(argument);

					if (objectType != null && parameter.TypeReference.Type != objectType.Type)
						return false;
					i++;
				}
				return true;
			}
			return false;
		}

		protected string CreateMapKey(INode expression, bool typedArguments)
		{
			if (expression is InvocationExpression)
			{
				InvocationExpression ivExpression = (InvocationExpression) expression;
				string methodKey = null;
				if (ivExpression.TargetObject is FieldReferenceExpression)
				{
					FieldReferenceExpression methodTargetObject = (FieldReferenceExpression) ivExpression.TargetObject;
					methodKey = methodTargetObject.FieldName;
				}
				else if (ivExpression.TargetObject is IdentifierExpression)
				{
					methodKey = ((IdentifierExpression) ivExpression.TargetObject).Identifier;
				}

				string argumentsString = GetArgumentsMap(ivExpression.Arguments, typedArguments);
				return methodKey + "(" + argumentsString + ")";
			}
			else if (expression is FieldReferenceExpression)
			{
				FieldReferenceExpression fieldReference = (FieldReferenceExpression) expression;
				return fieldReference.FieldName;
			}
			else if (expression is ObjectCreateExpression)
			{
				ObjectCreateExpression objectCreation = (ObjectCreateExpression) expression;
				string objectCreationKey = "new " + objectCreation.CreateType.Type;
				string parametersString = GetArgumentsMap(objectCreation.Parameters, typedArguments);
				return objectCreationKey + "(" + parametersString + ")";
			}
			else if (expression is MethodDeclaration)
			{
				MethodDeclaration method = (MethodDeclaration) expression;
				string methodDeclarationKey = method.Name;
				string parametersString = GetArgumentsMap(method.Parameters, typedArguments);
				return methodDeclarationKey + "(" + parametersString + ")";
			}
			return null;
		}

		protected string GetArgumentsMap(ArrayList arguments, bool withReturnType)
		{
			string argumentsMap = "";
			char argumentChar = 'a';

			foreach (INode argument in arguments)
			{
				if (withReturnType)
				{
					string argumentType = "";
					TypeReference argumentTypeReference = GetExpressionType((Expression) argument);
					if (argumentTypeReference != null)
					{
						argumentType = argumentTypeReference.Type;
						if (TypeReference.PrimitiveTypesJavaReverse.Contains(argumentType))
							argumentType = (string) TypeReference.PrimitiveTypesJavaReverse[argumentType];
					}
					argumentsMap += argumentType + " " + argumentChar + ",";
				}
				else
					argumentsMap += argumentChar + ",";

				argumentChar++;
			}
			if (argumentsMap.EndsWith(","))
				argumentsMap = argumentsMap.Substring(0, argumentsMap.Length - 1);

			return argumentsMap;
		}

		protected bool ContainsMapping(TypeMapping mapping, Expression expression, out string key)
		{
			string methodKey = CreateMapKey(expression, false);
			string typedArgumentsMethodKey = CreateMapKey(expression, true);
			if (mapping != null && (mapping.Members.Contains(methodKey) || mapping.Members.Contains(typedArgumentsMethodKey)))
			{
				if (mapping.Members.Contains(methodKey))
					key = methodKey;
				else key = typedArgumentsMethodKey;
				return true;
			}
			key = null;
			return false;
		}

		protected internal string GetCode(INode node)
		{
			CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor();
			PrettyPrintOptions options = (PrettyPrintOptions) outputVisitor.Options;
			options.SpacesAfterComma = false;
			node.AcceptVisitor(outputVisitor, null);
			return outputVisitor.Text;
		}
	}
}