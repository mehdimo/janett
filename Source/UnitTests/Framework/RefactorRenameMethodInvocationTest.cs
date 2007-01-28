namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class RefactorRenameMethodInvocationTest : RenameMethodInvocationRefactoring
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			CodeBase.Mappings = new Mappings(@"../../../Translator/Mappings/IKVM");
		}

		[TearDown]
		public void TearDown()
		{
			CodeBase.Types.Clear();
		}

		[Test]
		public void InheritedMethods()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[1];
			TypeDeclaration ty3 = (TypeDeclaration) ns.Children[2];

			CodeBase.Types.Add("Test.C", ty1);
			CodeBase.Types.Add("Test.A", ty2);
			CodeBase.Types.Add("Test.B", ty3);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InheritedMethodUsingInnerClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ty1.Children[0];
			TypeDeclaration ty3 = (TypeDeclaration) ns.Children[1];

			CodeBase.Types.Add("Test.A", ty1);
			CodeBase.Types.Add("Test.A.Ab", ty2);
			CodeBase.Types.Add("Test.B", ty3);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ExternalLibrariesMethods()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			CodeBase.Types.Add("Test.A", ty1);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InnerInternalLibMethod()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ty1.Children[0];
			CodeBase.Types.Add("Test.A", ty1);
			CodeBase.Types.Add("Test.A$InnerA", ty2);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InheritedMethodFromExternalLib()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[1];
			TypeDeclaration ty2 = (TypeDeclaration) ty1.Children[0];
			CodeBase.Types.Add("Test.A", ty1);
			CodeBase.Types.Add("Test.A.InnerA", ty2);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void TestN()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[1];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[2];
			CodeBase.Types.Add("Test.A", ty1);
			CodeBase.Types.Add("Test.B", ty2);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}