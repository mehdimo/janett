namespace Janett.Translator
{
	using Framework;

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