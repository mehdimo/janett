namespace Janett.Commons
{
	using System.Collections;
	using System.IO;

	public class KeyValuePairReader
	{
		private IDictionary sections = new Hashtable();

		public string GetKey(string key)
		{
			return GetKey("Default", key);
		}

		public string GetKey(string section, string key)
		{
			return GetKeys(section)[key];
		}

		public KeyValuesDictionary GetKeys()
		{
			return GetKeys("Default");
		}

		public KeyValuesDictionary GetKeys(string section)
		{
			if (sections.Contains(section))
				return (KeyValuesDictionary) sections[section];
			else
				return new KeyValuesDictionary();
		}

		public KeyValuePairReader()
		{
		}

		public KeyValuePairReader(string file)
		{
			string section = "Default";
			IDictionary keys = new KeyValuesDictionary();
			sections.Add(section, keys);
			using (StreamReader streamReader = new StreamReader(file))
			{
				string line = streamReader.ReadLine();
				while (line != null)
				{
					line = line.Trim();
					if (line.StartsWith("["))
					{
						section = line.Substring(1, line.Length - 2);
						keys = new KeyValuesDictionary();
						sections.Add(section, keys);
					}
					else if (line != "" && !line.StartsWith("-") && !line.StartsWith("#"))
					{
						int sepratorIndex = line.IndexOfAny(new char[] {'-', '=', ':'});
						string key = line.Substring(0, sepratorIndex);
						string value = line.Substring(sepratorIndex + 1);
						keys.Add(key, value);
					}
					line = streamReader.ReadLine();
				}
			}
		}
	}
}