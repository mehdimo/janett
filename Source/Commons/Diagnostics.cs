namespace Janett.Commons
{
	using System.Collections;
	using System.Collections.Specialized;
	using System.Diagnostics;

	public class Diagnostics
	{
		public static IDictionary Properties = new ListDictionary();
		public static IDictionary Break;
		private static bool breaked = false;

		public static void BreakOn(string propertiesString)
		{
			if (propertiesString == "")
				return;
			int endOfLine = propertiesString.IndexOf('\r');
			if (endOfLine == 0)
				return;

			Break = new ListDictionary();
			if (endOfLine != -1)
				propertiesString = propertiesString.Substring(0, endOfLine);
			string[] keyValues = propertiesString.Split(',');
			foreach (string keyValue in keyValues)
			{
				int index = keyValue.IndexOf('=');
				string key = keyValue.Substring(0, index);
				string value = keyValue.Substring(index + 1);
				Break.Add(key, value);
			}
		}

		public static void Set(string key, string value)
		{
			Properties[key] = value;
			if (Break == null)
				return;
			bool breakDebugger = true;
			foreach (DictionaryEntry entry in Break)
			{
				if (!Properties.Contains(entry.Key) || Properties[entry.Key].ToString() != entry.Value.ToString())
				{
					breakDebugger = false;
					break;
				}
			}
			if (!breaked && breakDebugger)
			{
				breaked = true;
				Debugger.Break();
			}
		}

		public static string GetString()
		{
			string str = "";
			foreach (DictionaryEntry entry in Properties)
			{
				str += entry.Key + "=" + entry.Value + ",";
			}
			return str.TrimEnd(',');
		}

		public static void Remove(string key)
		{
			Properties.Remove(key);
		}
	}
}