namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class DotClassTransformerTest : DotClassTransformer
	{
		[Test]
		public void DotClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void DotClass_IKVM()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			Mode = "IKVM";
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Mode = null;
		}

		[Test]
		public void FullNameDotClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ArrayDotClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void FullNameArrayDotClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}