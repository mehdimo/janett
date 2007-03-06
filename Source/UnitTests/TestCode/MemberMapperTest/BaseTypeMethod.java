package Test;
import junit.framework.TestCase;
public class A extends TestCase
{
	public void testAssertAreEquals()
	{
		String wp;
		String wp2;
		assertEquals(wp, wp2);
		assertEquals("message:",wp, wp2);
		assertTrue(wp.CompareTo(wp2) < 0);
	}
}
