package Test;
public class MyTest extends TestCase
{
	public void Method()
	{
		AC ac = new AC()
		{
			public void AC_Method()
			{
				assertEquals("abc", "abc");
			}
		};
	}
}
