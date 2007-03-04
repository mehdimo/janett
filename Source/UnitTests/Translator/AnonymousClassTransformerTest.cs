namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class AnonymousClassTransformerTest : AnonymousClassTransformer
	{
		[TearDown]
		public void TearDown()
		{
			CodeBase.Types.Clear();
		}

		[Test]
		public void SingleAnonymousClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit com = TestUtil.ParseProgram(program);
			VisitCompilationUnit(com, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(com));
		}

		[Test]
		public void MultiAnonymousClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit com = TestUtil.ParseProgram(program);

			VisitCompilationUnit(com, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(com));
		}

		[Test]
		public void AnonymousClassInheritsProjectClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void CallAnonymousClassWithArguments()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty3 = (TypeDeclaration) ns.Children[1];

			CodeBase.Types.Add("BaseComplete.Locker", ty1);
			CodeBase.Types.Add("BaseComplete.Lock", ty3);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AnonymousClassWithAnonymousConstructor()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void CallAnonymousClassInStaticMethodDeclaration()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[1];

			CodeBase.Types.Add("Test.A", ty1);
			CodeBase.Types.Add("Test.B", ty2);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void CallAnonymousClassInInterfaceField()
		{
			string program = @"package Test;
									public interface IT
									{
										public static B Feild = new B(){};
									}";
			string expected = TestUtil.NamespaceMemberParse(@"
									public interface IT
									{
										public static B Feild = new AnonymousClassB1();
										public class AnonymousClassB1 : B
										{
											public AnonymousClassB1() {}
										}
									}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AnonumousClassHasField()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AnonymousClassWithBaseConstructor()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration tyA = (TypeDeclaration) ns.Children[0];
			TypeDeclaration tyB = (TypeDeclaration) ns.Children[1];
			TypeDeclaration tyC = (TypeDeclaration) ns.Children[2];
			TypeDeclaration tyD = (TypeDeclaration) ns.Children[3];
			TypeDeclaration tyF = (TypeDeclaration) tyA.Children[1];

			CodeBase.Types.Add("Test.A", tyA);
			CodeBase.Types.Add("Test.B", tyB);
			CodeBase.Types.Add("Test.C", tyC);
			CodeBase.Types.Add("Test.D", tyD);
			CodeBase.Types.Add("Test.A.F", tyF);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AnonymousClassUseInheritedEnclosingMethod()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}