namespace Janett.Framework
{
	using System.Collections;
	using System.Collections.Generic;

	using Commons;

	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.NRefactory.PrettyPrinter;
	using ICSharpCode.NRefactory.Visitors;

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
				Diagnostics.Set("Namespace", ((NamespaceDeclaration) node).Name);
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

					if (objectType != null && !AreSameTypes(parameter.TypeReference, objectType))
						return false;
					i++;
				}
				return true;
			}
			return false;
		}

		protected bool AreSameTypes(TypeReference parameterType, TypeReference argumentType)
		{
			if (argumentType == TypeReference.Null)
				return true;
			string parameterFullType = GetFullName(parameterType);
			string argumentFullType = GetFullName(argumentType);

			if (parameterFullType == argumentFullType || parameterType.Type.ToUpper() == argumentType.Type.ToUpper())
				return true;
			else if (CodeBase.Types.Contains(argumentFullType))
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) CodeBase.Types[argumentFullType];
				return Extends(typeDeclaration, parameterType);
			}
			return false;
		}

		protected bool AreEqualTypes(TypeReference parameterType, TypeReference argumentType)
		{
			string parameterFullType = GetFullName(parameterType);
			string argumentFullType = GetFullName(argumentType);

			if (parameterFullType == argumentFullType || parameterType.Type.ToUpper() == argumentType.Type.ToUpper())
				return true;
			else return false;
		}

		protected bool Extends(TypeDeclaration typeDeclaration, TypeReference typeReference)
		{
			bool extends = false;
			string typeFullName = GetFullName(typeReference);
			foreach (TypeReference baseType in typeDeclaration.BaseTypes)
			{
				string baseTypeName = GetFullName(baseType);
				if (baseTypeName == typeFullName)
					return true;
				else if (CodeBase.Types.Contains(baseTypeName))
				{
					TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[baseTypeName];
					extends = Extends(baseTypeDeclaration, typeReference);
					if (extends)
						return extends;
				}
			}
			return extends;
		}

		protected string CreateMapKey(INode expression, ArgumentMapType typedArguments)
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

		protected string GetArgumentsMap<T>(List<T> arguments, ArgumentMapType argumentMapType)
		{
			string argumentsMap = "";
			char argumentChar = 'a';

			int expressiondCount = 0;
			foreach (INode argument in arguments)
			{
				if (argumentMapType == ArgumentMapType.Typed)
				{
					string argumentType = "";
					TypeReference argumentTypeReference = GetExpressionType((Expression) argument);
					if (argumentTypeReference != null)
					{
						argumentType = GetFullName(argumentTypeReference);
						if (TypeReference.PrimitiveTypesJavaReverse.ContainsKey(argumentType))
							argumentType = (string) TypeReference.PrimitiveTypesJavaReverse[argumentType];
						if (argumentType == "java.lang.String")
							argumentType = "String";
					}
					argumentsMap += argumentType + " " + argumentChar + ",";
				}
				else if (argumentMapType == ArgumentMapType.Untyped)
					argumentsMap += argumentChar + ",";
				else if (argumentMapType == ArgumentMapType.Expression)
				{
					if (argument is Expression)
					{
						string str = argumentChar.ToString();
						if (expressiondCount == 0)
						{
							str = GetTargetString((Expression) argument);
							expressiondCount++;
						}
						str = RemoveNamespace(str);
						argumentsMap += str + ",";
					}
				}

				argumentChar++;
			}
			if (argumentsMap.EndsWith(","))
				argumentsMap = argumentsMap.Substring(0, argumentsMap.Length - 1);

			return argumentsMap;
		}

		private string RemoveNamespace(string expression)
		{
			if (expression.IndexOf(".") != -1)
			{
				string[] expressionParts = expression.Split('.');
				string ns = "";
				foreach (string str in expressionParts)
				{
					if (ns == "")
						ns = str;
					else
						ns += "." + str;
					if (codeBase.Types.ExternalLibraries.Contains(ns))
						break;
				}
				if (ns != expression)
					return expression.Replace(ns + ".", "");
			}
			return expression;
		}

		protected bool ContainsMapping(TypeMapping mapping, INode expression, out string key)
		{
			string methodKey = CreateMapKey(expression, ArgumentMapType.Untyped);
			string typedArgumentsMethodKey = CreateMapKey(expression, ArgumentMapType.Typed);
			if (mapping != null && (mapping.Members.Contains(methodKey) || mapping.Members.Contains(typedArgumentsMethodKey)))
			{
				if (mapping.Members.Contains(methodKey))
					key = methodKey;
				else key = typedArgumentsMethodKey;
				return true;
			}
			else
			{
				string expressionedMethodKey = CreateMapKey(expression, ArgumentMapType.Expression);
				if (mapping != null && mapping.Members.Contains(expressionedMethodKey))
				{
					key = expressionedMethodKey;
					return true;
				}
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

		protected bool IsAbstractClass(TypeDeclaration typeDeclaration)
		{
			return (typeDeclaration.Type == ClassType.Class && AstUtil.ContainsModifier(typeDeclaration, Modifiers.Abstract));
		}

		protected List<INode> GetAccessibleMethods(TypeDeclaration typeDeclaration)
		{
			List<INode> methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
			if (typeDeclaration.BaseTypes.Count > 0)
			{
				foreach (TypeReference baseType in typeDeclaration.BaseTypes)
				{
					string fullName = GetFullName(baseType);
					if (!IsInExternalLibraries(fullName) && CodeBase.Types.Contains(fullName))
					{
						TypeDeclaration type = (TypeDeclaration) CodeBase.Types[fullName];
						methods.AddRange(GetAccessibleMethods(type));
					}
				}
			}
			return methods;
		}

		protected bool IsInExternalLibraries(string name)
		{
			foreach (string nameSpace in CodeBase.Types.ExternalLibraries)
			{
				if (name.StartsWith(nameSpace + "."))
					return true;
			}
			return false;
		}

		protected string GetTargetString(Expression targetObject)
		{
			Stack stack = new Stack();
			Expression target = targetObject;
			while (target is FieldReferenceExpression)
			{
				string str = ((FieldReferenceExpression) target).FieldName;
				stack.Push(str);
				target = ((FieldReferenceExpression) target).TargetObject;
			}
			if (target is IdentifierExpression)
				stack.Push(((IdentifierExpression) target).Identifier);
			if (target is TypeReferenceExpression)
				stack.Push(((TypeReferenceExpression) target).TypeReference.Type);
			string item;
			string result = "";
			while (stack.Count != 0)
			{
				item = (string) stack.Pop();
				result += item + ".";
			}
			result = result.TrimEnd('.');
			return result;
		}

		protected void ReplaceCurrentNode(INode oldNode, INode[] newNodes, object data)
		{
			ReplaceCurrentNode(oldNode, newNodes);
			INode currentReplacedExpression = GetCurrentReplacedNode(newNodes);
			currentReplacedExpression.AcceptVisitor(this, data);
		}

		private INode GetCurrentReplacedNode(INode[] nodes)
		{
			foreach (INode node in nodes)
			{
				if (node.StartLocation.X == -1 && node.StartLocation.Y == -1)
					return node;
			}
			return nodes[0];
		}

		private void ReplaceCurrentNode(INode oldNode, INode[] newNodes)
		{
			BlockStatement blockStatement = GetAppropriateBlock(oldNode);

			ArrayList beforeCurrentNodes = new ArrayList();
			ArrayList afterCurrentNodes = new ArrayList();
			INode newCurrentNode = Partition(newNodes, beforeCurrentNodes, afterCurrentNodes);

			int index = 0;
			if (newNodes.Length > 1)
				index = GetIndex(blockStatement.Children, oldNode);

			ReplaceCurrentNode(newCurrentNode);

			if (blockStatement != null)
			{
				if (blockStatement.Parent is IfElseStatement || blockStatement.Parent is ElseIfSection)
				{
					beforeCurrentNodes.AddRange(afterCurrentNodes);
					afterCurrentNodes.Clear();
				}
				if (afterCurrentNodes.Count > 0)
					InsertSidesOfCurrentNode(afterCurrentNodes, blockStatement, index, Position.After);

				if (beforeCurrentNodes.Count > 0)
					InsertSidesOfCurrentNode(beforeCurrentNodes, blockStatement, index, Position.Before);
			}
		}

		private BlockStatement GetAppropriateBlock(INode node)
		{
			if (IsInIfCondition(node))
			{
				IfElseStatement ifElse = (IfElseStatement) AstUtil.GetParentOfType(node, typeof(IfElseStatement));
				if (ifElse.TrueStatement[0] is BlockStatement)
					return (BlockStatement) ifElse.TrueStatement[0];
			}
			else if (IsInElseIfCondition(node))
			{
				ElseIfSection elseIf = (ElseIfSection) AstUtil.GetParentOfType(node, typeof(ElseIfSection));
				if (elseIf.EmbeddedStatement is BlockStatement)
					return (BlockStatement) elseIf.EmbeddedStatement;
			}
			return (BlockStatement) AstUtil.GetParentOfType(node, typeof(BlockStatement));
		}

		private bool IsInIfCondition(INode node)
		{
			return IsInCondition(node, typeof(IfElseStatement), GetIfConditionHashCode);
		}

		private bool IsInElseIfCondition(INode node)
		{
			return IsInCondition(node, typeof(ElseIfSection), GetElseIfConditionHashCode);
		}

		private bool IsInCondition(INode node, System.Type conditionType, GetConditionHashCode GetConditionHashCode)
		{
			INode lastNode = node;
			INode currentNode = node.Parent;
			while (currentNode != null && !(currentNode is BlockStatement))
			{
				if (currentNode.GetType() == conditionType)
				{
					if (GetConditionHashCode(currentNode) == lastNode.GetHashCode())
						return true;
					else
						return false;
				}
				else
				{
					lastNode = currentNode;
					currentNode = currentNode.Parent;
				}
			}
			return false;
		}

		private delegate int GetConditionHashCode(INode node);

		private int GetIfConditionHashCode(INode node)
		{
			IfElseStatement ifElse = (IfElseStatement) node;
			return ifElse.Condition.GetHashCode();
		}

		private int GetElseIfConditionHashCode(INode node)
		{
			ElseIfSection elseIf = (ElseIfSection) node;
			return elseIf.Condition.GetHashCode();
		}

		private INode Partition(INode[] nodes, ArrayList beforeCurrentNodes, ArrayList afterCurrentNodes)
		{
			if (nodes.Length == 1)
				return nodes[0];
			else
			{
				INode current = null;
				foreach (INode node in nodes)
				{
					if (node.StartLocation.X == -1 && node.StartLocation.Y == -1)
						current = node;
					else if (current == null)
						beforeCurrentNodes.Add(node);
					else
						afterCurrentNodes.Add(node);
				}

				if (current == null && beforeCurrentNodes.Count > 0)
				{
					current = nodes[0];
					beforeCurrentNodes.RemoveAt(0);
					afterCurrentNodes.AddRange(beforeCurrentNodes);
					beforeCurrentNodes.Clear();
				}
				return current;
			}
		}

		private void InsertSidesOfCurrentNode(IList nodes, BlockStatement blockStatement, int index, Position position)
		{
			List<INode> coll = new List<INode>();
			foreach (INode node in nodes)
			{
				INode nde = node;
				if (node is Expression)
					nde = new ExpressionStatement((Expression) node);
				nde.Parent = blockStatement;
				coll.Add(nde);
			}
			if (position == Position.After)
				index++;
			blockStatement.Children.InsertRange(index, coll);
		}

		private int GetIndex(List<INode> list, INode node)
		{
			int index = 0;
			INode parent = AstUtil.GetParentOfType(node, typeof(ExpressionStatement));
			if (parent == null)
				parent = AstUtil.GetParentOfType(node, typeof(LocalVariableDeclaration));
			if (parent == null)
				parent = AstUtil.GetParentOfType(node, typeof(AssignmentExpression));
			if (parent != null)
			{
				int nodeHashCode = parent.GetHashCode();
				foreach (INode nd in list)
				{
					int hashCode = nd.GetHashCode();
					if (nodeHashCode == hashCode)
						return index;
					index++;
				}
			}
			return 0;
		}

		protected bool MatchArguments(List<ParameterDeclarationExpression> parameters, List<Expression> arguments)
		{
			if (parameters.Count == arguments.Count)
			{
				int index = 0;
				foreach (Expression argument in arguments)
				{
					TypeReference argumentType = GetExpressionType(argument);
					TypeReference parameterType = ((ParameterDeclarationExpression) parameters[index]).TypeReference;
					if (!AreSameTypes(parameterType, argumentType))
						return false;
					index++;
				}
				return true;
			}
			else
				return false;
		}
	}

	public enum ArgumentMapType
	{
		Untyped,
		Typed,
		Expression
	}

	public enum Position
	{
		Before,
		After
	}
}