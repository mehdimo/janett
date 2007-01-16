namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	public class RenameNamespaceRefactoring : Transformer
	{
		public string From;
		public string To;

		public override object TrackedVisitTypeReference(TypeReference typeReference, object data)
		{
			typeReference.Type = Replace(typeReference.Type);
			return base.TrackedVisitTypeReference(typeReference, data);
		}

		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			namespaceDeclaration.Name = Replace(namespaceDeclaration.Name);
			return base.TrackedVisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		private string Replace(string name)
		{
			string newName = name.Replace(From, To);
			if (name == From || newName == name)
				return newName;
			string leading = newName.Substring(To.Length + 1);
			string[] parts = leading.Split('.');
			string newLeading = "";
			foreach (string part in parts)
			{
				newLeading += part[0].ToString().ToUpper() + part.Substring(1) + ".";
			}
			newName = newName.Substring(0, To.Length) + "." + newLeading.TrimEnd('.');
			return newName;
		}

		public override object TrackedVisitUsing(Using @using, object data)
		{
			@using.Name = Replace(@using.Name);
			if (@using.IsAlias)
				@using.Alias.Type = Replace(@using.Alias.Type);
			return base.TrackedVisitUsing(@using, data);
		}
	}
}