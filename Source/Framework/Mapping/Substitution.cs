namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class Substitution : Transformer
	{
		public Expression Identifier;

		public void Substitute(INode expression, IList args)
		{
			expression.AcceptVisitor(this, args);
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (identifierExpression.StartLocation.X == -1 && identifierExpression.StartLocation.Y == -1)
			{
				if (identifierExpression.Identifier.Length == 1 && data != null)
				{
					char ch = identifierExpression.Identifier[0];
					int index = ch - 'a';
					IList arguments = (IList) data;
					if (index > -1 && index < arguments.Count)
					{
						INode node = (INode) arguments[index];
						ReplaceCurrentNode(node);
					}
				}
				else if (identifierExpression.Identifier == "id" && Identifier != null)
					ReplaceCurrentNode(Identifier);
			}
			return null;
		}
	}
}