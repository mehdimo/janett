namespace Janett.Commons
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CommandLineAttribute : Attribute
	{
		protected string name;
		protected bool optional;
		protected string alternateName;
		protected string description;

		public string Name
		{
			get { return name; }
		}

		public bool Optional
		{
			get { return optional; }
			set { optional = value; }
		}

		public string AlternateName
		{
			get { return alternateName; }
			set { alternateName = value; }
		}

		public string Description
		{
			get
			{
				if (description != null)
					return description;
				else
					return name;
			}
			set { description = value; }
		}

		public CommandLineAttribute(string name)
		{
			this.name = name;
		}
	}
}