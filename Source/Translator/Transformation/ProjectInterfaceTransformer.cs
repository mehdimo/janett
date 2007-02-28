namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class ProjectInterfaceTransformer : InterfaceTransformer
	{
		public ProjectInterfaceTransformer()
		{
			fieldsClassSuffix = "_Fields";
		}

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			object ret = base.TrackedVisitTypeDeclaration(typeDeclaration, data);
			if (typeDeclaration.Type == ClassType.Interface)
			{
				ArrayList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
				ArrayList typeDeclarations = AstUtil.GetChildrenWithType(typeDeclaration, typeof(TypeDeclaration));

				typeDeclaration.Children.Clear();
				typeDeclaration.Children.AddRange(methods);
				RemoveMethodsModifier(methods);

				SeperateTypes(typeDeclaration, typeDeclarations);
			}
			return ret;
		}

		private void RemoveMethodsModifier(IList methods)
		{
			foreach (MethodDeclaration method in methods)
				method.Modifier = Modifiers.None;
		}

		private void SeperateTypes(TypeDeclaration typeDeclaration, ArrayList typeDeclarations)
		{
			NamespaceDeclaration namespaceDeclaration = (NamespaceDeclaration) AstUtil.GetParentOfType(typeDeclaration, typeof(NamespaceDeclaration));
			foreach (TypeDeclaration type in typeDeclarations)
			{
				CodeBase.References.Add(typeDeclaration.Name + "." + type.Name, namespaceDeclaration.Name + "." + type.Name);
				namespaceDeclaration.Children.Add(type);
				IList typeMethods = AstUtil.GetChildrenWithType(type, typeof(MethodDeclaration));
				if (type.Type == ClassType.Interface)
					RemoveMethodsModifier(typeMethods);
			}
		}

		protected override void ApplyModifiers(IList fields)
		{
			foreach (FieldDeclaration fieldDeclaration in fields)
			{
				if (!AstUtil.ContainsModifier(fieldDeclaration, Modifiers.Public))
					AstUtil.AddModifierTo(fieldDeclaration, Modifiers.Public);
				if (fieldDeclaration.TypeReference.RankSpecifier.Length != 0)
					AstUtil.AddModifierTo(fieldDeclaration, Modifiers.Static);
				else if (!AstUtil.ContainsModifier(fieldDeclaration, Modifiers.Static))
					AstUtil.AddModifierTo(fieldDeclaration, Modifiers.Const);
			}
		}
	}
}