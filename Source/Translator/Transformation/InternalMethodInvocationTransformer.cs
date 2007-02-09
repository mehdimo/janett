namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class InternalMethodInvocationTransformer : MethodRelatedTransformer
	{
		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			if (invocationExpression.TargetObject is FieldReferenceExpression)
			{
				FieldReferenceExpression targetObject = (FieldReferenceExpression) invocationExpression.TargetObject;
				string methodName = targetObject.FieldName;
				TypeDeclaration typeDeclaration = GetEnclosingTypeDeclaration(invocationExpression);
				TypeDeclaration thisTypeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(invocationExpression, typeof(TypeDeclaration));
				if (typeDeclaration != null && IsTestFixture(thisTypeDeclaration))
				{
					IList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
					IList specialMethods = GetMethods(methods, methodName);
					if (ContainsInternalMethod(specialMethods))
					{
						Expression replacedExpression;
						MethodDeclaration method = (MethodDeclaration) specialMethods[0];
						bool staticMethod = AstUtil.ContainsModifier(method, Modifiers.Static);
						replacedExpression = CreateReflectionInvocation(invocationExpression, staticMethod);
						if (invocationExpression.Parent is Expression || invocationExpression.Parent is VariableDeclaration)
						{
							TypeReference returnType = GetInternalMethodReturnType(specialMethods);
							CastExpression castExpression = new CastExpression(returnType, replacedExpression, CastType.Cast);
							replacedExpression = castExpression;
						}
						ReplaceCurrentNode(replacedExpression);
					}
				}
			}
			return base.TrackedVisitInvocationExpression(invocationExpression, data);
		}

		private InvocationExpression CreateReflectionInvocation(InvocationExpression invocationExpression, bool staticMethod)
		{
			ArrayList arguments = new ArrayList();
			FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression) invocationExpression.TargetObject;
			Expression invoker = fieldReferenceExpression.TargetObject;

			TypeReferenceExpression helper = new TypeReferenceExpression("Helpers.ReflectionHelper");
			FieldReferenceExpression call = new FieldReferenceExpression(helper, "CallInternalMethod");

			string name = fieldReferenceExpression.FieldName;
			PrimitiveExpression methodName = new PrimitiveExpression(name, '"' + name + '"');
			arguments.Add(methodName);
			if (staticMethod)
				arguments.Add(new PrimitiveExpression(null, "null"));
			else
				arguments.Add(invoker);

			ArrayInitializerExpression arrayInitializer = new ArrayInitializerExpression(invocationExpression.Arguments);
			TypeReference reference = AstUtil.GetTypeReference("object", invocationExpression);
			reference.RankSpecifier = new int[1];
			ArrayCreateExpression argArray = new ArrayCreateExpression(reference, arrayInitializer);
			arguments.Add(argArray);

			return new InvocationExpression(call, arguments);
		}

		private IList GetMethods(IList methods, string methodName)
		{
			IList result = new ArrayList();
			foreach (MethodDeclaration method in methods)
			{
				if (method.Name == methodName)
					result.Add(method);
			}
			return result;
		}

		private bool ContainsInternalMethod(IList methods)
		{
			foreach (MethodDeclaration method in methods)
			{
				if (IsInternalMethod(method))
					return true;
			}
			return false;
		}

		private TypeReference GetInternalMethodReturnType(IList methods)
		{
			foreach (MethodDeclaration method in methods)
			{
				if (IsInternalMethod(method))
					return method.TypeReference;
			}
			return null;
		}

		private bool IsInternalMethod(MethodDeclaration method)
		{
			if (AstUtil.ContainsModifier(method, Modifiers.Internal) || AstUtil.ContainsModifier(method, Modifiers.Protected))
				return true;
			return false;
		}

		private bool IsTestFixture(TypeDeclaration typeDeclaration)
		{
			if (IsDerivedFrom(typeDeclaration, "junit.framework.TestCase"))
				return true;
			foreach (AttributeSection attributeSection in typeDeclaration.Attributes)
			{
				foreach (Attribute attribute in attributeSection.Attributes)
				{
					if (attribute.Name == "NUnit.Framework.TestFixture")
						return true;
				}
			}
			return false;
		}
	}
}