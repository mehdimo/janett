namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class MemberMapperTest_IKVM : MemberMapper
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			CodeBase.Types.LibrariesFolder = @"../../../Translator/Libraries";
			CodeBase.Mappings = new Mappings(@"../../../Translator/Mappings/IKVM");
			Mode = "IKVM";
		}

		[Test]
		public void ThisTargetGetClass()
		{
			string program = TestUtil.PackageMemberParse("public class A { public void Method() {this.getClass(); }}");
			string expected = TestUtil.NamespaceMemberParse("public class A { public void Method() {java.lang.Object.instancehelper_getClass(this); }}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration type = (TypeDeclaration) ns.Children[0];
			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", type);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ThisTargetGetClassInherited()
		{
			string program = TestUtil.PackageMemberParse("public class A extends TestCase{ public A() {this.getClass(); }}");
			string expected = TestUtil.NamespaceMemberParse("public class A : TestCase{ public A() {java.lang.Object.instancehelper_getClass(this); }}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration type = (TypeDeclaration) ns.Children[0];
			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", type);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void FieldTwoLayerInvocation()
		{
			string program = TestUtil.StatementParse("Field f; f.getName().indexOf('$')");
			string expected = TestUtil.StatementParse("Field f; java.lang.String.instancehelper_indexOf(f.getName(), '$')");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			CompilationUnit cv = TestUtil.ParseProgram(expected);
			UsingDeclaration usiDec = new UsingDeclaration("Field", new TypeReference("java.lang.reflect.Field"));
			((NamespaceDeclaration) cu.Children[0]).Children.Insert(0, usiDec);
			((NamespaceDeclaration) cv.Children[0]).Children.Insert(0, usiDec);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(TestUtil.GenerateCode(cv), TestUtil.GenerateCode(cu));
		}

		[Test]
		public void MethiodInDefaultType()
		{
			string program = TestUtil.StatementParse("getClass();");
			string expected = TestUtil.CSharpStatementParse("java.lang.Object.instancehelper_getClass(this);");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration type = (TypeDeclaration) ns.Children[0];

			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.Test", type);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}