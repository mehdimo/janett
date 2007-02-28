namespace Test
{
	[NUnit.Framework.TestFixture()]
	public class TestA
	{
		public void testMethod()
		{
			Test test = (Test) Helpers.ReflectionHelper.InstantiateClass(typeof(Test), new object[]{"TestClass", 0});
		}
	}
}