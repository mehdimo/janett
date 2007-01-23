package Test;
public class A
{
	private int Calculate(int count) {return 0;}

	public class BinA
	{
		int dataCount = 10;
		public void UsageIt(){int a = Calculate(dataCount) + Calculate(dataCount + 1);}
	}
}