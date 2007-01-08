namespace Test
{
	[NUnit.Framework.TestFixture()]
	public class TestA
	{
		public void testMethod()
		{
			A a = null;
			Helpers.ReflectionHelper.CallInternalMethod("method", a, new object[] {0, 1});
			int r = (int)Helpers.ReflectionHelper.CallInternalMethod("method", a, new object[] {1, 0});
		}
	}
}