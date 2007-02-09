namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class StubTransformer : Transformer
	{
		public string Inherit;

		public override object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			RemoveCurrentNode();
			return null;
		}

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (Inherit == null)
				return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
			typeDeclaration.Children.Clear();
			typeDeclaration.BaseTypes.Clear();
			typeDeclaration.BaseTypes.Add(new TypeReference(Inherit));
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