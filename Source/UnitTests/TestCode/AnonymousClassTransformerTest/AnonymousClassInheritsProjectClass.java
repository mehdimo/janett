package Test;
public class Synchro
{
	int waitTime = 100;
	public Lock Synch()
	{
		bool locked = true;
		return new Lock()
		{
			public bool obtain()
			{
				if (locked)
				{
					Wait(waitTime);
					return true;
				}
				return false;
			}
		}
	}
	public void Wait(int time)
	{
		for (int i = 1; i < time; i++) StopWait();
	}
	public void StopWait(){}
}
public abstract class Lock
{
	public abstract bool obtain();
}