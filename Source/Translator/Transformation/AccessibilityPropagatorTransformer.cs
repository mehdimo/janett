namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class AccessibilityPropagatorTransformer : HierarchicalTraverser
	{
		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(methodDeclaration, typeof(TypeDeclaration));
			if (AstUtil.ContainsModifier(methodDeclaration, Modifiers.Protected))
			{
				if (ImplementInheritors(typeDeclaration, methodDeclaration)
				    || ImplementSiblings(typeDeclaration, methodDeclaration))
				{
					AstUtil.ReplaceModifiers(methodDeclaration, Modifiers.Protected, Modifiers.Public);
				}
			}

			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}

		private bool IsPublicDefined(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			IList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
			if (Contains(methods, methodDeclaration))
			{
				int index = IndexOf(methods, methodDeclaration);
				MethodDeclaration method = (MethodDeclaration) methods[index];
				if (AstUtil.ContainsModifier(method, Modifiers.Public))
					return true;
			}
			return false;
		}

		protected override bool VerifyMethodCondition(TypeDeclaration type, MethodDeclaration method)
		{
			return IsPublicDefined(type, method);
		}

		protected override bool VerifyTypeCondition(TypeDeclaration type, bool detailedCondition)
		{
			return detailedCondition;
		}
	}
}