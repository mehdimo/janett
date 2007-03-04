namespace Test
{
	using java.lang;
	public class InterfaceUsage : java.lang.Object
	{
		protected internal int x;
		protected internal string src;


		public InterfaceUsage(int arg)
		{
			InitInterfaceUsage();
			x += arg;
		}

		public void Method()
		{
			int max = Test.Util.Interface_Fields.Max;
		}
		private void InitInterfaceUsage()
		{
			x = 10;
			scr = "";
		}
	}
}
