namespace Janett.Framework
{
	using System;

	public class ModeAttribute : Attribute
	{
		public string Name;

		public ModeAttribute(string name)
		{
			Name = name;
		}
	}
}