package Test;
import junit.framework.TestCase;
public class A extends TestCase
{
	public void testAssertAreEquals()
	{
		string wp;
		string wp2;
		assertEquals(wp, wp2);
		assertEquals("message:",wp, wp2);
		assertTrue(wp.CompareTo(wp2) < 0);
	}
}