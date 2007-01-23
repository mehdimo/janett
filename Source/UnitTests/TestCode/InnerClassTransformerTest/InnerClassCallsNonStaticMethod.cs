namespace Test
{
	public class A
	{
		private int Calculate(int count) {return 0;}

		public class BinA
		{
			int dataCount = 10;
			public void UsageIt(){int a = A.Calculate(dataCount) + A.Calculate(dataCount + 1);}
			A A;
			public BinA(A A)
			{
				this.A = A;
			}
		}
	}
}