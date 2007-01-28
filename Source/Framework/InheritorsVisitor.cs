namespace Janett.Framework
{
	using ICSharpCode.NRefactory.Ast;

	public class InheritorsVisitor : Transformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			string fullName = GetFullName(typeDeclaration);
			foreach (TypeReference baseType in typeDeclaration.BaseTypes)
			{
				string fullBaseType = GetFullName(baseType);
				if (fullBaseType == "java.lang.Object" || fullBaseType == "System.Object")
					continue;
				CodeBase.Inheritors.Add(fullBaseType, fullName);
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}
	}
}