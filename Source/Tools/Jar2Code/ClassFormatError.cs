using System;

internal class ClassFormatError : ApplicationException
{
	internal ClassFormatError(string msg, params object[] p)
		: base(string.Format(msg, p))
	{
	}
}