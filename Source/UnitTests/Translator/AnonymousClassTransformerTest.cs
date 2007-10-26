namespace Janett.Translator
{
	using Framework;

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

		[Test]
		public void NestedAnonymousClass()
		{
			string program = TestUtil.PackageMemberParse(@"
								public class NestedAnon
								{
									public void exec(){}
									public void firstLevel()
									{
										String arg = null;
										Weight w = new Weight()
										{
											public void secondLevel()
											{
												exec();
												Score s = new Score(arg)
												{
													public void thirdLevel()
													{
													}
												};
											}
										};
									}
								}
								public class Score
								{
									public Score(String arg){}
								}");
			string expected = TestUtil.NamespaceMemberParse(@"
								public class NestedAnon
								{
									public void exec(){}
									public void firstLevel()
									{
										String arg = null;
										Weight w = new AnonymousClassWeight1(this, arg);
									}
									private class AnonymousClassWeight1 : Weight
									{
										public AnonymousClassWeight1(NestedAnon enclosingInstance, String arg)
										{
											this.enclosingInstance = enclosingInstance;
											this.arg = arg;
										}
										public void secondLevel()
										{
											enclosingInstance.exec();
											Score s = new AnonymousClassScore2(arg, this);
										}
										private NestedAnon enclosingInstance;
										private String arg;
										public NestedAnon Enclosing_Instance {
											get { return enclosingInstance; }
										}

										private class AnonymousClassScore2 : Score
										{
											public AnonymousClassScore2(String arg, AnonymousClassWeight1 enclosingInstance) : base(arg)
											{
												this.enclosingInstance = enclosingInstance;
											}
											public void thirdLevel()
											{
											}
											private AnonymousClassWeight1 enclosingInstance;
											public AnonymousClassWeight1 Enclosing_Instance {
												get { return enclosingInstance; }
											}
										}
									}
								}
								public class Score
								{
									public Score(String arg){}
								}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = this.CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}