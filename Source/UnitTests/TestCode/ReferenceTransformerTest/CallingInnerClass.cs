namespace Test
{
	public class A
	{
		public void Method()
		{
			new InnerA(this);
		}

		public class InnerA
		{
		}
	}
}