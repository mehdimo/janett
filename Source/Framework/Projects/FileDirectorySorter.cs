namespace Janett.Framework
{
	using System.Collections;

	public class FileDirectoryComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			string file1 = x.ToString();
			string file2 = y.ToString();

			return Compare(file1, file2);
		}

		private int Compare(string file1, string file2)
		{
			int file1Index = file1.IndexOf('\\');
			int file2Index = file2.IndexOf('\\');
			if (file1Index == -1)
			{
				if (file2Index == -1)
					return file1.CompareTo(file2);
				else
					return -1;
			}
			else
			{
				if (file2Index == -1)
					return 1;
			}

			string file1First = file1.Substring(0, file1Index);
			string file2First = file2.Substring(0, file2Index);

			if (file1First != file2First)
				return file1First.CompareTo(file2First);
			else
				return Compare(file1.Substring(file1Index + 1), file2.Substring(file2Index + 1));
		}
	}
}