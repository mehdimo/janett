namespace Janett.Tools
{
	using ICSharpCode.NRefactory.Ast;

	using NUnit.Framework;

	[TestFixture]
	public class SignatureParserTest
	{
		[Test]
		public void GetReturnType()
		{
			SignatureParser parser = new SignatureParser();
			TypeReference typeReference = parser.GetReturnType("()I");
			Assert.AreEqual("java.lang.Integer", typeReference.Type);
			Assert.IsNull(typeReference.RankSpecifier);

			typeReference = parser.GetReturnType("()[B");
			Assert.AreEqual("java.lang.Byte", typeReference.Type);
			Assert.AreEqual(new int[1], typeReference.RankSpecifier);

			Assert.AreEqual("java.lang.Boolean", parser.GetReturnType("(Ljava.lang.Object;)Z").Type);

			typeReference = parser.GetReturnType("(C)Ljava.lang.Character$UnicodeBlock;");
			Assert.AreEqual("java.lang.Character.UnicodeBlock", typeReference.Type);

			typeReference = parser.GetReturnType("(Ljava.lang.String;)Ljava.lang.Package;");
			Assert.AreEqual("java.lang.Package", typeReference.Type);

			typeReference = parser.GetReturnType("()[Ljava.lang.Package;");
			Assert.AreEqual("java.lang.Package", typeReference.Type);
			Assert.AreEqual(new int[1], typeReference.RankSpecifier);
		}

		[Test]
		public void GetArguments()
		{
			SignatureParser parser = new SignatureParser();
			Assert.AreEqual(0, parser.GetArgumentTypes("()I").Length);

			TypeReference[] types = parser.GetArgumentTypes("(java.lang.Object;)Z");
			Assert.AreEqual(1, types.Length);
			Assert.AreEqual("java.lang.Object", types[0].Type);

			types = parser.GetArgumentTypes("([java.lang.String;)Ljava.lang.Package;");
			Assert.AreEqual("java.lang.String", types[0].Type);
			Assert.AreEqual(new int[1], types[0].RankSpecifier);

			types = parser.GetArgumentTypes("(DD)V");
			Assert.AreEqual(2, types.Length);
			Assert.AreEqual("java.lang.Double", types[0].Type);
			Assert.AreEqual("java.lang.Double", types[1].Type);

			types = parser.GetArgumentTypes("(II[BI)V");
			Assert.AreEqual(4, types.Length);
			Assert.AreEqual("java.lang.Integer", types[0].Type);
			Assert.AreEqual("java.lang.Integer", types[1].Type);
			Assert.AreEqual("java.lang.Byte", types[2].Type);
			Assert.AreEqual(new int[1], types[2].RankSpecifier);
			Assert.AreEqual("java.lang.Integer", types[3].Type);
		}
	}
}