namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class IKVMDifferencesTransformerTest : IKVMDifferencesTransformer
	{
		[Test]
		public void ComparatorDerivedClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			Mode = "IKVM";
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Cloneable_Clone()
		{
			string program = "package Test; public class A implements Cloneable { protected Object clone(){return null;} }";
			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			string expected = "namespace Test {public class A : Cloneable { public Object clone(){return null;} }}";
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}