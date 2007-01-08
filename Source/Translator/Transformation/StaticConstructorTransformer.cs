namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class StaticConstructorTransformer : Transformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			TypeDeclaration replacedType = typeDeclaration;
			ArrayList typeChildren = new ArrayList();
			typeChildren.AddRange(typeDeclaration.Children);

			IList constructors = AstUtil.GetChildrenWithType(typeDeclaration, typeof(ConstructorDeclaration));
			IList staticConstructors = GetStaticConstructors(constructors);
			ConstructorDeclaration replacedConstructor = MergeStaticConstructors(staticConstructors);

			if (replacedConstructor != null)
			{
				int staticConstructorIndex;
				typeChildren = RemoveStaticConstructors(typeChildren, out staticConstructorIndex);
				typeChildren.Insert(staticConstructorIndex, replacedConstructor);
				replacedType.Children = typeChildren;

				ReplaceCurrentNode(replacedType);
			}

			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		private ConstructorDeclaration MergeStaticConstructors(IList staticConstructors)
		{
			ConstructorDeclaration replacedConstructor = null;
			if (staticConstructors.Count > 1)
			{
				replacedConstructor = (ConstructorDeclaration) staticConstructors[0];
				for (int i = 1; i < staticConstructors.Count; i++)
				{
					ArrayList children = ((ConstructorDeclaration) staticConstructors[i]).Body.Children;
					replacedConstructor.Body.Children.AddRange(children);
				}
			}
			return replacedConstructor;
		}

		private IList GetStaticConstructors(IList constructors)
		{
			IList list = new ArrayList();
			foreach (ConstructorDeclaration constructor in constructors)
			{
				if (AstUtil.ContainsModifier(constructor, Modifiers.Static))
					list.Add(constructor);
			}
			return list;
		}

		private ArrayList RemoveStaticConstructors(ArrayList typeChildren, out int firstStaticCotrIndex)
		{
			ArrayList cleanList = new ArrayList();
			firstStaticCotrIndex = 0;
			int index = 0;
			foreach (INode child in typeChildren)
			{
				if (child is ConstructorDeclaration && AstUtil.ContainsModifier((ConstructorDeclaration) child, Modifiers.Static))
				{
					if (firstStaticCotrIndex == 0)
						firstStaticCotrIndex = index;
				}
				else
					cleanList.Add(child);

				index++;
			}
			return cleanList;
		}
	}
}