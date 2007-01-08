namespace Test
{
	public class A
	{
		public class InnerA
		{
			public A Method()
			{
				return A;
			}
			A A;
		}
	}
}