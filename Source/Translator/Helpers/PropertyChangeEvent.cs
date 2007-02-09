namespace Helpers
{
	using System;
	using System.ComponentModel;

	public class PropertyChangeEvent : PropertyChangedEventArgs
	{
		public PropertyChangeEvent(string propertyName) : base(propertyName)
		{
		}

		public object getNewValue()
		{
			throw new NotImplementedException();
		}
	}
}