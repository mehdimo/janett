namespace Janett.Framework
{
	using System.Collections;
	using System.Collections.Generic;

	using ICSharpCode.NRefactory.Ast;

	public class ImplementPropertyRegionTransformer : HierarchicalTraverser
	{
		public bool ShouldAddAccessor(TypeDeclaration typeDeclaration, string methodName, TypeReference typeReference)
		{
			MethodDeclaration method;
			method = new MethodDeclaration(methodName, Modifiers.Public, null, null, null);
			if (methodName.StartsWith("set"))
			{
				method.TypeReference = new TypeReference("void");
				List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
				parameters.Add(new ParameterDeclarationExpression(typeReference, "Parameter1"));
				method.Parameters = parameters;
			}
			else
				method.TypeReference = typeReference;

			if (ImplementInheritors(typeDeclaration, method) || ImplementSiblings(typeDeclaration, method))
				return true;
			else
				return false;
		}

		protected override bool VerifyTypeCondition(TypeDeclaration typeDeclaration, bool detailedCondition)
		{
			return detailedCondition;
		}

		protected override bool VerifyMethodCondition(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			IList properties = AstUtil.GetChildrenWithType(typeDeclaration, typeof(PropertyDeclaration));
			if (HasSectionFor(properties, methodDeclaration))
				return true;
			else
			{
				IList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
				return Contains(methods, methodDeclaration);
			}
		}

		private bool HasSectionFor(IList properties, MethodDeclaration methodDeclaration)
		{
			string accessorType = methodDeclaration.Name.Substring(0, 3);
			string propertyName = methodDeclaration.Name.Substring(3);
			foreach (PropertyDeclaration property in properties)
			{
				if (property.Name == propertyName)
					return (accessorType == "get" && property.HasGetRegion) || (accessorType == "set" && property.HasSetRegion);
			}
			return false;
		}
	}
}