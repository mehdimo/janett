namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class AccessorRefactoringTest : AccessorRefactoring
	{
		[SetUp]
		public void SetUp()
		{
			CodeBase.Types.Clear();
			CodeBase.Inheritors.Clear();
			CodeBase.References.Clear();
		}

		[Test]
		public void Setter()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Assert.AreEqual(1, CodeBase.References.Count);
		}

		[Test]
		public void NonSetter()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Assert.AreEqual(0, CodeBase.References.Count);
		}

		[Test]
		public void Getter()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Assert.AreEqual(1, CodeBase.References.Count);
		}

		[Test]
		public void NonGetter()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Assert.AreEqual(0, CodeBase.References.Count);
		}

		[Test]
		public void SetterGetter()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Assert.AreEqual(2, CodeBase.References.Count);
		}

		[Test]
		public void MultiSetterGetter()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Assert.AreEqual(4, CodeBase.References.Count);
		}

		[Test]
		public void InterfaceSetterGetter()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[1];
			TypeDeclaration ty3 = (TypeDeclaration) ns.Children[2];

			CodeBase.Types.Add("Test.IShape", ty1);
			CodeBase.Types.Add("Test.Shape", ty2);
			CodeBase.Types.Add("Test.Rectangle", ty3);

			InheritorsVisitor inheritorsVisitor = new InheritorsVisitor();
			inheritorsVisitor.CodeBase = CodeBase;
			inheritorsVisitor.VisitCompilationUnit(cu, null);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Assert.AreEqual(6, CodeBase.References.Count);
		}

		[Test]
		public void AccessorsWithoutField()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
			Assert.AreEqual(0, CodeBase.References.Count);
		}

		[Test]
		public void Siblings()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);

			InheritorsVisitor inheritorsVisitor = new InheritorsVisitor();
			inheritorsVisitor.CodeBase = CodeBase;
			inheritorsVisitor.VisitCompilationUnit(cu, null);

			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InheritedTypesFromTypeHasAccessor()
		{
			string program = TestUtil.GetInput();
			CompilationUnit cu = TestUtil.ParseProgram(program);
			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);
			InheritorsVisitor inheritorsVisitor = new InheritorsVisitor();
			inheritorsVisitor.CodeBase = CodeBase;
			inheritorsVisitor.VisitCompilationUnit(cu, null);

			VisitCompilationUnit(cu, null);

			Assert.AreEqual(4, CodeBase.References.Count);
			Assert.IsTrue(CodeBase.References.Contains("Test.IGraph.getDegree"));
			Assert.IsTrue(CodeBase.References.Contains("Test.ITree.getDegree"));
			Assert.IsTrue(CodeBase.References.Contains("Test.GenericTree.getDegree"));
			Assert.IsTrue(CodeBase.References.Contains("Test.BinaryTree.getDegree"));
		}

		[Test]
		public void AccessorWithSameNameAsInnerClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();
			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AccessorWithSameNameAsClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();
			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void SetterAndGetterWithDifferentAccessibility()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();
			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void NotDeclaredAccessor()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();
			CompilationUnit cu = TestUtil.ParseProgram(program);

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;

			InheritorsVisitor inheritorsVisitor = new InheritorsVisitor();
			inheritorsVisitor.CodeBase = CodeBase;

			typesVisitor.VisitCompilationUnit(cu, null);
			inheritorsVisitor.VisitCompilationUnit(cu, null);

			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}