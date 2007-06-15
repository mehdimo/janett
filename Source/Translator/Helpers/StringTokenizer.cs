namespace Helpers
{
	using System.Collections;
	using System.Text;

	public class StringTokenizer
	{
		private string[] parts;
		private int i = 0;

		public StringTokenizer(string str, string delimiters)
		{
			char[] chars = Encoding.UTF8.GetChars(Encoding.UTF8.GetBytes(delimiters));
			string[] splitted = str.Split(chars);

			ArrayList list = new ArrayList();
			for (int i = 0; i < splitted.Length; i++)
				if (splitted[i] != "")
					list.Add(splitted[i]);

			parts = (string[]) list.ToArray(typeof(string));
		}

		public StringTokenizer(string str, string delimiters, bool flag) : this(str, delimiters)
		{
		}

		public bool hasMoreTokens()
		{
			return (i < parts.Length);
		}

		public string nextToken()
		{
			return parts[i++];
		}

		public int countTokens()
		{
			return parts.Length;
		}
	}
}