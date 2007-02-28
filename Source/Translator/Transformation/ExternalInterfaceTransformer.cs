namespace Janett.Translator
{
	using Janett.Framework;

	[Explicit]
	[Mode("IKVM")]
	public class ExternalInterfaceTransformer : InterfaceTransformer
	{
		public ExternalInterfaceTransformer()
		{
			fieldsClassSuffix = ".__Fields";
		}
	}
}