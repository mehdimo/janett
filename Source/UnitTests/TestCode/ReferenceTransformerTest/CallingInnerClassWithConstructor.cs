namespace Test
{
	public class A
	{
		public void Method()
		{
			new InnerA(0, this);
			new InnerA(0, "", this);
		}

		public class InnerA
		{
			public InnerA(int code, A A)
			{
			}
			public InnerA(int code, String name, A A)
			{
			}
		}
	}
}