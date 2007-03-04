namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class InitializerBlockTransformer : Transformer
	{
		public override object TrackedVisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			const string initializerBlock = "InitializerBlock";
			if (constructorDeclaration.Name == initializerBlock)
			{
				TypeDeclaration type = (TypeDeclaration) constructorDeclaration.Parent;
				string initName = "Init" + type.Name;
				MethodDeclaration initMethod = GetInitMethod(type);
				initMethod.Body.Children.AddRange(constructorDeclaration.Body.Children);
				Expression initInvocation = new InvocationExpression(new IdentifierExpression(initName));
				ExpressionStatement initInvocationStatement = new ExpressionStatement(initInvocation);

				IList constructors = AstUtil.GetChildrenWithType(type, typeof(ConstructorDeclaration));
				if (constructors.Count > 1)
				{
					foreach (ConstructorDeclaration constructor in constructors)
					{
						if (constructor.Name != initializerBlock && !HasInitInvocation(constructor))
							constructor.Body.Children.Insert(0, initInvocationStatement);
					}
				}
				else if (((ConstructorDeclaration) constructors[0]).Name == initializerBlock)
				{
					ConstructorDeclaration constructor = new ConstructorDeclaration(type.Name, Modifiers.Public, null, null);
					constructor.Body = new BlockStatement();
					constructor.Body.AddChild(initInvocationStatement);
					type.AddChild(constructor);
				}
				RemoveCurrentNode();
			}
			return base.TrackedVisitConstructorDeclaration(constructorDeclaration, data);
		}

		private bool HasInitInvocation(ConstructorDeclaration constructor)
		{
			IList stms = AstUtil.GetChildrenWithType(constructor.Body, typeof(ExpressionStatement));
			foreach (ExpressionStatement statement in stms)
			{
				Expression expression = statement.Expression;
				if (expression is InvocationExpression && ((InvocationExpression) expression).TargetObject is IdentifierExpression)
				{
					IdentifierExpression identifierExpression = (IdentifierExpression) ((InvocationExpression) expression).TargetObject;
					return identifierExpression.Identifier == "Init" + constructor.Name;
				}
			}
			return false;
		}

		private MethodDeclaration GetInitMethod(TypeDeclaration type)
		{
			IList methods = AstUtil.GetChildrenWithType(type, typeof(MethodDeclaration));
			string initName = "Init" + type.Name;
			foreach (MethodDeclaration methodDeclaration in methods)
			{
				if (methodDeclaration.Name == initName)
					return methodDeclaration;
			}

			MethodDeclaration initMethod = new MethodDeclaration(initName, Modifiers.Private, new TypeReference("void"), null, null);
			initMethod.Body = new BlockStatement();
			type.AddChild(initMethod);

			return initMethod;
		}
	}
}