namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class AccessibilityTransformerTest : AccessibilityTransformer
	{
		[Test]
		public void Field()
		{
			string program = TestUtil.TypeMemberParse("String name;");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			FieldDeclaration fd = (FieldDeclaration) ty.Children[0];

			Modifiers expectedModifier = Modifiers.Internal | Modifiers.Protected;
			Assert.IsNotNull(fd);
			Assert.AreEqual(expectedModifier, fd.Modifier);
		}

		[Test]
		public void MethodWithOtherModifiers()
		{
			string program = TestUtil.TypeMemberParse("static final string GetName(){}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];

			Modifiers expectedModifier = Modifiers.Static | Modifiers.Final | Modifiers.Internal | Modifiers.Protected;
			Assert.IsNotNull(md);
			Assert.AreEqual(expectedModifier, md.Modifier);
		}

		[Test]
		public void StaticConstructor()
		{
			string program = TestUtil.TypeMemberParse("static Test(){}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			ConstructorDeclaration md = (ConstructorDeclaration) ty.Children[0];

			Assert.IsNotNull(md);
			Assert.AreEqual(Modifiers.Static, md.Modifier);
		}

		[Test]
		public void NonModifierClass()
		{
			string program = TestUtil.PackageMemberParse("class A {}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];

			Assert.IsNotNull(ty);
			Assert.AreEqual(Modifiers.Public, ty.Modifier);
		}

		[Test]
		public void ProtectedInnerClass()
		{
			string program = TestUtil.PackageMemberParse("public class A {protected class InnerA {}}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ty1.Children[0];

			Assert.IsNotNull(ty2);
			Assert.AreEqual(Modifiers.Protected | Modifiers.Internal, ty2.Modifier);
		}

		[Test]
		public void NonModifierConstructor()
		{
			string program = TestUtil.TypeMemberParse("Test(){}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			ConstructorDeclaration md = (ConstructorDeclaration) ty.Children[0];

			Assert.IsNotNull(md);
			Assert.AreEqual(Modifiers.Internal | Modifiers.Protected, md.Modifier);
		}

		[Test]
		public void HasAccessibility()
		{
			string program = TestUtil.TypeMemberParse("private static ConstantSize DIALOG_MARGIN_X = Sizes.DLUX7;");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			FieldDeclaration md = (FieldDeclaration) ty.Children[0];

			Modifiers expectedModifier = Modifiers.Private | Modifiers.Static;
			Assert.IsNotNull(md);
			Assert.AreEqual(expectedModifier, md.Modifier);

			program = TestUtil.TypeMemberParse("public int ID;");
			cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			string expected = TestUtil.CSharpTypeMemberParse("public int ID;");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Protected()
		{
			string program = TestUtil.TypeMemberParse("protected static float CalculateArea(float radius){return 3.14 * radus * radius;}");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];

			Modifiers expectedModifier = Modifiers.Protected | Modifiers.Internal | Modifiers.Static;
			Assert.AreEqual(expectedModifier, md.Modifier);
		}
	}
}