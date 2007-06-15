namespace Test
{
	public interface SystemConstants : Constants
	{
	}

	public class SystemConstants_Fields
	{
		public const int Index = Test.Constants_Fields.Default + 10;
	}

	public interface Constants
	{
	}

	public class Constants_Fields
	{
		public const int Default = 1;
	}
}