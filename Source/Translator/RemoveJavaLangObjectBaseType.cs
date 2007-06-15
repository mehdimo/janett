namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	[Mode("DotNet")]
	[Explicit]
	public class RemoveJavaLangObjectBaseType : Transformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			TypeReference objectBase = new TypeReference("java.lang.Object");
			typeDeclaration = RemoveBaseTypeFrom(typeDeclaration, objectBase);

			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}
	}
}