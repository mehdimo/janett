namespace Janett.Translator
{
	using System.Collections;
	using System.Collections.Specialized;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class AnonymousClassTransformer : Transformer
	{
		private class DataObject
		{
			public DataObject(TypeDeclaration parentClass, ObjectCreateExpression objectCreation, IDictionary list)
			{
				this.parentClass = parentClass;
				this.objectCreation = objectCreation;
				this.list = list;
			}

			public TypeDeclaration parentClass;
			public TypeDeclaration anonymous;
			public ObjectCreateExpression objectCreation;
			public IDictionary list;
		}

		private int count = 0;
		private IDictionary unknownFields = new ListDictionary();

		public override object TrackedVisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			count = 0;
			return base.TrackedVisitCompilationUnit(compilationUnit, data);
		}

		public override object TrackedVisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			if (!objectCreateExpression.AnonymousClass.IsNull)
			{
				count ++;
				unknownFields.Clear();

				TypeDeclaration parentClass = (TypeDeclaration) AstUtil.GetParentOfType(objectCreateExpression, typeof(TypeDeclaration));

				TypeDeclaration anonymousTypeDeclaration = objectCreateExpression.AnonymousClass;
				string anonymousName = "AnonymousClass" + objectCreateExpression.CreateType.Type + count.ToString();
				if (anonymousName.IndexOf('.') != -1)
					anonymousName = anonymousName.Replace('.', '_');
				anonymousTypeDeclaration.Name = anonymousName;

				if (parentClass.Type == ClassType.Class)
					AstUtil.ReplaceModifiers(anonymousTypeDeclaration, Modifiers.Public, Modifiers.Private);

				TypeReference baseType = AstUtil.GetTypeReference(objectCreateExpression.CreateType.Type, objectCreateExpression);
				anonymousTypeDeclaration.BaseTypes.Add(baseType);
				baseType.Parent = anonymousTypeDeclaration;
				anonymousTypeDeclaration.Parent = new TypeDeclaration(Modifiers.None, null);
				anonymousTypeDeclaration.Parent = parentClass;

				parentClass.Children.Add(anonymousTypeDeclaration);
				objectCreateExpression.CreateType.Type = anonymousTypeDeclaration.Name;

				DataObject dataObject = new DataObject(parentClass, objectCreateExpression, null);
				dataObject.anonymous = anonymousTypeDeclaration;
				GetAnonymousFields(anonymousTypeDeclaration, dataObject);
				IDictionary anonymousFields = unknownFields;

				ArrayList constructorParameters = new ArrayList();

				ParameterDeclarationExpression parentInstance = new ParameterDeclarationExpression(
					AstUtil.GetTypeReference(parentClass.Name, objectCreateExpression.Parent), "enclosingInstance");
				if (! IsInStaticScope(objectCreateExpression))
					constructorParameters.Add(parentInstance);

				foreach (DictionaryEntry entry in anonymousFields)
				{
					if (entry.Value is TypeReference)
					{
						TypeReference typeReference = (TypeReference) entry.Value;
						ParameterDeclarationExpression pm = new ParameterDeclarationExpression(typeReference, (string) entry.Key);
						constructorParameters.Add(pm);
					}
				}

				IList constructors = AstUtil.GetChildrenWithType(anonymousTypeDeclaration, typeof(ConstructorDeclaration));
				foreach (ConstructorDeclaration cd in constructors)
					cd.Name = anonymousTypeDeclaration.Name;
				AddAnonymousClassConstructor(objectCreateExpression, anonymousTypeDeclaration, constructorParameters);
				AddAnonymousClassFields(anonymousTypeDeclaration, constructorParameters);
				if (! IsInStaticScope(objectCreateExpression))
					AddAnonymousClassProperties(anonymousTypeDeclaration);

				IList objectCreationParameters = GetObjectCreationParameters(objectCreateExpression);
				foreach (FieldDeclaration parameter in objectCreationParameters)
				{
					string parameterName = ((VariableDeclaration) parameter.Fields[0]).Name;

					if (parameterName == "enclosingInstance")
						objectCreateExpression.Parameters.Add(new ThisReferenceExpression());
					else
						objectCreateExpression.Parameters.Add(
							new IdentifierExpression(parameterName));
				}

				string anonymousFullName = GetFullName(parentClass) + "." + anonymousTypeDeclaration.Name;
				CodeBase.Types.Add(anonymousFullName, anonymousTypeDeclaration);
				objectCreateExpression.AnonymousClass = null;
				return base.TrackedVisitObjectCreateExpression(objectCreateExpression, data);
			}
			return base.TrackedVisitObjectCreateExpression(objectCreateExpression, data);
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (data != null)
			{
				string identifier = identifierExpression.Identifier;
				DataObject dataObject = (DataObject) data;

				if (!IsInvocation(identifierExpression) && ! dataObject.list.Contains(identifier))
				{
					string staticFullName = GetStaticFullName(identifier, identifierExpression.Parent);
					if (staticFullName == null || !CodeBase.Types.Contains(staticFullName))
					{
						TypeReference typeReference = GetExpressionType(identifierExpression, dataObject.objectCreation);
						if (typeReference != null)
						{
							if (!unknownFields.Contains(identifier))
								unknownFields.Add(identifier, typeReference);
							if (!dataObject.list.Contains(identifier))
								dataObject.list.Add(identifier, typeReference);
						}
					}
				}
				else if (IsInvocation(identifierExpression))
				{
					TypeDeclaration typeDeclaration = dataObject.anonymous;
					if (typeDeclaration != null && typeDeclaration.Name.StartsWith("AnonymousClass"))
					{
						TypeDeclaration enclosingType = (TypeDeclaration) typeDeclaration.Parent;
						IList enclosingMethods = GetAccessibleMethods(enclosingType);
						if (ContainsMethod(enclosingMethods, identifier))
						{
							FieldReferenceExpression fieldReferenceExpression = new FieldReferenceExpression(new IdentifierExpression("enclosingInstance"), identifier);
							fieldReferenceExpression.Parent = identifierExpression.Parent;
							ReplaceCurrentNode(fieldReferenceExpression);
						}
					}
				}
			}

			return null;
		}

		private IDictionary GetAnonymousFields(TypeDeclaration anonymousTypeDeclaration, DataObject dataObject)
		{
			dataObject.list = new Hashtable();
			IList fields = AstUtil.GetChildrenWithType(anonymousTypeDeclaration, typeof(FieldDeclaration));
			foreach (FieldDeclaration field in fields)
			{
				base.TrackedVisitFieldDeclaration(field, dataObject);
			}

			IList constructors = AstUtil.GetChildrenWithType(anonymousTypeDeclaration, typeof(ConstructorDeclaration));
			foreach (ConstructorDeclaration constructor in constructors)
			{
				base.TrackedVisitConstructorDeclaration(constructor, dataObject);
			}
			IList methods = AstUtil.GetChildrenWithType(anonymousTypeDeclaration, typeof(MethodDeclaration));
			foreach (MethodDeclaration method in methods)
			{
				IDictionary varsInMethod = GetVarsAndFields(method);
				dataObject.list = varsInMethod;
				base.TrackedVisitMethodDeclaration(method, dataObject);
			}
			return null;
		}

		private IDictionary GetVarsAndFields(MethodDeclaration method)
		{
			IList varList = AstUtil.GetChildrenWithType(method.Body, typeof(LocalVariableDeclaration));
			IDictionary list = new Hashtable();

			foreach (LocalVariableDeclaration localVariable in varList)
			{
				VariableDeclaration variable = (VariableDeclaration) localVariable.Variables[0];
				list.Add(variable.Name, localVariable.TypeReference.Type);
			}
			foreach (ParameterDeclarationExpression parameter in method.Parameters)
				list.Add(parameter.ParameterName, parameter.TypeReference.Type);

			return list;
		}

		private bool IsInvocation(IdentifierExpression identifier)
		{
			if (identifier.Parent is InvocationExpression)
			{
				InvocationExpression idParent = (InvocationExpression) identifier.Parent;
				if (idParent.TargetObject is IdentifierExpression)
				{
					IdentifierExpression idParentTarget = (IdentifierExpression) idParent.TargetObject;
					if (idParentTarget.Identifier == identifier.Identifier)
						return true;
				}
			}
			return false;
		}

		private IList GetObjectCreationParameters(ObjectCreateExpression objectCreateExpression)
		{
			IList resList = new ArrayList();

			TypeDeclaration eqType = objectCreateExpression.AnonymousClass;
			if (eqType != null)
			{
				IList list = AstUtil.GetChildrenWithType(eqType, typeof(FieldDeclaration));
				ArrayList constructors = AstUtil.GetChildrenWithType(eqType, typeof(ConstructorDeclaration));
				if (constructors.Count > 0)
				{
					ConstructorDeclaration constructorDeclaration = (ConstructorDeclaration) constructors[0];
					resList = MaskList(list, constructorDeclaration.Parameters);
				}
			}
			return resList;
		}

		private void AddAnonymousClassConstructor(ObjectCreateExpression obc, TypeDeclaration typeDeclaration, ArrayList parameters)
		{
			ArrayList constructorParameters = new ArrayList();
			ArrayList baseArguments = GetBaseConstructorParameters(obc);
			constructorParameters.AddRange(baseArguments);
			constructorParameters.AddRange(parameters);
			ConstructorDeclaration constructor = new ConstructorDeclaration(typeDeclaration.Name, Modifiers.Public, constructorParameters, null);
			constructor.Body = new BlockStatement();
			constructor.Parent = typeDeclaration;
			string initializerType = null;
			IList constructors = AstUtil.GetChildrenWithType(typeDeclaration, typeof(ConstructorDeclaration));
			if (constructors.Count > 0)
				initializerType = "this";
			else if (baseArguments.Count > 0)
				initializerType = "base";

			InitializeConstructor(constructor, baseArguments, initializerType);

			foreach (ParameterDeclarationExpression paremeter in parameters)
			{
				ThisReferenceExpression thisReference = new ThisReferenceExpression();
				FieldReferenceExpression left = new FieldReferenceExpression(thisReference, paremeter.ParameterName);
				IdentifierExpression right = new IdentifierExpression(paremeter.ParameterName);
				AssignmentExpression assignment = new AssignmentExpression(left, AssignmentOperatorType.Assign, right);
				Statement stm = new ExpressionStatement(assignment);

				constructor.Body.Children.Add(stm);
			}

			typeDeclaration.Children.Insert(0, constructor);
		}

		private void AddAnonymousClassFields(TypeDeclaration typeDeclaration, IList fields)
		{
			foreach (ParameterDeclarationExpression parameter in fields)
			{
				FieldDeclaration field = new FieldDeclaration(null,
				                                              parameter.TypeReference, Modifiers.Private);

				field.Fields.Add(new VariableDeclaration(parameter.ParameterName));
				field.Parent = typeDeclaration;

				typeDeclaration.Children.Add(field);
			}
		}

		private void AddAnonymousClassProperties(TypeDeclaration typeDeclaration)
		{
			TypeReference typeReference = AstUtil.GetTypeReference(((TypeDeclaration) typeDeclaration.Parent).Name, typeDeclaration);
			PropertyDeclaration property =
				new PropertyDeclaration(Modifiers.Public, null, "Enclosing_Instance", null);
			property.TypeReference = typeReference;

			ReturnStatement returnStm = new ReturnStatement(new IdentifierExpression("enclosingInstance"));
			BlockStatement block = new BlockStatement();
			block.Children.Add(returnStm);
			property.GetRegion = new PropertyGetRegion(block, null);

			typeDeclaration.Children.Add(property);
		}

		private ArrayList GetBaseConstructorParameters(ObjectCreateExpression obc)
		{
			ArrayList result = new ArrayList();
			TypeDeclaration anonymousClass = obc.AnonymousClass;
			if (obc.Parameters.Count > 0)
			{
				TypeReference baseType = (TypeReference) anonymousClass.BaseTypes[0];
				string baseTypeName = GetFullName(baseType);
				if (CodeBase.Types.Contains(baseTypeName))
				{
					TypeDeclaration baseTypeDeclaration = (TypeDeclaration) CodeBase.Types[baseTypeName];
					IList cons = AstUtil.GetChildrenWithType(baseTypeDeclaration, typeof(ConstructorDeclaration));
					foreach (ConstructorDeclaration constructor in cons)
					{
						if (MatchArguments(constructor.Parameters, obc.Parameters))
							result.AddRange(constructor.Parameters);
					}
				}
			}
			return result;
		}

		private bool IsInStaticScope(ObjectCreateExpression objectCreation)
		{
			MethodDeclaration methodDeclaration = (MethodDeclaration) AstUtil.GetParentOfType(objectCreation, typeof(MethodDeclaration));
			if (methodDeclaration != null && AstUtil.ContainsModifier(methodDeclaration, Modifiers.Static))
				return true;
			FieldDeclaration fieldDeclaration = (FieldDeclaration) AstUtil.GetParentOfType(objectCreation, typeof(FieldDeclaration));
			if (fieldDeclaration != null && AstUtil.ContainsModifier(fieldDeclaration, Modifiers.Static))
				return true;
			return false;
		}

		private IList MaskList(IList workList, IList maskList)
		{
			IList result = new ArrayList();

			foreach (FieldDeclaration fieldDeclaration in workList)
			{
				foreach (ParameterDeclarationExpression pm in maskList)
				{
					if (pm.ParameterName == ((VariableDeclaration) fieldDeclaration.Fields[0]).Name)
						result.Add(fieldDeclaration);
				}
			}
			return result;
		}

		private void InitializeConstructor(ConstructorDeclaration constructor, ArrayList initializerArguments, string initializerType)
		{
			if (initializerType != null)
			{
				constructor.ConstructorInitializer = new ConstructorInitializer();

				if (initializerType == "base")
					constructor.ConstructorInitializer.ConstructorInitializerType = ConstructorInitializerType.Base;
				else if (initializerType == "this")
					constructor.ConstructorInitializer.ConstructorInitializerType = ConstructorInitializerType.This;

				constructor.ConstructorInitializer.Arguments = new ArrayList();
				if (initializerArguments != null)
				{
					foreach (ParameterDeclarationExpression pm in initializerArguments)
					{
						IdentifierExpression argument = new IdentifierExpression(pm.ParameterName);
						constructor.ConstructorInitializer.Arguments.Add(argument);
					}
				}
			}
		}

		private bool ContainsMethod(IList methods, string methodName)
		{
			foreach (MethodDeclaration method in methods)
			{
				if (method.Name == methodName && !AstUtil.ContainsModifier(method, Modifiers.Static))
					return true;
			}
			return false;
		}
	}
}