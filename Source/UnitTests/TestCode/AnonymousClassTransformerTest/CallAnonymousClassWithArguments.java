package BaseComplete;
public class Locker
{
	public Lock Method()
	{
		string lockName;
		return new Lock(lockName, 100)
				{
					public void LockIt()
					{
					}
				};
	}
}
public abstract class Lock
{
	public Lock(string name, int time)
	{
	}
	public abstract void LockIt();
}