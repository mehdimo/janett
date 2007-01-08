namespace Test
{
	using RandomAccessFile = java.io.RandomAccessFile;
	public class A
	{
		public class InnerA : RandomAccessFile
		{
		}
		InnerA file = null;
		public void Method()
		{
			file.seek(1);
		}
	}
}