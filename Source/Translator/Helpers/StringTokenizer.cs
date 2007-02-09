namespace Helpers
{
	public class StringTokenizer
	{
		private string[] parts;
		private int i = 0;

		public StringTokenizer(string str, string delimiters)
		{
			parts = str.Split(delimiters[0]);
		}

		public bool hasMoreTokens()
		{
			return (i < parts.Length);
		}

		public string nextToken()
		{
			return parts[i++];
		}
	}
}