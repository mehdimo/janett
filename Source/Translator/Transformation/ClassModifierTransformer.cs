namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class ClassModifierTransformer : Transformer
	{
		private Modifiers removableModifier = Modifiers.Static;

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			AstUtil.RemoveModifierFrom(typeDeclaration, removableModifier);
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}
	}
}