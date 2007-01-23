namespace Test
{
	public class A
	{
		private int Calculate() {return 0;}

		public class BinA
		{
			public BinA(int code, A A)
			{
				this.A = A;
			}
			public BinA(int code, String name, A A)
			{
				this.A = A;
			}
			public void UsageIt()
			{
				int a = A.Calculate();
			}
			A A;
		}
	}
}