namespace Test
{
	public class A
	{
		public void Me() { new AnonymousClassB1(this); }
		private class AnonymousClassB1 : B
		{
			public AnonymousClassB1(A enclosingInstance) 
			{
				this.enclosingInstance = enclosingInstance;
			}
			int id;
			private A enclosingInstance;
			public A Enclosing_Instance
			{ get {return enclosingInstance;} }
		}
	}
}