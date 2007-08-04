namespace Test
{
	public class Synchro
	{

		int waitTime = 100;
		public Lock Synch()
		{
			bool locked = true;
			return new AnonymousClassLock1(this, locked, waitTime);
		}
		public void Wait(int time)
		{
			for (int i = 1; i < time; i++) StopWait();
		}
		public void StopWait(){}

		private class AnonymousClassLock1 : Lock
		{
			public AnonymousClassLock1(Synchro enclosingInstance, bool locked, int waitTime)
			{
				this.enclosingInstance = enclosingInstance;
				this.locked = locked;
				this.waitTime = waitTime;
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
			private bool locked;
			private int waitTime;
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