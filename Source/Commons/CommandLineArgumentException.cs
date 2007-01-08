namespace Janett.Commons
{
	using System;

	public class CommandLineArgumentException : ArgumentException
	{
		public CommandLineArgumentException(string message) : base(message)
		{
		}
	}
}