namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class InheritedTypesExcludeTransformer : Transformer
	{
		public IDictionary ParentTypes;

		private IList methods = new ArrayList();

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			string parentType;
			if (HasExcludedMethod(typeDeclaration, out parentType))
			{
				methods = (IList) ParentTypes[parentType];
				return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
			}
			return null;
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (methods.Contains(methodDeclaration.Name))
			{
				RemoveCurrentNode();
				return null;
			}
			else
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
				if (methods.Contains(identifierExpression.Identifier) && data is IList)
					((IList) data).Add(invocationExpression);
			}
			return base.TrackedVisitInvocationExpression(invocationExpression, data);
		}

		private bool HasExcludedMethod(TypeDeclaration typeDeclaration, out string type)
		{
			if (typeDeclaration.BaseTypes.Count > 0)
			{
				foreach (TypeReference baseType in typeDeclaration.BaseTypes)
				{
					string fullName = GetFullName(baseType);
					if (ParentTypes.Contains(fullName))
					{
						type = fullName;
						return true;
					}
					else if (CodeBase.Types.Contains(fullName))
					{
						TypeDeclaration typeDec = (TypeDeclaration) CodeBase.Types[fullName];
						bool has = HasExcludedMethod(typeDec, out type);
						if (has)
							return has;
					}
				}
			}
			type = null;
			return false;
		}
	}
}