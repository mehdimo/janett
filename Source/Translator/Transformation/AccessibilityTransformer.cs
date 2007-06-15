namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class AccessibilityTransformer : Transformer
	{
		private Modifiers defaultModifier = Modifiers.Internal | Modifiers.Protected;

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			Modifiers typeDefaultModifier = Modifiers.Public;

			if (HasNoAccessibility(typeDeclaration))
			{
				AstUtil.AddModifierTo(typeDeclaration, typeDefaultModifier);
			}
			else if (typeDeclaration.Parent is TypeDeclaration)
			{
				if (AstUtil.ContainsModifier(typeDeclaration, Modifiers.Protected))
					AstUtil.AddModifierTo(typeDeclaration, Modifiers.Internal);
			}

			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		public override object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) fieldDeclaration.Parent;
			if (typeDeclaration.Type != ClassType.Interface)
			{
				if (HasNoAccessibility(fieldDeclaration))
				{
					AstUtil.AddModifierTo(fieldDeclaration, defaultModifier);
				}
			}

			return base.TrackedVisitFieldDeclaration(fieldDeclaration, data);
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (((TypeDeclaration) methodDeclaration.Parent).Type != ClassType.Interface)
			{
				if (HasNoAccessibility(methodDeclaration))
				{
					AstUtil.AddModifierTo(methodDeclaration, defaultModifier);
				}
			}
			if (AstUtil.ContainsModifier(methodDeclaration, Modifiers.Protected))
			{
				AstUtil.AddModifierTo(methodDeclaration, Modifiers.Internal);
			}

			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}

		public override object TrackedVisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if (HasNoAccessibility(constructorDeclaration))
			{
				if (!AstUtil.ContainsModifier(constructorDeclaration, Modifiers.Static))
					AstUtil.AddModifierTo(constructorDeclaration, defaultModifier);
			}
			else if (AstUtil.ContainsModifier(constructorDeclaration, Modifiers.Protected))
			{
				AstUtil.AddModifierTo(constructorDeclaration, Modifiers.Internal);
			}

			return base.TrackedVisitConstructorDeclaration(constructorDeclaration, data);
		}

		private bool HasNoAccessibility(AttributedNode node)
		{
			if (!AstUtil.ContainsModifier(node, Modifiers.Public) &&
			    !AstUtil.ContainsModifier(node, Modifiers.Private) &&
			    !AstUtil.ContainsModifier(node, Modifiers.Protected))
				return true;

			return false;
		}
	}
}