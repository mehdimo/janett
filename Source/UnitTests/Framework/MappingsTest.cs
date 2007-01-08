namespace Janett.Framework
{
	using System.Collections;

	using NUnit.Framework;

	[TestFixture]
	public class MappingsTest
	{
		[Test]
		public void GetMappings()
		{
			string folder = @"../../Framework/TestData/Mappings";
			Mappings mapping = new Mappings(folder);

			Assert.IsNotNull(mapping);
			Assert.AreEqual(8, mapping.Count);

			IList specials = GetSpecialMaps(mapping.Keys);
			Assert.AreEqual(1, specials.Count);

			Assert.IsNotNull(mapping["java.lang.StringBuffer"]);

			IDictionary ressField = mapping["java.lang.StringBuffer"].Members;
			Assert.AreEqual(4, ressField.Count);
		}

		private IList GetSpecialMaps(ICollection cols)
		{
			IList list = new ArrayList();
			foreach (string str in cols)
			{
				if (str.IndexOf('.') == -1)
					list.Add(str);
			}
			return list;
		}
	}
}