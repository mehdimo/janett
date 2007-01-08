namespace Test
{
	public class A
	{

		public void Method()
		{
			int i = new AnonymousClassCalc1(this);
		}
		private class AnonymousClassCalc1 : Calc
		{
			public AnonymousClassCalc1(A enclosingInstance) : this()
			{
				this.enclosingInstance = enclosingInstance;
			}
			public AnonymousClassCalc1()
			{
				int arg = 10;
			}
			private A enclosingInstance;
			public A Enclosing_Instance {
				get {return enclosingInstance; }
			}
		}
	}
}