namespace Janett.Commons
{
	using System;

	[AttributeUsage(AttributeTargets.Class)]
	public class NamedAttribute : Attribute
	{
		public string Name;

		public NamedAttribute(string name)
		{
			this.Name = name;
		}

		public override bool Equals(Object attribute)
		{
			if (attribute is NamedAttribute)
			{
				NamedAttribute attr = (NamedAttribute) attribute;
				return (attr.Name == Name || attr.Name.StartsWith(Name + ",") || attr.Name.EndsWith("," + Name) || attr.Name.IndexOf("," + Name + ",") != -1);
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			if (Name != null)
				return Name.GetHashCode();
			else
				return 0;
		}
	}
}