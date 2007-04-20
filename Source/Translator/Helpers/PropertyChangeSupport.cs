namespace Helpers
{
	using System;

	public class PropertyChangeSupport
	{
		public PropertyChangeSupport(object obj)
		{
		}

		public void addPropertyChangeListener(string name, PropertyChangeListener listener)
		{
			throw new NotImplementedException();
		}

		public void addPropertyChangeListener(PropertyChangeListener listener)
		{
			throw new NotImplementedException();
		}

		public void removePropertyChangeListener(string name, PropertyChangeListener listener)
		{
			throw new NotImplementedException();
		}

		public void removePropertyChangeListener(PropertyChangeListener listener)
		{
			throw new NotImplementedException();
		}

		public void firePropertyChange(string s, object testString, object newTestString)
		{
		}
	}
}