namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class ToArrayTransformerTest : ToArrayTransformer
	{
		[Test]
		public void Test()
		{
			string program = TestUtil.StatementParse("Set ls; ls.ToArray(new string[5]);");
			string expected = TestUtil.CSharpStatementParse("Set ls; ls.ToArray(typeof(string));");

			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}