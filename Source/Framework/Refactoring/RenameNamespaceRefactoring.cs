namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class RenameNamespaceRefactoring : Transformer
	{
		public string From;
		public string To;

		public override object TrackedVisitTypeReference(TypeReference typeReference, object data)
		{
			typeReference.Type = Replace(typeReference.Type);
			return base.TrackedVisitTypeReference(typeReference, data);
		}

		public override object TrackedVisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			FieldReferenceExpression replacedExp = fieldReferenceExpression;
			string target = GetTargetString(fieldReferenceExpression.TargetObject);
			string replaced = Replace(target);
			if (target != replaced)
			{
				replacedExp.TargetObject = new IdentifierExpression(replaced);
				ReplaceCurrentNode(replacedExp);
			}
			return base.TrackedVisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		public override object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			namespaceDeclaration.Name = Replace(namespaceDeclaration.Name);
			return base.TrackedVisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		private string Replace(string name)
		{
			string newName = name.Replace(From, To);
			if (name == From || newName == name)
				return newName;
			string leading = newName.Substring(To.Length + 1);
			string[] parts = leading.Split('.');
			string newLeading = "";
			foreach (string part in parts)
			{
				newLeading += part[0].ToString().ToUpper() + part.Substring(1) + ".";
			}
			newName = newName.Substring(0, To.Length) + "." + newLeading.TrimEnd('.');
			return newName;
		}

		public override object TrackedVisitUsing(Using @using, object data)
		{
			@using.Name = Replace(@using.Name);
			if (@using.IsAlias)
				@using.Alias.Type = Replace(@using.Alias.Type);
			return base.TrackedVisitUsing(@using, data);
		}

		private string GetTargetString(Expression targetObject)
		{
			Stack stack = new Stack();
			Expression target = targetObject;
			while (target is FieldReferenceExpression)
			{
				string str = ((FieldReferenceExpression) target).FieldName;
				stack.Push(str);
				target = ((FieldReferenceExpression) target).TargetObject;
			}
			if (target is IdentifierExpression)
				stack.Push(((IdentifierExpression) target).Identifier);
			string item;
			string result = "";
			while (stack.Count != 0)
			{
				item = (string) stack.Pop();
				result += item + ".";
			}
			result = result.TrimEnd('.');
			return result;
		}
	}
}