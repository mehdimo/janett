namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class StaticConstructorTransformerTest : StaticConstructorTransformer
	{
		[Test]
		public void Test()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}