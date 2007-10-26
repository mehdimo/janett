namespace Janett.Translator
{
	using System.Collections;
	using System.Collections.Generic;

	using Framework;

	using ICSharpCode.NRefactory.Ast;

	public class StaticConstructorTransformer : Transformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			TypeDeclaration replacedType = typeDeclaration;
			List<INode> typeChildren = new List<INode>();
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
					IEnumerable<INode> children = ((ConstructorDeclaration) staticConstructors[i]).Body.Children;
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

		private List<INode> RemoveStaticConstructors(List<INode> typeChildren, out int firstStaticCotrIndex)
		{
			List<INode> cleanList = new List<INode>();
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