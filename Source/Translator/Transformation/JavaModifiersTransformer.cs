namespace Janett.Translator
{
	using System.Collections.Generic;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class JavaModifiersTransformer : Transformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (AstUtil.ContainsModifier(typeDeclaration, Modifiers.Final))
			{
				AstUtil.ReplaceModifiers(typeDeclaration, Modifiers.Final, Modifiers.Sealed);
				ReplaceCurrentNode(typeDeclaration);
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (AstUtil.ContainsModifier(methodDeclaration, Modifiers.Synchronized))
			{
				List<Expression> positionalArgs = new List<Expression>();
				TypeReferenceExpression system = new TypeReferenceExpression("System.Runtime.CompilerServices.MethodImplOptions");
				FieldReferenceExpression attributeArgument = new FieldReferenceExpression(system, "Synchronized");
				positionalArgs.Add(attributeArgument);

				AttributeSection attributeSection = CreateAttributeSection("System.Runtime.CompilerServices.MethodImplAttribute", positionalArgs);
				MethodDeclaration replacedMethod = methodDeclaration;
				replacedMethod.Attributes.Add(attributeSection);
				attributeSection.Parent = replacedMethod;
				ReplaceCurrentNode(replacedMethod);
			}
			return base.TrackedVisitMethodDeclaration(methodDeclaration, data);
		}

		public override object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			if (AstUtil.ContainsModifier(fieldDeclaration, Modifiers.Transient))
			{
				AttributeSection ats = CreateAttributeSection("System.NonSerializedAttribute", null);
				FieldDeclaration replacedField = fieldDeclaration;
				replacedField.Attributes.Add(ats);
				ReplaceCurrentNode(replacedField);
			}
			return base.TrackedVisitFieldDeclaration(fieldDeclaration, data);
		}

		private AttributeSection CreateAttributeSection(string attributeName, List<Expression> args)
		{
			Attribute attribute = new Attribute(attributeName, args, null);
			List<Attribute> attributes = new List<Attribute>();
			attributes.Add(attribute);
			AttributeSection attributeSection = new AttributeSection("", attributes);
			attribute.Parent = attributeSection;
			return attributeSection;
		}
	}
}