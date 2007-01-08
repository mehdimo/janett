namespace Janett.Framework
{
	using System.Collections;
	using System.IO;
	using System.Xml;

	using Janett.Commons;

	public class Project
	{
		public string Folder;
		public string OutputFolder;
		public string Name;
		public string Template;
		public string RelPath;
		public string Guid;
		public string AssemblyName;
		public string Reference;

		private XmlDocument projectDocument;

		public FileSet FileSet;

		private IDictionary includes = new SortedList(new FileDirectoryComparer());

		public void Create()
		{
			projectDocument = new XmlDocument();
			string projectContents = Template;
			projectContents = projectContents.Replace("#AssemblyName#", AssemblyName);
			projectContents = projectContents.Replace("#GUID#", "{" + Guid + "}");
			projectDocument.LoadXml(projectContents);
		}

		public void AddFile(string file, bool codeFile)
		{
			includes.Add(file, codeFile);
		}

		public void AddAssemblyReference(string path)
		{
			XmlNode referencePath = projectDocument.SelectSingleNode("/VisualStudioProject/CSHARP/Build/References");
			XmlElement elem = projectDocument.CreateElement("Reference");
			AddAttribute(elem, "Name", Path.GetFileNameWithoutExtension(path));
			AddAttribute(elem, "AssemblyName", Path.GetFileNameWithoutExtension(path));
			AddAttribute(elem, "HintPath", path);
			referencePath.AppendChild(elem);
		}

		public void AddProjectReference(Project p)
		{
			XmlNode referencePath = projectDocument.SelectSingleNode("/VisualStudioProject/CSHARP/Build/References");
			XmlElement elem = projectDocument.CreateElement("Reference");
			AddAttribute(elem, "Name", p.Name);
			AddAttribute(elem, "Project", "{" + p.Guid + "}");
			AddAttribute(elem, "Package", "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");
			referencePath.AppendChild(elem);
		}

		public void AddLink(string path)
		{
			XmlNode includeNode = projectDocument.SelectSingleNode("/VisualStudioProject/CSHARP/Files/Include");
			XmlElement elem = projectDocument.CreateElement("File");
			AddAttribute(elem, "RelPath", Path.GetFileName(path));
			AddAttribute(elem, "Link", path);
			AddAttribute(elem, "SubType", "Code");
			AddAttribute(elem, "BuildAction", "Compile");
			includeNode.AppendChild(elem);
		}

		public void Save()
		{
			foreach (DictionaryEntry entry in includes)
			{
				string file = (string) entry.Key;
				bool codeFile = (bool) entry.Value;
				XmlNode includeNode = projectDocument.SelectSingleNode("/VisualStudioProject/CSHARP/Files/Include");
				XmlElement elem = projectDocument.CreateElement("File");
				AddAttribute(elem, "RelPath", file);
				if (codeFile)
				{
					AddAttribute(elem, "SubType", "Code");
					AddAttribute(elem, "BuildAction", "Compile");
				}
				else
					AddAttribute(elem, "BuildAction", "Content");
				includeNode.AppendChild(elem);
			}
			using (StreamWriter streamWriter = new StreamWriter(Path.Combine(OutputFolder, Name + ".csproj")))
			{
				ProjectTextWriter textWriter = new ProjectTextWriter();

				ProjectXmlWriter xmlWriter = new ProjectXmlWriter(textWriter);
				xmlWriter.Formatting = Formatting.Indented;
				xmlWriter.Indentation = 4;
				projectDocument.Save(xmlWriter);

				string text = textWriter.ToString();
				text = text.Replace("=\"", "= \"");
				streamWriter.WriteLine(text + "\r\n");
			}
		}

		private void AddAttribute(XmlElement elem, string name, string value)
		{
			XmlAttribute attr = projectDocument.CreateAttribute(name);
			attr.Value = value;
			elem.Attributes.Append(attr);
		}
	}
}