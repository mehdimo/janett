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
	}
}