namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class AccessorRefactoring : MethodRelatedTransformer
	{
		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) methodDeclaration.Parent;
			IList fields = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
			if (methodDeclaration.Name.Length > 3)
			{
				string name = methodDeclaration.Name.Substring(3);
				if (IsAccessor(methodDeclaration, fields)
				    || IsInterfaceOrAbstract(typeDeclaration) && ImplementInheritors(typeDeclaration, methodDeclaration)
				    || ImplementSiblings(typeDeclaration, methodDeclaration))
				{
					TypeReference typeReference = GetAccessorTypeReference(methodDeclaration);
					IList properties = AstUtil.GetChildrenWithType(typeDeclaration, typeof(PropertyDeclaration));
					PropertyDeclaration propertyDeclaration = GetProperty(properties, name, methodDeclaration.Modifier, typeReference);
					if (methodDeclaration.Name.StartsWith("set"))
						AddSetSection(propertyDeclaration, methodDeclaration, fields);
					else if (methodDeclaration.Name.StartsWith("get"))
						AddGetSection(propertyDeclaration, methodDeclaration, fields);
					AddToReferences(typeDeclaration, methodDeclaration.Name, propertyDeclaration.Name);
					if (!HasProperty(properties, name))
						ReplaceCurrentNode(propertyDeclaration);
					else
						RemoveCurrentNode();
				}
			}
			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}

		private bool IsAccessor(MethodDeclaration methodDeclaration, IList fields)
		{
			if (methodDeclaration.Name.StartsWith("set") && methodDeclaration.Parameters.Count == 1)
			{
				TypeReference type = ((ParameterDeclarationExpression) methodDeclaration.Parameters[0]).TypeReference;
				return FieldExists(fields, methodDeclaration, type);
			}
			else if (methodDeclaration.Name.StartsWith("get") && methodDeclaration.Parameters.Count == 0)
			{
				TypeReference type = methodDeclaration.TypeReference;
				return FieldExists(fields, methodDeclaration, type);
			}
			else
				return false;
		}

		private bool AreEqual(TypeReference first, TypeReference second)
		{
			return (first.Type == second.Type && first.RankSpecifier.Length == second.RankSpecifier.Length);
		}

		private void AddSetSection(PropertyDeclaration propertyDeclaration, MethodDeclaration setterMethod, IList fields)
		{
			AddGetSet(propertyDeclaration, setterMethod, fields);
			string parameter = ((ParameterDeclarationExpression) setterMethod.Parameters[0]).ParameterName;
			propertyDeclaration.SetRegion.Block.AcceptChildren(this, parameter);
		}

		private void AddGetSection(PropertyDeclaration propertyDeclaration, MethodDeclaration getterMethod, IList fields)
		{
			AddGetSet(propertyDeclaration, getterMethod, fields);
		}

		private void AddGetSet(PropertyDeclaration propertyDeclaration, MethodDeclaration accessorMethod, IList fields)
		{
			BlockStatement block;
			string fieldName = accessorMethod.Name.Substring(3);
			TypeDeclaration typeDeclaration = (TypeDeclaration) accessorMethod.Parent;
			if (IsInterface(typeDeclaration) || (IsAbstractClass(typeDeclaration) && !HasField(fields, fieldName) && accessorMethod.Body is NullBlockStatement))
				block = NullBlockStatement.Instance;
			else
			{
				block = new BlockStatement();
				block.Children.AddRange(accessorMethod.Body.Children);
			}
			if (accessorMethod.Name.StartsWith("get"))
				propertyDeclaration.GetRegion = new PropertyGetRegion(block, null);
			else if (accessorMethod.Name.StartsWith("set"))
				propertyDeclaration.SetRegion = new PropertySetRegion(block, null);
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (data != null && data is string)
			{
				string identifier = identifierExpression.Identifier;
				if (identifier == (string) data)
					ReplaceCurrentNode(new IdentifierExpression("value"));
			}
			return base.TrackedVisitIdentifierExpression(identifierExpression, data);
		}

		private void AddToReferences(TypeDeclaration typeDeclaration, string accessorName, string PropertyName)
		{
			string fullType = GetFullName(typeDeclaration);
			CodeBase.References.Add(fullType + "." + accessorName, PropertyName);
			if (IsInterface(typeDeclaration))
			{
				foreach (string inheritor in CodeBase.Inheritors[fullType])
				{
					TypeDeclaration inheritorType = (TypeDeclaration) CodeBase.Types[inheritor];
					if (IsInterface(inheritorType))
						CodeBase.References.Add(inheritor + "." + accessorName, PropertyName);
				}
			}
		}

		private bool ImplementInheritors(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			string fullName = GetFullName(typeDeclaration);
			bool flag = false;
			foreach (string inherited in CodeBase.Inheritors[fullName])
			{
				if (CodeBase.Types.Contains(inherited))
				{
					TypeDeclaration inheritedType = (TypeDeclaration) CodeBase.Types[inherited];
					bool definedAccessor = IsAccessor(inheritedType, methodDeclaration);
					bool abstractDefinedAccessor = IsAbstractClass(inheritedType) && definedAccessor;
					if (!IsInterfaceOrAbstract(inheritedType) || abstractDefinedAccessor)
						flag = definedAccessor;
					else
						flag = ImplementInheritors(inheritedType, methodDeclaration);
				}
				if (flag)
					return flag;
			}
			return false;
		}

		private bool ImplementSiblings(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			bool implemented = false;
			foreach (TypeReference baseType in typeDeclaration.BaseTypes)
			{
				string fullBaseType = GetFullName(baseType);
				TypeDeclaration superType = (TypeDeclaration) CodeBase.Types[fullBaseType];
				if (superType != null)
				{
					implemented = ImplementInheritors(superType, methodDeclaration);
					if (!implemented)
						implemented = ImplementSiblings(superType, methodDeclaration);
				}
				if (implemented)
					return true;
			}
			return implemented;
		}

		private bool IsAccessor(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			IList methods = AstUtil.GetChildrenWithType(typeDeclaration, typeof(MethodDeclaration));
			if (Contains(methods, methodDeclaration))
			{
				IList fields = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
				return IsAccessor(methodDeclaration, fields);
			}
			else
			{
				string propertyName = methodDeclaration.Name.Substring(3);
				IList properties = AstUtil.GetChildrenWithType(typeDeclaration, typeof(PropertyDeclaration));
				return HasProperty(properties, propertyName);
			}
		}

		private bool FieldExists(IList fields, MethodDeclaration methodDeclaration, TypeReference type)
		{
			string name = methodDeclaration.Name.Substring(3);
			FieldDeclaration field = GetField(fields, name);
			return (field != null) && AreEqual(field.TypeReference, type);
		}

		private bool HasField(IList fields, string fieldName)
		{
			return (GetField(fields, fieldName) != null);
		}

		private FieldDeclaration GetField(IList fields, string name)
		{
			foreach (FieldDeclaration field in fields)
			{
				if (name.ToUpper() == ((VariableDeclaration) field.Fields[0]).Name.ToUpper())
					return field;
			}
			return null;
		}

		private bool HasProperty(IList properties, string propertyName)
		{
			return (GetProperty(properties, propertyName) != null);
		}

		private PropertyDeclaration GetProperty(IList properties, string name, Modifiers modifier, TypeReference typeReference)
		{
			PropertyDeclaration property = GetProperty(properties, name);
			if (property == null)
			{
				property = new PropertyDeclaration(modifier, null, name, null);
				property.TypeReference = typeReference;
				return property;
			}
			else
				return property;
		}

		private PropertyDeclaration GetProperty(IList properties, string name)
		{
			foreach (PropertyDeclaration property in properties)
			{
				if (property.Name == name)
					return property;
			}
			return null;
		}

		private bool IsInterfaceOrAbstract(TypeDeclaration typeDeclaration)
		{
			return IsInterface(typeDeclaration) || IsAbstractClass(typeDeclaration);
		}

		private bool IsInterface(TypeDeclaration typeDeclaration)
		{
			return typeDeclaration.Type == ClassType.Interface;
		}

		private TypeReference GetAccessorTypeReference(MethodDeclaration accessorMethod)
		{
			if (accessorMethod.Name.StartsWith("get"))
				return accessorMethod.TypeReference;
			else if (accessorMethod.Name.StartsWith("set"))
				return ((ParameterDeclarationExpression) accessorMethod.Parameters[0]).TypeReference;
			else
				return null;
		}
	}
}