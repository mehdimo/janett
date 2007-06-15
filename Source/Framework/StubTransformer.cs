namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class StubTransformer : Transformer
	{
		private ArrayList stubMembers = new ArrayList();

		public string Inherit;
		public string Members;

		public bool ShouldStubMember(string name)
		{
			return (stubMembers[0].ToString() == "*" || stubMembers.Contains(name));
		}

		public override object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			if (ShouldStubMember(((VariableDeclaration) fieldDeclaration.Fields[0]).Name))
				RemoveCurrentNode();
			return null;
		}

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			stubMembers.AddRange(Members.Split(','));
			if (Inherit == null)
				return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
			typeDeclaration.Children.Clear();
			typeDeclaration.BaseTypes.Clear();
			typeDeclaration.BaseTypes.Add(new TypeReference(Inherit));
			return null;
		}

		public override object TrackedVisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if (ShouldStubMember(constructorDeclaration.Name))
				return base.TrackedVisitConstructorDeclaration(constructorDeclaration, data);
			else
				return null;
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (ShouldStubMember(methodDeclaration.Name))
				return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
			else
				return null;
		}

		public override object TrackedVisitBlockStatement(BlockStatement blockStatement, object data)
		{
			blockStatement.Children.Clear();
			TypeReference notImplmentedException = new TypeReference("System.NotImplementedException");
			ObjectCreateExpression objectCreate = new ObjectCreateExpression(notImplmentedException, new ArrayList());
			blockStatement.Children.Add(new ThrowStatement(objectCreate));
			return null;
		}
	}
}