namespace Helpers
{
	using System;
	using System.Collections;

	public class SystemHelper
	{
		private static IDictionary properties = new Hashtable();

		static SystemHelper()
		{
			string osName = Environment.OSVersion.ToString();
			if (osName.StartsWith("Microsoft Windows NT 5.1"))
				properties.Add("os.name", "Windows XP");
			properties.Add("user.name", Environment.UserName);
		}

		public static string getProperty(object key)
		{
			return properties[key].ToString();
		}
	}
}