namespace Janett.Framework
{
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;

	public class MappingIdentifierMarker : Transformer
	{
		public void MarkIdentifiers(CompilationUnit compilationUnit)
		{
			VisitCompilationUnit(compilationUnit, null);
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			identifierExpression.StartLocation = new Location(-1, -1);
			return null;
		}
	}
}