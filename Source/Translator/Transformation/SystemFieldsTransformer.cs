namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	[Mode("IKVM")]
	public class SystemFieldsTransformer : Transformer
	{
		private IDictionary fields;

		public SystemFieldsTransformer()
		{
			fields = new Hashtable();

			fields.Add("_in", "@in");
			fields.Add("_out", "@out");
		}

		public override object TrackedVisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			string fieldName = fieldReferenceExpression.FieldName;
			if (fields.Contains(fieldName))
			{
				if (fieldReferenceExpression.TargetObject is IdentifierExpression)
				{
					IdentifierExpression identifierExpression = (IdentifierExpression) fieldReferenceExpression.TargetObject;
					if (identifierExpression.Identifier == "System")
						fieldReferenceExpression.FieldName = (string) fields[fieldName];
				}
			}
			return base.TrackedVisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (identifierExpression.Identifier == "System")
			{
				TypeReferenceExpression typeReferenceExpression = new TypeReferenceExpression("java.lang.System");
				ReplaceCurrentNode(typeReferenceExpression);
			}
			return base.TrackedVisitIdentifierExpression(identifierExpression, data);
		}
	}
}