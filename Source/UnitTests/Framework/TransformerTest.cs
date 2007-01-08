namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class TransformerTest : Transformer
	{
		[Test]
		public void FindMethod()
		{
			string program = TestUtil.TypeMemberParse(@"
								public int Method(int a) {return a;}
								public int Method(string a, int b) {return 0;}

								public void Main()
								{
									string p = null;
									int q = 0;

									Method(q);
									Method(p, q);
								}");

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration typeDeclaration = (TypeDeclaration) ns.Children[0];
			MethodDeclaration methodDeclaration = (MethodDeclaration) typeDeclaration.Children[2];
			InvocationExpression invocation1 = (InvocationExpression) ((ExpressionStatement) methodDeclaration.Body.Children[2]).Expression;
			InvocationExpression invocation2 = (InvocationExpression) ((ExpressionStatement) methodDeclaration.Body.Children[3]).Expression;

			MethodDeclaration foundMethod1 = GetMethodDeclarationOf(typeDeclaration, invocation1);
			Assert.IsNotNull(foundMethod1);
			Assert.AreEqual("Method", foundMethod1.Name);
			Assert.AreEqual(1, foundMethod1.Parameters.Count);
			ParameterDeclarationExpression parameter = (ParameterDeclarationExpression) foundMethod1.Parameters[0];
			Assert.AreEqual("int", parameter.TypeReference.Type);
			Assert.AreEqual("a", parameter.ParameterName);

			MethodDeclaration foundMethod2 = GetMethodDeclarationOf(typeDeclaration, invocation2);
			Assert.IsNotNull(foundMethod2);
			Assert.AreEqual("Method", foundMethod2.Name);
			Assert.AreEqual(2, foundMethod2.Parameters.Count);
			parameter = (ParameterDeclarationExpression) foundMethod2.Parameters[0];
			Assert.AreEqual("string", parameter.TypeReference.Type);
			Assert.AreEqual("a", parameter.ParameterName);
		}

		[Test]
		public void GetMethodDeclarationOf()
		{
			string program = TestUtil.PackageMemberParse(@"
					public class DC extends AC
					{
						public int A(string a) {return 0;}

						public void Main()
						{
							string p = null;
							int q = 0;

							A(p, q);
						}
					}

					public class AC
					{
						public int A(string arg1, int arg2) { return 0;}
					}");
			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration dcType = (TypeDeclaration) ns.Children[0];
			MethodDeclaration mt = (MethodDeclaration) dcType.Children[1];
			InvocationExpression invocation = (InvocationExpression) ((ExpressionStatement) mt.Body.Children[2]).Expression;
			TypesVisitor vis = new TypesVisitor();

			vis.CodeBase = CodeBase;
			vis.VisitCompilationUnit(compilationUnit, null);

			MethodDeclaration foundMethod = GetMethodDeclarationOf(dcType, invocation);
			Assert.IsNotNull(foundMethod);
			Assert.AreEqual("A", foundMethod.Name);
			Assert.AreEqual(2, foundMethod.Parameters.Count);
			ParameterDeclarationExpression parameter = (ParameterDeclarationExpression) foundMethod.Parameters[0];
			Assert.AreEqual("string", parameter.TypeReference.Type);
			Assert.AreEqual("arg1", parameter.ParameterName);
		}

		[Test]
		public void GetEnclosingTypeDeclaration_Identifier()
		{
			string program = TestUtil.StatementParse("A a; a.MethodA();");

			TypeDeclaration typeDeclared = new TypeDeclaration(Modifiers.Public, null);
			typeDeclared.Name = "A";
			MethodDeclaration md = new MethodDeclaration("MethodA", Modifiers.Public,
			                                             new TypeReference("void"), null, null);
			md.Parent = typeDeclared;
			typeDeclared.Children.Add(md);
			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", typeDeclared);

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			ns.Children.Add(typeDeclared);
			TypeDeclaration type = (TypeDeclaration) ns.Children[0];
			MethodDeclaration method = (MethodDeclaration) type.Children[0];
			ExpressionStatement expression = (ExpressionStatement) method.Body.Children[1];
			InvocationExpression invocation = (InvocationExpression) expression.Expression;
			TypeDeclaration resultType = GetEnclosingTypeDeclaration(invocation);

			Assert.IsNotNull(resultType);
			Assert.AreEqual("A", resultType.Name);
			Assert.AreEqual(1, resultType.Children.Count);
		}

		[Test]
		public void GetEnclosingTypeDeclaration_FieldReference()
		{
			string program = TestUtil.PackageMemberParse(@"
									public class A
									{
										public void MethodA(){}
									}
									public class B
									{
										public A a;
									}
									public class C
									{
										B b;
										public void MethodC()
										{
											b.a.MethodA();
										}
									}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];

			TypeDeclaration typeA = (TypeDeclaration) ns.Children[0];
			TypeDeclaration typeB = (TypeDeclaration) ns.Children[1];
			TypeDeclaration typeC = (TypeDeclaration) ns.Children[2];

			MethodDeclaration method = (MethodDeclaration) typeC.Children[1];
			ExpressionStatement expression = (ExpressionStatement) method.Body.Children[0];
			InvocationExpression invocation = (InvocationExpression) expression.Expression;

			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.C", typeC);
			CodeBase.Types.Add("Test.B", typeB);
			CodeBase.Types.Add("Test.A", typeA);

			TypeDeclaration expectedType = GetEnclosingTypeDeclaration(invocation);
			Assert.IsNotNull(expectedType);
			Assert.AreEqual("A", expectedType.Name);
			Assert.AreEqual(1, expectedType.Children.Count);
		}

		[Test]
		public void IsInvocationForMethod()
		{
			string program = @"
							package Test;
							public class One
							{
								public int A(int a) {return a;}
								
								public int A(string a, int b) {return 0;}

								public void Main()
								{
									string p = null;
									int q = 0;
									Two two = new Two();

									A(q);
									two.A(q);
									A(p, q);
								}
							}

							public class Two
							{
								public string A(int a)
								{
									return a.ToString();
								}
							}";

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration tyOne = (TypeDeclaration) ns.Children[0];
			MethodDeclaration oneMethod1Args = (MethodDeclaration) tyOne.Children[0];
			MethodDeclaration oneMethod2Args = (MethodDeclaration) tyOne.Children[1];
			MethodDeclaration oneMethodMain = (MethodDeclaration) tyOne.Children[2];

			InvocationExpression invocation1 = ((ExpressionStatement) oneMethodMain.Body.Children[3]).Expression as InvocationExpression;
			InvocationExpression invocation2 = ((ExpressionStatement) oneMethodMain.Body.Children[4]).Expression as InvocationExpression;
			InvocationExpression invocation3 = ((ExpressionStatement) oneMethodMain.Body.Children[5]).Expression as InvocationExpression;

			TypeDeclaration tyTwo = (TypeDeclaration) ns.Children[1];
			MethodDeclaration twoMethod = (MethodDeclaration) tyTwo.Children[0];

			Assert.IsTrue(IsInvocationForMethod(oneMethod1Args, invocation1));
			Assert.IsTrue(IsInvocationForMethod(twoMethod, invocation1));
			Assert.IsTrue(IsInvocationForMethod(oneMethod1Args, invocation2));
			Assert.IsTrue(IsInvocationForMethod(twoMethod, invocation2));
			Assert.IsTrue(IsInvocationForMethod(oneMethod2Args, invocation3));
			Assert.IsFalse(IsInvocationForMethod(oneMethod1Args, invocation3));
		}
	}
}