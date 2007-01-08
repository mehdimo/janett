namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class ImportTransformer : Transformer
	{
		public override object TrackedVisitUsing(Using usi, object data)
		{
			if (usi.Name.EndsWith(".*"))
			{
				usi.Name = usi.Name.Substring(0, usi.Name.LastIndexOf('.'));
			}
			else
			{
				string name = usi.Name.Substring(usi.Name.LastIndexOf('.') + 1);
				usi.Alias = AstUtil.GetTypeReference(usi.Name, usi);
				usi.Name = name;
			}
			return base.TrackedVisitUsing(usi, data);
		}
	}
}