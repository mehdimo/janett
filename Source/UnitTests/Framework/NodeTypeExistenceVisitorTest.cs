namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class NodeTypeExistenceVisitorTest
	{
		private NodeTypeExistenceVisitor nodeTypeExistenceVisitor;

		[Test]
		public void ThisReference()
		{
			nodeTypeExistenceVisitor = new NodeTypeExistenceVisitor(typeof(ThisReferenceExpression));

			string program = TestUtil.StatementParse("this.name = name;");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			AssignmentExpression assignment = (AssignmentExpression) TestUtil.GetStatementNodeOf(cu, 0);
			nodeTypeExistenceVisitor.VisitAssignmentExpression(assignment, null);
			Assert.IsTrue(nodeTypeExistenceVisitor.Contains);
		}
	}
}