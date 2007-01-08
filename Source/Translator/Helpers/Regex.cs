namespace Helpers
{
	using System.Collections;

	public class Regex
	{
		public static string[] Split(string input, string pattern)
		{
			//Replace with non-capturing group
			int pIndex = pattern.IndexOf("(");
			while (pIndex != -1 && pIndex != pattern.Length - 1)
			{
				if (pattern[pIndex + 1] != '?')
					pattern = pattern.Insert(pIndex + 1, "?:");
				pIndex = pattern.IndexOf("(", pIndex + 1);
			}

			string[] parts = System.Text.RegularExpressions.Regex.Split(input, pattern);

			ArrayList list = new ArrayList();
			for (int i = 0; i < parts.Length - 1; i++)
				list.Add(parts[i]);

			if (parts[parts.Length - 1] != "")
				list.Add(parts[parts.Length - 1]);

			return (string[]) list.ToArray(typeof(string));
		}
	}
}