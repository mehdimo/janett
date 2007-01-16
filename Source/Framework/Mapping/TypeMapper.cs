namespace Janett.Framework
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	public class TypeMapper : UsageRemoverTransformer
	{
		public override object TrackedVisitTypeReference(TypeReference typeReference, object data)
		{
			string type = GetFullName(typeReference);
			string ns = null;
			if (type.LastIndexOf('.') != -1)
				ns = type.Substring(0, type.LastIndexOf('.'));
			if (CodeBase.Mappings.Contains(type) && !IsInvocationTarget(typeReference))
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
					string mappedType = CodeBase.Mappings[type].Target;
					TypeReferenceExpression rpe = new TypeReferenceExpression(mappedType);
					rpe.Parent = identifierExpression.Parent;
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

		private bool IsInvocationTarget(TypeReference typeReference)
		{
			if (typeReference.Parent is TypeReferenceExpression)
				return (typeReference.Parent.Parent is FieldReferenceExpression);
			else 
				return false;
		}
	}
}