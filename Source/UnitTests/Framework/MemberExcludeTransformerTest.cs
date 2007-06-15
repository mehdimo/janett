namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class MemberExcludeTransformerTest
	{
		private MemberExcludeTransformer memberExcludeTransformer;

		[Test]
		public void CurrentTypeMethod()
		{
			memberExcludeTransformer = new MemberExcludeTransformer("RemoveMethod");
			string program = TestUtil.TypeMemberParse("public void Method() {int i; RemoveMethod(); i = 0;} private void RemoveMethod() {}");
			string expected = TestUtil.CSharpTypeMemberParse("public void Method() {int i; i = 0;}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			memberExcludeTransformer.VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InnerType()
		{
			memberExcludeTransformer = new MemberExcludeTransformer("$MyInnerType");
			string program = TestUtil.TypeMemberParse("public class MyInnerType{}");
			string expected = TestUtil.NamespaceMemberParse("public class Test{}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			memberExcludeTransformer.VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}