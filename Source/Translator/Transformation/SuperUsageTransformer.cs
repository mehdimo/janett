namespace Janett.Translator
{
	using Framework;

	using ICSharpCode.NRefactory.Ast;

	[Explicit]
	public class SuperUsageTransformer : Transformer
	{
		public override object TrackedVisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			if (IsArgument(baseReferenceExpression))
			{
				ThisReferenceExpression thisReference = new ThisReferenceExpression();
				thisReference.Parent = baseReferenceExpression.Parent;
				ReplaceCurrentNode(thisReference);
			}
			return base.TrackedVisitBaseReferenceExpression(baseReferenceExpression, data);
		}

		private bool IsArgument(BaseReferenceExpression baseReference)
		{
			if (baseReference.Parent is InvocationExpression)
			{
				InvocationExpression invocationExpression = (InvocationExpression) baseReference.Parent;

				int baseHashCode = baseReference.GetHashCode();
				foreach (Expression argument in invocationExpression.Arguments)
				{
					if (argument is BaseReferenceExpression && argument.GetHashCode() == baseHashCode)
						return true;
				}
			}
			return false;
		}
	}
}