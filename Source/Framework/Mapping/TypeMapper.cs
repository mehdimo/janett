namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class TypeMapper : UsageRemoverTransformer
	{
		public override object TrackedVisitTypeReference(TypeReference typeReference, object data)
		{
			string type = GetFullName(typeReference);
			string ns = type.Substring(0, type.LastIndexOf('.'));
			if (CodeBase.Mappings.Contains(type))
			{
				TypeReference dotNetType = typeReference;
				dotNetType.Type = CodeBase.Mappings[type].Target;
				if (!(Removeables.Contains(type) || Removeables.Contains(ns)))
				{
					Removeables.Add(type);
					Removeables.Add(ns);
				}

				if (!UsedTypes.Contains(dotNetType.Type))
					UsedTypes.Add(dotNetType.Type);

				ReplaceCurrentNode(dotNetType);
			}
			else
			{
				if (!UsedTypes.Contains(type))
					UsedTypes.Add(type);
				if (!UsedTypes.Contains(ns))
					UsedTypes.Add(ns);
			}

			return null;
		}

		public override object TrackedVisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			if (fieldReferenceExpression.TargetObject is FieldReferenceExpression)
			{
				FieldReferenceExpression targetObject = (FieldReferenceExpression) fieldReferenceExpression.TargetObject;
				if (targetObject.TargetObject is IdentifierExpression)
				{
					IdentifierExpression id = (IdentifierExpression) targetObject.TargetObject;
					if (id.Identifier == "Helpers")
					{
						string helper = id.Identifier + "." + targetObject.FieldName;
						if (!UsedTypes.Contains(helper))
							UsedTypes.Add(helper);
					}
				}
			}
			return base.TrackedVisitFieldReferenceExpression(fieldReferenceExpression, data);
		}

		public override object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			IList staticTypes = new ArrayList();

			staticTypes.Add("java.lang.String");
			staticTypes.Add("java.lang.Object");

			string type = GetStaticFullName(identifierExpression.Identifier, identifierExpression);
			if (type != null)
			{
				if (!staticTypes.Contains(type) && CodeBase.Mappings.Contains(type))
				{
					Expression rpe = GetExpression(CodeBase.Mappings[type].Target);
					ReplaceCurrentNode(rpe);
				}
				else if (CodeBase.Types.Contains(type))
				{
					if (!UsedTypes.Contains(type))
						UsedTypes.Add(type);
				}
			}
			return null;
		}

		private Expression GetExpression(string expressionString)
		{
			if (expressionString.IndexOf('.') != -1)
			{
				IdentifierExpression ide = new IdentifierExpression(expressionString.Substring(0, expressionString.IndexOf('.')));
				return AstUtil.CreateFiledReferenceExpression(ide, expressionString.Substring(expressionString.IndexOf('.') + 1));
			}
			return new IdentifierExpression(expressionString);
		}

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (Mode == "DotNet")
			{
				string testCaseClass = "NUnit.Framework.TestCase";
				int index = GetBaseTypeIndex(typeDeclaration, testCaseClass);
				if (index != -1)
				{
					Removeables.Add(testCaseClass);
					TypeDeclaration replacedType = RemoveBaseTypeFrom(typeDeclaration, (TypeReference) typeDeclaration.BaseTypes[index]);
					ReplaceCurrentNode(replacedType);
				}
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}
	}
}