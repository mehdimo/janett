namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class InnerClassTransformerTest : InnerClassTransformer
	{
		[Test]
		public void InnerMemberAccessibility()
		{
			string program = TestUtil.TypeMemberParse(@"public class InnerA {private void setName(){}}");
			string expected = TestUtil.CSharpTypeMemberParse("public class InnerA {internal void setName(){}}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InnerClassCallsNonStaticMethod()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InnerClassWithConstructor()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void FieldUsegeAtInnerClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InnerFieldInArguments()
		{
			string program = TestUtil.TypeMemberParse(@"
										private int _in;
										public class BinTest
										{
											public BinTest(int _in){ int _out = _in;}
										}");

			string expected = TestUtil.CSharpTypeMemberParse(@"
										private int _in;
										public class BinTest
										{
											public BinTest(int _in){ int _out = _in;}
										}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InheritedMethodUsingAtInnerClass()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty2 = (TypeDeclaration) ty1.Children[0];
			TypeDeclaration ty3 = (TypeDeclaration) ns.Children[1];
			TypeDeclaration ty4 = (TypeDeclaration) ns.Children[2];

			CodeBase.Types.Add("Test.A", ty1);
			CodeBase.Types.Add("Test.A.Ab", ty2);
			CodeBase.Types.Add("Test.B", ty3);
			CodeBase.Types.Add("Test.Ac", ty4);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}