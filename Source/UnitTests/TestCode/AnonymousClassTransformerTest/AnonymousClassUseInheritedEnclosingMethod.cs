namespace Test
{
	public class MyTest : TestCase
	{
		public void Method()
		{
			AC ac = new AnonymousClassAC1(this);
		}
															
		private class AnonymousClassAC1 : AC
		{
			public AnonymousClassAC1(MyTest enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			public void AC_Method()
			{
				assertEquals("abc", "abc");
			}
			private MyTest enclosingInstance;
			public MyTest Enclosing_Instance
			{
				get { return enclosingInstance; }
			}
		}
	}
}
