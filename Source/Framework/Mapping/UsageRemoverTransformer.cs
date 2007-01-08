namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class UsageRemoverTransformer : Transformer
	{
		protected IList Removeables = new ArrayList();
		public IList UsedTypes = new ArrayList();

		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			Removeables.Clear();
			UsedTypes.Clear();

			NamespaceDeclaration replaced = namespaceDeclaration;
			IList types = AstUtil.GetChildrenWithType(replaced, typeof(TypeDeclaration));
			foreach (TypeDeclaration typeDeclaration in types)
			{
				VisitTypeDeclaration(typeDeclaration, data);
			}
			ReplaceCurrentNode(replaced);
			return base.TrackedVisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		public override object TrackedVisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			Using usi = (Using) usingDeclaration.Usings[0];
			if (usi.IsAlias)
			{
				string type = usi.Alias.Type;
				string usingNamespace = type.Substring(0, type.LastIndexOf('.'));

				if (usingDeclaration.Parent is NamespaceDeclaration)
				{
					NamespaceDeclaration namespaceDeclaration = (NamespaceDeclaration) usingDeclaration.Parent;
					if (namespaceDeclaration.Name == usingNamespace)
					{
						RemoveCurrentNode();
					}
					else if (usingNamespace.StartsWith(namespaceDeclaration.Name))
					{
						string movedType = namespaceDeclaration.Name + usingNamespace.Substring(usingNamespace.LastIndexOf('.'));
						if (CodeBase.Types.Contains(movedType))
							RemoveCurrentNode();
					}
				}
			}

			Intersect(Removeables, UsedTypes);
			if (Removeables.Count > 0)
			{
				if ((!usi.IsAlias && Removeables.Contains(usi.Name)) || (usi.IsAlias && Removeables.Contains(usi.Alias.Type)))
					RemoveCurrentNode();
			}
			if (UsedTypes.Count > 0)
			{
				if ((!usi.IsAlias && !UsedTypes.Contains(usi.Name)) || (usi.IsAlias && !UsedTypes.Contains(usi.Alias.Type)))
					RemoveCurrentNode();
			}
			return base.TrackedVisitUsingDeclaration(usingDeclaration, data);
		}

		private void Intersect(IList baseList, IList list)
		{
			foreach (string item in list)
			{
				if (baseList.Contains(item))
					baseList.Remove(item);
			}
		}
	}
}