namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class MethodRelatedTransformerTest : MethodRelatedTransformer
	{
		[Test]
		public void Equals()
		{
			string program = TestUtil.TypeMemberParse("public string Sentence(string title , string text);");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration pgMethod = (MethodDeclaration) ty.Children[0];
			ParameterDeclarationExpression p1 = new ParameterDeclarationExpression(new TypeReference("string"), "title");
			p1.TypeReference.RankSpecifier = new int[] {};
			ParameterDeclarationExpression p2 = new ParameterDeclarationExpression(new TypeReference("string"), "text");
			p2.TypeReference.RankSpecifier = new int[] {};
			ArrayList argList = new ArrayList();
			argList.Add(p1);
			argList.Add(p2);
			MethodDeclaration exMethod = new MethodDeclaration("Sentence",
			                                                   Modifiers.Public,
			                                                   new TypeReference("string"),
			                                                   argList, null);

			Assert.IsTrue(Equals(exMethod, pgMethod));

			string program2 = TestUtil.TypeMemberParse("public string Sentence(string title , string[] text);");
			cu = TestUtil.ParseProgram(program2);

			ns = (NamespaceDeclaration) cu.Children[0];
			ty = (TypeDeclaration) ns.Children[0];
			pgMethod = (MethodDeclaration) ty.Children[0];
			Assert.IsFalse(Equals(exMethod, pgMethod));
		}

		[Test]
		public void IsDerivedFrom()
		{
			string program = TestUtil.GetInput();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration TC = (TypeDeclaration) ns.Children[0];
			TypeDeclaration TB = (TypeDeclaration) ns.Children[1];
			TypeDeclaration TA = (TypeDeclaration) ns.Children[2];
			CodeBase.Types.Add("Test.C", TC);
			CodeBase.Types.Add("Test.B", TB);
			CodeBase.Types.Add("Test.A", TA);

			Assert.IsTrue(IsDerivedFrom(TC, "Test.B"));
			Assert.IsTrue(IsDerivedFrom(TC, "Test.A"));
			Assert.IsTrue(IsDerivedFrom(TB, "Test.A"));

			Assert.IsFalse(IsDerivedFrom(TB, "Test.D"));
		}

		[Test]
		public void Contains()
		{
			string program = TestUtil.GetInput();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			IList methods = AstUtil.GetChildrenWithType(ty, typeof(MethodDeclaration));
			ParameterDeclarationExpression pm = new ParameterDeclarationExpression(
				new TypeReference("int"), "from");
			pm.TypeReference.RankSpecifier = new int[] {};
			ArrayList al = new ArrayList();
			al.Add(pm);
			MethodDeclaration myMethod = new MethodDeclaration("Distance", Modifiers.Protected,
			                                                   new TypeReference("int"),
			                                                   al, null);
			Assert.IsTrue(Contains(methods, myMethod));
		}

		[Test]
		public void IndexOf()
		{
			string program = TestUtil.GetInput();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			IList methodDecList = AstUtil.GetChildrenWithType(ty, typeof(MethodDeclaration));

			ParameterDeclarationExpression p1 = new ParameterDeclarationExpression(new TypeReference("Circle"), "circle");
			p1.TypeReference.RankSpecifier = new int[] {};
			ArrayList md1Param = new ArrayList();
			md1Param.Add(p1);
			MethodDeclaration md1 = new MethodDeclaration("GetRadius", Modifiers.Private, new TypeReference("int"), md1Param, null);

			int md1Index = IndexOf(methodDecList, md1);
			Assert.AreEqual(1, md1Index);

			MethodDeclaration md2 = new MethodDeclaration("ToString", Modifiers.Protected, new TypeReference("string"), null, null);
			int md2Index = IndexOf(methodDecList, md2);
			Assert.AreEqual(-1, md2Index);
		}
	}
}