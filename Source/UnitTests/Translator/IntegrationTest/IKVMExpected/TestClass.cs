namespace Test.Integration
{
	using java.lang;

	using TestCase = junit.framework.TestCase;

	[NUnit.Framework.TestFixture()]
	public class TestClass : TestCase
	{

		protected override void setUp()
		{
		}

		protected override void tearDown()
		{
		}

		[NUnit.Framework.Test()]
		public void TestMethod()
		{
			AbstractClass abc = null;
			string testName = abc.Name;
			abc.Name = testName;
			Integer a = new Integer(0);
			int i = a.intValue();
			AbstractClass.InnerAbstractClass inf = new AnonymousClassAbstractClass_InnerAbstractClass1(i, this);
			Integer[] b = null;
			bool ab = (a == null) || (b == null);
			int j = 0;
			assertEquals("message :", i, j);
		}
		private class AnonymousClassAbstractClass_InnerAbstractClass1 : AbstractClass.InnerAbstractClass
		{
			public AnonymousClassAbstractClass_InnerAbstractClass1(int num, TestClass enclosingInstance) : base(num)
			{
				this.enclosingInstance = enclosingInstance;
			}
			public override void InnerAbstractClassMethod(int num)
			{
			}
			private TestClass enclosingInstance;
			public TestClass Enclosing_Instance {
				get { return enclosingInstance; }
			}
		}

	}
}
