namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class ClassModifierTransformerTest : ClassModifierTransformer
	{
		[Test]
		public void Test()
		{
			string program = TestUtil.PackageMemberParse("private static class DialogBaseUnits{}");
			string expected = TestUtil.NamespaceMemberParse("private class DialogBaseUnits{}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}