namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class Refactoring : Transformer
	{
		protected bool IsMethodInExternalTypes(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			bool found = false;
			foreach (TypeReference baseType in typeDeclaration.BaseTypes)
			{
				string fullName = GetFullName(baseType);
				if (!found && CodeBase.Types.Contains(fullName))
				{
					TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[fullName];

					if (IsInExternalLibraries(fullName) || fullName.StartsWith("Helpers."))
					{
						IList methods = AstUtil.GetChildrenWithType(baseTypeDeclaration, typeof(MethodDeclaration));
						if (ContainsMethod(methods, methodDeclaration))
							found = true;
						else
							found = IsMethodInExternalTypes(baseTypeDeclaration, methodDeclaration);
					}
					else
						found = IsMethodInExternalTypes(baseTypeDeclaration, methodDeclaration);
					if (found)
						break;
				}
			}
			return found;
		}

		protected bool ContainsMethod(IList methodsList, MethodDeclaration method)
		{
			foreach (MethodDeclaration methodDeclaration in methodsList)
			{
				if (methodDeclaration.Name == method.Name)
					return true;
			}
			return false;
		}
	}
}