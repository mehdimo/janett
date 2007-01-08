namespace Janett.Commons
{
	using System.Collections;
	using System.IO;
	using System.Text.RegularExpressions;

	public class FileSet
	{
		private string baseDirectory;

		private ArrayList includePatterns = new ArrayList();
		private ArrayList excludePatterns = new ArrayList();

		private IList includesRegex;
		private IList excludesRegex;

		private IList files;
		private IList directories;

		public ArrayList Includes
		{
			get { return includePatterns; }
			set { includePatterns = value; }
		}

		public ArrayList Excludes
		{
			get { return excludePatterns; }
			set { excludePatterns = value; }
		}

		public IList Files
		{
			get
			{
				if (files == null)
					files = GetFiles();
				return files;
			}
			set { files = value; }
		}

		public IList Directories
		{
			get
			{
				if (directories == null)
					directories = GetDirectories();
				return directories;
			}
			set { directories = value; }
		}

		public FileSet()
		{
		}

		public FileSet(string baseDirectory)
		{
			this.baseDirectory = baseDirectory;
		}

		public void Load()
		{
			if (baseDirectory == null)
				baseDirectory = ".";
			baseDirectory = Path.GetFullPath(GetPathWithApporopriateDirectorySeparatorChar(baseDirectory));
			IList preparedIncludePatterns = PreparePatterns(baseDirectory, includePatterns);
			IList preparedExcludePatterns = PreparePatterns(baseDirectory, excludePatterns);
			includesRegex = new ArrayList();
			excludesRegex = new ArrayList();
			foreach (string pattern in preparedIncludePatterns)
				includesRegex.Add(GetRegex(pattern));
			foreach (string pattern in preparedExcludePatterns)
				excludesRegex.Add(GetRegex(pattern));
		}

		private IList GetFiles()
		{
			ArrayList allFiles = new ArrayList();
			if (baseDirectory != null)
				allFiles.AddRange(GetAllFiles(baseDirectory));

			IList includes = new ArrayList();
			if (includePatterns.Count != 0)
				includes = GetMatches(allFiles, includesRegex);

			IList excludes = GetMatches(allFiles, excludesRegex);
			foreach (string file in excludes)
			{
				if (includes.Contains(file))
					includes.Remove(file);
			}
			return includes;
		}

		private IList GetDirectories()
		{
			ArrayList allFolders = new ArrayList();
			if (baseDirectory != null)
				allFolders.AddRange(GetAllFolders(baseDirectory));

			IList includes = new ArrayList();
			if (includePatterns.Count != 0)
				includes = GetMatches(allFolders, includesRegex);

			IList excludes = GetMatches(allFolders, excludesRegex);
			foreach (string file in excludes)
			{
				if (includes.Contains(file))
					includes.Remove(file);
			}
			return includes;
		}

		private ArrayList GetAllFiles(string folderPath)
		{
			ArrayList result = new ArrayList();
			foreach (string file in Directory.GetFiles(folderPath))
				result.Add(file);
			foreach (string folder in Directory.GetDirectories(folderPath))
				result.AddRange(GetAllFiles(folder));
			return result;
		}

		private ArrayList GetAllFolders(string folderPath)
		{
			ArrayList result = new ArrayList();
			foreach (string folder in Directory.GetDirectories(folderPath))
			{
				result.Add(folder);
				result.AddRange(GetAllFolders(folder));
			}
			return result;
		}

		private IList GetMatches(ArrayList allFiles, IList regexes)
		{
			IList result = new ArrayList();
			foreach (string file in allFiles)
			{
				if (IsMatch(file, regexes))
					result.Add(file);
			}
			return result;
		}

		private bool IsMatch(string file, IList regexes)
		{
			foreach (Regex reg in regexes)
			{
				Match m = reg.Match(file);
				if (m != null && m.Value == file)
					return true;
			}
			return false;
		}

		private Regex GetRegex(string pattern)
		{
			string regexPattern = GetRegexPattern(pattern);
			return new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		private string GetRegexPattern(string pattern)
		{
			string regexPattern = pattern;
			if (pattern.EndsWith("**"))
				regexPattern = pattern + @"\*";
			regexPattern = regexPattern.Replace(@"\", @"\\");
			regexPattern = regexPattern.Replace(".", @"\.");
			regexPattern = regexPattern.Replace("?", "<");
			regexPattern = regexPattern.Replace(@"\\**", @"\\<>");
			regexPattern = regexPattern.Replace("*", "::");
			regexPattern = regexPattern.Replace(@"\\<>", @"(\\::)*");
			regexPattern = regexPattern.Replace("<", @"([0-9a-zA-Z\._-]?)");
			regexPattern = regexPattern.Replace("::", @"[0-9a-zA-Z\._-]*");
			regexPattern = regexPattern.Replace('\\', Path.DirectorySeparatorChar);
			return regexPattern;
		}

		private ArrayList PreparePatterns(string baseDir, ArrayList patterns)
		{
			ArrayList result = new ArrayList();
			foreach (string pattern in patterns)
			{
				string p = GetPathWithApporopriateDirectorySeparatorChar(pattern);
				string preparedPattern = "";
				if (baseDir != null)
				{
					preparedPattern += Path.GetFullPath(baseDir) + Path.DirectorySeparatorChar;
					preparedPattern += p;
				}
				else
				{
					preparedPattern += Path.GetFullPath(".") + Path.DirectorySeparatorChar + p;
				}

				result.Add(preparedPattern);
			}

			IList extraPreparedPattern = new ArrayList();
			foreach (string preparedPattern in result)
			{
				if (preparedPattern.EndsWith("**"))
					extraPreparedPattern.Add(preparedPattern.Substring(0, preparedPattern.Length - 3));
			}
			result.AddRange(extraPreparedPattern);

			return result;
		}

		private string GetPathWithApporopriateDirectorySeparatorChar(string path)
		{
			if (path != null)
			{
				string result = path;
				result = result.Replace('/', Path.DirectorySeparatorChar);
				result = result.Replace('\\', Path.DirectorySeparatorChar);
				return result;
			}
			else
			{
				return null;
			}
		}
	}
}