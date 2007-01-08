namespace BaseComplete
{
	public class Locker
	{
		public Lock Method()
		{
			string lockName;
			return new AnonymousClassLock1(lockName, 100, this);
		}

		private class AnonymousClassLock1 : Lock
		{
			public AnonymousClassLock1(string name, int time, Locker enclosingInstance) : base(name, time)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public void LockIt()
			{
			}
			private Locker enclosingInstance;
			public Locker Enclosing_Instance
			{
				get { return enclosingInstance; }
			}
		}
	}
	public abstract class Lock
	{
		public Lock(string name, int time)
		{
		}
		public abstract void LockIt();
	}
}