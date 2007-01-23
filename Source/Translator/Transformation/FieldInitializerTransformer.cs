namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class FieldInitializerTransformer : Transformer
	{
		public override object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			VariableDeclaration field = (VariableDeclaration) fieldDeclaration.Fields[0];
			TypeDeclaration typeDeclaration = (TypeDeclaration) fieldDeclaration.Parent;

			NodeTypeExistenceVisitor nodeTypeExistenceVisitor = new NodeTypeExistenceVisitor(typeof(ThisReferenceExpression));
			field.Initializer.AcceptVisitor(nodeTypeExistenceVisitor, null);
			if (field.Initializer != null && (field.Initializer is InvocationExpression || IsArrayCreation(fieldDeclaration) || nodeTypeExistenceVisitor.Contains)
			    && !AstUtil.ContainsModifier(fieldDeclaration, Modifiers.Static))
			{
				IList constructors = AstUtil.GetChildrenWithType(typeDeclaration, typeof(ConstructorDeclaration));

				IdentifierExpression left = new IdentifierExpression(field.Name);
				Expression right = field.Initializer;
				AssignmentExpression assignmentExpression = new AssignmentExpression(left, AssignmentOperatorType.Assign, right);
				ExpressionStatement ExpressionStatement = new ExpressionStatement(assignmentExpression);
				field.Initializer = null;
				ConstructorDeclaration constructorDeclaration = null;
				ExpressionStatement.Parent = constructorDeclaration;

				foreach (ConstructorDeclaration consDec in constructors)
				{
					if (!AstUtil.ContainsModifier(consDec, Modifiers.Static))
					{
						if (consDec.Parameters.Count == 0)
						{
							constructorDeclaration = consDec;
							constructorDeclaration.Body.Children.Add(ExpressionStatement);
							constructorDeclaration.Parent = typeDeclaration;
							return base.TrackedVisitFieldDeclaration(fieldDeclaration, data);
						}
						else
						{
							consDec.ConstructorInitializer = new ConstructorInitializer();
							consDec.ConstructorInitializer.ConstructorInitializerType = ConstructorInitializerType.This;
						}
					}
				}
				constructorDeclaration = GetConstructor(ExpressionStatement, typeDeclaration);
				constructorDeclaration.Parent = typeDeclaration;
				return base.TrackedVisitFieldDeclaration(fieldDeclaration, data);
			}
			return base.TrackedVisitFieldDeclaration(fieldDeclaration, data);
		}

		private bool IsArrayCreation(FieldDeclaration fieldDeclaration)
		{
			VariableDeclaration field = (VariableDeclaration) fieldDeclaration.Fields[0];
			if ((field.Initializer is ArrayCreateExpression) && (fieldDeclaration.TypeReference.RankSpecifier.Length > 0))
			{
				ArrayCreateExpression arrayCreateExpression = (ArrayCreateExpression) field.Initializer;
				if (arrayCreateExpression.Arguments.Count > 0)
				{
					foreach (Expression argument in arrayCreateExpression.Arguments)
					{
						if (!(argument is PrimitiveExpression))
							return true;
					}
				}
			}
			return false;
		}

		private ConstructorDeclaration GetConstructor(ExpressionStatement expression, TypeDeclaration typeDeclaration)
		{
			ConstructorDeclaration constructorDeclaration;
			constructorDeclaration = new ConstructorDeclaration(typeDeclaration.Name, Modifiers.Public, null, null);
			constructorDeclaration.Body = new BlockStatement();
			constructorDeclaration.Body.Children.Add(expression);
			typeDeclaration.Children.Add(constructorDeclaration);
			return constructorDeclaration;
		}
	}
}