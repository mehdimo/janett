namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class NullableValueTypeTransformer : Transformer
	{
		private IDictionary types = new Hashtable();
		private IDictionary values = new Hashtable();

		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			if (types.Count == 0)
			{
				types.Add("java.lang.Boolean", "System.Boolean");
				if (Mode == "DotNet")
				{
					types.Add("java.lang.Integer", "System.Int32");
					types.Add("java.util.Date", "System.DateTime");
					types.Add("java.util.Calendar", "System.DateTime");
				}
				foreach (string type in types.Keys)
				{
					if (type == "java.lang.Boolean")
						values.Add(type, new PrimitiveExpression(false, "false"));
					else
					{
						IdentifierExpression target = new IdentifierExpression((string) types[type]);
						FieldReferenceExpression expression = new FieldReferenceExpression(target, "MinValue");
						values.Add(type, expression);
					}
				}
			}
			return base.TrackedVisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		public override object TrackedVisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			Expression left = binaryOperatorExpression.Left;
			TypeReference leftType = GetExpressionType(left);
			if (leftType != null && (leftType.RankSpecifier == null || leftType.RankSpecifier.Length == 0) && (binaryOperatorExpression.Right is PrimitiveExpression))
			{
				string fullName = GetFullName(leftType);
				if (types.Contains(fullName))
				{
					Expression minValue = (Expression) values[fullName];
					PrimitiveExpression nullRight = (PrimitiveExpression) binaryOperatorExpression.Right;
					if (nullRight.Value == null)
					{
						BinaryOperatorExpression replacedBinOP = binaryOperatorExpression;
						replacedBinOP.Right = minValue;
						ReplaceCurrentNode(replacedBinOP);
					}
				}
			}
			return base.TrackedVisitBinaryOperatorExpression(binaryOperatorExpression, data);
		}

		public override object TrackedVisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			if (variableDeclaration.Initializer != null && variableDeclaration.Initializer is PrimitiveExpression)
			{
				PrimitiveExpression pe = (PrimitiveExpression) variableDeclaration.Initializer;
				if (pe.Value == null)
				{
					TypeReference typeReference = null;
					if (variableDeclaration.Parent is LocalVariableDeclaration)
						typeReference = ((LocalVariableDeclaration) variableDeclaration.Parent).TypeReference;
					else if (variableDeclaration.Parent is FieldDeclaration)
						typeReference = ((FieldDeclaration) variableDeclaration.Parent).TypeReference;
					if (typeReference != null && (typeReference.RankSpecifier == null || typeReference.RankSpecifier.Length == 0))
					{
						string fullName = GetFullName(typeReference);
						if (types.Contains(fullName))
						{
							Expression minValue = (Expression) values[fullName];
							variableDeclaration.Initializer = minValue;
							minValue.Parent = variableDeclaration;
						}
					}
				}
			}
			return base.TrackedVisitVariableDeclaration(variableDeclaration, data);
		}

		public override object TrackedVisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			if (returnStatement.Expression is PrimitiveExpression)
			{
				PrimitiveExpression pe = (PrimitiveExpression) returnStatement.Expression;
				if (pe.Value == null)
				{
					INode member = AstUtil.GetParentOfType(returnStatement, typeof(MethodDeclaration));
					if (member != null)
					{
						TypeReference typeReference = ((MethodDeclaration) member).TypeReference;
						if (typeReference.RankSpecifier == null || typeReference.RankSpecifier.Length == 0)
						{
							string fullName = GetFullName(typeReference);
							if (types.Contains(fullName))
							{
								Expression minValue = (Expression) values[fullName];
								returnStatement.Expression = minValue;
								minValue.Parent = returnStatement;
							}
						}
					}
				}
			}
			return base.TrackedVisitReturnStatement(returnStatement, data);
		}

		public override object TrackedVisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			if (assignmentExpression.Right is PrimitiveExpression)
			{
				PrimitiveExpression pe = (PrimitiveExpression) assignmentExpression.Right;
				if (pe.Value == null)
				{
					TypeReference leftType = GetExpressionType(assignmentExpression.Left);
					if (leftType != null && (leftType.RankSpecifier == null || leftType.RankSpecifier.Length == 0))
					{
						string fullName = GetFullName(leftType);
						if (types.Contains(fullName))
						{
							Expression minValue = (Expression) values[fullName];
							assignmentExpression.Right = minValue;
							minValue.Parent = assignmentExpression;
						}
					}
				}
			}
			return base.TrackedVisitAssignmentExpression(assignmentExpression, data);
		}

		public override object TrackedVisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			if (conditionalExpression.TrueExpression is PrimitiveExpression && ((PrimitiveExpression) conditionalExpression.TrueExpression).Value == null)
			{
				TypeReference typeReference = GetExpressionType(conditionalExpression.FalseExpression);
				if (typeReference.RankSpecifier == null || typeReference.RankSpecifier.Length == 0)
				{
					string fullName = GetFullName(typeReference);
					if (types.Contains(fullName))
					{
						Expression minValue = (Expression) values[fullName];
						conditionalExpression.TrueExpression = minValue;
					}
				}
			}
			else if (conditionalExpression.FalseExpression is PrimitiveExpression && ((PrimitiveExpression) conditionalExpression.FalseExpression).Value == null)
			{
				TypeReference typeReference = GetExpressionType(conditionalExpression.TrueExpression);

				if (typeReference.RankSpecifier == null || typeReference.RankSpecifier.Length == 0)
				{
					string fullName = GetFullName(typeReference);
					if (types.Contains(fullName))
					{
						Expression minValue = (Expression) values[fullName];
						conditionalExpression.TrueExpression = minValue;
					}
				}
			}
			return base.TrackedVisitConditionalExpression(conditionalExpression, data);
		}

		public override object TrackedVisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			string fullName = GetFullName(objectCreateExpression.CreateType);
			if (CodeBase.Types.Contains(fullName))
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) CodeBase.Types[fullName];
				ArrayList args = ReplaceNullArguments(typeDeclaration, objectCreateExpression.Parameters, objectCreateExpression);
				objectCreateExpression.Parameters = args;
			}
			return base.TrackedVisitObjectCreateExpression(objectCreateExpression, data);
		}

		public override object TrackedVisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			if (constructorInitializer.ConstructorInitializerType == ConstructorInitializerType.This)
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(constructorInitializer, typeof(TypeDeclaration));
				ArrayList args = ReplaceNullArguments(typeDeclaration, constructorInitializer.Arguments, constructorInitializer);
				constructorInitializer.Arguments = args;
			}
			return base.TrackedVisitConstructorInitializer(constructorInitializer, data);
		}

		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			if (invocationExpression.TargetObject is IdentifierExpression)
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(invocationExpression, typeof(TypeDeclaration));
				ArrayList args = ReplaceNullArguments(typeDeclaration, invocationExpression.Arguments, invocationExpression);
				invocationExpression.Arguments = args;
			}
			return base.TrackedVisitInvocationExpression(invocationExpression, data);
		}

		private ArrayList ReplaceNullArguments(TypeDeclaration typeDeclaration, ArrayList ArgExpressions, INode node)
		{
			int index = 0;
			ArrayList args = new ArrayList();

			foreach (Expression argument in ArgExpressions)
			{
				bool flag = true;
				if (argument is PrimitiveExpression && ((PrimitiveExpression) argument).Value == null)
				{
					ParametrizedNode member = GetAssociateMember(typeDeclaration, node, ArgExpressions);
					if (member != null)
					{
						TypeReference typeReference = ((ParameterDeclarationExpression) member.Parameters[index]).TypeReference;
						if (typeReference.RankSpecifier == null || typeReference.RankSpecifier.Length == 0)
						{
							string fullTypeName = GetFullName(typeReference);
							if (types.Contains(fullTypeName))
							{
								Expression minValue = (Expression) values[fullTypeName];
								args.Add(minValue);
								flag = false;
							}
						}
					}
				}
				if (flag)
					args.Add(argument);

				index++;
			}
			return args;
		}

		private ParametrizedNode GetAssociateMember(TypeDeclaration typeDeclaration, INode node, ArrayList argExpressions)
		{
			if (node is InvocationExpression)
				return GetMethodDeclarationOf(typeDeclaration, (InvocationExpression) node);
			else
			{
				IList constructors = AstUtil.GetChildrenWithType(typeDeclaration, typeof(ConstructorDeclaration));
				foreach (ConstructorDeclaration constructor in constructors)
				{
					if (ContainsOnlyOneConstructorWithParamCount(constructors, constructor, argExpressions.Count) ||
					    MatchArguments(constructor.Parameters, argExpressions))
						return constructor;
				}
			}
			return null;
		}

		private bool ContainsOnlyOneConstructorWithParamCount(IList constructors, ConstructorDeclaration constructorDeclaration, int argumentsCount)
		{
			if (constructorDeclaration.Parameters.Count == argumentsCount)
			{
				int i = 0;
				foreach (ConstructorDeclaration cond in constructors)
				{
					if (cond.Parameters.Count == argumentsCount)
						i++;
				}
				return i == 1;
			}
			else
				return false;
		}
	}
}