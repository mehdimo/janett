namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class SubstitutionTest
	{
		private Substitution substitution;
		private MappingIdentifierMarker marker;

		[TestFixtureSetUp]
		public void SetUp()
		{
			substitution = new Substitution();
			marker = new MappingIdentifierMarker();
		}

		[Test]
		public void SubstitutionInArguments()
		{
			string program = TestUtil.StatementParse("substring(start, end);");
			CompilationUnit cv = TestUtil.ParseProgram(program);
			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cv, 0);
			string mapProgram = TestUtil.StatementParse("Substring(a, b - a);");
			CompilationUnit cu = TestUtil.ParseProgram(mapProgram);
			marker.MarkIdentifiers(cu);

			substitution.TrackedVisitCompilationUnit(cu, ivc.Arguments);
			string expected = TestUtil.CSharpStatementParse("Substring(start, end - start);");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void SubstitutionInTarget()
		{
			string program = TestUtil.StatementParse("startsWith(suffix, startIndex + 5);");
			CompilationUnit cv = TestUtil.ParseProgram(program);
			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cv, 0);
			string mapProgram = TestUtil.StatementParse("Substring(b).Startswith(a);");
			CompilationUnit cu = TestUtil.ParseProgram(mapProgram);
			marker.MarkIdentifiers(cu);

			substitution.TrackedVisitCompilationUnit(cu, ivc.Arguments);
			string expected = TestUtil.CSharpStatementParse("Substring(startIndex + 5).Startswith(suffix);");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}