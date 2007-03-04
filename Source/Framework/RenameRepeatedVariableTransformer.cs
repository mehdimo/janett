namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class RenameRepeatedVariableTransformer : Transformer
	{
		private VariableRenamer renamer = new VariableRenamer();
		private IDictionary localVariables = new Hashtable();
		private IDictionary renamedVariables = new Hashtable();

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			localVariables.Clear();
			renamedVariables.Clear();
			renamer.Reset();
			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}

		public override object TrackedVisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			BlockStatement blockStatement = (BlockStatement) AstUtil.GetParentOfType(variableDeclaration, typeof(BlockStatement));
			if (blockStatement != null)
			{
				int hashCode = blockStatement.GetHashCode();
				if (!(blockStatement.Parent is MethodDeclaration) && !localVariables.Contains(variableDeclaration.Name))
					localVariables.Add(variableDeclaration.Name, variableDeclaration);
				else if (HasConflict(variableDeclaration))
				{
					string newName = renamer.GetNewName(variableDeclaration.Name);
					renamedVariables.Add(variableDeclaration.Name + "_" + hashCode, newName);
					variableDeclaration.Name = newName;
				}
			}
			return base.TrackedVisitVariableDeclaration(variableDeclaration, data);
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (localVariables.Contains(identifierExpression.Identifier))
			{
				BlockStatement blockStatement = (BlockStatement) AstUtil.GetParentOfType(identifierExpression, typeof(BlockStatement));
				int hashCode = blockStatement.GetHashCode();
				if (hashCode != GetScopeHashCode(identifierExpression.Identifier))
				{
					string renamedVariableName = GetModifiedName(blockStatement, identifierExpression.Identifier);
					if (renamedVariableName != null)
						identifierExpression.Identifier = renamedVariableName;
				}
			}
			return base.TrackedVisitIdentifierExpression(identifierExpression, data);
		}

		private string GetModifiedName(BlockStatement blockStatement, string identifier)
		{
			int hashCode;
			while (blockStatement != null)
			{
				hashCode = blockStatement.GetHashCode();
				string identifierHash = identifier + "_" + hashCode;
				if (renamedVariables.Contains(identifierHash))
					return (string) renamedVariables[identifierHash];
				else
					blockStatement = (BlockStatement) AstUtil.GetParentOfType(blockStatement, typeof(BlockStatement));
			}
			return null;
		}

		private bool HasConflict(VariableDeclaration variableDeclaration)
		{
			if (localVariables.Contains(variableDeclaration.Name))
			{
				VariableDeclaration value = (VariableDeclaration) localVariables[variableDeclaration.Name];
				BlockStatement block = (BlockStatement) AstUtil.GetParentOfType(value, typeof(BlockStatement));
				return HasConflictingVariable(block, variableDeclaration);
			}
			return false;
		}

		private bool HasConflictingVariable(BlockStatement blockStatement, VariableDeclaration variableDeclaration)
		{
			INode parentScope = AstUtil.GetParentOfType(blockStatement, typeof(BlockStatement));
			if (parentScope != null)
			{
				int enclosing = parentScope.GetHashCode();
				INode varScope = GetParentScope(variableDeclaration);
				int varCode = varScope.GetHashCode();
				if (varCode == enclosing)
					return true;
				else
					return HasConflictingVariable((BlockStatement) parentScope, variableDeclaration);
			}
			else
				return false;
		}

		private int GetScopeHashCode(string key)
		{
			if (localVariables.Contains(key))
			{
				VariableDeclaration value = (VariableDeclaration) localVariables[key];
				BlockStatement block = (BlockStatement) AstUtil.GetParentOfType(value, typeof(BlockStatement));
				return block.GetHashCode();
			}
			return -1;
		}

		private INode GetParentScope(VariableDeclaration variableDeclaration)
		{
			INode parentScope = variableDeclaration.Parent;
			while (!(parentScope is BlockStatement || parentScope is ForStatement))
			{
				parentScope = parentScope.Parent;
			}
			if (parentScope is BlockStatement || parentScope is ForStatement)
				return parentScope;
			else
				return null;
		}
	}
}