namespace Janett.Translator
{
	using System.Collections;

	using ICSharpCode.NRefactory.Ast;

	using Janett.Framework;

	public class SerializableTransformer : UsageRemoverTransformer
	{
		private string serializableType = "java.io.Serializable";

		public override object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			int index = GetBaseTypeIndex(typeDeclaration, serializableType);
			if (index != -1)
			{
				Removeables.Add(serializableType);
				ArrayList attributes = new ArrayList();
				Attribute attribute = new Attribute("System.SerializableAttribute", null, null);
				attributes.Add(attribute);
				AttributeSection attributeSection = new AttributeSection("", attributes);

				typeDeclaration.Attributes.Add(attributeSection);
				attributeSection.Parent = typeDeclaration;

				typeDeclaration = RemoveBaseTypeFrom(typeDeclaration, (TypeReference) typeDeclaration.BaseTypes[index]);
			}
			return base.TrackedVisitTypeDeclaration(typeDeclaration, data);
		}
	}
}