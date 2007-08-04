namespace Janett.Translator
{
	using System.Collections;
	using System.Collections.Generic;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class InnerClassTransformer : MethodRelatedTransformer
	{
		public override object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			ReplaceModifiers(fieldDeclaration);
			return base.TrackedVisitFieldDeclaration(fieldDeclaration, data);
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			ReplaceModifiers(methodDeclaration);
			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}

		public override object TrackedVisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			ReplaceModifiers(constructorDeclaration);
			return base.TrackedVisitConstructorDeclaration(constructorDeclaration, data);
		}

		private void ReplaceModifiers(AttributedNode member)
		{
			if (member.Parent.Parent is TypeDeclaration)
				AstUtil.ReplaceModifiers(member, Modifiers.Private, Modifiers.Internal);
		}

		public override object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			if (invocationExpression.TargetObject is IdentifierExpression)
			{
				TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(invocationExpression, typeof(TypeDeclaration));
				IdentifierExpression methodIdentifier = (IdentifierExpression) invocationExpression.TargetObject;

				if (typeDeclaration.Parent is TypeDeclaration)
				{
					List<ParameterDeclarationExpression> argList = new List<ParameterDeclarationExpression>();
					int i = 0;
					foreach (Expression argument in invocationExpression.Arguments)
					{
						TypeReference argumentType = GetExpressionType(argument);
						if (argumentType != null)
						{
							string argType = argumentType.Type;

							TypeReference typeReference = new TypeReference(argType);
							ParameterDeclarationExpression parameterExpression = new ParameterDeclarationExpression(typeReference, "arg" + i);
							parameterExpression.TypeReference.RankSpecifier = new int[0];
							i++;
							argList.Add(parameterExpression);
						}
					}
					MethodDeclaration argMethod = new MethodDeclaration(methodIdentifier.Identifier, Modifiers.None, null, argList, null);

					IList parentMethods = GetAccessibleMethods((TypeDeclaration) typeDeclaration.Parent);
					if (Contains(parentMethods, argMethod))
					{
						int methodIndex = IndexOf(parentMethods, argMethod);
						argMethod = (MethodDeclaration) parentMethods[methodIndex];
						if (!AstUtil.ContainsModifier(argMethod, Modifiers.Static))
						{
							string parentTypeName = ((TypeDeclaration) typeDeclaration.Parent).Name;
							AddInstanceField(typeDeclaration, parentTypeName);
							AddProperConstructor(typeDeclaration, parentTypeName);

							FieldReferenceExpression newReference = new FieldReferenceExpression(
								new IdentifierExpression(parentTypeName),
								((IdentifierExpression) invocationExpression.TargetObject).Identifier);
							InvocationExpression newInvication = new InvocationExpression(newReference, invocationExpression.Arguments);
							newInvication.Parent = invocationExpression.Parent;

							ReplaceCurrentNode(newInvication);
						}
					}
				}
			}
			return base.TrackedVisitInvocationExpression(invocationExpression, data);
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) AstUtil.GetParentOfType(identifierExpression, typeof(TypeDeclaration));

			if (typeDeclaration != null && typeDeclaration.Parent is TypeDeclaration && !IsInvocation(identifierExpression))
			{
				IList parentFields = AstUtil.GetChildrenWithType(typeDeclaration.Parent, typeof(FieldDeclaration));
				IList innerFields = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
				FieldDeclaration field = new FieldDeclaration(null);

				field.Fields.Add(new VariableDeclaration(identifierExpression.Identifier));
				if (!ContainsField(innerFields, field, false) &&
				    !IdentifierDeclaredInParameter(identifierExpression) &&
				    ContainsField(parentFields, field, false))
				{
					string parentTypeName = ((TypeDeclaration) typeDeclaration.Parent).Name;
					AddInstanceField(typeDeclaration, parentTypeName);
					IdentifierExpression ins = new IdentifierExpression(parentTypeName);
					FieldReferenceExpression fieldReferenceExpression = new FieldReferenceExpression(ins, identifierExpression.Identifier);
					fieldReferenceExpression.Parent = identifierExpression.Parent;

					ReplaceCurrentNode(fieldReferenceExpression);
				}
			}
			return base.TrackedVisitIdentifierExpression(identifierExpression, data);
		}

		private bool IdentifierDeclaredInParameter(IdentifierExpression identifierExpression)
		{
			ConstructorDeclaration cd = (ConstructorDeclaration) AstUtil.GetParentOfType(identifierExpression, typeof(ConstructorDeclaration));
			MethodDeclaration md = (MethodDeclaration) AstUtil.GetParentOfType(identifierExpression, typeof(MethodDeclaration));
			return IsContainIdentifier(cd, identifierExpression) || IsContainIdentifier(md, identifierExpression);
		}

		private bool IsContainIdentifier(ParametrizedNode parametrizedNode, IdentifierExpression identifierExpression)
		{
			if (parametrizedNode != null)
				foreach (ParameterDeclarationExpression pm in parametrizedNode.Parameters)
					if (pm.ParameterName == identifierExpression.Identifier)
						return true;
			return false;
		}

		private void AddInstanceField(TypeDeclaration typeDeclaration, string instanceFieldName)
		{
			FieldDeclaration newField = GetInstanceField(instanceFieldName);
			IList fields = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
			if (!ContainsField(fields, newField, true))
			{
				typeDeclaration.AddChild(newField);
				newField.Parent = typeDeclaration;
			}
		}

		private void AddProperConstructor(TypeDeclaration typeDeclaration, string instanceFieldName)
		{
			TypeReference type = new TypeReference(instanceFieldName);
			ParameterDeclarationExpression fieldParameter = new ParameterDeclarationExpression(type, instanceFieldName);

			FieldReferenceExpression fieldReference = new FieldReferenceExpression(new ThisReferenceExpression(), instanceFieldName);
			IdentifierExpression right = new IdentifierExpression(instanceFieldName);
			AssignmentExpression assignment = new AssignmentExpression(fieldReference, AssignmentOperatorType.Assign, right);
			ExpressionStatement expressionStatement = new ExpressionStatement(assignment);
			string fullName = GetFullName(typeDeclaration);

			IList constructors = AstUtil.GetChildrenWithType(typeDeclaration, typeof(ConstructorDeclaration));
			if (constructors.Count == 0)
			{
				string name = typeDeclaration.Name;
				List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
				parameters.Add(fieldParameter);
				ConstructorDeclaration constructorDeclaration = new ConstructorDeclaration(name, Modifiers.Public, parameters, null);

				constructorDeclaration.Body = new BlockStatement();
				constructorDeclaration.Body.AddChild(expressionStatement);
				constructorDeclaration.Parent = typeDeclaration;
				CodeBase.References.Add("Cons:" + fullName, null);

				typeDeclaration.Children.Add(constructorDeclaration);
			}
			else
			{
				foreach (ConstructorDeclaration constructor in constructors)
				{
					if (!ContainsParameter(constructor.Parameters, fieldParameter))
					{
						constructor.Parameters.Add(fieldParameter);
						if (constructor.ConstructorInitializer != null)
						{
							ConstructorInitializer ci = constructor.ConstructorInitializer;
							if (ci.ConstructorInitializerType == ConstructorInitializerType.This)
								ci.Arguments.Add(new IdentifierExpression(fieldParameter.ParameterName));
						}
						constructor.Body.Children.Insert(0, expressionStatement);
					}
				}
				if (!CodeBase.References.Contains("Cons:" + fullName))
					CodeBase.References.Add("Cons:" + fullName, null);
			}
		}

		private bool ContainsParameter(IList parameters, ParameterDeclarationExpression parameterDeclaration)
		{
			foreach (ParameterDeclarationExpression parameter in parameters)
			{
				if (parameterDeclaration.ParameterName == parameter.ParameterName &&
				    parameterDeclaration.TypeReference.Type == parameter.TypeReference.Type)
					return true;
			}
			return false;
		}

		private bool ContainsField(IList fields, FieldDeclaration field, bool typeCheck)
		{
			foreach (FieldDeclaration fieldDeclaration in fields)
			{
				if (((VariableDeclaration) fieldDeclaration.Fields[0]).Name == ((VariableDeclaration) field.Fields[0]).Name)
				{
					if (typeCheck)
						return fieldDeclaration.TypeReference.Type == field.TypeReference.Type;
					return true;
				}
			}
			return false;
		}

		private FieldDeclaration GetInstanceField(string parentTypeName)
		{
			FieldDeclaration newField = new FieldDeclaration(null);
			newField.TypeReference = new TypeReference(parentTypeName);
			newField.Fields.Add(new VariableDeclaration(parentTypeName));
			return newField;
		}

		private bool IsInvocation(IdentifierExpression identifier)
		{
			if (identifier.Parent is InvocationExpression)
			{
				InvocationExpression idParent = (InvocationExpression) identifier.Parent;
				if (idParent.TargetObject is IdentifierExpression)
				{
					IdentifierExpression idParentTarget = (IdentifierExpression) idParent.TargetObject;
					if (idParentTarget.Identifier == identifier.Identifier)
						return true;
				}
			}
			return false;
		}
	}
}