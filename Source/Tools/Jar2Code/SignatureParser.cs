namespace Janett.Tools
{
	using System.Collections;
	using System.Text.RegularExpressions;

	using ICSharpCode.NRefactory.Ast;

	public class SignatureParser
	{
		private IDictionary types;
		private Regex methodRegex;
		private Regex fieldRegex;

		public SignatureParser()
		{
			types = new Hashtable();
			types.Add("B", "java.lang.Byte");
			types.Add("C", "java.lang.Character");
			types.Add("D", "java.lang.Double");
			types.Add("F", "java.lang.Float");
			types.Add("I", "java.lang.Integer");
			types.Add("J", "java.lang.Long");
			types.Add("S", "java.lang.Short");
			types.Add("Z", "java.lang.Boolean");
			types.Add("V", "java.lang.Void");

			string typePattern = @"\[{0,1}L{0,1}([A-Z]|[\w\.$]+;)";
			string methodPattern = string.Format(@"\(((?<param>{0}))*\)(?<ret>{0})", typePattern);

			methodRegex = new Regex(methodPattern, RegexOptions.Compiled);
			fieldRegex = new Regex(typePattern, RegexOptions.Compiled);
		}

		public TypeReference GetFieldType(string sig)
		{
			Match m = fieldRegex.Match(sig);
			return GetTypeReference(m.Value);
		}

		public TypeReference GetReturnType(string sig)
		{
			Match m = methodRegex.Match(sig);
			string val = m.Groups["ret"].Value;
			return GetTypeReference(val);
		}

		public TypeReference GetTypeReference(string val)
		{
			string type = val.TrimStart('[', 'L').Replace("$", ".").TrimEnd(';');
			if (types.Contains(type))
				type = (string) types[type];
			TypeReference typeRef = new TypeReference(type);
			if (val.StartsWith("["))
				typeRef.RankSpecifier = new int[1] {0};
			return typeRef;
		}

		public TypeReference[] GetArgumentTypes(string sig)
		{
			Match m = methodRegex.Match(sig);
			int count = m.Groups["param"].Captures.Count;
			TypeReference[] arguments = new TypeReference[count];
			for (int i = 0; i < count; i++)
				arguments[i] = GetTypeReference(m.Groups["param"].Captures[i].Value);
			return arguments;
		}
	}
}