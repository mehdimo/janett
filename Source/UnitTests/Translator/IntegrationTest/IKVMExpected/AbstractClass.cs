namespace Test.Integration
{
	using java.lang;
	public abstract class AbstractClass : java.lang.Object, Interface
	{
		private string name;
		public string Name {

			get { return this.name; }
			set { this.name = value; }
		}

		protected bool existSimilarFieldAndMethod = true;
		protected internal bool ExistSimilarFieldAndMethod()
		{
			string methodName = Name;
			Name = methodName;
			return existSimilarFieldAndMethod;
		}

		public abstract void AbstractClassMethod();

		public abstract class InnerAbstractClass : java.lang.Object
		{
			public InnerAbstractClass(int num)
			{
			}
			public abstract void InnerAbstractClassMethod(int num);
		}
		public abstract string InterfaceMethod1(string _string);
	}
}
