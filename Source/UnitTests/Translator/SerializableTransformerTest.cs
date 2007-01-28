namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class SerializableTransformerTest : SerializableTransformer
	{
		[Test]
		public void Serializable()
		{
			string program = TestUtil.PackageMemberParse("import java.io.Serializable; public class A implements Serializable, ICollection{}");
			string expected = TestUtil.NamespaceMemberParse("[System.SerializableAttribute()] public class A : ICollection{}");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void SerializableFullName()
		{
			string program = TestUtil.PackageMemberParse("import java.io.Serializable; public class A implements ICollection, java.io.Serializable, BClass{}");
			string expected = TestUtil.NamespaceMemberParse("[System.SerializableAttribute()] public class A : ICollection, BClass{}");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}

		[Test]
		public void Interface()
		{
			string program = TestUtil.PackageMemberParse("import java.io.Serializable; public interface A implements Serializable{}");
			string expected = TestUtil.NamespaceMemberParse("public interface A{}");

			CompilationUnit cu = TestUtil.ParseProgram(program);

			VisitCompilationUnit(cu, null);
			TestUtil.CodeEqual(expected, TestUtil.GenerateCode(cu));
		}
	}
}