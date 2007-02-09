namespace Helpers
{
	using System.IO;

	public class File
	{
		public static FileInfo[] getFiles(FileInfo dir)
		{
			string[] filePaths = Directory.GetFiles(dir.FullName);
			FileInfo[] files = new FileInfo[filePaths.Length];
			for (int i = 0; i < filePaths.Length; i++)
			{
				files[i] = new FileInfo(filePaths[i]);
			}
			return files;
		}

		public static bool isDirectory(FileInfo info)
		{
			return false;
		}
	}
}