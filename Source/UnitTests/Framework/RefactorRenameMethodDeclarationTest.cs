namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class RefactorRenameMethodDeclarationTest : RenameMethodDeclarationRefactoring
	{
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			CodeBase.Types.LibrariesFolder = @"../../../Translator/Libraries";
		}

		[Test]
		public void InternalMethods()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InheritedMethodInExternalLib()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];

			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[1];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[2];

			CodeBase.Types.Add("Test.RefactorTest", ty1);
			CodeBase.Types.Add("Test.AbstractTest", ty2);

			CodeBase.Types.ExternalLibraries.Add("junit.framework");
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void SimilarMethodNameAndTypeName()
		{
			string program = TestUtil.PackageMemberParse("public class TestRefactoring { public void testRefactoring() {} }");
			string expected = TestUtil.NamespaceMemberParse("public class TestRefactoring { public void TestRefactoring_() {} }");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}