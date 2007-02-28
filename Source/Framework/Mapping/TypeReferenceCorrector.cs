namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class TypeReferenceCorrector : Transformer
	{
		public override object TrackedVisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			if (!IsMethodInvocation(fieldReferenceExpression) && ReachToInvocation(fieldReferenceExpression))
			{
				string targetString = GetTargetString(fieldReferenceExpression);
				if (targetString.StartsWith("id"))
					return null;
				string suffix = "__";
				if (targetString.IndexOf(suffix) != -1)
				{
					if (targetString.EndsWith(suffix))
						targetString = targetString.Substring(0, targetString.Length - suffix.Length);
					else
						return base.TrackedVisitFieldReferenceExpression(fieldReferenceExpression, data);
				}

				TypeReferenceExpression typeExpression = new TypeReferenceExpression(targetString);
				typeExpression.Parent = fieldReferenceExpression.Parent;
				ReplaceCurrentNode(typeExpression);
				return null;
			}
			return base.TrackedVisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		private bool ReachToInvocation(Expression expression)
		{
			if (expression is FieldReferenceExpression)
			{
				FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression) expression;
				INode node = fieldReferenceExpression.Parent.Parent;
				if (node is InvocationExpression)
					return true;
				else if (node is FieldReferenceExpression)
				{
					if (IsMethodInvocation((FieldReferenceExpression) node))
						return true;
					else return ReachToInvocation((Expression) node.Parent);
				}
			}
			return false;
		}

		private bool IsMethodInvocation(FieldReferenceExpression fieldReference)
		{
			if (fieldReference.Parent is InvocationExpression)
			{
				InvocationExpression invocationExpression = (InvocationExpression) fieldReference.Parent;
				if (invocationExpression.TargetObject is FieldReferenceExpression)
				{
					FieldReferenceExpression invocationReference = (FieldReferenceExpression) invocationExpression.TargetObject;
					return (fieldReference.FieldName == invocationReference.FieldName);
				}
			}
			return false;
		}

		protected string GetTargetString(Expression targetObject)
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