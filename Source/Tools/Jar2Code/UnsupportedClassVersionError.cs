internal class UnsupportedClassVersionError : ClassFormatError
{
	internal UnsupportedClassVersionError(string msg)
		: base(msg)
	{
	}
}