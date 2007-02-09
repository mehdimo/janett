namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	using NUnit.Framework;

	[TestFixture]
	public class AbstractClassTransformerTest : AbstractClassTransformer
	{
		[SetUp]
		public void SetUp()
		{
			CodeBase.Types.Clear();
		}

		[Test]
		public void AbstractMethod()
		{
			string program = TestUtil.GetInput();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			TypeDeclaration type = ((NamespaceDeclaration) cu.Children[0]).Children[0] as TypeDeclaration;
			TypeDeclaration type2 = ((NamespaceDeclaration) cu.Children[0]).Children[1] as TypeDeclaration;

			CodeBase.Types.Add("Test.A", type);
			CodeBase.Types.Add("Test.B", type2);

			VisitCompilationUnit(cu, null);

			Assert.AreEqual(1, type.Children.Count);

			Assert.IsTrue(type.Children[0] is MethodDeclaration);
			Assert.AreEqual("M2", ((MethodDeclaration) type.Children[0]).Name);
		}

		[Test]
		public void MultiLevel()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			TypeDeclaration type1 = ((NamespaceDeclaration) cu.Children[0]).Children[0] as TypeDeclaration;
			TypeDeclaration type2 = ((NamespaceDeclaration) cu.Children[0]).Children[1] as TypeDeclaration;
			TypeDeclaration type3 = ((NamespaceDeclaration) cu.Children[0]).Children[2] as TypeDeclaration;

			CodeBase.Types.Add("Test.A", type1);
			CodeBase.Types.Add("Test.B", type2);
			CodeBase.Types.Add("Test.IC", type3);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void TestCaseDerived()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Polymorphism()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}