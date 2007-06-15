namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	using NUnit.Framework;

	[TestFixture]
	public class SameProjectAndExternalTypeNameTransformerTest : SameProjectAndExternalTypeNameTransformer
	{
		[Test]
		public void Test()
		{
			string projectType = TestUtil.PackageMemberParse("public interface Calendar { void getTime(); }");
			string program = TestUtil.PackageMemberParse(@"import java.util.Calendar; 
															public class MyCalendar 
															{ 
																public Calendar getCalendar()
																{
																	return Calendar.getInstance();
																}
															}");
			string expected = TestUtil.NamespaceMemberParse(@"public class MyCalendar 
																{ 
																	public java.util.Calendar getCalendar()
																	{
																		return java.util.Calendar.getInstance();
																	}
																}");

			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;

			CompilationUnit cu1 = TestUtil.ParseProgram(projectType);
			typesVisitor.VisitCompilationUnit(cu1, null);

			CompilationUnit cu2 = TestUtil.ParseProgram(program);
			typesVisitor.VisitCompilationUnit(cu2, null);

			VisitCompilationUnit(cu2, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu2));
		}
	}
}