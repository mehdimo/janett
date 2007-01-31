namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class MethodExcludeTransformer : Transformer
	{
		public IList Methods = new ArrayList();

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (Methods.Contains(methodDeclaration.Name))
				RemoveCurrentNode();
			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}
	}
}