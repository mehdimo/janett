namespace Janett.Framework
{
	using System;
	using System.Collections;
	using System.IO;

	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.SharpZipLib.Zip;

	using Janett.Commons;

	public class TypeDictionary : SortedList
	{
		public IList ExternalLibraries = new ArrayList();
		public string LibrariesFolder = "Libraries";

		private IDictionary zipCache = new Hashtable();
		private AstUtil AstUtil = new AstUtil();

		private SupportedLanguage language;

		public TypeDictionary(SupportedLanguage language)
		{
			this.language = language;
		}

		public override bool Contains(object key)
		{
			bool contains = base.Contains(key);
			if (contains)
				return (base[key] != null);
			if (!Directory.Exists(LibrariesFolder))
				return false;

			string fileContent = GetFile(key.ToString());
			if (fileContent == null)
			{
				base.Add(key, null);
				return false;
			}

			StringReader reader = new StringReader(fileContent);
			IParser parser = ParserFactory.CreateParser(language, reader);
			parser.ParseMethodBodies = true;
			parser.Parse();
			CompilationUnit compilationUnit = parser.CompilationUnit;
			ParentVisitor parentVisitor = new ParentVisitor();
			parentVisitor.VisitCompilationUnit(compilationUnit, null);
			NamespaceDeclaration ns = (NamespaceDeclaration) compilationUnit.Children[0];
			TypeDeclaration typeDeclaration = (TypeDeclaration) ns.Children[0];
			if (key.ToString().IndexOf('$') != -1)
			{
				string innerType = key.ToString().Substring(key.ToString().IndexOf('$') + 1);
				typeDeclaration = GetInnerType(typeDeclaration, innerType);
			}
			string externalLib = ns.Name;
			if (!ExternalLibraries.Contains(externalLib))
				ExternalLibraries.Add(externalLib);

			base.Add(key, typeDeclaration);

			return true;
		}

		private string GetFile(string key)
		{
			string file = GetFileName(key);
			if (file != null)
				return FileSystemUtil.ReadFile(file);
			else
			{
				foreach (string zipFile in Directory.GetFiles(LibrariesFolder, "*.zip"))
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

		private string GetFileName(object key)
		{
			string typeName = key.ToString();
			string folder = LibrariesFolder;

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