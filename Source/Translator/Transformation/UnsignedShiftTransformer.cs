namespace Janett.Translator
{
	using Framework;

	using ICSharpCode.NRefactory.Ast;

	public class UnsignedShiftTransformer : Transformer
	{
		public override object TrackedVisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			if (binaryOperatorExpression.Op == BinaryOperatorType.UnsignedShiftRight)
			{
				binaryOperatorExpression.Op = BinaryOperatorType.ShiftRight;
				CastExpression castExpression = GetCastExpression(binaryOperatorExpression);
				ReplaceCurrentNode(castExpression);
			}
			else if (binaryOperatorExpression.Op == BinaryOperatorType.UnsignedShiftRightAssign)
			{
				Expression left = binaryOperatorExpression.Left;
				Expression right = new BinaryOperatorExpression(left, BinaryOperatorType.ShiftRight, binaryOperatorExpression.Right);
				right.Parent = binaryOperatorExpression.Parent;
				CastExpression castExpression = GetCastExpression((BinaryOperatorExpression) right);
				right.Parent = castExpression;
				AssignmentExpression assignment = new AssignmentExpression(left, AssignmentOperatorType.Assign, castExpression);
				assignment.Parent = binaryOperatorExpression.Parent;

				ReplaceCurrentNode(assignment);
			}
			return base.TrackedVisitBinaryOperatorExpression(binaryOperatorExpression, data);
		}

		private CastExpression GetCastExpression(BinaryOperatorExpression binaryOperatorExpression)
		{
			TypeReference leftType = GetExpressionType(binaryOperatorExpression.Left);

			CastExpression castedUnsignedShift = new CastExpression(new TypeReference("u" + leftType.Type), binaryOperatorExpression, CastType.Cast);
			ParenthesizedExpression parenthesizedCastedUnsignedShift = new ParenthesizedExpression(castedUnsignedShift);
			return new CastExpression(new TypeReference(leftType.Type), parenthesizedCastedUnsignedShift, CastType.Cast);
		}
	}
}