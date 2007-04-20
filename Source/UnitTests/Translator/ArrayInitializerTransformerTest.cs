namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class ArrayInitializerTransformerTest : ArrayInitializerTransformer
	{
		[Test]
		public void ArrayCreationWithName()
		{
			string program = TestUtil.StatementParse(@"
												int[][] newRowGroups = new int[][]{{5, 6}}; int x = 0;");
			string expected = TestUtil.CSharpStatementParse(@"
												int[][] newRowGroups = new int[1][];
												newRowGroups[0] = new int[]{5, 6};
												int x = 0;");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ArrayCreationWithoutName()
		{
			string program = TestUtil.StatementParse("Method(new int[][]{{5, 6}});");
			string expected = TestUtil.CSharpStatementParse(@"
															int[][] ints = new int[1][]; 
															ints[0] = new int[] {5, 6};
															Method(ints);");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ArrayCreationWithNameAndMultiInit()
		{
			string program = TestUtil.StatementParse(@"
												int[][] newRowGroups = new int[][]{{5, 6},{7, 8}}; int x = 0;");
			string expected = TestUtil.CSharpStatementParse(@"
												int[][] newRowGroups = new int[2][];
												newRowGroups[0] = new int[]{5, 6};
												newRowGroups[1] = new int[]{7, 8};
												int x = 0;");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ArrayCreationWithoutNameAndMultiInit()
		{
			string program = TestUtil.StatementParse("Method(new int[][]{{5, 6},{7, 8}});");
			string expected = TestUtil.CSharpStatementParse(@"
															int[][] ints = new int[2][]; 
															ints[0] = new int[] {5, 6};
															ints[1] = new int[] {7, 8};
															Method(ints);");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}
