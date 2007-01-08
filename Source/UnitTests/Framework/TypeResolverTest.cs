namespace Janett.Framework
{
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class TypeResolverTest : TypeResolver
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			CodeBase = new CodeBase(SupportedLanguage.Java);
			AstUtil = new AstUtil();

			CodeBase.Types.LibrariesFolder = @"../../../Translator/Libraries";
			CodeBase.Mappings = new Mappings();
			CodeBase.Mappings.Add("String", new TypeMapping("string"));
		}

		[Test]
		public void Imported()
		{
			string program = TestUtil.PackageMemberParse(@"import java.util.List; import java.util.ArrayList; public class A extends ArrayList {}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration nsChild = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration tyChild = (TypeDeclaration) nsChild.Children[2];
			TypeReference tyRef = (TypeReference) tyChild.BaseTypes[0];
			string fullName = GetFullName(tyRef);
			Assert.IsNotNull(fullName);
			Assert.AreEqual("java.util.ArrayList", fullName);
		}

		[Test]
		public void BaseType()
		{
			string program = TestUtil.PackageMemberParse(@"public class A extends StringBuffer{}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration nsChild = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration tyChild = (TypeDeclaration) nsChild.Children[0];
			TypeReference tyRef = (TypeReference) tyChild.BaseTypes[0];
			string fullName = GetFullName(tyRef);
			Assert.IsNotNull(fullName);
			Assert.AreEqual("java.lang.StringBuffer", fullName);
		}

		[Test]
		public void JavaLang()
		{
			string program = TestUtil.PackageMemberParse(@"import java.util.List; import java.io.*; public class A extends StringBuffer{}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration nsChild = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration tyChild = (TypeDeclaration) nsChild.Children[2];
			TypeReference tyRef = (TypeReference) tyChild.BaseTypes[0];
			string fullName = GetFullName(tyRef);
			Assert.IsNotNull(fullName);
			Assert.AreEqual("java.lang.StringBuffer", fullName);
		}

		[Test]
		public void ProjectTypes()
		{
			string program = @"
								package Janett.Framework;
								public class Transformer
								{
									public class InnerClass
									{
										
									}
								}";
			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ty1.Children[0];

			string ty1Name = GetFullName(ty1);
			string ty2Name = GetFullName(ty2);

			Assert.IsNotNull(ty1Name);
			Assert.IsNotNull(ty2Name);

			Assert.AreEqual("Janett.Framework.Transformer", ty1Name);
			Assert.AreEqual("Janett.Framework.Transformer.InnerClass", ty2Name);
		}
	}
}