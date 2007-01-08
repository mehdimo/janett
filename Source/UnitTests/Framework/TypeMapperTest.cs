namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class TypeMapperTest : TypeMapper
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			CodeBase.Mappings = new Mappings(@"../../../Translator/Mappings/DotNet");
		}

		[Test]
		public void StringBuffer_StringBuilder()
		{
			CheckStatement("StringBuffer sbf = new StringBuffer();",
			               "System.Text.StringBuilder sbf = new System.Text.StringBuilder();",
			               "java.lang.StringBuffer");

			CheckTypeMember("StringBuffer strBuff; StringBuffer Clonize(StringBuffer sb) {}",
			                "System.Text.StringBuilder strBuff; System.Text.StringBuilder Clonize(System.Text.StringBuilder sb) {}",
			                "java.lang.StringBuffer");
		}

		[Test]
		public void Map_IDictionary()
		{
			CheckTypeMember("Map dict; Map Merge(Map arg) {return null;}",
			                "System.Collections.IDictionary dict; System.Collections.IDictionary Merge(System.Collections.IDictionary arg) {return null;}",
			                "java.util.Map");
		}

		[Test]
		public void Object()
		{
			CheckTypeMember("public Object GetMember(Object obj){return ((Object)obj).Member;}",
			                "public object GetMember(object obj){return ((object)obj).Member;}",
			                "java.lang.Object");
		}

		[Test]
		public void Boolean()
		{
			CheckTypeMember("public boolean IsTrue(boolean flage){return galg;}",
			                "public bool IsTrue(bool flage){return galg;}",
			                "java.lang.Boolean");
		}

		[Test]
		public void String()
		{
			CheckTypeMember("public String Append(String str, String app){return String.Format(sf,str,app);}",
			                "public string Append(string str, string app){return String.Format(sf,str,app);}",
			                "java.lang.String");
		}

		[Test]
		public void UnsupportedOperationException()
		{
			CheckStatement("new UnsupportedOperationException(message);",
			               "new System.NotSupportedException(message);",
			               "java.lang.UnsupportedOperationException");
		}

		[Test]
		public void Throwable()
		{
			CheckStatement("Throwable throwable;",
			               "System.Exception throwable;",
			               "java.lang.Throwable");
		}

		[Test]
		public void TestCase()
		{
			string program = TestUtil.PackageMemberParse(@"import junit.framework.TestCase; 
															public class Test extends TestCase { }");
			string expected = TestUtil.NamespaceMemberParse("public class Test{ }");
			CompilationUnit cu = TestUtil.ParseProgram(program);
			Mode = "DotNet";
			VisitCompilationUnit(cu, null);

			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		private void CheckTypeMember(string java, string expectedS, string type)
		{
			string program = TestUtil.TypeMemberParse(java);
			CompilationUnit cu = TestUtil.ParseProgram(program);
			AddUsing(cu, type);

			string expected = TestUtil.CSharpTypeMemberParse(expectedS);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		private void AddUsing(CompilationUnit cu, string type)
		{
			cu.Children.Add(new UsingDeclaration(type));
			VisitCompilationUnit(cu, null);
		}

		private void CheckStatement(string java, string expectedS, string type)
		{
			string program = TestUtil.StatementParse(java);
			CompilationUnit cu = TestUtil.ParseProgram(program);
			AddUsing(cu, type);
			string expected = TestUtil.CSharpStatementParse(expectedS);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}