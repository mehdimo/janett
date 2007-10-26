namespace Janett.Translator
{
	using System.Collections;

	using Framework;

	using ICSharpCode.NRefactory.Ast;

	[Mode("DotNet")]
	public class ToArrayTransformer : Transformer
	{
		private IList collectionTypes = new ArrayList();

		public ToArrayTransformer()
		{
			collectionTypes.Add("Collection");
			collectionTypes.Add("List");
			collectionTypes.Add("LinkedList");
			collectionTypes.Add("ArrayList");
			collectionTypes.Add("Set");
			collectionTypes.Add("HashSet");
		}

		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			if (invocationExpression.TargetObject is FieldReferenceExpression)
			{
				FieldReferenceExpression targetObject = (FieldReferenceExpression) invocationExpression.TargetObject;

				if (targetObject.FieldName == "toArray" || targetObject.FieldName == "ToArray")
				{
					Expression invoker = targetObject.TargetObject;
					TypeReference invokerType = GetExpressionType(invoker);
					if (invokerType != null && collectionTypes.Contains(invokerType.Type))
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