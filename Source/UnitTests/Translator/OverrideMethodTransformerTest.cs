namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class OverrideMethodTransformerTest : OverridedMethodTransformer
	{
		[TearDown]
		public void TearDown()
		{
			CodeBase.Types.Clear();
		}

		[Test]
		public void AbstractClass()
		{
			string program = TestUtil.GetInput();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[1];
			CodeBase.Types.Add("Test.Rectangle", ty1);
			CodeBase.Types.Add("Test.Shape", ty2);

			VisitCompilationUnit(cu, null);

			MethodDeclaration md1 = (MethodDeclaration) ty1.Children[1];
			MethodDeclaration md2 = (MethodDeclaration) ty1.Children[2];

			Assert.AreEqual((Modifiers.Public | Modifiers.Override), md1.Modifier);
			Assert.AreEqual((Modifiers.Public | Modifiers.Override), md2.Modifier);
		}

		[Test]
		public void NonAbstractClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[1];
			CodeBase.Types.Add("Test.B", ty1);
			CodeBase.Types.Add("Test.A", ty2);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void HierarchyOverriding()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration tyC = (TypeDeclaration) ns.Children[0];
			TypeDeclaration tyB = (TypeDeclaration) ns.Children[1];
			TypeDeclaration tyA = (TypeDeclaration) ns.Children[2];

			CodeBase.Types.Add("Test.A", tyA);
			CodeBase.Types.Add("Test.B", tyB);
			CodeBase.Types.Add("Test.C", tyC);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void OverrideMethodWithPartialFullType()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[1];
			TypeDeclaration ty3 = (TypeDeclaration) ty2.Children[0];

			CodeBase.Types.Add("Test.Test", ty1);
			CodeBase.Types.Add("Test.A", ty2);
			CodeBase.Types.Add("Test.A.B", ty3);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}