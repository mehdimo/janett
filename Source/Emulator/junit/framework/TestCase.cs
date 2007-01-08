namespace junit.framework
{
	public abstract class TestCase : Assert
	{
		private string fName;

		public TestCase()
		{
			fName = null;
		}

		public TestCase(string name)
		{
			fName = name;
		}

		public int countTestCases()
		{
			return 1;
		}

		protected virtual void setUp()
		{
		}

		protected virtual void tearDown()
		{
		}

		public string toString()
		{
			return getName() + "(" + GetType().Name + ")";
		}

		public string getName()
		{
			return fName;
		}

		public void setName(string name)
		{
			fName = name;
		}

		[NUnit.Framework.SetUp]
		public void SU()
		{
			setUp();
		}

		[NUnit.Framework.TearDown]
		protected void TD()
		{
			tearDown();
		}
	}
}