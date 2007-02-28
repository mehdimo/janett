namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public abstract class InterfaceTransformer : Transformer
	{
		protected string fieldsClassSuffix;

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (typeDeclaration.Type == ClassType.Interface)
				CreateInterfaceFieldsClass(typeDeclaration);
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		private void CreateInterfaceFieldsClass(TypeDeclaration typeDeclaration)
		{
			IList fields = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
			if (fields.Count > 0)
			{
				TypeDeclaration fieldsClass = new TypeDeclaration(Modifiers.Public, null);
				fieldsClass.Type = ClassType.Class;
				fieldsClass.Name = typeDeclaration.Name + fieldsClassSuffix;
				fieldsClass.Children.AddRange(fields);
				fieldsClass.Parent = typeDeclaration.Parent;
				ApplyModifiers(fields);

				NamespaceDeclaration nsDeclaration = (NamespaceDeclaration) AstUtil.GetParentOfType(typeDeclaration, typeof(NamespaceDeclaration));
				nsDeclaration.AddChild(fieldsClass);

				string fullName = GetFullName(fieldsClass);

				CodeBase.Types[fullName] = fieldsClass;
				string typeName = GetFullName(typeDeclaration);

				CodeBase.References[typeName] = fullName;
			}
		}

		protected virtual void ApplyModifiers(IList fields)
		{
		}
	}
}