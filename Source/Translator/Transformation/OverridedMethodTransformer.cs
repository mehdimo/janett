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
			CheckForOverriding(typeDeclaration, methodDeclaration);

			return null;
		}

		private void CheckForOverriding(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			if (IsClass(typeDeclaration) && typeDeclaration.BaseTypes.Count > 0)
			{
				string baseType = GetFullName(((TypeReference) typeDeclaration.BaseTypes[0]));
				bool flag = true;
				if (Mode == "DotNet")
					flag = !ExistsInExternalExceptObject(baseType);
				if (CodeBase.Types.Contains(baseType) && flag && !IsOverridableObjectMethod(baseType, methodDeclaration.Name))
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
						else
							CheckForOverriding(baseTypeDeclaration, methodDeclaration);
					}
				}
			}
		}

		private bool IsOverridableObjectMethod(string type, string method)
		{
			IList list = new ArrayList();
			list.Add("toString");
			list.Add("hashCode");
			list.Add("equals");

			return (type == "java.lang.Object" && !list.Contains(method));
		}

		private bool ExistsInExternalExceptObject(string typeName)
		{
			string ns = typeName.Substring(0, typeName.LastIndexOf('.'));

			if (CodeBase.Types.ExternalLibraries.Contains(ns))
			{
				if (typeName != "java.lang.Object")
					return true;
				else
					return false;
			}
			else
				return false;
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