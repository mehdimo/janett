namespace Janett.Translator
{
	using Framework;

	using ICSharpCode.NRefactory.Ast;

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