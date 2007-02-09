namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	[Mode("DotNet")]
	public class ToArrayTransformer : Transformer
	{
		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			if (invocationExpression.TargetObject is FieldReferenceExpression)
			{
				FieldReferenceExpression targetObject = (FieldReferenceExpression) invocationExpression.TargetObject;

				if (targetObject.FieldName == "toArray" || targetObject.FieldName == "ToArray")
				{
					Expression invoker = targetObject.TargetObject;
					TypeReference invokerType = GetExpressionType(invoker);
					if (invokerType != null && (invokerType.Type == "List" || invokerType.Type == "Set" || invokerType.Type == "Collection"))
					{
						if (invocationExpression.Arguments.Count == 1)
						{
							Expression argExpression = (Expression) invocationExpression.Arguments[0];
							if (argExpression is ArrayCreateExpression)
							{
								InvocationExpression newInvocation = invocationExpression;
								TypeReference old = ((ArrayCreateExpression) argExpression).CreateType;
								TypeReference tr = new TypeReference(old.Type);
								TypeOfExpression tof = new TypeOfExpression(tr);
								tr.Parent = tof;
								tof.Parent = newInvocation;
								newInvocation.Arguments.Clear();
								newInvocation.Arguments.Add(tof);

								ReplaceCurrentNode(newInvocation);
							}
						}
					}
				}
			}
			return base.TrackedVisitInvocationExpression(invocationExpression, data);
		}
	}
}