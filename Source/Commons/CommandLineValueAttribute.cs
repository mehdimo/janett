namespace Janett.Commons
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CommandLineValueAttribute : CommandLineAttribute
	{
		private int place = -1;

		public int Place
		{
			get { return place; }
			set { place = value; }
		}

		public CommandLineValueAttribute(string name) : base(name)
		{
		}

		public CommandLineValueAttribute(string name, int place) : base(name)
		{
			this.place = place;
		}
	}
}