namespace Janett.Framework
{
	using ICSharpCode.NRefactory;

	using NUnit.Framework;

	[TestFixture]
	public class TypeDictionaryTest : TypeDictionary
	{
		public TypeDictionaryTest() : base(SupportedLanguage.Java)
		{
		}

		[TestFixtureSetUp]
		public void SetUp()
		{
			LibrariesFolder = @"../../Framework/TestData/Libraries";
		}

		[Test]
		public void Folder()
		{
			string type = "net.host.Rectangle";
			Assert.IsTrue(Contains(type));
		}

		[Test]
		public void ZipFileWithDot()
		{
			string type = "com.company.product.Black";
			Assert.IsTrue(Contains(type));
		}

		[Test]
		public void ZipFile()
		{
			string type = "org.foundation.project.util.FileManager";
			Assert.IsTrue(Contains(type));
		}

		[Test]
		public void InnerInterface()
		{
			string type = "net.host.Document$Page";
			Assert.IsTrue(Contains(type));
		}
	}
}