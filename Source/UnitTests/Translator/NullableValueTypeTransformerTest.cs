namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class NullableValueTypeTransformerTest : NullableValueTypeTransformer
	{
		[Test]
		public void Int()
		{
			string program = TestUtil.StatementParse("int a; bool b = (a == null)");
			string expected = TestUtil.CSharpStatementParse("int a; bool b = (a == System.Int32.MinValue)");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void IntWithRank()
		{
			string program = TestUtil.StatementParse("int[] a; bool b = (a == null)");
			string expected = TestUtil.CSharpStatementParse("int[] a; bool b = (a == null)");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}