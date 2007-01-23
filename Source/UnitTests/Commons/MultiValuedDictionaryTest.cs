namespace Janett.Commons
{
	using System.Collections;

	using NUnit.Framework;

	[TestFixture]
	public class MultiValuedDictionaryTest
	{
		[Test]
		public void AddValues()
		{
			MultiValuedDictionary multiValuedDictionary = new MultiValuedDictionary();

			multiValuedDictionary.Add("A", "AA");
			multiValuedDictionary.Add("A", "AB");

			multiValuedDictionary.Add("B", "BB");
			multiValuedDictionary.Add("B", "BC");

			Assert.AreEqual(2, multiValuedDictionary.Count);
			IList item0 = multiValuedDictionary["A"];
			Assert.AreEqual(2, item0.Count);
		}
	}
}