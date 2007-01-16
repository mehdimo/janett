namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	[Mode("IKVM")]
	public class ShortenReferencesTransformer : Transformer
	{
		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			IList addingUsings = new ArrayList();

			base.TrackedVisitNamespaceDeclaration(namespaceDeclaration, addingUsings);

			if (addingUsings.Count > 0)
			{
				NamespaceDeclaration replaceNamespace = namespaceDeclaration;
				ArrayList children = namespaceDeclaration.Children;
				INode[] childMembers = new INode[children.Count];
				children.CopyTo(childMembers);
				replaceNamespace.Children.Clear();
				foreach (UsingDeclaration usi in addingUsings)
					replaceNamespace.Children.Add(usi);
				foreach (INode node in childMembers)
				{
					replaceNamespace.Children.Add(node);
				}
				ReplaceCurrentNode(replaceNamespace);
			}
			return null;
		}

		public override object TrackedVisitTypeReference(TypeReference typeReference, object data)
		{
			string type = typeReference.Type;
			if (type.StartsWith("System.") && typeReference.Parent != null)
			{
				string name = type.Substring(type.LastIndexOf('.') + 1);

				TypeReference newTypeReference = typeReference;
				newTypeReference.Type = name;
				ReplaceCurrentNode(newTypeReference);

				AddUsing(typeReference, data, type);
			}
			return null;
		}

		public override object TrackedVisitAttribute(Attribute attribute, object data)
		{
			string name = attribute.Name;
			if (name.StartsWith("System."))
			{
				string newName = name.Substring(name.LastIndexOf('.') + 1);
				attribute.Name = newName;
				AddUsing(attribute, data, name);
			}
			return base.TrackedVisitAttribute(attribute, data);
		}

		private bool ContainsUsing(IList usingList, UsingDeclaration usingDec)
		{
			foreach (UsingDeclaration usi in usingList)
			{
				Using usingDeclared = (Using) usingDec.Usings[0];
				Using usiUsing = (Using) usi.Usings[0];

				if (usiUsing.IsAlias && usingDeclared.IsAlias)
				{
					if (usiUsing.Name == usingDeclared.Name &&
					    usiUsing.Alias.Type == usingDeclared.Alias.Type)
						return true;
				}
				else if (!(usiUsing.IsAlias && usingDeclared.IsAlias))
				{
					if (usingDeclared.Name == usiUsing.Name)
						return true;
				}
			}
			return false;
		}

		private void AddUsing(INode currentNode, object data, string name)
		{
			string ns = name.Substring(name.LastIndexOf('.') + 1);

			NamespaceDeclaration namespaceDeclaration = (NamespaceDeclaration) AstUtil.GetParentOfType(currentNode, typeof(NamespaceDeclaration));
			UsingDeclaration usingDeclaration = new UsingDeclaration(ns);
			usingDeclaration.Parent = namespaceDeclaration;
			((Using) usingDeclaration.Usings[0]).Alias = AstUtil.GetTypeReference(name, usingDeclaration);
			IList usings = AstUtil.GetChildrenWithType(namespaceDeclaration, typeof(UsingDeclaration));
			if (! ContainsUsing(usings, usingDeclaration) && !ContainsUsing((IList) data, usingDeclaration))
				((IList) data).Add(usingDeclaration);
		}
	}
}