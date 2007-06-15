namespace Helpers
{
	public class SystemHelper
	{
		private static Properties properties = new Properties();

		static SystemHelper()
		{
			properties.Add("os.name", System.Environment.OSVersion.ToString());
			properties.Add("os.user", System.Environment.UserName);
		}

		public static string getProperty(object key)
		{
			return properties[key].ToString();
		}
	}
}