namespace Janett.Framework
{
	using System;
	using System.Collections;
	using System.IO;

	using Commons;

	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.SharpZipLib.Zip;

	public class TypeDictionary : SortedList
	{
		public IList Visitors = new ArrayList();
		public IList ExternalLibraries = new ArrayList();
		public string LibrariesFolder = "Libraries";
		public string HelpersFolder = "Helpers";

		private IDictionary zipCache = new Hashtable();
		private AstUtil AstUtil = new AstUtil();

		private SupportedLanguage language;

		public TypeDictionary(SupportedLanguage language)
		{
			this.language = language;
		}

		private void Visit(CompilationUnit compilationUnit)
		{
			foreach (IAstVisitor visitor in Visitors)
				visitor.VisitCompilationUnit(compilationUnit, null);
		}

		public override bool Contains(object key)
		{
			bool contains = base.Contains(key);
			if (contains)
				return (base[key] != null);

			bool exists = false;
			if (Directory.Exists(LibrariesFolder))
				exists = LoadFile(key, LibrariesFolder, language, true);
			if (!exists)
			{
				if (Directory.Exists(HelpersFolder))
					exists = LoadFile(key, HelpersFolder, SupportedLanguage.CSharp, false);
			}
			if (!exists)
				base.Add(key, null);
			return exists;
		}

		private bool LoadFile(object key, string folder, SupportedLanguage supportedLanguage, bool visit)
		{
			TypeDeclaration typeDeclaration;
			string fileContent = GetFile(key.ToString(), folder, supportedLanguage);
			if (fileContent == null)
				return false;

			StringReader reader = new StringReader(fileContent);
			IParser parser = ParserFactory.CreateParser(supportedLanguage, reader);
			parser.ParseMethodBodies = true;
			parser.Parse();
			CompilationUnit compilationUnit = parser.CompilationUnit;
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			typeDeclaration = GetTypeDeclaration(ns, key.ToString());
			if (key.ToString().IndexOf('$') != -1)
			{
				string innerType = key.ToString().Substring(key.ToString().IndexOf('$') + 1);
				typeDeclaration = GetInnerType(typeDeclaration, innerType);
			}
			string externalLib = ns.Name;
			if (!ExternalLibraries.Contains(externalLib) && externalLib != "Helpers")
				ExternalLibraries.Add(externalLib);

			base.Add(key, typeDeclaration);
			if (visit)
				Visit(compilationUnit);

			return true;
		}

		private TypeDeclaration GetTypeDeclaration(NamespaceDeclaration namespaceDeclaration, string name)
		{
			IList types = AstUtil.GetChildrenWithType(namespaceDeclaration, typeof(TypeDeclaration));
			foreach (TypeDeclaration typeDeclaration in types)
			{
				if (name.StartsWith(namespaceDeclaration.Name + "." + typeDeclaration.Name))
					return typeDeclaration;
			}
			return (TypeDeclaration) namespaceDeclaration.Children[namespaceDeclaration.Children.Count - 1];
		}

		private string GetFile(string key, string folder, SupportedLanguage language)
		{
			string file = GetFileName(key, folder, language);
			if (file != null)
				return FileSystemUtil.ReadFile(file);
			else
			{
				foreach (string zipFile in Directory.GetFiles(folder, "*.zip"))
				{
					string zipFileName = Path.GetFileNameWithoutExtension(zipFile);
					if (key.StartsWith(zipFileName + "."))
					{
						ZipFile zf = GetZipFile(zipFile);
						if (key.IndexOf('$') != -1)
							key = key.Substring(0, key.IndexOf('$'));
						string zipKey = key.Replace('.', '/') + "." + GetExtension(language);
						ZipEntry ze = zf.GetEntry(zipKey);
						if (ze != null)
							return ReadZipEntry(zf, ze);
					}
				}
			}
			return null;
		}

		private string GetFileName(object key, string folder, SupportedLanguage language)
		{
			string typeName = key.ToString();
			if (folder == HelpersFolder && !typeName.StartsWith("Helpers."))
				return null;
			typeName = typeName.Replace("Helpers.", "");

			if (typeName.IndexOf('.') != -1)
			{
				string subFolder = typeName.Substring(0, typeName.LastIndexOf('.'));
				subFolder = subFolder.Replace('.', '\\');
				folder = Path.Combine(folder, subFolder);
				typeName = typeName.Substring(typeName.LastIndexOf('.') + 1);
			}
			if (typeName.IndexOf('$') != -1)
				typeName = typeName.Substring(0, typeName.IndexOf('$'));
			folder = Path.GetFullPath(folder);
			if (Directory.Exists(folder))
			{
				string file = Path.Combine(folder, typeName) + "." + GetExtension(language);
				if (File.Exists(file))
					return file;
			}

			return null;
		}

		private ZipFile GetZipFile(string zipFileName)
		{
			if (zipCache.Contains(zipFileName))
				return (ZipFile) zipCache[zipFileName];
			else
			{
				ZipFile zipFile = new ZipFile(zipFileName);
				zipCache.Add(zipFileName, zipFile);
				return zipFile;
			}
		}

		private string ReadZipEntry(ZipFile zipFile, ZipEntry zipEntry)
		{
			Stream stream = zipFile.GetInputStream(zipEntry);
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

		private string GetExtension(SupportedLanguage lang)
		{
			if (lang == SupportedLanguage.CSharp)
				return "cs";
			else if (lang == SupportedLanguage.Java)
				return "java";
			else
				throw new NotSupportedException();
		}

		private TypeDeclaration GetInnerType(TypeDeclaration typeDeclaration, string innerTypeName)
		{
			IList children = AstUtil.GetChildrenWithType(typeDeclaration, typeof(TypeDeclaration));
			foreach (TypeDeclaration type in children)
			{
				if (type.Name == innerTypeName)
					return type;
			}
			return typeDeclaration;
		}
	}
}