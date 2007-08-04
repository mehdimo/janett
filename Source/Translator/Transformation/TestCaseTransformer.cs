namespace Janett.Translator
{
	using System.Collections.Generic;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class TestCaseTransformer : MethodRelatedTransformer
	{
		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (IsDerivedFrom(typeDeclaration, "junit.framework.TestCase"))
			{
				if (!(typeDeclaration.Name.StartsWith("Abstract")))
				{
					TypeDeclaration newType = typeDeclaration;
					Attribute attr = new Attribute("NUnit.Framework.TestFixture", null, null);
					List<Attribute> attributes = new List<Attribute>();
					attributes.Add(attr);
					AttributeSection attrSection = new AttributeSection(null, attributes);
					newType.Attributes.Add(attrSection);

					ReplaceCurrentNode(newType);
				}
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}

		public override object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) methodDeclaration.Parent;
			string testcase = "junit.framework.TestCase";
			if (typeDeclaration != null && ((IsDerivedFrom(typeDeclaration, testcase) &&
			                                 !(typeDeclaration.Name.StartsWith("Abstract"))) || IsAllTestRunner(typeDeclaration.Name)))
			{
				if (methodDeclaration.Name == "main" || methodDeclaration.Name == "suite")
					RemoveCurrentNode();

				else if (methodDeclaration.Name.StartsWith("test"))
				{
					MethodDeclaration replaced = methodDeclaration;
					Attribute attr = new Attribute("NUnit.Framework.Test", null, null);
					List<Attribute> attributes = new List<Attribute>();
					attributes.Add(attr);
					AttributeSection testAttribute = new AttributeSection(null, attributes);
					replaced.Attributes.Add(testAttribute);
					AstUtil.RemoveModifierFrom(replaced, Modifiers.Static);

					ReplaceCurrentNode(replaced);
				}
				else if (methodDeclaration.Name == "setUp" || methodDeclaration.Name == "tearDown")
				{
					if (Mode == "DotNet")
					{
						MethodDeclaration replaced = methodDeclaration;
						replaced.Modifier = Modifiers.Public;

						string attributeName = "NUnit.Framework.SetUp";
						if (methodDeclaration.Name == "tearDown")
							attributeName = "NUnit.Framework.TearDown";

						Attribute attribute = new Attribute(attributeName, null, null);
						List<Attribute> attributes = new List<Attribute>();
						attributes.Add(attribute);
						AttributeSection attributeSection = new AttributeSection(null, attributes);
						replaced.Attributes.Add(attributeSection);

						ReplaceCurrentNode(replaced);
					}
					else if (Mode == "IKVM")
					{
						methodDeclaration.Modifier = Modifiers.Protected | Modifiers.Override;
					}
				}
			}
			return null;
		}

		private bool IsAllTestRunner(string typeName)
		{
			return typeName.IndexOf("All") != -1 && typeName.IndexOf("Test") != -1;
		}

		public override object TrackedVisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			TypeDeclaration typeDeclaration = (TypeDeclaration) constructorDeclaration.Parent;
			if (IsDerivedFrom(typeDeclaration, "junit.framework.TestCase"))
			{
				if (constructorDeclaration.Parameters.Count > 0)
					RemoveCurrentNode();
			}
			return null;
		}
	}
}