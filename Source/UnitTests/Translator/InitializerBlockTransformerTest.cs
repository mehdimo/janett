namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class InitializerBlockTransformerTest : InitializerBlockTransformer
	{
		[Test]
		public void InitializerBlock()
		{
			string program = TestUtil.TypeMemberParse("int x; {x = 10;}");
			string expected = TestUtil.CSharpTypeMemberParse("int x; private void InitTest() {x = 10;} public Test(){InitTest();} ");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void MergeInConstructor()
		{
			string program = TestUtil.TypeMemberParse("int x; {x = 10;} public Test() {int i = 1;}");
			string expected = TestUtil.CSharpTypeMemberParse("int x; public Test(){InitTest(); int i = 1;} private void InitTest() {x = 10;}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void MultiInitializerBlock()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void MultiInitializerBlockWithMultiConstructor()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}