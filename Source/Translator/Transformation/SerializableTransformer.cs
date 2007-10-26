namespace Janett.Translator
{
	using System.Collections.Generic;

	using Framework;

	using ICSharpCode.NRefactory.Ast;

	public class SerializableTransformer : UsageRemoverTransformer
	{
		private string serializableType = "java.io.Serializable";

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			int index = GetBaseTypeIndex(typeDeclaration, serializableType);
			if (index != -1)
			{
				Removeables.Add(serializableType);
				if (typeDeclaration.Type == ClassType.Class)
				{
					List<Attribute> attributes = new List<Attribute>();
					Attribute attribute = new Attribute("System.SerializableAttribute", null, null);
					attributes.Add(attribute);
					AttributeSection attributeSection = new AttributeSection("", attributes);

					typeDeclaration.Attributes.Add(attributeSection);
					attributeSection.Parent = typeDeclaration;
				}
				typeDeclaration = RemoveBaseTypeFrom(typeDeclaration, (TypeReference) typeDeclaration.BaseTypes[index]);
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}
	}
}