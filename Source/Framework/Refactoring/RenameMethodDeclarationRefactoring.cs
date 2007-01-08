namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	public class RenameMethodDeclarationRefactoring : Refactoring
	{
		public IRenamer Renamer = new PascalStyleMethodRenamer();

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) methodDeclaration.Parent;

			if (typeDeclaration.BaseTypes.Count > 0)
			{
				if (IsMethodInExternalTypes(typeDeclaration, methodDeclaration))
					return null;
			}
			string methodName = methodDeclaration.Name;
			methodDeclaration.Name = Renamer.GetNewName(methodName);
			if (methodDeclaration.Name == typeDeclaration.Name)
				methodDeclaration.Name += "_";

			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}
	}
}