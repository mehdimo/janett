package Test;
public class A
{
	public synchronized void ChangeListener(PropertyChangeListener listener)
	{
		changeSupport.addPropertyChangeListener(listener);
	}
}