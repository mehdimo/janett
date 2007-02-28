namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	using NUnit.Framework;

	[TestFixture]
	public class AccessibilityPropagatorTransformerTest : AccessibilityPropagatorTransformer
	{
		[Test]
		public void PublicMethodInheritedProtected()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);

			CodeBase.Inheritors.Add("Test.Shape", "Test.SimpleShape");
			CodeBase.Inheritors.Add("Test.Shape", "Test.PolygonShape");
			CodeBase.Inheritors.Add("Test.SimpleShape", "Test.Rectangle");
			CodeBase.Inheritors.Add("Test.PolygonShape", "Test.ComplexShape");

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}