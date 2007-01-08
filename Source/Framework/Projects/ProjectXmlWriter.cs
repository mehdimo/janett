namespace Janett.Framework
{
	using System.Xml;

	public class ProjectXmlWriter : XmlTextWriter
	{
		private ProjectTextWriter writer;

		public ProjectXmlWriter(ProjectTextWriter w) : base(w)
		{
			writer = w;
		}

		public override void WriteStartDocument()
		{
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (Formatting == Formatting.Indented)
				Indent();
			base.WriteStartAttribute(prefix, localName + " ", ns);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			writer.Depth++;
			base.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteFullEndElement()
		{
			writer.Depth--;
			base.WriteFullEndElement();
		}

		public override void WriteEndElement()
		{
			writer.Depth--;
			Indent();
			base.WriteEndElement();
		}

		private void Indent()
		{
			writer.WriteLine();
			for (int i = 0; i < (Indentation * writer.Depth) - 1; i++)
				writer.Write(IndentChar);
		}
	}
}