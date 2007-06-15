namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class NullableValueTypeTransformerTest : NullableValueTypeTransformer
	{
		[TestFixtureSetUp]
		public void SetUp()
		{
			Mode = "DotNet";
		}

		[Test]
		public void Int()
		{
			string program = TestUtil.StatementParse("int a; bool b = (a == null)");
			string expected = TestUtil.CSharpStatementParse("int a; bool b = (a == System.Int32.MinValue)");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void IntWithRank()
		{
			string program = TestUtil.StatementParse("int[] a; bool b = (a == null)");
			string expected = TestUtil.CSharpStatementParse("int[] a; bool b = (a == null)");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void DateTime()
		{
			string program = TestUtil.StatementParse("java.util.Date date; bool b =(date == null);");
			string expected = TestUtil.CSharpStatementParse("java.util.Date date; bool b = (date == System.DateTime.MinValue);");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ConditionalExpression()
		{
			string program = TestUtil.StatementParse("return (false) ? null : new java.util.Date();");
			string expected = TestUtil.CSharpStatementParse("return (false) ? System.DateTime.MinValue : new java.util.Date();");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ConstructorInitializerArgument()
		{
			string program = TestUtil.TypeMemberParse("public class MyDate{public MyDate(java.util.Date date){} public MyDate(long d){} public MyDate(){this(null);}}");
			string expected = TestUtil.CSharpTypeMemberParse("public class MyDate{public MyDate(java.util.Date date){} public MyDate(long d){} public MyDate():this(System.DateTime.MinValue){}}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}