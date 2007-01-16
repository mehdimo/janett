namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class UsingVisitorTest : UsingVisitor
	{
		[TearDown]
		public void TearDown()
		{
			Usings.Clear();
		}

		[Test]
		public void Using()
		{
			string program = "package Test; import junit.framework.TestCase; import NUnit.Framework;";
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			Assert.AreEqual(2, Usings.Count);
		}

		[Test]
		public void Attribute()
		{
			string program = "package Test; [NUnit.Framework.TestFixtureAttribute] public class A {} ";
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			Assert.AreEqual(1, Usings.Count);
		}

		[Test]
		public void TypeReference()
		{
			string program = TestUtil.StatementParse("NUnit.Framework.Assert assert;");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			Assert.AreEqual(1, Usings.Count);
		}
	}
}