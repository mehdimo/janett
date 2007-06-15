namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class SameFieldAndMethodNameTransformer : Transformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			ArrayList fields = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
			if (fields.Count > 0)
			{
				IList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
				foreach (FieldDeclaration fieldDeclaration in fields)
					RenameFeildNameSimilarToMethods(fieldDeclaration, methods);
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		private void RenameFeildNameSimilarToMethods(FieldDeclaration fieldDeclaration, IList methods)
		{
			if (HasSimilarName(fieldDeclaration, methods))
			{
				VariableDeclaration declaration = (VariableDeclaration) fieldDeclaration.Fields[0];
				TypeDeclaration typeDeclaration = (TypeDeclaration) fieldDeclaration.Parent;
				string fullName = GetFullName(typeDeclaration);
				string key = fullName + "." + declaration.Name;
				string newName = declaration.Name + "_Field";

				CodeBase.References.Add(key, newName);

				declaration.Name = newName;
			}
		}

		private bool HasSimilarName(FieldDeclaration fieldDeclaration, IList methods)
		{
			VariableDeclaration declaration = (VariableDeclaration) fieldDeclaration.Fields[0];
			string variableName = declaration.Name;

			foreach (MethodDeclaration methodDeclaration in methods)
			{
				if (variableName == methodDeclaration.Name)
					return true;
			}
			return false;
		}
	}
}