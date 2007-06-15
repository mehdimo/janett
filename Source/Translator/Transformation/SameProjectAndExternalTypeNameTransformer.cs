namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	[Explicit]
	public class SameProjectAndExternalTypeNameTransformer : Transformer
	{
		private IDictionary similarTypes;

		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			similarTypes = new Hashtable();
			IList usings = AstUtil.GetChildrenWithType(namespaceDeclaration, typeof(UsingDeclaration));
			foreach (UsingDeclaration usingDeclaration in usings)
			{
				Using usi = (Using) usingDeclaration.Usings[0];
				string fullName = GetFullyQualifiedName(usi);
				string type = GetShortReferenceTypeName(usi);
				string projectType = namespaceDeclaration.Name + "." + type;
				if (CodeBase.Types.Contains(projectType))
					similarTypes.Add(type, fullName);
			}
			if (similarTypes.Count < 1)
				return null;
			else
				return base.TrackedVisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		public override object TrackedVisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			Using usi = (Using) usingDeclaration.Usings[0];
			string type = GetShortReferenceTypeName(usi);
			if (similarTypes.Contains(type))
				RemoveCurrentNode();
			return null;
		}

		public override object TrackedVisitTypeReference(TypeReference typeReference, object data)
		{
			if (similarTypes.Contains(typeReference.Type))
				typeReference.Type = (string) similarTypes[typeReference.Type];
			return base.TrackedVisitTypeReference(typeReference, data);
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (similarTypes.Contains(identifierExpression.Identifier))
			{
				string fullTypeName = (string) similarTypes[identifierExpression.Identifier];
				TypeReferenceExpression typeReferenceExpression = new TypeReferenceExpression(fullTypeName);
				ReplaceCurrentNode(typeReferenceExpression);
			}
			return base.TrackedVisitIdentifierExpression(identifierExpression, data);
		}

		private string GetFullyQualifiedName(Using usi)
		{
			if (usi.IsAlias)
				return usi.Alias.Type;
			else
				return usi.Name;
		}

		private string GetShortReferenceTypeName(Using usi)
		{
			if (usi.IsAlias)
				return usi.Name;
			else
				return usi.Name.Substring(usi.Name.LastIndexOf('.') + 1);
		}
	}
}