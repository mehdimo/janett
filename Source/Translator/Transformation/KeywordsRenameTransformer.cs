namespace Janett.Translator
{
	using System.Collections;

	using Framework;

	using ICSharpCode.NRefactory.Ast;

	public class KeywordsRenameTransformer : Transformer
	{
		private IList keywords = new ArrayList();

		public KeywordsRenameTransformer()
		{
			keywords.Add("is");
			keywords.Add("params");
			keywords.Add("in");
			keywords.Add("out");
			keywords.Add("lock");
			keywords.Add("base");

			keywords.Add("string");
			keywords.Add("object");
			keywords.Add("bool");
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (keywords.Contains(identifierExpression.Identifier))
				identifierExpression.Identifier = "_" + identifierExpression.Identifier;
			return base.TrackedVisitIdentifierExpression(identifierExpression, data);
		}

		public override object TrackedVisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			string field = fieldReferenceExpression.FieldName;
			if (keywords.Contains(field))
			{
				fieldReferenceExpression.FieldName = "_" + field;
			}
			return base.TrackedVisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		public override object TrackedVisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			if (keywords.Contains(parameterDeclarationExpression.ParameterName))
				parameterDeclarationExpression.ParameterName = "_" + parameterDeclarationExpression.ParameterName;
			return base.TrackedVisitParameterDeclarationExpression(parameterDeclarationExpression, data);
		}

		public override object TrackedVisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			if (keywords.Contains(variableDeclaration.Name))
				variableDeclaration.Name = "_" + variableDeclaration.Name;
			return base.TrackedVisitVariableDeclaration(variableDeclaration, data);
		}
	}
}