namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	public class TypeReferenceCorrector : Transformer
	{
		private bool projectTypes;

		public TypeReferenceCorrector() : this(false)
		{
		}

		public TypeReferenceCorrector(bool projectTypes)
		{
			this.projectTypes = projectTypes;
		}

		public override object TrackedVisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			string targetString;
			if (projectTypes)
			{
				targetString = GetTargetString(fieldReferenceExpression);
				if (CodeBase.Mappings.Contains(targetString))
				{
					TypeReferenceExpression typeExpression = new TypeReferenceExpression(targetString);
					typeExpression.Parent = fieldReferenceExpression.Parent;
					ReplaceCurrentNode(typeExpression);
					return null;
				}
			}
			else if (!IsMethodInvocation(fieldReferenceExpression) && ReachToInvocation(fieldReferenceExpression))
			{
				targetString = GetTargetString(fieldReferenceExpression);
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
	}
}