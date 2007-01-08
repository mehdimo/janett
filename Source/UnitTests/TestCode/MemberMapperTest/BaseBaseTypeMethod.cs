namespace Test
{
	using junit.framework.TestCase;
	public class A : TestCase
	{}
	public class B : A
	{
		public void testAssertAreEquals()
		{
			double expected;
			double actual;
			NUnit.Framework.Assert.AreEqual(expected, actual, 0);
			NUnit.Framework.Assert.AreEqual(expected, actual, 0.01);
			StringBuffer sb;
			StringBuffer sf = sb.Append(".");
			NUnit.Framework.Assert.AreEqual(sb, sf);
		}
	}
}