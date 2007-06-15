namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class MemberExcludeTransformer : Transformer
	{
		public string ExcludedType;
		public IList ExcludedMembers;

		private IList Methods = new ArrayList();
		private IList InnerTypes = new ArrayList();

		public MemberExcludeTransformer(string members)
		{
			string arguments = members;
			string[] splitedMembers = arguments.Split(',');
			foreach (string member in splitedMembers)
			{
				if (member.StartsWith("$"))
					InnerTypes.Add(member.Substring(1));
				else
					Methods.Add(member);
			}
		}

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (InnerTypes.Contains(typeDeclaration.Name))
				RemoveCurrentNode();
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (Methods.Contains(methodDeclaration.Name))
			{
				ExcludedType = GetFullName((TypeDeclaration) methodDeclaration.Parent);
				if (ExcludedMembers == null)
					ExcludedMembers = new ArrayList();
				ExcludedMembers.Add(methodDeclaration.Name);
				RemoveCurrentNode();
			}
			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}

		public override object TrackedVisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			IList list = new ArrayList();
			base.TrackedVisitExpressionStatement(expressionStatement, list);
			if (list.Count > 0)
				RemoveCurrentNode();
			return null;
		}

		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			if (invocationExpression.TargetObject is IdentifierExpression)
			{
				IdentifierExpression identifierExpression = (IdentifierExpression) invocationExpression.TargetObject;
				if (Methods.Contains(identifierExpression.Identifier) && data is IList)
					((IList) data).Add(invocationExpression);
			}
			return base.TrackedVisitInvocationExpression(invocationExpression, data);
		}
	}
}