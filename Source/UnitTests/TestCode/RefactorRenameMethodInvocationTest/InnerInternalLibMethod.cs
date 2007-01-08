namespace Test
{
	public class A
	{
		public class InnerA
		{
			public void seek() {}
			public void cloner()
			{
				InnerA ia = null;
				ia.Seek();
			}
		}
	}
}