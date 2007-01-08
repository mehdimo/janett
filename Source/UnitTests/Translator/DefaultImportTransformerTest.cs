namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class DefaultImportTransformerTest : DefaultImportTransformer
	{
		[Test]
		public void Import_JavaLang()
		{
			string program = "package Test; import java.util.List; public class A {}";
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			string expected = "namespace Test { using java.lang.*; using java.util.List; public class A {} }";
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}