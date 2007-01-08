namespace Test
{
	public class A : B
	{
		public C Method()
		{
			D result;
			return new AnonymousClassC1(result, this);
		}
		public class F : D{}
		private class AnonymousClassC1 : C
		{
			public AnonymousClassC1(D d, A enclosingInstance) : base(d)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private A enclosingInstance;
			public A Enclosing_Instance{ get { return enclosingInstance;}}
		}
	}
	public class B
	{

	}
	public class C 
	{ 
		public C(D d){} 
	}
	public class D 
	{
	}
}