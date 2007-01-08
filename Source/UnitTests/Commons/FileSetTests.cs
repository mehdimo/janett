namespace Janett.Commons
{
	using System.Collections;
	using System.IO;

	using NUnit.Framework;

	[TestFixture]
	public class FileSetTests
	{
		[Test]
		public void WithoutIncludesAndExcludes()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			ArrayList excludes = new ArrayList();

			ArrayList expectedFiles = new ArrayList();
			ArrayList expectedFolders = new ArrayList();

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void WithAllIncludes()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add(@"**\*");

			ArrayList excludes = new ArrayList();

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Abstract.build");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Common.include");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml\XmlUtil.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\3.txt");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\4.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\test.txt");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\XmlUtil.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\xml\1.txt");

			ArrayList expectedFolders = new ArrayList();
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Source");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Xml");

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void IncludesWithDoubleStarAndStar()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add("**/Source/*");

			ArrayList excludes = new ArrayList();

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\3.txt");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\4.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\test.txt");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\XmlUtil.cs");

			ArrayList expectedFolders = new ArrayList();
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml");

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void Test()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add("**/Source/**/*.cs");

			ArrayList excludes = new ArrayList();

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml\XmlUtil.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\4.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\XmlUtil.cs");

			ArrayList expectedFolders = new ArrayList();

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void IncludesThatIsMeaninglessAndReturnsZeroFile()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add("**/Source");

			ArrayList excludes = new ArrayList();
			ArrayList expectedFiles = new ArrayList();

			ArrayList expectedFolders = new ArrayList();
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Source");

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void ExcludeThatIsMeaningless()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add(@"**\*");

			ArrayList excludes = new ArrayList();
			excludes.Add("**/Source");

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Abstract.build");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Common.include");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml\XmlUtil.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\3.txt");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\4.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\test.txt");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\XmlUtil.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\xml\1.txt");

			ArrayList expectedFolders = new ArrayList();
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library\test");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Xml");

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void ExcludeThatIsMeaningfull()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add(@"**\*");

			ArrayList excludes = new ArrayList();
			excludes.Add("**/Source/*");

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Abstract.build");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Common.include");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml\XmlUtil.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\xml\1.txt");

			ArrayList expectedFolders = new ArrayList();
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library\test");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Source");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Xml");

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void IncludesWithDoubleStarInStartAndEndOfPattern()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add("**/Test/**");

			ArrayList excludes = new ArrayList();

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test2.cs");

			ArrayList expectedFolders = new ArrayList();
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test");

			FileSet fs = AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
			Assert.AreEqual(1, fs.Includes.Count);
		}

		[Test]
		public void IncludesWithDoubleStarInStarAndEndOfPatternAndQuestionMark()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add("**/Library/**/Test/Test?.?s");

			ArrayList excludes = new ArrayList();

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test2.cs");

			ArrayList expectedFolders = new ArrayList();
			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void IncludesWithDoubleStarAndStarDotFileType()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add("**/Source/*.txt");

			ArrayList excludes = new ArrayList();

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\3.txt");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\test.txt");

			ArrayList expectedFolders = new ArrayList();
			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void IncludesWithDoubleStar()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add("Commons/**");

			ArrayList excludes = new ArrayList();

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml\XmlUtil.cs");

			ArrayList expectedFolders = new ArrayList();
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml");

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void WithExcludes()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add("**/*");

			ArrayList excludes = new ArrayList();
			excludes.Add("**/Source/*");

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Abstract.build");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Common.include");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\Xml\XmlUtil.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\xml\1.txt");

			ArrayList expectedFolders = new ArrayList();
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library\test");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Source");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Xml");

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		[Test]
		public void ExcludesWithDoubleStarInStartAndEndOfPattern()
		{
			string baseDir = @"..\..\Commons\TestData\Directory";

			ArrayList includes = new ArrayList();
			includes.Add(@"**\*");

			ArrayList excludes = new ArrayList();
			excludes.Add("**/xml/**");

			ArrayList expectedFiles = new ArrayList();
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Abstract.build");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Common.include");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test\Test2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\1.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Commons\Source\2.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\3.txt");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\4.cs");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\test.txt");
			expectedFiles.Add(@"..\..\Commons\TestData\Directory\Source\XmlUtil.cs");

			ArrayList expectedFolders = new ArrayList();
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Library\Test");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Commons\Source");
			expectedFolders.Add(@"..\..\Commons\TestData\Directory\Source");

			AssertFileSetFilesAndDirectories(baseDir, includes, excludes, expectedFiles, expectedFolders);
		}

		private FileSet AssertFileSetFilesAndDirectories(string baseDir, ArrayList includes, ArrayList excludes,
		                                                 ArrayList expectedFiles, ArrayList expectedDirectories)
		{
			excludes.Add("**/.svn/**");
			expectedFiles = PrepareExpected(expectedFiles);
			expectedDirectories = PrepareExpected(expectedDirectories);
			FileSet fs = new FileSet(baseDir);
			fs.Includes = includes;
			fs.Excludes = excludes;
			fs.Load();

			Compare(expectedFiles, fs.Files);
			Compare(expectedDirectories, fs.Directories);
			return fs;
		}

		private static void Compare(IList expected, IList results)
		{
			string resultsFormat = "Expected '{0}' does not exists in results";
			for (int i = 0; i < expected.Count; i++)
			{
				if (i >= results.Count)
					Assert.Fail(string.Format(resultsFormat, expected[i]));
				if (expected[i].ToString().ToLower() != results[i].ToString().ToLower())
					Assert.Fail(string.Format(resultsFormat + ". There is '{1}' in place of that", expected[i], results[i]));
			}
			string expectedFormat = "'{0}' in results is not expected";

			for (int i = 0; i < results.Count; i++)
			{
				if (i >= expected.Count)
					Assert.Fail(string.Format(expectedFormat, results[i]));
				if (expected[i].ToString().ToLower() != results[i].ToString().ToLower())
					Assert.Fail(string.Format(expectedFormat, results[i]));
			}
		}

		private static ArrayList PrepareExpected(ArrayList expectedFiles)
		{
			ArrayList newExpected = new ArrayList();
			foreach (string file in expectedFiles)
			{
				newExpected.Add(Path.GetFullPath(file));
			}
			return newExpected;
		}
	}
}