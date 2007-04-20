namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class MemberMapperTest : MemberMapper
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			CodeBase.Types.LibrariesFolder = @"../../../Translator/Libraries";
			CodeBase.Mappings = new Mappings(@"../../../Translator/Mappings/DotNet");
		}

		[Test]
		public void InstanceMethod()
		{
			string program = TestUtil.StatementParse("String id; id.toString();");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("String id; id.ToString();");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));

			program = TestUtil.StatementParse("Object id; id.toString();");
			cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			expected = TestUtil.CSharpStatementParse("Object id; id.ToString();");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ArrayMethod()
		{
			string program = TestUtil.StatementParse("String[] id; id.clone();");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("String[] id; id.Clone();");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void PrimitiveArgument()
		{
			string program = TestUtil.StatementParse("String str; str.substring(10);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("String str; str.Substring(10);");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));

			program = TestUtil.StatementParse("String chr; String str; str.substring(10, str.indexOf(chr));");
			cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			expected = TestUtil.CSharpStatementParse("String chr; String str; str.Substring(10, str.IndexOf(chr) - (10));");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void IdentifierArgument()
		{
			string program = TestUtil.StatementParse("char semiColon; String str; str.endsWith(semiColon);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("char semiColon; String str; str.EndsWith(semiColon);");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void MethodToMethodChain()
		{
			string program = TestUtil.StatementParse("String currentTag; int starter; int offset; if (currentTag.startsWith(starter, offset)){}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("String currentTag; int starter; int offset; if (currentTag.Substring(offset).StartsWith(starter)){}");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void MethodToIndexer()
		{
			string program = TestUtil.StatementParse("String str; char ch = str.charAt(0);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("String str; char ch = str[0];");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void MethodToProperty()
		{
			string program = TestUtil.StatementParse("String str; int i = str.length();");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("String str; int i = str.Length;");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AddingMethodCalls()
		{
			string program = TestUtil.StatementParse("String str; bool isEqual = str.equalsIgnoreCase(stm);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("String str; bool isEqual = str.ToUpper().Equals(stm.ToUpper());");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InstanceToStatic()
		{
			string program = TestUtil.StatementParse("String str; str.split(str);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("String str; Helpers.Regex.Split(str, str);");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void FieldToField()
		{
			string program = TestUtil.StatementParse("long maxx = Long.MAX_VALUE;");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("long maxx = long.MaxValue;");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void FieldToFieldViaInheritence()
		{
			string program = TestUtil.PackageMemberParse(@"
								import javax.swing.JLabel;
								public class Test
								{
									public void Method() { int left = JLabel.LEFT;}
								}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.NamespaceMemberParse(@"
								using javax.swing.JLabel;
								public class Test
								{
									public void Method() { int left = Helpers.SwingConstants.LEFT;}
								}");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void RemovingConstructor()
		{
			string program = TestUtil.StatementParse("int ten = new Integer(10);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("int ten = 10;");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void MethodChain()
		{
			string program = TestUtil.PackageMemberParse(@"
				import java.util.Map; 
				public class Test { 
					public void Method() { Map map; obj = map.keySet().iterator(); }
				}");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.NamespaceMemberParse(@"
				using java.util.Map; 
				public class Test { 
					public void Method() { Map map; obj = map.Keys.GetEnumerator();} 
				}");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AddingCast()
		{
			string program = TestUtil.GetInput();
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.GetExpected();
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void StaticToStatic()
		{
			string program = TestUtil.StatementParse("int degree; float f = Math.sqrt(degree);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("int degree; float f = Math.Sqrt(degree);");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void StaticToInstance()
		{
			string program = TestUtil.StatementParse("Double.compare(x, y);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("x.CompareTo(y);");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void ReplacingInvocationWithAssignment()
		{
			string program = TestUtil.StatementParse("double ls; long bt = Double.doubleToLongBits(ls);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("double ls; long bt = (long) ls;");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void CastedIdentifier()
		{
			string program = TestUtil.StatementParse("object lhs; ((Comparable)lhs).compareTo(0);");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.CSharpStatementParse("object lhs; ((Comparable)lhs).CompareTo(0);");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Inheritance()
		{
			string program = TestUtil.GetInput();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.GetExpected();

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InheritanceInCode()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration td1 = (TypeDeclaration) ns.Children[0];
			TypeDeclaration td2 = (TypeDeclaration) ns.Children[1];

			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", td1);
			CodeBase.Types.Add("Test.ClassifierException", td2);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void RemovingInvocation()
		{
			string program = TestUtil.PackageMemberParse("import java.lang.reflect.AccessibleObject; class Test { void Method() { Field[] fields; AccessibleObject.setAccessible(fields, true); } }");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.NamespaceMemberParse("using java.lang.reflect.AccessibleObject; class Test { void Method() { Field[] fields; } }");
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void BaseTypeMethod()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void BaseBaseTypeMethod()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration typeA = (TypeDeclaration) ns.Children[1];
			TypeDeclaration typeB = (TypeDeclaration) ns.Children[2];
			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.A", typeA);
			CodeBase.Types.Add("Test.B", typeB);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void This()
		{
			string program = TestUtil.StatementParse("this.getClass();");
			string expected = TestUtil.CSharpStatementParse("this.GetType();");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration type = (TypeDeclaration) ns.Children[0];

			CodeBase.Types.Clear();
			CodeBase.Types.Add("Test.Test", type);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Base_Clone()
		{
			string program = TestUtil.PackageMemberParse("public class A extends java.lang.Object implements Cloneable{ public Object clone() {return super.clone();}}");
			string expected = TestUtil.NamespaceMemberParse("public class A : java.lang.Object,Cloneable{ public Object clone(){ return base.MemberwiseClone();}}");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Instance_Clone()
		{
			string program = TestUtil.PackageMemberParse(@"
						import java.awt.Insets;
						public class A {Insets insets; public Object clone() { Object obj = insets.clone(); }}");
			string expected = TestUtil.NamespaceMemberParse(@"
						using java.awt.Insets;
						public class A {Insets insets; public Object clone() { Object obj = insets.Clone(); }}");

			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void TwoDimensionArray()
		{
			string program = TestUtil.StatementParse("int[][] matrix; len = matrix[0].length;");
			string expected = TestUtil.CSharpStatementParse("int[][] matrix; len = matrix[0].Length;");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void AddingInvocationInConstructorArgument()
		{
			string program = TestUtil.GetInput();
			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);

			string expected = TestUtil.GetExpected();
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void FieldMethodInvocation()
		{
			string program = TestUtil.StatementParse("System._out.print(10)");
			string expected = TestUtil.CSharpStatementParse("System.Console.Out.Write(10)");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void RemovingMethodFromChain()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void GetArgumentsMap()
		{
			string program = TestUtil.StatementParse("String str; int index; MyMethod(str.toString(), index + 10, str.indexOf(str));");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 2);

			string withOutType = GetArgumentsMap(ivc.Arguments, false);
			string withType = GetArgumentsMap(ivc.Arguments, true);
			string expectedWithType = "String a,int b,int c";
			string expectedWithOutType = "a,b,c";
			Assert.AreEqual(expectedWithOutType, withOutType);
			Assert.AreEqual(expectedWithType, withType);
		}

		[Test]
		public void GetReplacedExpression()
		{
			string program = TestUtil.StatementParse("str.startsWith(id.subString(id.indexOf('.')).trim())");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			InvocationExpression javaInvocation = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 0);

			string mapKey = "StartsWith(a)";
			InvocationExpression csharpInvocation = (InvocationExpression) GetReplacedExpression(javaInvocation, mapKey);

			Assert.IsNotNull(csharpInvocation);
			Assert.IsTrue(csharpInvocation.TargetObject is FieldReferenceExpression);

			FieldReferenceExpression fr = (FieldReferenceExpression) csharpInvocation.TargetObject;
			Assert.AreEqual("StartsWith", fr.FieldName);
			Assert.AreEqual(1, csharpInvocation.Arguments.Count);
			Assert.IsTrue(csharpInvocation.Arguments[0] is InvocationExpression);
		}

		[Test]
		public void InstanceMethodDeclaration()
		{
			string program = TestUtil.PackageMemberParse("public class A extends java.lang.Object {public int hashCode(){return 0;} }");
			string expected = TestUtil.NamespaceMemberParse("public class A : java.lang.Object {public override int GetHashCode(){return 0;} }");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration type = (TypeDeclaration) ns.Children[0];
			MethodDeclaration method = (MethodDeclaration) type.Children[0];
			AstUtil.AddModifierTo(method, Modifiers.Override);
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void LibrariyInheritedInstanceMethod()
		{
			string program = TestUtil.GetInput();
			string expected = TestUtil.GetExpected();

			CompilationUnit cu = TestUtil.ParseProgram(program);
			TypesVisitor typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = CodeBase;
			typesVisitor.VisitCompilationUnit(cu, null);

			NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
			TypeDeclaration type = (TypeDeclaration) ns.Children[1];
			MethodDeclaration method = (MethodDeclaration) type.Children[0];
			AstUtil.AddModifierTo(method, Modifiers.Override);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void InvocationContainsMappingSymbols()
		{
			string program = TestUtil.StatementParse("StringBuffer buf; String id; buf.append(b).append(id);");
			string expected = TestUtil.CSharpStatementParse("StringBuffer buf; String id; buf.Append(b).Append(id);");

			CompilationUnit cu = TestUtil.ParseProgram(program);
			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}