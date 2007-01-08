namespace Janett.Commons
{
	using System.Collections;

	using NUnit.Framework;

	[TestFixture]
	public class KeyValuePairsReaderTest
	{
		[Test]
		public void Default()
		{
			KeyValuePairReader reader = new KeyValuePairReader(@"../../Commons/TestData/Default.options");
			KeyValuesDictionary keys = reader.GetKeys();
			CheckKey(keys, "Key1", "Value1");
			CheckKey(keys, "Key2", "Value2");

			string value = keys["NotExists"];
			Assert.AreEqual("", value);

			Assert.AreEqual("Value1", reader.GetKey("Key1"));
			Assert.AreEqual("Value2", reader.GetKey("Key2"));

			string[] values = keys.GetValues("Key3");
			Assert.AreEqual(3, values.Length);
			Assert.AreEqual("Value3", values[0]);
			Assert.AreEqual("Value4", values[1]);
			Assert.AreEqual("Value5", values[2]);

			values = keys.GetValues("NotExists");
			Assert.AreEqual(0, values.Length);
		}

		[Test]
		public void Sections()
		{
			KeyValuePairReader reader = new KeyValuePairReader(@"../../Commons/TestData/WithSections.options");

			IDictionary defaultSection = reader.GetKeys();
			CheckKey(defaultSection, "Key", "Value");

			IDictionary section1 = reader.GetKeys("Section1");
			CheckKey(section1, "Key1", "Value1");
			CheckKey(section1, "Key2", "Value2");

			Assert.AreEqual("Value1", reader.GetKey("Section1", "Key1"));
			Assert.AreEqual("Value2", reader.GetKey("Section1", "Key2"));

			IDictionary section2 = reader.GetKeys("Section2");
			CheckKey(section2, "Key3", "Value3");
			CheckKey(section2, "Key4", "Value4");

			Assert.AreEqual("Value3", reader.GetKey("Section2", "Key3"));
			Assert.AreEqual("Value4", reader.GetKey("Section2", "Key4"));

			IDictionary na = reader.GetKeys("NotExists");
			Assert.AreEqual(0, na.Count);
		}

		private static void CheckKey(IDictionary keys, string key, string value)
		{
			Assert.IsTrue(keys.Contains(key));
			Assert.AreEqual(value, keys[key]);
		}
	}
}