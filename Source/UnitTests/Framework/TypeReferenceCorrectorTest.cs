namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class TypeReferenceCorrectorTest
	{
		private TypeReferenceCorrector typeReferenceCorrector;

		[TestFixtureSetUp]
		public void SetUp()
		{
			typeReferenceCorrector = new TypeReferenceCorrector();
		}

		[Test]
		public void Java_Lang_String()
		{
			string program = TestUtil.StatementParse("java.lang.String.instancehelper_indexOf(str, pre);");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			typeReferenceCorrector.TrackedVisitCompilationUnit(cu, null);
			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 0);
			FieldReferenceExpression invocationTarget = (FieldReferenceExpression) ivc.TargetObject;

			Assert.IsTrue(invocationTarget.TargetObject is TypeReferenceExpression);
			Assert.AreEqual("java.lang.String", ((TypeReferenceExpression) invocationTarget.TargetObject).TypeReference.Type);
		}

		[Test]
		public void Helpers_Regex()
		{
			string program = TestUtil.StatementParse("Helpers.Regex.split(str, delim);");
			CompilationUnit cu = TestUtil.ParseProgram(program);

			typeReferenceCorrector.TrackedVisitCompilationUnit(cu, null);
			InvocationExpression ivc = (InvocationExpression) TestUtil.GetStatementNodeOf(cu, 0);
			FieldReferenceExpression invocationTarget = (FieldReferenceExpression) ivc.TargetObject;

			Assert.IsTrue(invocationTarget.TargetObject is TypeReferenceExpression);
			Assert.AreEqual("Helpers.Regex", ((TypeReferenceExpression) invocationTarget.TargetObject).TypeReference.Type);
		}
	}
}