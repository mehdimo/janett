namespace Janett.Framework
{
	using System.Collections;
	using System.Collections.Generic;

	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class AstUtilTest
	{
		private AstUtil AstUtil = new AstUtil();

		[Test]
		public void GetChildrenWithType()
		{
			string program = TestUtil.TypeMemberParse(@"
									private int x;
									private string y;
									public int X;
									public string Y;

									public void A(){}
									public void B(){}
									public void C(){}");

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration typeDeclaration = (TypeDeclaration) ns.Children[0];

			List<INode> fieldList = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
			List<INode> methodList = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));

			Assert.IsNotNull(fieldList);
			Assert.AreEqual(4, fieldList.Count);
			FieldDeclaration fieldDeclaration = fieldList[0] as FieldDeclaration;
			Assert.AreEqual("x", ((VariableDeclaration) fieldDeclaration.Fields[0]).Name);

			Assert.IsNotNull(methodList);
			Assert.AreEqual(3, methodList.Count);
			MethodDeclaration methodDeclaration = methodList[0] as MethodDeclaration;
			Assert.AreEqual("A", methodDeclaration.Name);
		}

		[Test]
		public void GetParentOfType()
		{
			string program = TestUtil.TypeMemberParse(@"
								public void A()
								{
									if(a)
									{
										a = b;
									}
								}");

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration typeDeclaration = (TypeDeclaration) ns.Children[0];
			MethodDeclaration method = (MethodDeclaration) typeDeclaration.Children[0];
			IfElseStatement ifElse = (IfElseStatement) method.Body.Children[0];
			ExpressionStatement statement = (ExpressionStatement) ((BlockStatement) ifElse.TrueStatement[0]).Children[0];

			INode parent = AstUtil.GetParentOfType(statement, typeof(IfElseStatement));
			Assert.IsTrue(parent is IfElseStatement);

			parent = AstUtil.GetParentOfType(statement, typeof(MethodDeclaration));
			Assert.IsTrue(parent is MethodDeclaration);

			parent = AstUtil.GetParentOfType(statement, typeof(TypeDeclaration));
			Assert.IsTrue(parent is TypeDeclaration);
		}
	}
}