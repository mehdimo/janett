namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class SystemFieldsTransformerTest : SystemFieldsTransformer
	{
		[Test]
		public void Test()
		{
			string program = TestUtil.StatementParse("System._out.print(10); System.err.close();");
			string expected = TestUtil.CSharpStatementParse("java.lang.System.@out.print(10); java.lang.System.err.close();");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}