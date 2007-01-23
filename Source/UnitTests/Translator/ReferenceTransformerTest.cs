namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	using NUnit.Framework;

	[TestFixture]
	public class ReferenceTransformerTest : ReferenceTransformer
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			CodeBase.Mappings = new Mappings();
		}

		[TearDown]
		public void TearDown()
		{
			CodeBase.Types.Clear();
			CodeBase.References.Clear();
		}

		[Test]
		public void InterfaceFieldsClass()
		{
			CodeBase.References.Add("Test.IClassifier", "Test.IClassifier_Fields");
			string program = TestUtil.TypeMemberParse("public void Main(){IClassifier.Type = null;}");
			string expected = TestUtil.CSharpTypeMemberParse("public void Main(){Test.IClassifier_Fields.Type = null;}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InterfaceFieldsUsageViaInheritance()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[1];
			TypeDeclaration ty3 = (TypeDeclaration) ns.Children[2];

			CodeBase.Types.Add("Test.A", ty1);
			CodeBase.Types.Add("Test.IT", ty2);
			CodeBase.Types.Add("Test.IT_Fields", ty3);

			CodeBase.References.Add("Test.IT", "Test.IT_Fields");

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InterfaceInnerType()
		{
			string program = "package Test; public class A extends Interface.InterfaceInnerClass {}";
			string expected = "namespace Test {public class A : InterfaceInnerClass {} }";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			CodeBase.References.Add("Interface.InterfaceInnerClass", "InterfaceInnerClass");

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void CallingInnerClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			TypeDeclaration tyin = (TypeDeclaration) ty.Children[1];

			CodeBase.References.Add("Cons:Test.A.InnerA", null);
			CodeBase.Types.Add("Test.A", ty);
			CodeBase.Types.Add("Test.A.InnerA", tyin);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void CallingInnerClassWithConstructor()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			TypeDeclaration tyin = (TypeDeclaration) ty.Children[1];

			CodeBase.References.Add("Cons:Test.A.InnerA", null);
			CodeBase.Types.Add("Test.A", ty);
			CodeBase.Types.Add("Test.A.InnerA", tyin);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}