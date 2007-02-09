namespace Test.Integration
{


	[NUnit.Framework.TestFixture()]
	public class TestClass
	{
		[NUnit.Framework.SetUp()]

		public void SetUp()
		{
		}
		[NUnit.Framework.TearDown()]

		public void TearDown()
		{
		}

		[NUnit.Framework.Test()]
		public void TestMethod()
		{
			AbstractClass abc = null;
			System.Type clazz = GetType();
			string testName = abc.Name;
			abc.Name = testName;
			int a = 0;
			int i = a;
			AbstractClass.InnerAbstractClass inf = new AnonymousClassAbstractClass_InnerAbstractClass1(i, this);
			int[] b = null;
			bool ab = (a == System.Int32.MinValue) || (b == null);
			int j = 0;
			NUnit.Framework.Assert.AreEqual(i, j, "message :");
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
			internal TestClass enclosingInstance;
			public TestClass Enclosing_Instance {
				get { return enclosingInstance; }
			}
		}

	}
}
