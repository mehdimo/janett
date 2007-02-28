namespace Janett
{
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;

	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.NRefactory.PrettyPrinter;

	using Janett.Commons;
	using Janett.Framework;

	using NUnit.Framework;

	public class TestUtil
	{
		public static CompilationUnit ParseProgram(string program)
		{
			return ParseProgram(program, SupportedLanguage.Java);
		}

		public static CompilationUnit ParseProgram(string program, SupportedLanguage language)
		{
			IParser parser = ParserFactory.CreateParser(language, new StringReader(program));
			parser.ParseMethodBodies = true;
			parser.Parse();

			Assert.IsNotNull(parser.Errors);
			Assert.IsNotNull(parser.CompilationUnit);
			Assert.IsNotNull(parser.CompilationUnit.Children);
			Assert.IsNotNull(parser.CompilationUnit.Children[0]);
			Assert.IsTrue(parser.CompilationUnit.Children.Count > 0);

			ParentVisitor parentVisitor = new ParentVisitor();
			parentVisitor.VisitCompilationUnit(parser.CompilationUnit, null);

			return parser.CompilationUnit;
		}

		public static string StatementParse(string statement)
		{
			if (statement[statement.Length - 1] != '}' && statement[statement.Length - 1] != ';')
				statement += ';';
			return TypeMemberParse("public void TestMethod(){" + statement + "}");
		}

		public static string CSharpStatementParse(string statement)
		{
			if (statement[statement.Length - 1] != '}' && statement[statement.Length - 1] != ';')
				statement += ';';
			return CSharpTypeMemberParse("public void TestMethod(){" + statement + "}");
		}

		public static string TypeMemberParse(string typeMember)
		{
			return PackageMemberParse("public class Test {" + typeMember + "}");
		}

		public static string CSharpTypeMemberParse(string typeMember)
		{
			return NamespaceMemberParse("public class Test {" + typeMember + "}");
		}

		public static string PackageMemberParse(string type)
		{
			return "package Test;" + type;
		}

		public static string NamespaceMemberParse(string type)
		{
			return "namespace Test {" + type + "}";
		}

		public static string GenerateCode(CompilationUnit compilationUnit)
		{
			CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor();
			outputVisitor.VisitCompilationUnit(compilationUnit, null);
			return outputVisitor.Text;
		}

		public static void CodeEqual(string expected, string actual)
		{
			string pureExpected = expected.Replace("\r\n", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
			string pureActual = actual.Replace("\r\n", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
			Assert.AreEqual(pureExpected, pureActual, "\r\nConverted is: " + actual);
		}

		public static Expression GetStatementNodeOf(CompilationUnit compilationUnit, int index)
		{
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration ty = (TypeDeclaration) ns.Children[0];
			MethodDeclaration md = (MethodDeclaration) ty.Children[0];
			ExpressionStatement st = (ExpressionStatement) md.Body.Children[index];
			Expression exp = st.Expression;
			return exp;
		}

		public static string GetInput()
		{
			MethodBase method = GetCallingMethod();
			return GetFileOfMethod(method.DeclaringType.Name, method.Name, ".java");
		}

		private static MethodBase GetCallingMethod()
		{
			StackTrace stack = new StackTrace();
			int i = 0;
			while (stack.GetFrame(i).GetMethod().DeclaringType == typeof(TestUtil))
				i++;
			return stack.GetFrame(i).GetMethod();
		}

		public static string GetExpected()
		{
			MethodBase method = GetCallingMethod();
			string fileName = method.Name;
			if (method.DeclaringType.Namespace.IndexOf("Downgrader") != -1)
				fileName += "-Expected";
			return GetFileOfMethod(method.DeclaringType.Name, fileName, ".cs");
		}

		private static string GetFileOfMethod(string type, string methodName, string extension)
		{
			string path = Path.GetFullPath(Path.Combine(@"../../TestCode", type));
			string filePath = Path.Combine(path, methodName + extension);

			return FileSystemUtil.ReadFile(filePath);
		}
	}
}