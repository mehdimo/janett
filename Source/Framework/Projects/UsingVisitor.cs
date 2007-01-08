namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class UsingVisitor : Transformer
	{
		public IDictionary Usings = new SortedList();

		public override object TrackedVisitUsing(Using us, object data)
		{
			if (us.IsAlias)
				Add(us.Alias.Type);
			else
				Add(us.Name);
			return base.TrackedVisitUsing(us, data);
		}

		public override object TrackedVisitAttribute(Attribute attribute, object data)
		{
			string name = attribute.Name;
			if (name.IndexOf('.') != -1)
				Add(name);
			return base.TrackedVisitAttribute(attribute, data);
		}

		public override object TrackedVisitTypeReference(TypeReference typeReference, object data)
		{
			string name = typeReference.Type;
			if (name.IndexOf('.') != -1)
				Add(name);
			return base.TrackedVisitTypeReference(typeReference, data);
		}

		public override object TrackedVisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			Expression expression = fieldReferenceExpression.TargetObject;
			if (!(expression is IdentifierExpression))
			{
				string name = GetCode(expression);
				Add(name);
			}
			return base.TrackedVisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		private void Add(string name)
		{
			if (!Usings.Contains(name))
				Usings.Add(name, null);
		}
	}
}