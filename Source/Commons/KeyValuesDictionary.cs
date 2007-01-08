namespace Janett.Commons
{
	using System.Collections;

	public class KeyValuesDictionary : SortedList
	{
		public new string this[object key]
		{
			get
			{
				if (base.Contains(key))
					return (string) base[key];
				else
					return "";
			}
			set { base[key] = value; }
		}

		public string[] GetValues(string key)
		{
			if (!base.Contains(key) || this[key] == "")
				return new string[0];
			else
			{
				string value = this[key];
				return value.Split(new char[] {',', ';'});
			}
		}
	}
}