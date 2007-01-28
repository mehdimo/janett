namespace Janett.Framework
{
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class ExpressionTypeResolverTest : ExpressionTypeResolver
	{
		private string program;

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			CodeBase = new CodeBase(SupportedLanguage.Java);
			CodeBase.Types.LibrariesFolder = @"../../../Translator/Libraries";

			AstUtil = new AstUtil();

			TypeResolver = new TypeResolver();
			TypeResolver.CodeBase = CodeBase;
			TypeResolver.AstUtil = AstUtil;
		}

		[Test]
		public void LocalVariable()
		{
			string program = TestUtil.StatementParse("String name; name.toString();");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			InvocationExpression iv = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 1);
			FieldReferenceExpression fieldReference = (FieldReferenceExpression) iv.TargetObject;
			string ivIdType = GetType(fieldReference.TargetObject).Type;

			Assert.AreEqual("String", ivIdType);
		}

		[Test]
		public void LocalVariableInArguments()
		{
			program = TestUtil.StatementParse(@"
										Hashtable table = new Hashtable();
										string key = ""1"";

										table.Add(key , ""first"");");

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];
			ExpressionStatement expr = (ExpressionStatement) md.Body.Children[2];
			InvocationExpression inv = (InvocationExpression) expr.Expression;
			FieldReferenceExpression fr = (FieldReferenceExpression) inv.TargetObject;
			IdentifierExpression id = (IdentifierExpression) fr.TargetObject;

			string actual = GetType(id).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("Hashtable", actual);

			actual = GetType((Expression) inv.Arguments[0]).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("string", actual);
		}

		[Test]
		public void ExternalType()
		{
			CodeBase.Types.LibrariesFolder = @"../../../Translator/Libraries";

			string program = TestUtil.StatementParse("Map f; f.containsKey(a);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			UsingDeclaration usiDec = new UsingDeclaration("Map", new TypeReference("java.util.Map"));

			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 1);
			((NamespaceDeclaration) cu.Children[0]).Children.Insert(0, usiDec);
			TypeReference returnType = GetType(ivc);
			Assert.IsNotNull(returnType);
			Assert.AreEqual("java.lang.Boolean", returnType.Type);
		}

		[Test]
		public void Property()
		{
			program = @"
						namespace Test{
							public class A
							{
								public IList children;
								public void Method() 
								{
										foreach (INode child in children) {
											Debug.Assert(child != null);
											child.AcceptVisitor(visitor, data);
										}
								}
							}
						}";

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program, SupportedLanguage.CSharp);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[1];
			ForeachStatement f = (ForeachStatement) md.Body.Children[0];
			IdentifierExpression id = (IdentifierExpression) f.Expression;
			Assert.IsNotNull(id);

			Assert.AreEqual("IList", GetType(id).Type);
		}

		[Test]
		public void TypeMembers()
		{
			program = TestUtil.TypeMemberParse(@"
									private Hashtable table = new Hashtable();
									string key = ""1"";

									public void ATest()
									{
										table.Add(key , ""first"");
									}");

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[2];
			ExpressionStatement expr = (ExpressionStatement) md.Body.Children[0];
			InvocationExpression inv = (InvocationExpression) expr.Expression;
			FieldReferenceExpression fr = (FieldReferenceExpression) inv.TargetObject;
			IdentifierExpression id = (IdentifierExpression) fr.TargetObject;

			string actual = GetType(id).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("Hashtable", actual);

			actual = GetType((Expression) inv.Arguments[0]).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("string", actual);
		}

		[Test]
		public void Argument()
		{
			program = TestUtil.TypeMemberParse(@"
									public void ATest(string key, Hashtable table)
									{
										table.Add(key , ""first"");
									}");

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];
			ExpressionStatement expr = (ExpressionStatement) md.Body.Children[0];
			InvocationExpression inv = (InvocationExpression) expr.Expression;
			FieldReferenceExpression fr = (FieldReferenceExpression) inv.TargetObject;
			IdentifierExpression id = (IdentifierExpression) fr.TargetObject;

			string actual = GetType(id).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("Hashtable", actual);

			actual = GetType((Expression) inv.Arguments[0]).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("string", actual);
		}

		[Test]
		public void StaticClass()
		{
			program = TestUtil.PackageMemberParse(@"
								public class A
								{
									public void ATest()
									{
										bool flag = B.Test(1);
										int sq = Math.sqrt(10);
									}
								}

								public class B
								{
									public static bool Test(int arg){}
								}");

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			TypeDeclaration td = (TypeDeclaration) ns.Children[1];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];
			LocalVariableDeclaration flag = (LocalVariableDeclaration) md.Body.Children[0];
			LocalVariableDeclaration sq = (LocalVariableDeclaration) md.Body.Children[1];

			VariableDeclaration var = (VariableDeclaration) flag.Variables[0];
			InvocationExpression inv = (InvocationExpression) var.Initializer;
			FieldReferenceExpression fr = (FieldReferenceExpression) inv.TargetObject;
			IdentifierExpression id = (IdentifierExpression) fr.TargetObject;

			CodeBase.Mappings = new Mappings();
			CodeBase.Mappings.Add("Math", new TypeMapping("Math"));
			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.B", td);

			string actual = GetType(id).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("Test.B", actual);

			var = (VariableDeclaration) sq.Variables[0];
			inv = (InvocationExpression) var.Initializer;
			fr = (FieldReferenceExpression) inv.TargetObject;
			id = (IdentifierExpression) fr.TargetObject;

			string frameworkType = GetType(id).Type;
			Assert.IsNotNull(frameworkType);
			Assert.AreEqual("java.lang.Math", frameworkType);
		}

		[Test]
		public void CatchException()
		{
			program = TestUtil.StatementParse("try {} catch(Exception e) {e.ToString();}");

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];
			TryCatchStatement expr = (TryCatchStatement) md.Body.Children[0];
			CatchClause cc = (CatchClause) expr.CatchClauses[0];
			ExpressionStatement stmExpr = (ExpressionStatement) cc.StatementBlock.Children[0];
			InvocationExpression invok = (InvocationExpression) stmExpr.Expression;

			FieldReferenceExpression fr = (FieldReferenceExpression) invok.TargetObject;
			IdentifierExpression id = (IdentifierExpression) fr.TargetObject;

			string actual = GetType(id).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("Exception", actual);
		}

		[Test]
		public void LoopVariable()
		{
			string program = TestUtil.StatementParse(@"for (int index = 0; index < 10; index++) 
																{ result = index; }");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];
			ForStatement forStm = (ForStatement) md.Body.Children[0];
			ExpressionStatement stm = (ExpressionStatement) forStm.EmbeddedStatement.Children[0];

			AssignmentExpression assi = (AssignmentExpression) stm.Expression;
			IdentifierExpression ide = (IdentifierExpression) assi.Right;

			TypeReference typeReference = GetType(ide);
			Assert.IsNotNull(typeReference);
			Assert.AreEqual("int", typeReference.Type);
		}

		[Test]
		public void Invocation()
		{
			string program = TestUtil.StatementParse("String word; word.toLowerCase();");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 1);
			TypeReference retType = GetType(ivc);
			Assert.IsNotNull(retType);
			Assert.AreEqual("java.lang.String", retType.Type);
		}

		[Test]
		public void InvocationWithArguments()
		{
			string program = TestUtil.StatementParse("String word; String str; word.indexOf(str);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 2);
			TypeReference retType = GetType(ivc);
			Assert.IsNotNull(retType);
			Assert.AreEqual("java.lang.Integer", retType.Type);
		}

		[Test]
		public void InstanceInvocation()
		{
			string program = @"
								package Test;
								public class B
								{
									public void Method()
									{
										A a;
										a.GetName().trim();
									}
								}
								public class A
								{
									public string GetName(){};
								}";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			TypeDeclaration tA = (TypeDeclaration) ((NamespaceDeclaration) cu.Children[0]).Children[1];
			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", tA);
			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 1);
			FieldReferenceExpression fr = (FieldReferenceExpression) ivc.TargetObject;
			string ivType = GetType(fr.TargetObject).Type;
			Assert.AreEqual("string", ivType);
		}

		[Test]
		public void FieldReference()
		{
			string program = @"
								package Test;
								public class B
								{
									public void Method()
									{
										A.Name.toString();
									}
								}
								public class A
								{
									public static string Name;
								}";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			TypeDeclaration tA = (TypeDeclaration) ((NamespaceDeclaration) cu.Children[0]).Children[1];
			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", tA);
			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 0);
			FieldReferenceExpression fr = (FieldReferenceExpression) ivc.TargetObject;
			string ivType = GetType(fr.TargetObject).Type;
			Assert.AreEqual("string", ivType);
		}

		[Test]
		public void Primitive()
		{
			string program = TestUtil.StatementParse(@"""test"".toUpperCase();");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			InvocationExpression iv = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 0);
			FieldReferenceExpression fieldReference = (FieldReferenceExpression) iv.TargetObject;
			string ivIdType = GetType(fieldReference.TargetObject).Type;

			Assert.AreEqual("string", ivIdType);
		}

		[Test]
		public void PrimitiveArgument()
		{
			program = TestUtil.StatementParse(@"
										Hashtable table = new Hashtable();
										table.Add(1 , ""first"");");

			CompilationUnit compilationUnit = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];
			ExpressionStatement expr = (ExpressionStatement) md.Body.Children[1];
			InvocationExpression inv = (InvocationExpression) expr.Expression;

			string actual = GetType((Expression) inv.Arguments[0]).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("int", actual);

			actual = GetType((Expression) inv.Arguments[1]).Type;
			Assert.IsNotNull(actual);
			Assert.AreEqual("string", actual);
		}

		[Test]
		public void Indexer()
		{
			string program = TestUtil.StatementParse("string[] keys; keys[1].toString();");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			InvocationExpression iv = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 1);
			FieldReferenceExpression fieldReference = (FieldReferenceExpression) iv.TargetObject;
			string ivIdType = GetType(fieldReference.TargetObject).Type;

			Assert.AreEqual("string", ivIdType);
		}

		[Test]
		public void This()
		{
			string program = TestUtil.StatementParse(@"this.toString();");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			InvocationExpression iv = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 0);
			FieldReferenceExpression fieldReference = (FieldReferenceExpression) iv.TargetObject;
			string ivIdType = GetType(fieldReference.TargetObject).Type;

			Assert.AreEqual("Test", ivIdType);
		}

		[Test]
		public void ObjectCreation()
		{
			string program = @"
								package Test;
								public class A
								{
									public void Method()
									{
										new EqualsBuilder().append();
									}
								}
								
								public class EqualsBuilder
								{
									public EqualsBuilder append()
									{ return null; }
								}";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration type1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration type2 = (TypeDeclaration) ns.Children[1];

			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", type1);
			CodeBase.Types.Add("Test.EqualsBuilder", type2);

			Expression expression = TestUtil.GetStatementNodeOf(cu, 0);
			TypeReference expressionType = GetType(expression);

			Assert.IsNotNull(expressionType);
			Assert.AreEqual("EqualsBuilder", expressionType.Type);
		}

		[Test]
		public void Cast()
		{
			string program = @"package Test
								public class A
								{
									public void Method()
									{
										B b;
										a = ((StmExpression)b.Expr).Count;
									}
								}
								public class B
								{
									public Expression Expr;
								}
								public class Expression {}
								public class StmExpression extends Expression
								{
									public int Count;
								}";

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration ty0 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration ty1 = (TypeDeclaration) ns.Children[1];
			TypeDeclaration ty2 = (TypeDeclaration) ns.Children[2];
			TypeDeclaration ty3 = (TypeDeclaration) ns.Children[3];
			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", ty0);
			CodeBase.Types.Add("Test.B", ty1);
			CodeBase.Types.Add("Test.Expression", ty2);
			CodeBase.Types.Add("Test.StmExpression", ty3);

			MethodDeclaration md = (MethodDeclaration) ty0.Children[0];
			ExpressionStatement se = (ExpressionStatement) md.Body.Children[1];
			AssignmentExpression assignmentExpression = (AssignmentExpression) se.Expression;
			FieldReferenceExpression fr = (FieldReferenceExpression) assignmentExpression.Right;

			TypeReference ivType = GetType(fr);

			Assert.IsNotNull(ivType);
			Assert.AreEqual("int", ivType.Type);
		}

		[Test]
		public void InnerInterfaceMember()
		{
			string program = TestUtil.PackageMemberParse(@"
				import java.util.Map; 
				public class A 
				{ 
					public void Method() 
					{ 
						Map.Entry entry; 
						entry.getValue();
					}
				}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration type = (TypeDeclaration) ns.Children[1];
			MethodDeclaration method = (MethodDeclaration) type.Children[0];
			ExpressionStatement stm = (ExpressionStatement) method.Body.Children[1];
			InvocationExpression invocationExpression = (InvocationExpression) stm.Expression;
			TypeReference typeRef = GetType(invocationExpression);
			Assert.IsNotNull(typeRef);
			Assert.AreEqual("java.lang.Object", typeRef.Type);
		}
	}
}