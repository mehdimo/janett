namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class AccessorRefactoringHelper : HierarchicalTraverser
	{
		public bool SimilarNameExistsInDependedTypes(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			return SimilarPropertyNameExist(typeDeclaration, methodDeclaration)
			       || ImplementInheritors(typeDeclaration, methodDeclaration)
			       || ImplementSiblings(typeDeclaration, methodDeclaration);
		}

		protected override bool VerifyTypeCondition(TypeDeclaration typeDeclaration, bool detailedCondition)
		{
			return detailedCondition;
		}

		protected override bool VerifyMethodCondition(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			IList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
			string name = methodDeclaration.Name.Substring(3);

			if (Contains(methods, methodDeclaration))
			{
				IList innerTypes = AstUtil.GetChildrenWithType(typeDeclaration, typeof(TypeDeclaration));
				foreach (TypeDeclaration innerType in innerTypes)
				{
					if (innerType.Name == name)
						return true;
				}
				IList constructors = AstUtil.GetChildrenWithType(typeDeclaration, typeof(ConstructorDeclaration));
				foreach (ConstructorDeclaration constructorDeclaration in constructors)
				{
					if (constructorDeclaration.Name == name)
						return true;
				}
				foreach (MethodDeclaration method in methods)
				{
					if (method.Name == name || char.ToUpper(method.Name[0]) + method.Name.Substring(1) == name)
						return true;
				}
			}
			else
			{
				IList properties = AstUtil.GetChildrenWithType(typeDeclaration, typeof(PropertyDeclaration));
				return Contains(properties, name);
			}
			return false;
		}

		private bool SimilarPropertyNameExist(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			return VerifyMethodCondition(typeDeclaration, methodDeclaration);
		}

		private bool Contains(IList properties, string name)
		{
			foreach (PropertyDeclaration property in properties)
			{
				if (property.Name == name + "_Property")
					return true;
			}
			return false;
		}
	}
}