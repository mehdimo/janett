namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class AccessorRefactoring : HierarchicalTraverser
	{
		private IList fields;

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			fields = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
			base.TrackedVisitTypeDeclaration(typeDeclaration, data);

			AddNotDeclaredAccessor(typeDeclaration);
			return null;
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) methodDeclaration.Parent;
			if (methodDeclaration.Name.Length > 3)
			{
				if (IsAccessor(methodDeclaration, fields)
				    || IsInterfaceOrAbstract(typeDeclaration) && ImplementInheritors(typeDeclaration, methodDeclaration)
				    || ImplementSiblings(typeDeclaration, methodDeclaration))
				{
					string name = GetPropertyName(methodDeclaration);
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

		private string GetPropertyName(MethodDeclaration methodDeclaration)
		{
			string name = methodDeclaration.Name.Substring(3);
			AccessorRefactoringHelper accessorRefactoringHelper = new AccessorRefactoringHelper();
			accessorRefactoringHelper.CodeBase = this.CodeBase;
			TypeDeclaration typeDeclaration = (TypeDeclaration) methodDeclaration.Parent;
			if (accessorRefactoringHelper.SimilarNameExistsInDependedTypes(typeDeclaration, methodDeclaration))
				name += "_Property";
			return name;
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
			if (IsInterface(typeDeclaration) || (IsAbstractClass(typeDeclaration) && !HasField(fields, fieldName) && accessorMethod.Body == BlockStatement.Null))
				block = BlockStatement.Null;
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
			string key = fullType + "." + accessorName;
			if (!CodeBase.References.Contains(key))
				CodeBase.References.Add(key, PropertyName);
			else if (CodeBase.References.Contains(key))
			{
				if (CodeBase.References[key].ToString() != PropertyName)
					CodeBase.References[key] = PropertyName;
			}
			if (CodeBase.Inheritors.Contains(fullType))
			{
				foreach (string inheritor in CodeBase.Inheritors[fullType])
				{
					TypeDeclaration inheritorType = (TypeDeclaration) CodeBase.Types[inheritor];
					AddToReferences(inheritorType, accessorName, PropertyName);
				}
			}
		}

		protected override bool VerifyTypeCondition(TypeDeclaration typeDeclaration, bool detailedCondition)
		{
			return !IsInterfaceOrAbstract(typeDeclaration) ||
			       (IsAbstractClass(typeDeclaration) && detailedCondition);
		}

		protected override bool VerifyMethodCondition(TypeDeclaration typeDeclaration, MethodDeclaration methodDeclaration)
		{
			return IsAccessor(typeDeclaration, methodDeclaration);
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
			{
				if (modifier > property.Modifier)
					property.Modifier = modifier;
				return property;
			}
		}

		private PropertyDeclaration GetProperty(IList properties, string name)
		{
			foreach (PropertyDeclaration property in properties)
			{
				if (property.Name == name || property.Name == name + "_Property")
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

		private void AddNotDeclaredAccessor(TypeDeclaration typeDeclaration)
		{
			IList properties = AstUtil.GetChildrenWithType(typeDeclaration, typeof(PropertyDeclaration));
			foreach (PropertyDeclaration property in properties)
			{
				if (!property.HasGetRegion || !property.HasSetRegion)
				{
					string accessorType;
					if (!property.HasSetRegion)
						accessorType = "set";
					else
						accessorType = "get";

					string propertyName = property.Name;
					ImplementPropertyRegionTransformer implementPropertyRegionTransformer = new ImplementPropertyRegionTransformer();
					implementPropertyRegionTransformer.CodeBase = CodeBase;
					if (implementPropertyRegionTransformer.ShouldAddAccessor(typeDeclaration, accessorType + propertyName, property.TypeReference))
					{
						BlockStatement block;
						if (IsInterface(typeDeclaration) || (IsAbstractClass(typeDeclaration) && !HasField(fields, propertyName)))
							block = BlockStatement.Null;
						else
						{
							block = new BlockStatement();
							ObjectCreateExpression notSupported = new ObjectCreateExpression(new TypeReference("System.NotSupportedException"), null);
							ThrowStatement throwStatement = new ThrowStatement(notSupported);
							block.Children.Add(throwStatement);
						}
						if (accessorType == "get")
							property.GetRegion = new PropertyGetRegion(block, null);
						else
							property.SetRegion = new PropertySetRegion(block, null);
					}
				}
			}
		}
	}
}