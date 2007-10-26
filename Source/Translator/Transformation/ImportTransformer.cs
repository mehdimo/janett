namespace Janett.Translator
{
	using Framework;

	using ICSharpCode.NRefactory.Ast;

	public class ImportTransformer : Transformer
	{
		private string[] keys = new string[] {"event", "params", "ref"};

		public override object TrackedVisitUsing(Using usi, object data)
		{
			string key = ContainsKey(usi.Name);
			if (key != null)
				usi.Name = usi.Name.Replace(key, "@" + key);

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

		private string ContainsKey(string usingName)
		{
			foreach (string key in keys)
			{
				if (usingName.IndexOf('.' + key + '.') != -1)
					return key;
			}
			return null;
		}
	}
}