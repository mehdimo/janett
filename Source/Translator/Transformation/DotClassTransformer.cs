namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class DotClassTransformer : Transformer
	{
		public override object TrackedVisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			TypeReference typeReference = typeReferenceExpression.TypeReference;

			if (typeReference.Kind == TypeReferenceKind.DotClass)
			{
				Expression replacedExpression = GetReplacedExpression(typeReference);
				ReplaceCurrentNode(replacedExpression);
			}
			return base.TrackedVisitTypeReferenceExpression(typeReferenceExpression, data);
		}

		private InvocationExpression CreateGetClassMethodInvocation(TypeOfExpression typeOfExpression)
		{
			FieldReferenceExpression argument = new FieldReferenceExpression(typeOfExpression, "AssemblyQualifiedName");
			typeOfExpression.Parent = argument;
			ArrayList arguments = new ArrayList();
			arguments.Add(argument);
			IdentifierExpression methodIdentifier = new IdentifierExpression("java.lang.Class");
			FieldReferenceExpression methodReference = new FieldReferenceExpression(methodIdentifier, "forName");
			InvocationExpression invocationExpression = new InvocationExpression(methodReference, arguments);
			argument.Parent = invocationExpression;
			methodReference.Parent = invocationExpression;
			return invocationExpression;
		}

		private Expression GetReplacedExpression(TypeReference typeReference)
		{
			TypeOfExpression typeOfExpression = new TypeOfExpression(typeReference);
			Expression replacedExpression = typeOfExpression;

			if (Mode == "IKVM")
			{
				InvocationExpression methodInvocation = CreateGetClassMethodInvocation(typeOfExpression);
				replacedExpression = methodInvocation;
			}

			replacedExpression.Parent = typeReference.Parent.Parent;
			return replacedExpression;
		}
	}
}