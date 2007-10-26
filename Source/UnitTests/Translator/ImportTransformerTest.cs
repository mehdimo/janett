namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class ImportTransformerTest
	{
		private ImportTransformer importTransformer;

		[TestFixtureSetUp]
		public void SetUp()
		{
			importTransformer = new ImportTransformer();
		}

		[Test]
		public void Test()
		{
			string program = "package Test; import junit.framework.*; import java.lang.StringBuffer;";
			string expected = "namespace Test {using junit.framework; using StringBuffer = java.lang.StringBuffer;}";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			importTransformer.VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void CSharpKeywordInUsing()
		{
			string program = "package Test; import java.lang.ref.SoftReference; import javax.print.event.*; public class A {}";
			string expected = "namespace Test { using SoftReference = java.lang.@ref.SoftReference; using javax.print.@event; public class A {} }";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			importTransformer.Mode = "IKVM";
			importTransformer.VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}