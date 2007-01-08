namespace Janett.Commons
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CommandLineFlagAttribute : CommandLineAttribute
	{
		public CommandLineFlagAttribute(string name) : base(name)
		{
		}
	}
}