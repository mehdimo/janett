namespace Janett.Commons
{
	using System.Collections;

	using NUnit.Framework;

	[TestFixture]
	public class CommandLineParserTest
	{
		[Test]
		public void Parse()
		{
			string[] parameterArray = new string[] {"/Database", "TestDb", "/server", "TestServer"};
			ArrayList parameters = new ArrayList(parameterArray);

			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs), (string[]) parameters.ToArray(typeof(string)));
			CommandLineArgs args = (CommandLineArgs) parser.Parse();
			Assert.AreEqual("TestDb", args.Database);
			Assert.AreEqual("TestServer", args.Server);
			Assert.AreEqual(true, args.Check);

			parameters.Add("/Check-");
			parser = new CommandLineParser(typeof(CommandLineArgs), (string[]) parameters.ToArray(typeof(string)));
			args = (CommandLineArgs) parser.Parse();
			Assert.AreEqual(false, args.Check);
		}

		[Test]
		public void List()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"/Database", "TestDb", "/Excludes", "Tasks,Resources"});
			CommandLineArgs args = (CommandLineArgs) parser.Parse();
			Assert.IsNotNull(args.Excludes);
			Assert.AreEqual(2, args.Excludes.Count);
		}

		[Test]
		public void AlternameName()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"/Database", "TestDb", "/s", "TestServer"});
			CommandLineArgs args = (CommandLineArgs) parser.Parse();
			Assert.AreEqual("TestServer", args.Server);
		}

		[Test]
		public void Place()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgsPlace),
			                                                 new string[] {"Data", "/Database", "TestDb"});
			CommandLineArgsPlace args = (CommandLineArgsPlace) parser.Parse();
			Assert.AreEqual("Data", args.Compare);
			Assert.AreEqual("TestDb", args.Database);
		}

		[Test]
		public void Optional()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"/Database", "TestDb"});
			CommandLineArgs args = (CommandLineArgs) parser.Parse();
			Assert.AreEqual("Default", args.Server);
		}

		[Test, ExpectedException(typeof(CommandLineArgumentException), "Expected argument 'Database'")]
		public void NonOptional()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"/Server", "TestServer"});
			parser.Parse();
		}

		[Test, ExpectedException(typeof(CommandLineArgumentException), "Expected value for argument 'Server'")]
		public void ValueNotSpecifiedIndex()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"/Server"});
			parser.Parse();
		}

		[Test, ExpectedException(typeof(CommandLineArgumentException), "Expected value for argument 'Server'")]
		public void ValueNotSpecified()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"/Server", "/Database"});
			parser.Parse();
		}

		[Test]
		public void GetArgumentValue()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"/OtherOption", "Database", "/Database", "TestDb", "-Server", "TestServer"});
			Assert.AreEqual("TestDb", parser.GetArgumentValue("Database"));
			Assert.AreEqual("TestServer", parser.GetArgumentValue("Server"));
			Assert.AreEqual("Database", parser.GetArgumentValue("OtherOption"));
		}

		[Test]
		public void ArgumentExists()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"/Check", "-Server", "TestServer"});
			Assert.AreEqual(true, parser.ArgumentExists("Check"));
		}

		[Test]
		public void ColonSeparated()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"-server:TestServer"});
			Assert.AreEqual("TestServer", parser.GetArgumentValue("Server"));
		}

		[Test]
		public void WithTwoDash()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {"--Server", "TestServer"});
			Assert.AreEqual("TestServer", parser.GetArgumentValue("Server"));
		}

		[Test]
		public void Usage()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgs),
			                                                 new string[] {""});
			string expected = "Usage: UnitTest [/Server|/s <value>] /Database|/d <value> [/Check|/c] [/Excludes <value>]\r\n\r\n";
			expected += "/Server <value>             Server to compare\r\n";
			expected += "/Database <value>           Database to compare\r\n";
			expected += "/Check                      Check Connection\r\n";
			expected += "/Excludes <value>           Excludes\r\n";
//			System.Console.WriteLine(parser.Usage);
			Assert.AreEqual(expected, parser.Usage);
		}

		[Test]
		public void UsagePlace()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgsPlace),
			                                                 new string[] {""});
			string expected = "Usage: UnitTest <Compare> /Database <value>\r\n\r\n";
			expected += "<Compare>                   Compare Data or Schema\r\n";
			expected += "/Database <value>           Database to compare\r\n";
//			System.Console.WriteLine(parser.Usage);
			Assert.AreEqual(expected, parser.Usage);
		}

		[Test]
		public void PlaceOptional()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgsTwoPlace),
			                                                 new string[] {"Test", "Other"});
			CommandLineArgsTwoPlace cmd = (CommandLineArgsTwoPlace) parser.Parse();
			Assert.AreEqual("Default", cmd.Third);
		}

		[Test, ExpectedException(typeof(CommandLineArgumentException), "Missing parameters")]
		public void PlaceNotSpecfied()
		{
			CommandLineParser parser = new CommandLineParser(typeof(CommandLineArgsTwoPlace),
			                                                 new string[] {"Test"});
			parser.Parse();
		}
	}

	public class CommandLineArgsTwoPlace
	{
		[CommandLineValue("Compare", 1)]
		public string Compare;

		[CommandLineValue("Other", 2)]
		public string Other;

		[CommandLineValue("Third", 3, Optional=true)]
		public string Third = "Default";

		public string AnotherField;
	}

	public class CommandLineArgsPlace
	{
		[CommandLineValue("Compare", 1, Description="Compare Data or Schema")]
		public string Compare;

		[CommandLineValue("Database", Description = "Database to compare")]
		public string Database;
	}

	public class CommandLineArgs
	{
		[CommandLineValue("Server", Optional=true, AlternateName="s", Description = "Server to compare")]
		public string Server = "Default";

		[CommandLineValue("Database", AlternateName="d", Description = "Database to compare")]
		public string Database;

		[CommandLineFlag("Check", Optional=true, AlternateName="c", Description = "Check Connection")]
		public bool Check = true;

		[CommandLineValue("Excludes", Optional=true)]
		public ArrayList Excludes;
	}
}