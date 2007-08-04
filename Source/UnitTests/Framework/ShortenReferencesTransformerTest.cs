namespace Janett.Framework
{
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class ShortenReferencesTransformerTest : ShortenReferencesTransformer
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Mode = "IKVM";
		}

		[Test]
		public void OptimizeUsings()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program, SupportedLanguage.CSharp);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}