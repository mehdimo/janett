namespace Test
{
	public class A
	{
		[System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void ChangeListener(PropertyChangeListener listener) 
		{
			changeSupport.addPropertyChangeListener(listener);
		}
	}
}