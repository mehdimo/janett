namespace Janett.Translator
{
	using System.IO;

	using Janett.Commons;
	using Janett.Framework;

	using NUnit.Framework;

	[TestFixture]
	public class IntegrationTest
	{
		private Janett.Framework.Translator javaTranslator;

		[SetUp]
		public void SetUp()
		{
			javaTranslator = new JavaDotNetTranslator();

			javaTranslator.CreateProjects = false;
			javaTranslator.PreserveChanges = false;

			javaTranslator.InputFolder = @"../../Translator/IntegrationTest/Base";
			javaTranslator.Libraries = @"../../../Translator/Libraries";

			string class1File = Path.Combine(Path.GetFullPath(javaTranslator.InputFolder), "Class1.java");
			javaTranslator.MethodExcludes.Add(class1File, "methodToExclude");
		}

		[Test]
		public void DotNet()
		{
			javaTranslator.Mode = "DotNet";
			javaTranslator.OutputFolder = @"../DotNetTranslated";
			javaTranslator.Mappings = @"../../../Translator/Mappings/DotNet";

			string expectedFolder = @"../../Translator/IntegrationTest/DotNetExpected";

			javaTranslator.Execute();
			CheckResult(expectedFolder, javaTranslator.OutputFolder);
		}

		[Test]
		public void IKVM()
		{
			javaTranslator.Mode = "IKVM";
			javaTranslator.OutputFolder = @"../IKVMTranslated";
			javaTranslator.Mappings = @"../../../Translator/Mappings/IKVM";

			string expectedFolder = @"../../Translator/IntegrationTest/IKVMExpected";

			javaTranslator.Execute();
			CheckResult(expectedFolder, javaTranslator.OutputFolder);
		}

		private void CheckResult(string expectedFolder, string translatedFolder)
		{
			if (Directory.Exists(expectedFolder))
			{
				foreach (Source translated in javaTranslator.Sources.Values)
				{
					if (translated.CodeFile)
					{
						string filepath = translated.OutputFile.Replace(translatedFolder + Path.DirectorySeparatorChar, "");
						filepath = Path.Combine(expectedFolder, filepath);
						string expected = FileSystemUtil.ReadFile(filepath);
						TestUtil.CodeEqual(expected, translated.Code);
					}
				}
			}
		}
	}
}