namespace Janett.Framework
{
	using System;

	using ICSharpCode.NRefactory.Ast;

	public class NodeTypeExistenceVisitor : Transformer
	{
		private Type nodeType;
		public bool Contains;

		public NodeTypeExistenceVisitor(Type nodeType)
		{
			this.nodeType = nodeType;
		}

		protected override void BeginVisit(INode node)
		{
			if (node.GetType() == nodeType)
				Contains = true;
		}
	}
}