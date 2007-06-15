namespace Helpers
{
	using System.Collections;

	public class Properties : Hashtable
	{
		private static Hashtable properties;

		static Properties()
		{
			properties = new Hashtable();
		}

		public void load(System.IO.Stream stream)
		{
		}

		public string getProperty(string name, string def)
		{
			return properties[name].ToString();
		}

		public void setProperty(string key, string value)
		{
			properties[key] = value;
		}

		public IEnumerator propertyNames()
		{
			return properties.Keys.GetEnumerator();
		}

		public override ICollection Keys
		{
			get { return properties.Keys; }
		}

		public override object this[object key]
		{
			get { return properties[key.ToString()]; }
		}
	}
}