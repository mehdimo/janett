namespace Janett.Translator
{
	using System.Collections;
	using System.Collections.Generic;

	using Framework;

	using ICSharpCode.NRefactory.Ast;

	[Mode("IKVM")]
	public class IKVMDifferencesTransformer : MethodRelatedTransformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (IsDerivedFrom(typeDeclaration, "Comparator"))
			{
				IList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));

				MethodDeclaration equalsMethod = new MethodDeclaration("equals",
				                                                       Modifiers.Public, AstUtil.GetTypeReference("bool", typeDeclaration), new List<ParameterDeclarationExpression>(), null);
				equalsMethod.Parent = typeDeclaration;

				TypeReference argTypeReference = AstUtil.GetTypeReference("java.lang.Object", equalsMethod);
				argTypeReference.RankSpecifier = new int[] {};
				equalsMethod.Parameters.Add(new ParameterDeclarationExpression(argTypeReference, "obj"));

				if (Contains(methods, equalsMethod))
				{
					int index = IndexOf(methods, equalsMethod);
					MethodDeclaration method = (MethodDeclaration) methods[index];
					AstUtil.RemoveModifierFrom(method, Modifiers.Abstract);
					method.TypeReference.Type = "bool";
					CreateMethodImplementation(method);
				}
				else
				{
					CreateMethodImplementation(equalsMethod);
					typeDeclaration.Children.Add(equalsMethod);
					equalsMethod.Parent = typeDeclaration;
				}
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		private new bool IsDerivedFrom(TypeDeclaration childType, string parentTypeName)
		{
			bool result = false;
			if (childType.BaseTypes.Count > 0)
			{
				foreach (TypeReference baseType in childType.BaseTypes)
				{
					string parentType = baseType.Type;
					if (parentType == parentTypeName)
						result = true;
					else if (CodeBase.Types.Contains(parentType))
					{
						TypeDeclaration type = (TypeDeclaration) CodeBase.Types[parentType];
						result = IsDerivedFrom(type, parentTypeName);
					}
				}
			}
			return result;
		}

		private void CreateMethodImplementation(MethodDeclaration equalsMethod)
		{
			string fieldName = "instancehelper_" + equalsMethod.Name;
			IdentifierExpression targetObject = new IdentifierExpression("java.lang.Object");

			List<Expression> arguments = new List<Expression>();
			arguments.Add(new ThisReferenceExpression());
			string parameterName = ((ParameterDeclarationExpression) equalsMethod.Parameters[0]).ParameterName;
			IdentifierExpression identifier = new IdentifierExpression(parameterName);
			arguments.Add(identifier);

			FieldReferenceExpression fieldReferenceExpression = new FieldReferenceExpression(targetObject, fieldName);
			InvocationExpression invocationExpression = new InvocationExpression(fieldReferenceExpression, arguments);

			ReturnStatement returnStatement = new ReturnStatement(invocationExpression);
			BlockStatement block = new BlockStatement();
			block.Children.Add(returnStatement);
			equalsMethod.Body = block;
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (AstUtil.ContainsModifier(methodDeclaration, Modifiers.Protected))
			{
				if (IsOverrided(methodDeclaration))
					AstUtil.ReplaceModifiers(methodDeclaration, Modifiers.Protected, Modifiers.Public);
			}
			return null;
		}

		private bool IsOverrided(MethodDeclaration methodDeclaration)
		{
			TypeDeclaration type = (TypeDeclaration) methodDeclaration.Parent;
			string methodName = methodDeclaration.Name;
			if (methodName == "clone")
			{
				if (ContainsBaseType(type.BaseTypes, "Cloneable"))
					return true;
			}
			return false;
		}

		private bool ContainsBaseType(IList baseTypes, string type)
		{
			foreach (TypeReference baseType in baseTypes)
			{
				if (baseType.Type == type)
					return true;
			}
			return false;
		}
	}
}