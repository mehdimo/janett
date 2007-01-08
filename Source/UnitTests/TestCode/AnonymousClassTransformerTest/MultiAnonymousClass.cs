namespace Test
{
	public class Multi
	{
		public void Main()
		{
			outPut(str, new AnonymousClassArrayConstructor1(this));
			outPut (bit, new AnonymousClassArrayConstructor2(this));
		}

		private class AnonymousClassArrayConstructor1 : ArrayConstructor
		{
			public AnonymousClassArrayConstructor1(Multi enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public object Create (int length)
			{
				return new bool[length];
			}
			private Multi enclosingInstance;
			public Multi Enclosing_Instance
			{
				get { return enclosingInstance;}
			}
		}

		private class AnonymousClassArrayConstructor2 : ArrayConstructor
		{
			public AnonymousClassArrayConstructor2(Multi enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public object create (int length)
			{
				return new byte[length];
			}
			private Multi enclosingInstance;
			public Multi Enclosing_Instance
			{
				get { return enclosingInstance;}
			}
		}
	}
}