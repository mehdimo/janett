package Test;
import junit.framework.TestCase;
public class A extends TestCase
{}
public class B extends A
{
	public void testAssertAreEquals()
	{
		double expected;
		double actual;
		assertEquals(expected, actual, 0);
		assertEquals(expected, actual, 0.01);
		StringBuffer sb;
		StringBuffer sf = sb.append(".");
		assertEquals(sb, sf);
	}
}