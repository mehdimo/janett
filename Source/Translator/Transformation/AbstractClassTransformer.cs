namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class AbstractClassTransformer : MethodRelatedTransformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (IsAbstractClass(typeDeclaration))
			{
				IList currentClassMethods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
				IList methodsInParents = new ArrayList();
				methodsInParents = GetMethodsInParents(typeDeclaration, methodsInParents);
				methodsInParents = FilterImplementedMethods(methodsInParents);
				IList abstractMethods = GetDiffList(currentClassMethods, methodsInParents);

				if (abstractMethods.Count > 0)
				{
					TypeDeclaration replacedTypeDeclaration = typeDeclaration;
					foreach (MethodDeclaration method in abstractMethods)
					{
						MethodDeclaration newMethod;
						newMethod = new MethodDeclaration(method.Name,
						                                  Modifiers.Public | Modifiers.Abstract,
						                                  method.TypeReference,
						                                  method.Parameters,
						                                  method.Attributes);
						newMethod.Parent = replacedTypeDeclaration;
						replacedTypeDeclaration.Children.Add(newMethod);
					}
					ReplaceCurrentNode(replacedTypeDeclaration);
				}
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		private IList GetMethodsInParents(TypeDeclaration typeDeclaration, IList pmList)
		{
			if (typeDeclaration.BaseTypes.Count > 0)
			{
				foreach (TypeReference parentType in typeDeclaration.BaseTypes)
				{
					string baseType = GetFullName(parentType);
					if (CodeBase.Types.Contains(baseType))
					{
						TypeDeclaration parentTypeDeclaration = (TypeDeclaration) CodeBase.Types[baseType];
						pmList = GetMethods(parentTypeDeclaration, pmList);
						pmList = GetMethodsInParents(parentTypeDeclaration, pmList);
					}
				}
			}
			return pmList;
		}

		private IList GetMethods(TypeDeclaration type, IList MethodList)
		{
			IList list = AstUtil.GetChildrenWithType(type, typeof(MethodDeclaration));
			MethodList = Merge(MethodList, list);
			return MethodList;
		}

		private IList Merge(IList baseList, IList trailList)
		{
			foreach (MethodDeclaration trail in trailList)
			{
				if (!Contains(baseList, trail))
					baseList.Add(trail);
			}
			return baseList;
		}

		private IList GetDiffList(IList baseList, IList comparedList)
		{
			IList diffList = new ArrayList();

			foreach (MethodDeclaration method in comparedList)
			{
				if (!Contains(baseList, method) && method.Body is NullBlockStatement)
					diffList.Add(method);
			}
			return diffList;
		}

		private IList FilterImplementedMethods(IList methods)
		{
			IList list = new ArrayList();
			foreach (MethodDeclaration method in methods)
			{
				TypeDeclaration parentType = (TypeDeclaration) method.Parent;
				if (IsAbstractClass(parentType))
				{
					if (AstUtil.ContainsModifier(method, Modifiers.Abstract))
						list.Add(method);
				}
				else if (parentType.Type == ClassType.Interface)
					list.Add(method);
			}
			return list;
		}

		private bool IsAbstractClass(TypeDeclaration type)
		{
			return (type.Type == ClassType.Class && AstUtil.ContainsModifier(type, Modifiers.Abstract));
		}
	}
}