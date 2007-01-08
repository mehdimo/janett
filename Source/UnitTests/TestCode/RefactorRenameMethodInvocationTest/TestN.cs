namespace Test
{
	using System.IComparable;
	
	public class A
	{
		private static int DATE_LEN = Long.toString(1000l * 1000, Character.MAX_RADIX).length();
		public void Method(B bs)
		{
			bs.C[0].CompareTo(0);
		}
	}
	public class B
	{
		public IComparable[] C;
	}
}