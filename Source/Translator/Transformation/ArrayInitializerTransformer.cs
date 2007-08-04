namespace Janett.Translator
{
	using System.Collections.Generic;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class ArrayInitializerTransformer : Transformer
	{
		public override object TrackedVisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			string variableName = GetVariableName(arrayCreateExpression);
			List<Expression> initializerList = arrayCreateExpression.ArrayInitializer.CreateExpressions;
			Expression replacedExpression = arrayCreateExpression;
			if (initializerList.Count > 0 && initializerList[0] is CollectionInitializerExpression && data is InsertionBlockData)
			{
				ArrayCreateExpression replacedArrayCreateExpression = arrayCreateExpression;
				replacedArrayCreateExpression.ArrayInitializer = null;
				replacedArrayCreateExpression.Arguments.Add(new PrimitiveExpression(initializerList.Count, initializerList.Count.ToString()));

				string arrayTypeName = arrayCreateExpression.CreateType.Type + "s";
				Position position = Position.After;
				if (variableName == null)
				{
					variableName = arrayTypeName;
					position = Position.Before;
				}

				List<Statement> initStatements = GetArrayInitStatements(replacedArrayCreateExpression, variableName, initializerList);
				InsertionBlockData insertionBlockData = (InsertionBlockData) data;
				insertionBlockData.Block = (BlockStatement) AstUtil.GetParentOfType(replacedArrayCreateExpression, typeof(BlockStatement));
				insertionBlockData.BlockChildIndex = GetBlockChildIndex(replacedArrayCreateExpression, position);
				insertionBlockData.Statements = initStatements;

				if (variableName == arrayTypeName)
				{
					IdentifierExpression identifierExpression = new IdentifierExpression(variableName);
					replacedExpression = identifierExpression;

					VariableDeclaration variableDeclaration = new VariableDeclaration(variableName, arrayCreateExpression);
					LocalVariableDeclaration localVariable = new LocalVariableDeclaration(variableDeclaration);
					localVariable.TypeReference = arrayCreateExpression.CreateType;

					initStatements.Insert(0, localVariable);
				}

				ReplaceCurrentNode(replacedExpression);
			}

			return base.TrackedVisitArrayCreateExpression(arrayCreateExpression, data);
		}

		public override object TrackedVisitBlockStatement(BlockStatement blockStatement, object data)
		{
			BlockStatement replaced = blockStatement;
			InsertionBlockData insertionBlockData = new InsertionBlockData();
			base.TrackedVisitBlockStatement(blockStatement, insertionBlockData);
			if (insertionBlockData.Block != null && insertionBlockData.Statements.Count > 0)
			{
				if (blockStatement.GetHashCode() == insertionBlockData.Block.GetHashCode())
				{
					IList<INode> nodes = new List<INode>();
					foreach (Statement node in insertionBlockData.Statements)
						nodes.Add(node);
					replaced.Children.InsertRange(insertionBlockData.BlockChildIndex, nodes);
					ReplaceCurrentNode(replaced);
				}
			}
			return null;
		}

		private List<Statement> GetArrayInitStatements(ArrayCreateExpression arrayCreateExpression, string variableName, List<Expression> initializerList)
		{
			List<Statement> list = new List<Statement>();
			for (int idx = 0; idx < initializerList.Count; idx++)
			{
				AssignmentExpression assignment = InitArrayStatement(arrayCreateExpression, variableName, ((CollectionInitializerExpression) initializerList[idx]).CreateExpressions, idx);
				ExpressionStatement expressionStatement = new ExpressionStatement(assignment);
				list.Add(expressionStatement);
			}
			return list;
		}

		private int GetBlockChildIndex(Expression expression, Position position)
		{
			BlockStatement block = (BlockStatement) AstUtil.GetParentOfType(expression, typeof(BlockStatement));
			INode expressionParent = GetExpressionParent(expression);
			int index = -1;
			if (position == Position.After)
				index = 0;
			foreach (INode node in block.Children)
			{
				index++;
				if (node.GetHashCode() == expressionParent.GetHashCode())
					return index;
			}
			return 0;
		}

		private AssignmentExpression InitArrayStatement(ArrayCreateExpression arrayCreateExpression, string variableName, List<Expression> creatExpressions, int index)
		{
			IdentifierExpression identifierExpression = new IdentifierExpression(variableName);
			List<Expression> indexes = new List<Expression>();
			indexes.Add(new PrimitiveExpression(index, index.ToString()));
			IndexerExpression left = new IndexerExpression(identifierExpression, indexes);
			string createType = arrayCreateExpression.CreateType.Type;
			ArrayCreateExpression right = new ArrayCreateExpression(new TypeReference(createType, new int[1]));
			right.ArrayInitializer = new CollectionInitializerExpression(creatExpressions);
			return new AssignmentExpression(left, AssignmentOperatorType.Assign, right);
		}

		private string GetVariableName(ArrayCreateExpression arrayCreateExpression)
		{
			if (arrayCreateExpression.Parent is AssignmentExpression)
			{
				AssignmentExpression assignmentExpression = (AssignmentExpression) arrayCreateExpression.Parent;
				Expression left = assignmentExpression.Left;
				if (left is IdentifierExpression)
					return ((IdentifierExpression) left).Identifier;
				else
					return null;
			}
			else if (arrayCreateExpression.Parent is VariableDeclaration)
			{
				VariableDeclaration variableDeclaration = (VariableDeclaration) arrayCreateExpression.Parent;
				return variableDeclaration.Name;
			}

			else
				return null;
		}

		private INode GetExpressionParent(Expression expression)
		{
			if (expression.Parent is VariableDeclaration || expression.Parent is AssignmentExpression)
				return expression.Parent.Parent;
			return expression.Parent;
		}
	}

	public class InsertionBlockData
	{
		public BlockStatement Block;

		public int BlockChildIndex;
		public List<Statement> Statements;
	}
}