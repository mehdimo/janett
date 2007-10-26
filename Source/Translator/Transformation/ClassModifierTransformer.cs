namespace Janett.Translator
{
	using Framework;

	using ICSharpCode.NRefactory.Ast;

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