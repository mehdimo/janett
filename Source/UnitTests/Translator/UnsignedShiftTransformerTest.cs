namespace Janett.Translator
{
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class UnsignedShiftTransformerTest : UnsignedShiftTransformer
	{
		[Test]
		public void Int()
		{
			string program = TestUtil.StatementParse("int i; i = i >>> 3;");
			string expected = TestUtil.CSharpStatementParse("int i; i = (int)((uint)i >> 3);");

			CompilationUnit cu = TestUtil.ParseProgram(program, SupportedLanguage.Java);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void IntAssign()
		{
			string program = TestUtil.StatementParse("int i; i >>>= 2;");
			string expected = TestUtil.CSharpStatementParse("int i; i = (int)((uint)i >> 2);");

			CompilationUnit cu = TestUtil.ParseProgram(program, SupportedLanguage.Java);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Long()
		{
			string program = TestUtil.StatementParse("long l; l = l >>> 3;");
			string expected = TestUtil.CSharpStatementParse("long l; l = (long)((ulong)l >> 3);");

			CompilationUnit cu = TestUtil.ParseProgram(program, SupportedLanguage.Java);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}