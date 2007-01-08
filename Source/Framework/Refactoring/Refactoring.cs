namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class Refactoring : Transformer
	{
		protected bool IsInExternalLibraries(string name)
		{
			foreach (string nameSpace in CodeBase.Types.ExternalLibraries)
			{
				if (name.StartsWith(nameSpace + "."))
					return true;
			}
			return false;
		}

		protected bool IsMethodInExternalTypes(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			bool found = false;
			foreach (TypeReference baseType in typeDeclaration.BaseTypes)
			{
				string fullName = GetFullName(baseType);
				if (CodeBase.Types.Contains(fullName))
				{
					TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[fullName];

					if (IsInExternalLibraries(fullName))
					{
						IList methods = AstUtil.GetChildrenWithType(baseTypeDeclaration, typeof(MethodDeclaration));
						if (ContainsMethod(methods, methodDeclaration))
						{
							found = true;
							break;
						}
						if (!found)
							found = IsMethodInExternalTypes(baseTypeDeclaration, methodDeclaration);
					}
					else
						found = IsMethodInExternalTypes(baseTypeDeclaration, methodDeclaration);
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