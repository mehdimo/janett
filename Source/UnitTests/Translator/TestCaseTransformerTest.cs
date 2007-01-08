namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class TestCaseTransformerTest : TestCaseTransformer
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Mode = "DotNet";
		}

		[Test]
		public void AbstractTestCaseDerivedClass()
		{
			string program = TestUtil.PackageMemberParse("public class AbstractA extends TestCase{}");
			string expected = TestUtil.NamespaceMemberParse("public class AbstractA : TestCase{}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AddTestFixtureAttribute()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AddTestAttribute()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void RemoveTestMainMethod()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void RemoveParameteredTestConstructor()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AddSetUpAttribute()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			Mode = "DotNet";
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AddTearDownAttribute()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			Mode = "DotNet";
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}