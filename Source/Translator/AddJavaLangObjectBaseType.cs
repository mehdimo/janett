namespace Janett.Translator
{
	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	[Explicit]
	public class AddJavaLangObjectBaseType : Transformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (typeDeclaration.Name == "Object")
				return null;
			TypeReference baseType = null;
			if (typeDeclaration.Name == "")
			{
				ObjectCreateExpression oc = (ObjectCreateExpression) AstUtil.GetParentOfType(typeDeclaration, typeof(ObjectCreateExpression));
				baseType = oc.CreateType;
			}
			TypeDeclaration replaced = typeDeclaration;
			if (typeDeclaration.Type == ClassType.Class)
			{
				TypeReference typeReference = new TypeReference("java.lang.Object");
				if (baseType == null && replaced.BaseTypes.Count == 0)
				{
					replaced.BaseTypes.Insert(0, typeReference);
					typeReference.Parent = typeDeclaration;
				}
				else
				{
					if (baseType == null)
						baseType = (TypeReference) replaced.BaseTypes[0];
					string fullName = GetFullName(baseType);
					if (CodeBase.Types.Contains(fullName))
					{
						TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[fullName];
						if (baseTypeDeclaration.Type == ClassType.Interface)
						{
							replaced.BaseTypes.Insert(0, typeReference);
							typeReference.Parent = typeDeclaration;
						}
					}
				}
			}
			ReplaceCurrentNode(replaced);
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}
	}
}