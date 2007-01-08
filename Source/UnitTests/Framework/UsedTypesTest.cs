namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class UsedTypesTest : TypeMapper
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			CodeBase.Mappings = new Mappings(@"../../../Translator/Mappings/DotNet");
			CodeBase.Types.LibrariesFolder = @"../../../Translator/Libraries";
		}

		[Test]
		public void RemoveJavaUsungs()
		{
			CodeBase.Types.LibrariesFolder = @"../../../Translator/Libraries";
			string program = TestUtil.PackageMemberParse("import java.util.List; public class A {List list;}");
			string expected = TestUtil.NamespaceMemberParse("public class A {System.Collections.IList list;}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			Mode = "DotNet";
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Used()
		{
			string program = TestUtil.PackageMemberParse("public class A {object obj = new ToStringBuilder();}");
			string expected = TestUtil.NamespaceMemberParse("using ToStringBuilder = NClassifier.Util.ToStringBuilder; public class A {object obj = new ToStringBuilder();}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];

			UsingDeclaration us = new UsingDeclaration("ToStringBuilder", AstUtil.GetTypeReference("NClassifier.Util.ToStringBuilder", ns));
			ns.Children.Insert(0, us);
			us.Parent = ns;

			Mode = "DotNet";
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Used2()
		{
			string program = "package Test; import java.util; public class A {List list;}";
			string expected = "namespace Test {public class A {System.Collections.IList list;} }";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			Mode = "DotNet";
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Helpers()
		{
			string program = "package Test; public class A {public void Method() {Helpers.StringHelper.replaceAll();} }";
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			Assert.AreEqual(3, UsedTypes.Count);
			Assert.AreEqual("Helpers.StringHelper", UsedTypes[2]);
		}

		[Test]
		public void RemoveCurrentNamespaceUsings()
		{
			string program = @"package Janett.Translator;
								public class Translation
								{
								}";

			string expected = @"namespace Janett.Translator
								{
									public class Translation
									{
									}
								}";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];

			UsingDeclaration us1 = new UsingDeclaration("Refactoring", AstUtil.GetTypeReference("Janett.Translator.Refactoring", ns));
			UsingDeclaration us2 = new UsingDeclaration("Transformation", AstUtil.GetTypeReference("Janett.Translator.Transformation", ns));

			ns.Children.Insert(0, us2);
			ns.Children.Insert(0, us1);
			us1.Parent = ns;
			us2.Parent = ns;

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}