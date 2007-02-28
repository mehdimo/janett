namespace Test
{
	public interface IName
	{
	}
	public class IName_Fields
	{
		public static String NAME;
	}
	public interface IFamily
	{
	}
	public class IFamily_Fields
	{
		public static String FAMILY;
	}
	public class FullName : IName, IFamily
						  {
		public String getName()
		{
			return Test.IName_Fields.NAME + Test.IFamily_Fields.FAMILY;
		}
	}
}