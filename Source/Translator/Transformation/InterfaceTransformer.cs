namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class InterfaceTransformer : Transformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			string typeName = typeDeclaration.Name;

			if (typeDeclaration.Type == ClassType.Interface)
			{
				ArrayList fields = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
				ArrayList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
				ArrayList typeDeclarations = AstUtil.GetChildrenWithType(typeDeclaration, typeof(TypeDeclaration));

				if (typeDeclaration.Parent is NamespaceDeclaration)
				{
					NamespaceDeclaration namespaceDeclaration = (NamespaceDeclaration) typeDeclaration.Parent;
					if (fields.Count > 0)
					{
						TypeDeclaration fieldsClass = new TypeDeclaration(Modifiers.Public, null);

						ApplyModifiers(fields);

						fieldsClass.Type = ClassType.Class;
						fieldsClass.Name = typeName + "_Fields";
						fieldsClass.Children = fields;

						namespaceDeclaration.Children.Add(fieldsClass);
						string fieldsClassFullName = namespaceDeclaration.Name + "." + fieldsClass.Name;
						CodeBase.References.Add(namespaceDeclaration.Name + "." + typeDeclaration.Name, fieldsClassFullName);
						CodeBase.Types.Add(fieldsClassFullName, fieldsClass);
					}

					if (typeDeclarations.Count > 0)
					{
						foreach (TypeDeclaration type in typeDeclarations)
						{
							CodeBase.References.Add(typeDeclaration.Name + "." + type.Name, namespaceDeclaration.Name + "." + type.Name);
							namespaceDeclaration.Children.Add(type);
							IList typeMethods = AstUtil.GetChildrenWithType(type, typeof(MethodDeclaration));
							if (type.Type == ClassType.Interface)
							{
								foreach (MethodDeclaration method in typeMethods)
								{
									method.Modifier = Modifiers.None;
								}
							}
						}
					}

					if (methods.Count > 0)
					{
						foreach (MethodDeclaration method in methods)
							method.Modifier = Modifiers.None;
					}
				}

				TypeDeclaration replacedTypeDeclaration = typeDeclaration;

				replacedTypeDeclaration.Children = methods;
				replacedTypeDeclaration.Parent = typeDeclaration.Parent;

				ReplaceCurrentNode(replacedTypeDeclaration);
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		private void ApplyModifiers(ArrayList fields)
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