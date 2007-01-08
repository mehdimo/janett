namespace Test
{
	public class Synchro
	{

		int waitTime = 100;
		public Lock Synch()
		{
			bool locked = true;
			return new AnonymousClassLock1(this, waitTime, locked);
		}
		public void Wait(int time)
		{
			for (int i = 1; i < time; i++) StopWait();
		}
		public void StopWait(){}

		private class AnonymousClassLock1 : Lock
		{
			public AnonymousClassLock1(Synchro enclosingInstance, int waitTime, bool locked)
			{
				this.enclosingInstance = enclosingInstance;
				this.waitTime = waitTime;
				this.locked = locked;
			}
			public bool obtain()
			{
				if (locked)
				{
					enclosingInstance.Wait(waitTime);
					return true;
				}
				return false;
			}
			private Synchro enclosingInstance;
			private int waitTime;
			private bool locked;
			public Synchro Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
			}
		}
	}
	public abstract class Lock 
	{
		public abstract bool obtain();
	}
}