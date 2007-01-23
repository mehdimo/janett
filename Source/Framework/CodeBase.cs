namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory;

	using Janett.Commons;

	public class CodeBase
	{
		public TypeDictionary Types;
		public Mappings Mappings;
		public IDictionary References = new Hashtable();
		public MultiValuedDictionary Inheritors = new MultiValuedDictionary();

		public CodeBase(SupportedLanguage language)
		{
			Types = new TypeDictionary(language);
		}
	}
}