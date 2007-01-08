namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class FieldInitializerTransformerTest : FieldInitializerTransformer
	{
		[Test]
		public void WithArgumentlessConstructor()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void WithoutArgumentlessConstructor()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void WithoutConstructor()
		{
			string program = TestUtil.TypeMemberParse(@"private int init = initialize();
			                                          private int initialize() {return 0;}");

			string expected = TestUtil.CSharpTypeMemberParse(@"private int init;
			                                           private int initialize() {return 0;}
			                                           public Test(){init = initialize();}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void StaticField()
		{
			string program = TestUtil.TypeMemberParse(
				@"public static int init = initialize();
				private static int initialize() {return 0;}");

			string expected = TestUtil.CSharpTypeMemberParse(
				@"public static int init = initialize();
				private static int initialize() {return 0;}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ArrayInitializing()
		{
			string program = TestUtil.TypeMemberParse(@"public int[] _array = new int[_arraySize];
			                                          public int[] arr = new int[3];");

			string expected = TestUtil.CSharpTypeMemberParse(@"public int[] _array;
			                                           public int[] arr = new int[3];
			                                           public Test(){_array = new int[_arraySize];}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}