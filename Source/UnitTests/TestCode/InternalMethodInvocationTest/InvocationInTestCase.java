namespace Test
{
	[NUnit.Framework.TestFixture()]
	public class TestA
	{
		public void testMethod()
		{
			A a = null;
			a.method(0, 1);
			int r = a.method(1, 0);
		}
	}
}
