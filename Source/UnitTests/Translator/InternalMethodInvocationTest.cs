namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class InternalMethodInvocationTest : InternalMethodInvocationTransformer
	{
		[Test]
		public void InvocationInTestCase()
		{
			string program1 = TestUtil.TypeMemberParse("protected int method(int arg1, int arg2) {}");

			CompilationUnit cu = TestUtil.ParseProgram(program1);
			NamespaceDeclaration ns1 = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns1.Children[0];
			CodeBase.Types.Add("Test.A", ty1);

			string program2 = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu2 = TestUtil.ParseProgram(program2);
			NamespaceDeclaration ns2 = (NamespaceDeclaration) cu2.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ns2.Children[0];

			CodeBase.Types.Add("Test.TestA", ty2);
			VisitCompilationUnit(cu2, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu2));
		}

		[Test]
		public void StaticInvocationInTestCase()
		{
			string program1 = TestUtil.TypeMemberParse("protected static int method(int arg1, int arg2) {}");

			CompilationUnit cu = TestUtil.ParseProgram(program1);
			NamespaceDeclaration ns1 = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns1.Children[0];
			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", ty1);

			string program2 = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu2 = TestUtil.ParseProgram(program2);
			NamespaceDeclaration ns2 = (NamespaceDeclaration) cu2.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ns2.Children[0];

			CodeBase.Types.Add("Test.TestA", ty2);
			VisitCompilationUnit(cu2, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu2));
		}
	}
}