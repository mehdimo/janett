namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class StaticPrimitiveTypeFieldsTransformerTest : StaticPrimitiveTypeFieldsTransformer
	{
		[Test]
		public void Test()
		{
			StaticPrimitiveTypeFieldsTransformer spt = new StaticPrimitiveTypeFieldsTransformer();

			string program = TestUtil.TypeMemberParse(@"static int i = 10;
														static String s = GetString();
														static String[] sa;
														static ArrayList list;");

			string expected = TestUtil.CSharpTypeMemberParse(@"const int i = 10;
																static String s = GetString();
																static String[] sa;
																static ArrayList list;");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			spt.VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}