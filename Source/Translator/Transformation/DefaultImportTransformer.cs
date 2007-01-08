namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	[Mode("IKVM")]
	public class DefaultImportTransformer : Transformer
	{
		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			NamespaceDeclaration replacedNamespace = namespaceDeclaration;
			UsingDeclaration usingDeclaration = new UsingDeclaration("java.lang.*");
			replacedNamespace.Children.Insert(0, usingDeclaration);

			ReplaceCurrentNode(replacedNamespace);

			return base.TrackedVisitNamespaceDeclaration(namespaceDeclaration, data);
		}
	}
}