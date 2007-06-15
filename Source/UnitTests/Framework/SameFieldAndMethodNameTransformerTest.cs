namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class SameFieldAndMethodNameTransformerTest : SameFieldAndMethodNameTransformer
	{
		[Test]
		public void SimiarFieldAndMethodName()
		{
			string program = TestUtil.TypeMemberParse("String text; int run = 0; public void run(){}");
			string expected = TestUtil.CSharpTypeMemberParse("String text; int run_Field = 0; public void run(){}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Assert.AreEqual(1, CodeBase.References.Count);
			Assert.IsTrue(CodeBase.References.Contains("Test.Test.run"));
		}
	}
}