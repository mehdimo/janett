namespace Janett.Commons
{
	using System.IO;

	public class FileSystemUtil
	{
		public static string ReadFile(string file)
		{
			using (StreamReader reader = new StreamReader(file))
			{
				return reader.ReadToEnd();
			}
		}

		public static void WriteFile(string file, string contents)
		{
			using (StreamWriter writer = new StreamWriter(file))
			{
				writer.Write(contents);
			}
		}
	}
}