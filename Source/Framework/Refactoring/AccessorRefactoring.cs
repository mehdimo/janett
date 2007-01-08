namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class AccessorRefactoring : Transformer
	{
		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) methodDeclaration.Parent;
			IList fields = AstUtil.GetChildrenWithType(typeDeclaration, typeof(FieldDeclaration));
			if (methodDeclaration.Name.Length > 3)
			{
				string name = methodDeclaration.Name.Substring(3);
				if (HasField(fields, name) && (IsSetter(methodDeclaration, fields) || IsGetter(methodDeclaration, fields)))
				{
					FieldDeclaration field = GetField(fields, name);
					IList properties = AstUtil.GetChildrenWithType(typeDeclaration, typeof(PropertyDeclaration));
					PropertyDeclaration propertyDeclaration = GetProperty(properties, name, field.TypeReference);

					if (methodDeclaration.Name.StartsWith("set"))
						AddSetSection(propertyDeclaration, methodDeclaration);
					else if (methodDeclaration.Name.StartsWith("get"))
						AddGetSection(propertyDeclaration, methodDeclaration);

					string fullType = GetFullName(typeDeclaration);
					CodeBase.References.Add(fullType + "." + methodDeclaration.Name, propertyDeclaration.Name);
					if (!HasProperty(properties, name))
						ReplaceCurrentNode(propertyDeclaration);
					else
						RemoveCurrentNode();
				}
			}
			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}

		private bool IsSetter(MethodDeclaration methodDeclaration, IList fields)
		{
			if (methodDeclaration.Name.StartsWith("set") && methodDeclaration.Parameters.Count == 1)
			{
				TypeReference paramType = ((ParameterDeclarationExpression) methodDeclaration.Parameters[0]).TypeReference;
				string name = methodDeclaration.Name.Substring(3);
				FieldDeclaration field = GetField(fields, name);
				return AreEqual(field.TypeReference, paramType);
			}
			else
				return false;
		}

		private bool IsGetter(MethodDeclaration methodDeclaration, IList fields)
		{
			if (methodDeclaration.Name.StartsWith("get") && methodDeclaration.Parameters.Count == 0)
			{
				TypeReference returnType = methodDeclaration.TypeReference;
				string name = methodDeclaration.Name.Substring(3);
				FieldDeclaration field = GetField(fields, name);
				return AreEqual(field.TypeReference, returnType);
			}
			else
				return false;
		}

		private bool AreEqual(TypeReference first, TypeReference second)
		{
			return (first.Type == second.Type && first.RankSpecifier.Length == second.RankSpecifier.Length);
		}

		private void AddSetSection(PropertyDeclaration propertyDeclaration, MethodDeclaration setterMethod)
		{
			BlockStatement setBlock = new BlockStatement();
			setBlock.Children.AddRange(setterMethod.Body.Children);
			string parameter = ((ParameterDeclarationExpression) setterMethod.Parameters[0]).ParameterName;
			setBlock.AcceptChildren(this, parameter);
			propertyDeclaration.SetRegion = new PropertySetRegion(setBlock, null);
		}

		private void AddGetSection(PropertyDeclaration propertyDeclaration, MethodDeclaration getterMethod)
		{
			BlockStatement setBlock = new BlockStatement();
			setBlock.Children.AddRange(getterMethod.Body.Children);
			propertyDeclaration.GetRegion = new PropertyGetRegion(setBlock, null);
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

		private PropertyDeclaration GetProperty(IList properties, string name, TypeReference typeReference)
		{
			PropertyDeclaration property = GetProperty(properties, name);
			if (property == null)
			{
				property = new PropertyDeclaration(Modifiers.Public, null, name, null);
				property.TypeReference = typeReference;
			}
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
	}
}