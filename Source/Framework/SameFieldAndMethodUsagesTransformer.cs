namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	public class SameFieldAndMethodUsagesTransformer : Transformer
	{
		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (!IsMethodInvocation(identifierExpression))
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(identifierExpression, typeof(TypeDeclaration));
				string fullName = GetFullName(typeDeclaration);
				string key = fullName + "." + identifierExpression.Identifier;
				if (CodeBase.References.Contains(key))
					identifierExpression.Identifier = (string) CodeBase.References[key];
			}
			return base.TrackedVisitIdentifierExpression(identifierExpression, data);
		}

		public override object TrackedVisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			if (!IsMethodInvocation(fieldReferenceExpression))
			{
				TypeReference typeReference = GetExpressionType(fieldReferenceExpression.TargetObject);
				if (typeReference != null)
				{
					string fullName = GetFullName(typeReference);
					string key = fullName + "." + fieldReferenceExpression.FieldName;
					if (CodeBase.References.Contains(key))
						fieldReferenceExpression.FieldName = (string) CodeBase.References[key];
				}
			}
			return base.TrackedVisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		private bool IsMethodInvocation(Expression expression)
		{
			if (expression.Parent is InvocationExpression)
			{
				InvocationExpression invocation = (InvocationExpression) expression.Parent;
				return expression.GetHashCode() == invocation.TargetObject.GetHashCode();
			}
			return false;
		}
	}
}