namespace Janett.Framework
{
	using System.Collections.Generic;

	using ICSharpCode.NRefactory.Ast;

	public class RemoveEmptyBlocksTransformer : Transformer
	{
		public override object TrackedVisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			if (ifElseStatement.HasElseStatements)
			{
				foreach (Statement stm in ifElseStatement.FalseStatement)
				{
					if (stm is BlockStatement && stm.Children.Count == 0)
						ifElseStatement.FalseStatement = null;
				}
			}
			if (ifElseStatement.HasElseIfSections)
			{
				List<ElseIfSection> elseIfSections = new List<ElseIfSection>();
				elseIfSections.AddRange(ifElseStatement.ElseIfSections);
				foreach (ElseIfSection stm in ifElseStatement.ElseIfSections)
				{
					if (stm.EmbeddedStatement is BlockStatement && stm.EmbeddedStatement.Children.Count == 0)
						elseIfSections.Remove(stm);
				}
				ifElseStatement.ElseIfSections = elseIfSections;
			}
			if (!ifElseStatement.HasElseIfSections && !ifElseStatement.HasElseStatements)
			{
				foreach (Statement stm in ifElseStatement.TrueStatement)
				{
					if (stm is BlockStatement && stm.Children.Count == 0)
						RemoveCurrentNode();
				}
			}

			return base.TrackedVisitIfElseStatement(ifElseStatement, data);
		}

		public override object TrackedVisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			if (tryCatchStatement.StatementBlock.Children.Count == 0)
				RemoveCurrentNode();
			return base.TrackedVisitTryCatchStatement(tryCatchStatement, data);
		}

		public override object TrackedVisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			if (doLoopStatement.EmbeddedStatement is BlockStatement && doLoopStatement.EmbeddedStatement.Children.Count == 0)
				RemoveCurrentNode();
			return base.TrackedVisitDoLoopStatement(doLoopStatement, data);
		}
	}
}