namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	[Mode("DotNet")]
	public class NullableValueTypeTransformer : Transformer
	{
		public override object TrackedVisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			IdentifierExpression target = new IdentifierExpression("System.Int32");
			FieldReferenceExpression intMinValue = new FieldReferenceExpression(target, "MinValue");
			ConvertNull(binaryOperatorExpression, intMinValue, "Integer", "int");

			return base.TrackedVisitBinaryOperatorExpression(binaryOperatorExpression, data);
		}

		private void ConvertNull(BinaryOperatorExpression binaryOperatorExpression, Expression right, params string[] types)
		{
			Expression left = binaryOperatorExpression.Left;
			TypeReference leftType = GetExpressionType(left);

			if (leftType != null)
			{
				string leftTypeName = leftType.Type;

				foreach (string type in types)
				{
					if (leftTypeName == type &&
					    (leftType.RankSpecifier == null || leftType.RankSpecifier.Length == 0) &&
					    binaryOperatorExpression.Right is PrimitiveExpression)
					{
						PrimitiveExpression nullRight = (PrimitiveExpression) binaryOperatorExpression.Right;
						if (nullRight.Value == null)
						{
							BinaryOperatorExpression replacedBinOP = binaryOperatorExpression;
							replacedBinOP.Right = right;
							ReplaceCurrentNode(replacedBinOP);
						}
					}
				}
			}
		}
	}
}