namespace Test
{
	using junit.framework.TestCase;
	public class A : TestCase
	{
		public void testAssertAreEquals()
		{
			string wp;
			string wp2;
			NUnit.Framework.Assert.AreEqual(wp, wp2);
			NUnit.Framework.Assert.AreEqual(wp, wp2,"message:");
			NUnit.Framework.Assert.IsTrue(wp.CompareTo(wp2) < 0);
		}
	}
}