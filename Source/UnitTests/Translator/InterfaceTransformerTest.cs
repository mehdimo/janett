namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class InterfaceTransformerTest : ProjectInterfaceTransformer
	{
		[SetUp]
		public void SetUp()
		{
			CodeBase.Types.Clear();
			CodeBase.References.Clear();
		}

		[Test]
		public void InterfaceFields()
		{
			string program = TestUtil.GetInput();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration iType = (TypeDeclaration) ns.Children[0];
			TypeDeclaration iType_Fields = (TypeDeclaration) ns.Children[1];

			Assert.AreEqual(ClassType.Interface, iType.Type);
			Assert.AreEqual(2, iType.Children.Count);
			Assert.AreEqual(Modifiers.None, ((MethodDeclaration) iType.Children[0]).Modifier);

			Assert.IsNotNull(iType_Fields);
			Assert.AreEqual("ITest_Fields", iType_Fields.Name);
			Assert.AreEqual(3, iType_Fields.Children.Count);
			Assert.AreEqual("int", ((FieldDeclaration) iType_Fields.Children[0]).TypeReference.Type);
		}

		[Test]
		public void InterfaceFieldsWithModifiers()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InterfaceInnerClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void TwoInterfaceFieldClass()
		{
			string program1 = TestUtil.PackageMemberParse("public interface IQuery {public String Name;}");
			string program2 = "package Package2; public interface IQuery {public int Id;}";

			CompilationUnit cu1 = TestUtil.ParseProgram(program1);
			CompilationUnit cu2 = TestUtil.ParseProgram(program2);

			VisitCompilationUnit(cu1, null);
			VisitCompilationUnit(cu2, null);

			Assert.AreEqual(2, CodeBase.References.Count);
			Assert.IsTrue(CodeBase.References.Contains("Test.IQuery"));
			Assert.AreEqual("Test.IQuery_Fields", CodeBase.References["Test.IQuery"]);
			Assert.IsTrue(CodeBase.References.Contains("Package2.IQuery"));
			Assert.AreEqual("Package2.IQuery_Fields", CodeBase.References["Package2.IQuery"]);
		}
	}
}