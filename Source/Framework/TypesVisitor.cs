namespace Janett.Framework
{
	using System;

	using ICSharpCode.NRefactory.Ast;

	public class TypesVisitor : Transformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (typeDeclaration.Name == "")
				return null;
			string typeName = GetFullName(typeDeclaration);
			if (typeName == null)
			{
				string file = GetFile(typeDeclaration);
				throw new ApplicationException(string.Format("Type '{0}' in file '{1}' does not defined in a package. " +
				                                             "Janett could not handle types with no package. Please exclude it.",
				                                             typeDeclaration.Name, file));
			}
			if (CodeBase.Types[typeName] == null)
				CodeBase.Types[typeName] = typeDeclaration;
			else
			{
				string oldFile = GetFile((TypeDeclaration) CodeBase.Types[typeName]);
				string newFile = GetFile(typeDeclaration);
				throw new ApplicationException(string.Format("There is type named '{0}' in both '{1}' and '{2}'." +
				                                             "Janett could not handle repeated types. Please exclude one of them. ",
				                                             typeName, oldFile, newFile));
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		private string GetFile(TypeDeclaration type)
		{
			CompilationUnit compilationUnit = (CompilationUnit) AstUtil.GetParentOfType(type, typeof(CompilationUnit));
			return ((TypeReference) compilationUnit.Parent).Type;
		}
	}
}