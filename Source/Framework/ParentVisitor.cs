namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.NRefactory.Visitors;

	public class ParentVisitor : AbstractAstVisitor
	{
		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			localVariableDeclaration.TypeReference.Parent = localVariableDeclaration;
			foreach (VariableDeclaration varDec in localVariableDeclaration.Variables)
				varDec.Parent = localVariableDeclaration;
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}

		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			indexerExpression.TargetObject.Parent = indexerExpression;
			foreach (Expression expr in indexerExpression.Indexes)
			{
				expr.Parent = indexerExpression;
			}
			return base.VisitIndexerExpression(indexerExpression, data);
		}

		public override object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			indexerDeclaration.GetRegion.Parent = indexerDeclaration;
			indexerDeclaration.SetRegion.Parent = indexerDeclaration;
			indexerDeclaration.TypeReference.Parent = indexerDeclaration;
			foreach (ParameterDeclarationExpression parameter in indexerDeclaration.Parameters)
				parameter.Parent = indexerDeclaration;
			return base.VisitIndexerDeclaration(indexerDeclaration, data);
		}

		public override object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
		{
			ifElseStatement.Condition.Parent = ifElseStatement;
			foreach (Statement st in ifElseStatement.TrueStatement)
			{
				st.Parent = ifElseStatement;
			}
			foreach (Statement st in ifElseStatement.FalseStatement)
			{
				st.Parent = ifElseStatement;
			}
			foreach (ElseIfSection elseIfSection in ifElseStatement.ElseIfSections)
			{
				elseIfSection.Parent = ifElseStatement;
			}
			return base.VisitIfElseStatement(ifElseStatement, data);
		}

		public override object VisitElseIfSection(ElseIfSection elseIfSection, object data)
		{
			elseIfSection.Condition.Parent = elseIfSection;
			elseIfSection.EmbeddedStatement.Parent = elseIfSection;
			return base.VisitElseIfSection(elseIfSection, data);
		}

		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			assignmentExpression.Left.Parent = assignmentExpression;
			assignmentExpression.Right.Parent = assignmentExpression;
			return base.VisitAssignmentExpression(assignmentExpression, data);
		}

		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			binaryOperatorExpression.Left.Parent = binaryOperatorExpression;
			binaryOperatorExpression.Right.Parent = binaryOperatorExpression;
			return base.VisitBinaryOperatorExpression(binaryOperatorExpression, data);
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			methodDeclaration.Body.Parent = methodDeclaration;
			methodDeclaration.TypeReference.Parent = methodDeclaration;
			foreach (ParameterDeclarationExpression parameter in methodDeclaration.Parameters)
				parameter.Parent = methodDeclaration;
			foreach (TemplateDefinition template in methodDeclaration.Templates)
				template.Parent = methodDeclaration;
			foreach (AttributeSection attribute in methodDeclaration.Attributes)
				attribute.Parent = methodDeclaration;
			foreach (INode throws in methodDeclaration.Throws)
				throws.Parent = methodDeclaration;
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}

		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			foreach (ParameterDeclarationExpression parameter in constructorDeclaration.Parameters)
				parameter.Parent = constructorDeclaration;
			foreach (INode throws in constructorDeclaration.Throws)
				throws.Parent = constructorDeclaration;
			constructorDeclaration.Body.Parent = constructorDeclaration;
			constructorDeclaration.ConstructorInitializer.Parent = constructorDeclaration;
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}

		public override object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			foreach (Expression argumet in constructorInitializer.Arguments)
				argumet.Parent = constructorInitializer;
			return base.VisitConstructorInitializer(constructorInitializer, data);
		}

		public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclaration, object data)
		{
			parameterDeclaration.TypeReference.Parent = parameterDeclaration;
			return base.VisitParameterDeclarationExpression(parameterDeclaration, data);
		}

		public override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			expressionStatement.Expression.Parent = expressionStatement;
			return base.VisitExpressionStatement(expressionStatement, data);
		}

		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			foreach (Statement st in blockStatement.Children)
			{
				st.Parent = blockStatement;
			}
			return base.VisitBlockStatement(blockStatement, data);
		}

		public override object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			returnStatement.Expression.Parent = returnStatement;
			return base.VisitReturnStatement(returnStatement, data);
		}

		public override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			objectCreateExpression.CreateType.Parent = objectCreateExpression;
			foreach (Expression ex in objectCreateExpression.Parameters)
			{
				ex.Parent = objectCreateExpression;
			}
			if (objectCreateExpression.AnonymousClass != TypeDeclaration.Null)
				objectCreateExpression.AnonymousClass.Parent = objectCreateExpression;
			return base.VisitObjectCreateExpression(objectCreateExpression, data);
		}

		public override object VisitCastExpression(CastExpression castExpression, object data)
		{
			castExpression.CastTo.Parent = castExpression;
			castExpression.Expression.Parent = castExpression;
			return base.VisitCastExpression(castExpression, data);
		}

		public override object VisitVariableDeclaration(VariableDeclaration varDec, object data)
		{
			varDec.Initializer.Parent = varDec;
			varDec.TypeReference.Parent = varDec;
			return base.VisitVariableDeclaration(varDec, data);
		}

		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			invocationExpression.TargetObject.Parent = invocationExpression;
			for (int i = 0; i < invocationExpression.Arguments.Count; i++)
			{
				((Expression) invocationExpression.Arguments[i]).Parent = invocationExpression;
			}
			foreach (TypeReference typeReference in invocationExpression.TypeArguments)
			{
				typeReference.Parent = invocationExpression;
			}
			return base.VisitInvocationExpression(invocationExpression, data);
		}

		public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			fieldReferenceExpression.TargetObject.Parent = fieldReferenceExpression;
			return base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			parenthesizedExpression.Expression.Parent = parenthesizedExpression;
			return base.VisitParenthesizedExpression(parenthesizedExpression, data);
		}

		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			unaryOperatorExpression.Expression.Parent = unaryOperatorExpression;
			return base.VisitUnaryOperatorExpression(unaryOperatorExpression, data);
		}

		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			propertyDeclaration.TypeReference.Parent = propertyDeclaration;
			propertyDeclaration.GetRegion.Parent = propertyDeclaration;
			propertyDeclaration.SetRegion.Parent = propertyDeclaration;
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}

		public override object VisitPropertyGetRegion(PropertyGetRegion getRegion, object data)
		{
			getRegion.Block.Parent = getRegion;
			return base.VisitPropertyGetRegion(getRegion, data);
		}

		public override object VisitPropertySetRegion(PropertySetRegion setRegion, object data)
		{
			setRegion.Block.Parent = setRegion;
			foreach (ParameterDeclarationExpression parameter in setRegion.Parameters)
				parameter.Parent = setRegion;
			return base.VisitPropertySetRegion(setRegion, data);
		}

		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			fieldDeclaration.TypeReference.Parent = fieldDeclaration;
			foreach (VariableDeclaration field in fieldDeclaration.Fields)
				field.Parent = fieldDeclaration;
			foreach (AttributeSection attribute in fieldDeclaration.Attributes)
				attribute.Parent = fieldDeclaration;
			return base.VisitFieldDeclaration(fieldDeclaration, data);
		}

		public override object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			typeOfIsExpression.Expression.Parent = typeOfIsExpression;
			typeOfIsExpression.TypeReference.Parent = typeOfIsExpression;
			return base.VisitTypeOfIsExpression(typeOfIsExpression, data);
		}

		public override object VisitTypeReference(TypeReference typeReference, object data)
		{
			foreach (TypeReference genericType in typeReference.GenericTypes)
				genericType.Parent = typeReference;
			return base.VisitTypeReference(typeReference, data);
		}

		public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			foreachStatement.Expression.Parent = foreachStatement;
			foreachStatement.TypeReference.Parent = foreachStatement;
			return base.VisitForeachStatement(foreachStatement, data);
		}

		public override object VisitForStatement(ForStatement forStatement, object data)
		{
			forStatement.Condition.Parent = forStatement;
			forStatement.EmbeddedStatement.Parent = forStatement;
			foreach (Statement statement in forStatement.Initializers)
				statement.Parent = forStatement;
			foreach (Statement iterator in forStatement.Iterator)
				iterator.Parent = forStatement;
			return base.VisitForStatement(forStatement, data);
		}

		public override object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			switchStatement.SwitchExpression.Parent = switchStatement;
			foreach (SwitchSection switchSection in switchStatement.SwitchSections)
				switchSection.Parent = switchStatement;
			return base.VisitSwitchStatement(switchStatement, data);
		}

		public override object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			foreach (CaseLabel switchLabel in switchSection.SwitchLabels)
				switchLabel.Parent = switchSection;
			return base.VisitSwitchSection(switchSection, data);
		}

		public override object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			caseLabel.Label.Parent = caseLabel;
			caseLabel.ToExpression.Parent = caseLabel;
			return base.VisitCaseLabel(caseLabel, data);
		}

		public override object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			conditionalExpression.Condition.Parent = conditionalExpression;
			conditionalExpression.TrueExpression.Parent = conditionalExpression;
			conditionalExpression.FalseExpression.Parent = conditionalExpression;
			return base.VisitConditionalExpression(conditionalExpression, data);
		}

		public override object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			tryCatchStatement.StatementBlock.Parent = tryCatchStatement;
			tryCatchStatement.FinallyBlock.Parent = tryCatchStatement;
			foreach (CatchClause catchClause in tryCatchStatement.CatchClauses)
				catchClause.Parent = tryCatchStatement;
			return base.VisitTryCatchStatement(tryCatchStatement, data);
		}

		public override object VisitCatchClause(CatchClause catchClause, object data)
		{
			catchClause.Condition.Parent = catchClause;
			catchClause.StatementBlock.Parent = catchClause;
			catchClause.TypeReference.Parent = catchClause;
			return base.VisitCatchClause(catchClause, data);
		}

		public override object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			uncheckedExpression.Expression.Parent = uncheckedExpression;
			return base.VisitUncheckedExpression(uncheckedExpression, data);
		}

		public override object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			uncheckedStatement.Block.Parent = uncheckedStatement;
			return base.VisitUncheckedStatement(uncheckedStatement, data);
		}

		public override object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			delegateDeclaration.ReturnType.Parent = delegateDeclaration;
			foreach (ParameterDeclarationExpression parameter in delegateDeclaration.Parameters)
				parameter.Parent = delegateDeclaration;
			return base.VisitDelegateDeclaration(delegateDeclaration, data);
		}

		public override object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			usingStatement.EmbeddedStatement.Parent = usingStatement;
			usingStatement.ResourceAcquisition.Parent = usingStatement;
			return base.VisitUsingStatement(usingStatement, data);
		}

		public override object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			doLoopStatement.Condition.Parent = doLoopStatement;
			doLoopStatement.EmbeddedStatement.Parent = doLoopStatement;
			return base.VisitDoLoopStatement(doLoopStatement, data);
		}

		public override object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			directionExpression.Expression.Parent = directionExpression;
			return base.VisitDirectionExpression(directionExpression, data);
		}

		public override object VisitThrowStatement(ThrowStatement throwStatement, object data)
		{
			throwStatement.Expression.Parent = throwStatement;
			return base.VisitThrowStatement(throwStatement, data);
		}

		public override object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			typeReferenceExpression.TypeReference.Parent = typeReferenceExpression;
			return base.VisitTypeReferenceExpression(typeReferenceExpression, data);
		}

		public override object VisitLockStatement(LockStatement lockStatement, object data)
		{
			lockStatement.LockExpression.Parent = lockStatement;
			return base.VisitLockStatement(lockStatement, data);
		}

		public override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			arrayCreateExpression.CreateType.Parent = arrayCreateExpression;
			arrayCreateExpression.ArrayInitializer.Parent = arrayCreateExpression;
			foreach (INode argument in arrayCreateExpression.Arguments)
				argument.Parent = arrayCreateExpression;

			return base.VisitArrayCreateExpression(arrayCreateExpression, data);
		}

		public override object VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			foreach (INode node in arrayInitializerExpression.CreateExpressions)
			{
				node.Parent = arrayInitializerExpression;
			}
			return base.VisitArrayInitializerExpression(arrayInitializerExpression, data);
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			foreach (TypeReference baseType in typeDeclaration.BaseTypes)
			{
				baseType.Parent = typeDeclaration;
			}
			foreach (INode node in typeDeclaration.Children)
				node.Parent = typeDeclaration;

			return base.VisitTypeDeclaration(typeDeclaration, data);
		}

		public override object VisitAttribute(Attribute attribute, object data)
		{
			foreach (Expression argument in attribute.NamedArguments)
				argument.Parent = attribute;
			foreach (Expression argument in attribute.PositionalArguments)
				argument.Parent = attribute;

			return base.VisitAttribute(attribute, data);
		}

		public override object VisitAttributeSection(AttributeSection attributeSection, object data)
		{
			foreach (Attribute attribute in attributeSection.Attributes)
				attribute.Parent = attributeSection;
			return base.VisitAttributeSection(attributeSection, data);
		}

		public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			foreach (INode node in namespaceDeclaration.Children)
				node.Parent = namespaceDeclaration;

			return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			foreach (Using us in usingDeclaration.Usings)
			{
				us.Parent = usingDeclaration;
			}
			return base.VisitUsingDeclaration(usingDeclaration, data);
		}

		public override object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			typeOfExpression.TypeReference.Parent = typeOfExpression;
			return base.VisitTypeOfExpression(typeOfExpression, data);
		}

		public override object VisitClassDeclarationStatement(ClassDeclarationStatement classDeclarationStatement, object data)
		{
			classDeclarationStatement.Type.Parent = classDeclarationStatement;
			return base.VisitClassDeclarationStatement(classDeclarationStatement, data);
		}
	}
}