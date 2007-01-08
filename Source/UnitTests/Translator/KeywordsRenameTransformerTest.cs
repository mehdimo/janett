namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class KeywordsRenameTransformerTest : KeywordsRenameTransformer
	{
		[Test]
		public void Test()
		{
			string program = @"
								package Test; 
								public class Test 
								{ 
									public void Method(String string)
									{
										int in;
										int out; 
										out = in.toInt();
									} 
								}";
			string expected = TestUtil.CSharpTypeMemberParse(@"
														public void Method(String _string)
														{
															int _in;
															int _out;
															_out = _in.toInt();
														}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}