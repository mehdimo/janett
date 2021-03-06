namespace Janett.Framework
{
	using System;
	using System.Collections.Generic;

	using ICSharpCode.NRefactory.Ast;

	public class AstUtil
	{
		public INode GetParentOfType(INode node, Type parentType)
		{
			INode parent = node.Parent;
			while (parent != null)
			{
				if (parent.GetType() == parentType)
					return parent;
				parent = parent.Parent;
			}
			return null;
		}

		public TypeReference GetTypeReference(string type, INode parent)
		{
			TypeReference typeRef = new TypeReference(type);
			typeRef.Parent = parent;
			return typeRef;
		}

		public List<INode> GetChildrenWithType(INode parentNode, Type specificType)
		{
			List<INode> list = new List<INode>();
			foreach (INode node in parentNode.Children)
			{
				if (node.GetType() == specificType)
					list.Add(node);
			}
			return list;
		}

		public bool ContainsModifier(AttributedNode node, Modifiers modifier)
		{
			return (node.Modifier & modifier) == modifier;
		}

		public void AddModifierTo(AttributedNode node, Modifiers modifier)
		{
			node.Modifier |= modifier;
		}

		public void RemoveModifierFrom(AttributedNode node, Modifiers modifier)
		{
			if (ContainsModifier(node, modifier))
				node.Modifier -= modifier;
		}

		public void ReplaceModifiers(AttributedNode node, Modifiers oldModifier, Modifiers newModifier)
		{
			if (ContainsModifier(node, oldModifier))
			{
				RemoveModifierFrom(node, oldModifier);
				AddModifierTo(node, newModifier);
			}
		}
	}
}