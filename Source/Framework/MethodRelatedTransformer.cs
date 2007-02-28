namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public abstract class MethodRelatedTransformer : Transformer
	{
		public bool Contains(IList methodsList, MethodDeclaration method)
		{
			foreach (MethodDeclaration methodDeclaration in methodsList)
			{
				if (Equals(methodDeclaration, method))
					return true;
			}
			return false;
		}

		protected int IndexOf(IList list, MethodDeclaration method)
		{
			int index = 0;
			foreach (INode element in list)
			{
				if ((element is MethodDeclaration))
					if (Equals((MethodDeclaration) element, method))
						return index;
				index ++;
			}

			return -1;
		}

		public bool Equals(MethodDeclaration firstMethod, MethodDeclaration secondMethod)
		{
			if (firstMethod.Name == secondMethod.Name)
			{
				if (firstMethod.Parameters.Count == secondMethod.Parameters.Count)
				{
					int index = 0;
					foreach (ParameterDeclarationExpression parameter in firstMethod.Parameters)
					{
						TypeReference parameterTypeReference = ((ParameterDeclarationExpression) secondMethod.Parameters[index]).TypeReference;
						string firstMethodParam = parameter.TypeReference.Type;
						if (firstMethodParam.IndexOf('.') != -1)
							firstMethodParam = firstMethodParam.Substring(firstMethodParam.LastIndexOf('.') + 1);
						string secondMethodParam = parameterTypeReference.Type;
						if (secondMethodParam.IndexOf('.') != -1)
							secondMethodParam = secondMethodParam.Substring(secondMethodParam.LastIndexOf('.') + 1);

						if ((firstMethodParam == secondMethodParam || AreEqualTypes(parameter.TypeReference, parameterTypeReference)) &&
						    parameter.TypeReference.RankSpecifier.Length == parameterTypeReference.RankSpecifier.Length)
							index++;
						else
							return false;
					}
					return true;
				}
			}
			return false;
		}

		public bool IsDerivedFrom(TypeDeclaration childType, string parentTypeName)
		{
			if (childType.BaseTypes.Count > 0)
			{
				string parentType = GetFullName(((TypeReference) childType.BaseTypes[0]));
				if (parentType == parentTypeName)
					return true;
				else if (CodeBase.Types.Contains(parentType))
				{
					TypeDeclaration type = (TypeDeclaration) CodeBase.Types[parentType];
					return IsDerivedFrom(type, parentTypeName);
				}
			}
			return false;
		}
	}
}