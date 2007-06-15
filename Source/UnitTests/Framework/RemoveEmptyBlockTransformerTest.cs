namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class RemoveEmptyBlockTransformerTest : RemoveEmptyBlocksTransformer
	{
		[Test]
		public void BlankIfStatement()
		{
			string program = TestUtil.StatementParse("if(true){}");
			string expected = TestUtil.CSharpTypeMemberParse("public void TestMethod(){}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void BlankIfWithElseStatement()
		{
			string program = TestUtil.StatementParse("if(true){} else {int x = 0;}");
			string expected = TestUtil.CSharpStatementParse("if(true){} else {int x = 0;}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void BlankIfWithElseIfStatement()
		{
			string program = TestUtil.StatementParse("if(true){} else if(false){int x = 0;}");
			string expected = TestUtil.CSharpStatementParse("if(true){} else if(false) {int x = 0;}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void BlankElseStatement()
		{
			string program = TestUtil.StatementParse("if(true){int x = 0;} else {}");
			string expected = TestUtil.CSharpStatementParse("if(true){int x = 0;}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void BlankIfAndElseIfAndElse()
		{
			string program = TestUtil.StatementParse("if(true){} else if (a){} else {}");
			string expected = TestUtil.CSharpTypeMemberParse("public void TestMethod(){}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void BlankIfElse()
		{
			string program = TestUtil.StatementParse("if(a){int x = 0;} else if(b){} else if(c){int y = 0;} else if(d){} else {}");
			string expected = TestUtil.CSharpStatementParse("if(a){int x = 0;} else if(c){int y = 0;}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void BlankTry()
		{
			string program = TestUtil.StatementParse("try{} catch(Exception ex){int x = 0;}");
			string expected = TestUtil.CSharpTypeMemberParse("public void TestMethod(){}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void While()
		{
			string program = TestUtil.StatementParse("while(true){}");
			string expected = TestUtil.CSharpTypeMemberParse("public void TestMethod(){}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}