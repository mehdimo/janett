namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	public abstract class HierarchicalTraverser : MethodRelatedTransformer
	{
		protected abstract bool VerifyTypeCondition(TypeDeclaration typeDeclaration, bool detailedCondition);
		protected abstract bool VerifyMethodCondition(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration);

		protected bool ImplementInheritors(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			string fullName = GetFullName(typeDeclaration);
			bool flag = false;
			if (fullName != null)
			{
				foreach (string inherited in CodeBase.Inheritors[fullName])
				{
					if (CodeBase.Types.Contains(inherited))
					{
						TypeDeclaration inheritedType = (TypeDeclaration) CodeBase.Types[inherited];
						bool detailedCondition = VerifyMethodCondition(inheritedType, methodDeclaration);
						if (VerifyTypeCondition(inheritedType, detailedCondition))
							flag = detailedCondition;
						else
							flag = ImplementInheritors(inheritedType, methodDeclaration);
					}
					if (flag)
						return flag;
				}
			}
			return false;
		}

		protected bool ImplementSiblings(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			bool implemented = false;
			foreach (TypeReference baseType in typeDeclaration.BaseTypes)
			{
				string fullBaseType = GetFullName(baseType);
				TypeDeclaration superType = (TypeDeclaration) CodeBase.Types[fullBaseType];
				if (superType != null)
				{
					implemented = ImplementInheritors(superType, methodDeclaration);
					if (!implemented)
						implemented = ImplementSiblings(superType, methodDeclaration);
				}
				if (implemented)
					return true;
			}
			return implemented;
		}
	}
}