namespace Janett.Translator
{
	using Framework;

	using ICSharpCode.NRefactory.Ast;

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
			string interfaceFields = TestUtil.PackageMemberParse("public class IClassifier_Fields{public object Type;}");
			CompilationUnit cuFields = TestUtil.ParseProgram(interfaceFields);
			NamespaceDeclaration ns = (NamespaceDeclaration) cuFields.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];

			CodeBase.Types.Add("Test.IClassifier_Fields", ty);
			string program = TestUtil.PackageMemberParse("import Test.IClassifier; public class Test { public void Main(){IClassifier.Type = null;} }");
			string expected = TestUtil.NamespaceMemberParse("using Test.IClassifier; public class Test {public void Main(){Test.IClassifier_Fields.Type = null;} }");
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

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);

			CodeBase.References.Add("Test.IT", "Test.IT_Fields");

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void CallingInheritedInterfaceFromInterfaceFields()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			string program2 = @"
					package Atom;
					public interface IMaterial
					{
					}
					public class IMaterial_Fields
					{
						public static int Atomic_Number;
					}
					public interface ISolid extends IMaterial
					{
					}";
			CompilationUnit cv = TestUtil.ParseProgram(program2);

			CompilationUnit cu = TestUtil.ParseProgram(program);

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);
			typesVisitor.VisitCompilationUnit(cv, null);

			ImportTransformer im = new ImportTransformer();
			im.VisitCompilationUnit(cu, null);

			CodeBase.References.Add("Atom.IMaterial", "Atom.IMaterial_Fields");

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InheritedFromMultiInterfaceWithFields()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);

			CodeBase.References.Add("Test.IName", "Test.IName_Fields");
			CodeBase.References.Add("Test.IFamily", "Test.IFamily_Fields");
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InnerClassUsesEnclosingInheritedInterfaceField()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);

			CodeBase.References.Add("Test.IDocument", "Test.IDocument_Fields");
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InterfaceFieldsUseParentInterfaceFields()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);

			CodeBase.References.Add("Test.Constants", "Test.Constants_Fields");
			CodeBase.References.Add("Test.SystemConstants", "Test.SystemConstants_Fields");
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ProjectAndLibraryTypeWithSameName()
		{
			string program = TestUtil.PackageMemberParse(@"
								import java.util.Calendar; 
								public class A { int h = Calendar.HOUR; } 
								public class Calendar_Fields {public static int Month = 0;}");
			string expected = TestUtil.NamespaceMemberParse(@"
								using java.util.Calendar; 
								public class A { int h = Calendar.HOUR; } 
								public class Calendar_Fields {public static int Month = 0;}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			CodeBase.References.Add("Test.Calendar", "Test.Calendar_Fields");

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);

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

			CodeBase.References.Add("Cons:Test.A$InnerA", null);
			CodeBase.Types.Add("Test.A", ty);
			CodeBase.Types.Add("Test.A$InnerA", tyin);

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

			CodeBase.References.Add("Cons:Test.A$InnerA", null);
			CodeBase.Types.Add("Test.A", ty);
			CodeBase.Types.Add("Test.A$InnerA", tyin);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}