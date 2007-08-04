namespace Janett.Framework
{
	using System;
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class TypeResolver
	{
		public CodeBase CodeBase;
		public AstUtil AstUtil;

		public string GetFullName(TypeReference typeReference)
		{
			NamespaceDeclaration nsd = (NamespaceDeclaration) AstUtil.GetParentOfType(typeReference, typeof(NamespaceDeclaration));
			if (nsd == null)
				return typeReference.Type;
			if (CodeBase.Types.Contains(typeReference.Type))
				return typeReference.Type;

			if (typeReference.Type.IndexOf(".") != -1)
			{
				int firstDot = typeReference.Type.IndexOf('.');
				if (firstDot == typeReference.Type.LastIndexOf('.'))
				{
					string enclosing = typeReference.Type.Substring(0, firstDot);
					string subType = typeReference.Type.Substring(firstDot + 1);
					if (CodeBase.Types.Contains(nsd.Name + "." + enclosing))
						return nsd.Name + "." + enclosing + "$" + subType;
					IList usingDeclarations = AstUtil.GetChildrenWithType(nsd, typeof(UsingDeclaration));
					foreach (UsingDeclaration usingDeclaration in usingDeclarations)
					{
						foreach (Using us in usingDeclaration.Usings)
						{
							if (us.IsAlias)
							{
								if (us.Alias.Type.EndsWith('.' + enclosing))
								{
									return us.Alias + "$" + subType;
								}
							}
							else
							{
								if (us.Name.EndsWith('.' + enclosing))
								{
									return us.Name + "$" + subType;
								}
								else
								{
									string nsName = us.Name;
									if (nsName.EndsWith(".*"))
										nsName = nsName.Substring(0, nsName.Length - 2);
									if (CodeBase.Types.Contains(nsName + "." + enclosing))
										return nsName + "." + enclosing + "$" + subType;
								}
							}
						}
					}
					return typeReference.Type;
				}
				else
					return typeReference.Type;
			}
			TypeDeclaration typeDec = (TypeDeclaration) AstUtil.GetParentOfType(typeReference, typeof(TypeDeclaration));
			if (typeDec != null)
			{
				if (typeDec.BaseTypes.Count > 0)
				{
					foreach (TypeReference baseTypeReference in typeDec.BaseTypes)
					{
						string fullBaseName = GetFullName(baseTypeReference, nsd);
						string typeName = fullBaseName + "$" + typeReference.Type;
						if (CodeBase.Types.Contains(typeName))
							return typeName;
					}
				}
				if (typeDec.Name == typeReference.Type)
					typeDec = (TypeDeclaration) AstUtil.GetParentOfType(typeDec, typeof(TypeDeclaration));
				if (typeDec != null)
				{
					while (typeDec.Parent is TypeDeclaration)
						typeDec = (TypeDeclaration) typeDec.Parent;
					string typeName = nsd.Name + "." + typeDec.Name + "$" + typeReference.Type;
					if (CodeBase.Types.Contains(typeName))
						return typeName;
				}
			}

			if (TypeReference.PrimitiveTypesJava.ContainsKey(typeReference.Type))
				return (string) TypeReference.PrimitiveTypesJava[typeReference.Type];

			IList nsUsings = AstUtil.GetChildrenWithType(nsd, typeof(UsingDeclaration));
			if (nsUsings.Count == 0)
			{
				CompilationUnit compilationUnit = (CompilationUnit) nsd.Parent;
				if (compilationUnit != null)
					nsUsings = AstUtil.GetChildrenWithType(compilationUnit, typeof(UsingDeclaration));
			}

			string typeR = GetAliasUsing(nsUsings, typeReference);
			if (typeR != null)
				return typeR;
			foreach (UsingDeclaration us in nsUsings)
			{
				Using uss = (Using) us.Usings[0];

				string usingName = uss.Name;
				if (usingName.EndsWith(".*"))
					usingName = usingName.Substring(0, usingName.Length - 2);
				if (uss.IsAlias)
				{
					if (uss.Alias.Type.EndsWith("." + typeReference.Type))
						return uss.Alias.Type;
					if (usingName == typeReference.Type)
						return uss.Alias.Type;
				}
				else if (usingName.EndsWith("." + typeReference.Type))
					return usingName;
				else if (CodeBase.Types.Contains(usingName + "." + typeReference.Type))
					return usingName + "." + typeReference.Type;
			}
			if (CodeBase.Types.Contains(nsd.Name + "." + typeReference.Type))
				return nsd.Name + "." + typeReference.Type;

			return "java.lang." + typeReference.Type;
		}

		private string GetFullName(TypeReference typeReference, NamespaceDeclaration namespaceDeclaration)
		{
			string fullNameQualified = namespaceDeclaration.Name + "." + typeReference.Type;
			if (CodeBase.Types.Contains(fullNameQualified))
				return fullNameQualified;
			else
				return null;
		}

		public string GetFullName(TypeDeclaration typeDeclaration)
		{
			if (typeDeclaration.Parent is NamespaceDeclaration)
			{
				NamespaceDeclaration namespaceDeclaration = (NamespaceDeclaration) typeDeclaration.Parent;
				return namespaceDeclaration.Name + "." + typeDeclaration.Name;
			}
			else if (typeDeclaration.Parent is TypeDeclaration)
			{
				TypeDeclaration parentType = (TypeDeclaration) typeDeclaration.Parent;
				return GetFullName(parentType) + '$' + typeDeclaration.Name;
			}
			else if (typeDeclaration.Parent is ClassDeclarationStatement)
			{
				ClassDeclarationStatement cd = (ClassDeclarationStatement) typeDeclaration.Parent;
				INode parentMember = cd.Parent.Parent;
				string memberName;
				if (parentMember is MethodDeclaration)
					memberName = ((MethodDeclaration) parentMember).Name;
				else
					memberName = ((ConstructorDeclaration) parentMember).Name;
				TypeDeclaration parentType = (TypeDeclaration) parentMember.Parent;
				return GetFullName(parentType) + "." + memberName + "." + typeDeclaration.Name;
			}
			return null;
		}

		public string GetStaticFullName(string identifier, INode parent)
		{
			if (Char.IsUpper(identifier[0]))
			{
				TypeReference typeRef = AstUtil.GetTypeReference(identifier, parent);
				return GetFullName(typeRef);
			}
			else
				return null;
		}

		private string GetAliasUsing(IList usings, TypeReference type)
		{
			foreach (UsingDeclaration usingDeclaration in usings)
			{
				Using us = (Using) usingDeclaration.Usings[0];
				if (us.IsAlias)
				{
					if (us.Name == type.Type)
						return us.Alias.Type;
				}
			}
			return null;
		}
	}
}