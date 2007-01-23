namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class OverridedMethodTransformer : MethodRelatedTransformer
	{
		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) methodDeclaration.Parent;
			if (IsClass(typeDeclaration) && typeDeclaration.BaseTypes.Count > 0)
			{
				string baseType = GetFullName(((TypeReference) typeDeclaration.BaseTypes[0]));
				if (CodeBase.Types.Contains(baseType))
				{
					TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[baseType];
					if (IsClass(baseTypeDeclaration))
					{
						IList abstractParentMethods = AstUtil.GetChildrenWithType(baseTypeDeclaration,
						                                                          typeof(MethodDeclaration));
						if (Contains(abstractParentMethods, methodDeclaration))
						{
							VirtualizeParentMethod(abstractParentMethods, methodDeclaration);
							MethodDeclaration overrideMethod;
							overrideMethod = new MethodDeclaration(methodDeclaration.Name,
							                                       methodDeclaration.Modifier | Modifiers.Override,
							                                       methodDeclaration.TypeReference,
							                                       methodDeclaration.Parameters,
							                                       methodDeclaration.Attributes);
							AstUtil.RemoveModifierFrom(overrideMethod, Modifiers.Virtual);
							overrideMethod.Body = methodDeclaration.Body;
							overrideMethod.Parent = methodDeclaration.Parent;

							ReplaceCurrentNode(overrideMethod);
						}
					}
				}
			}

			return null;
		}

		private void VirtualizeParentMethod(IList methods, MethodDeclaration methodDeclaration)
		{
			int parentIndex = IndexOf(methods, methodDeclaration);
			MethodDeclaration parentMethod = (MethodDeclaration) methods[parentIndex];
			if (!AstUtil.ContainsModifier(parentMethod, Modifiers.Abstract) && !AstUtil.ContainsModifier(parentMethod, Modifiers.Override))
				AstUtil.AddModifierTo(parentMethod, Modifiers.Virtual);

			MatchMethodsModifier(parentMethod, methodDeclaration);
		}

		private bool IsClass(TypeDeclaration type)
		{
			return (type.Type == ClassType.Class);
		}

		private void MatchMethodsModifier(MethodDeclaration baseMethod, MethodDeclaration method)
		{
			if (AstUtil.ContainsModifier(baseMethod, Modifiers.Protected | Modifiers.Internal))
			{
				if (!AstUtil.ContainsModifier(method, Modifiers.Protected | Modifiers.Internal))
					AstUtil.ReplaceModifiers(method, Modifiers.Public, Modifiers.Protected | Modifiers.Internal);
			}
		}
	}
}