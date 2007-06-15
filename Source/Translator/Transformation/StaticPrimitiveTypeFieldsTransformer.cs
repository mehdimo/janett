namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class StaticPrimitiveTypeFieldsTransformer : Transformer
	{
		public override object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			if (AstUtil.ContainsModifier(fieldDeclaration, Modifiers.Static) && IsJavaPrimitiveType(fieldDeclaration.TypeReference))
			{
				if (fieldDeclaration.TypeReference.RankSpecifier == null || fieldDeclaration.TypeReference.RankSpecifier.Length == 0)
				{
					VariableDeclaration field = (VariableDeclaration) fieldDeclaration.Fields[0];
					if (field.Initializer != null && (field.Initializer is PrimitiveExpression))
						AstUtil.ReplaceModifiers(fieldDeclaration, Modifiers.Static, Modifiers.Const);
				}
			}

			return base.TrackedVisitFieldDeclaration(fieldDeclaration, data);
		}

		private bool IsJavaPrimitiveType(TypeReference typeReference)
		{
			return (TypeReference.PrimitiveTypesJava.Contains(typeReference.Type));
		}
	}
}