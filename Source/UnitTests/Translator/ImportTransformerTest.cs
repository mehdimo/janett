namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class ImportTransformerTest
	{
		[Test]
		public void Test()
		{
			string program = "package Test; import junit.framework.*; import java.lang.StringBuffer;";
			string expected = "namespace Test {using junit.framework; using StringBuffer = java.lang.StringBuffer;}";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			ImportTransformer importTransformer = new ImportTransformer();
			importTransformer.VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}