namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class SuperUsageTransformerTest : SuperUsageTransformer
	{
		[Test]
		public void BaseAsParameter()
		{
			string program = TestUtil.StatementParse("toString(super); super.getClass();");
			string expected = TestUtil.CSharpStatementParse("toString(this); base.getClass();");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}