namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory;

	public class CodeBase
	{
		public TypeDictionary Types;
		public Mappings Mappings;
		public IDictionary References = new Hashtable();

		public CodeBase(SupportedLanguage language)
		{
			Types = new TypeDictionary(language);
		}
	}
}