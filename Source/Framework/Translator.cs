namespace Janett.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using System.Xml;

	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.NRefactory.PrettyPrinter;
	using ICSharpCode.NRefactory.Visitors;

	using Janett.Commons;

	public class Translator : Commandlet
	{
		[CommandLineValue("InputFolder", 1, AlternateName = "i", Description = "Input folder")]
		public string InputFolder;

		[CommandLineValue("OutputFolder", 2, AlternateName = "o", Optional = true, Description = "Output folder")]
		public string OutputFolder;

		[CommandLineValue("Mode", AlternateName = "m", Optional = true, Description = "IKVM/DotNet")]
		public string Mode = "DotNet";

		public string Mappings;

		[CommandLineValue("Solution", AlternateName = "s", Optional = true, Description = "Solution name")]
		public string Solution;

		[CommandLineValue("Package", AlternateName = "p", Optional = true, Description = "Java package. Flatten directories")]
		public string Package;

		[CommandLineValue("Namespace", AlternateName = "n", Optional = true, Description = "namespaces renamed from 'Package' to this")]
		public string Namespace;

		[CommandLineValue("ReferenceFolder", AlternateName = "r", Optional = true, Description = "Where assemblies are referenced (relative)")]
		public string ReferenceFolder;

		public string HelperDirectory = @"Helpers";
		public string Libraries = @"Libraries";
		public IList Projects = new ArrayList();

		[CommandLineFlag("PreserveChanges", AlternateName = "pres", Optional = true, Description = "Preserve changes in re-translation. (default)")]
		public bool PreserveChanges = true;

		[CommandLineFlag("CreateProjects", AlternateName = "proj", Optional = true, Description = "Create Visual Studio projects. (default)")]
		public bool CreateProjects = true;

		[CommandLineFlag("Diagnostics", AlternateName = "diag", Optional = true, Description = "Diagnostics mode.")]
		public bool DiagnosticsMode = false;

		public IDictionary Sources = new ListDictionary();
		public Discovery Discovery = new Discovery();
		private KeyValuePairReader options;

		protected CodeBase codeBase;
		private TypesVisitor typesVisitor;
		private ParentVisitor parentVisitor;

		private ProgressController progress = new ProgressController();
		private int sourceFileCount;
		private string diagnosticsFile;

		public IDictionary MethodExcludes = new Hashtable();

		public override void Execute()
		{
			DateTime start = DateTime.Now;
			if (ExecutableDirectory == null)
				ExecutableDirectory = Path.GetFullPath(".");

			diagnosticsFile = Path.Combine(ExecutableDirectory, "Exception.txt");
			if (DiagnosticsMode && File.Exists(diagnosticsFile))
				Diagnostics.BreakOn(FileSystemUtil.ReadFile(diagnosticsFile));

			InputFolder = Path.GetFullPath(InputFolder);
			if (!Directory.Exists(InputFolder))
				throw new ApplicationException("Input folder does not exists.");

			LoadOptions();
			SetDefaultValues();
			ValidateParameters();

			if (Projects.Count == 0)
			{
				Project project = GetDefaultProject();
				Projects.Add(project);
			}

			Mappings = Path.GetFullPath(Mappings);
			SetOutputFolder();

			Discovery.AddAssembly(Assembly.GetExecutingAssembly());
			Discovery.AddAssembly(GetType().Assembly);

			string folderName = Solution + "-" + Mode;
			string translatedFolder = "Translated" + Path.DirectorySeparatorChar + folderName;
			translatedFolder = Path.Combine(ExecutableDirectory, translatedFolder);
			string patchFile = Path.GetFullPath(Path.Combine(InputFolder, Mode + ".patch"));

			if (PreserveChanges)
				CreateDiff(patchFile, translatedFolder);

			Translate();

			if (CreateProjects)
				CreateSolutionAndProject();

			if (PreserveChanges)
			{
				progress.Increment("CopyTranslated");
				CopyDirectory(OutputFolder, translatedFolder);
			}

			if (File.Exists(patchFile))
			{
				progress.Increment("Patch");
				CallProcess("patch.exe", options.GetKey("Preservation", "PatchParameters") + " -d " + OutputFolder + " -i " + patchFile);
			}

			if (File.Exists(diagnosticsFile))
				File.Delete(diagnosticsFile);
			TimeSpan timespan = DateTime.Now - start;
			Console.WriteLine("\n\nTranslation took {0} seconds.", (int) timespan.TotalSeconds);
		}

		protected virtual void BeforeParse()
		{
		}

		protected virtual void ValidateParameters()
		{
		}

		private void LoadOptions()
		{
			if (ExecutableName != null)
			{
				string optionsFile = Path.Combine(ExecutableDirectory, ExecutableName + ".options");
				if (File.Exists(optionsFile))
					options = new KeyValuePairReader(optionsFile);
			}
			else
				options = new KeyValuePairReader();
		}

		private void SetOutputFolder()
		{
			if (OutputFolder == null)
				OutputFolder = Path.Combine(Path.GetDirectoryName(InputFolder), Path.GetFileName(InputFolder) + "-" + Mode);
			else if (!Path.IsPathRooted(OutputFolder))
				OutputFolder = Path.GetFullPath(Path.Combine(InputFolder, OutputFolder));
		}

		private void SetDefaultValues()
		{
			if (Mappings == null)
			{
				Mappings = "Mappings" + Path.DirectorySeparatorChar + Mode;
				if (ExecutableDirectory.ToLower().EndsWith("bin\\debug"))
				{
					string projectFolder = Path.GetFullPath(".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar);
					Mappings = projectFolder + Mappings;
					HelperDirectory = projectFolder + HelperDirectory;
					Libraries = projectFolder + Libraries;
				}
				Mappings = Path.Combine(ExecutableDirectory, Mappings);
				HelperDirectory = Path.Combine(ExecutableDirectory, HelperDirectory);
				Libraries = Path.Combine(ExecutableDirectory, Libraries);
				if (Solution == null)
					Solution = Path.GetFileName(InputFolder);
				LoadTranslateXml();
				if (ReferenceFolder == null)
					ReferenceFolder = "Lib";
			}
		}

		private string CreateDiff(string patchFile, string translatedFolder)
		{
			translatedFolder = Path.Combine(ExecutableDirectory, translatedFolder);

			if (Directory.Exists(OutputFolder))
			{
				if (Directory.Exists(translatedFolder))
				{
					progress.Increment("Diff");

					string parameters = options.GetKey("Preservation", "DiffParameters") + " \"{0}\" \"{1}\"";
					parameters = string.Format(parameters, translatedFolder, OutputFolder);
					string output = CallProcess("diff.exe", parameters);
					if (output == "")
					{
						if (File.Exists(patchFile))
							File.Delete(patchFile);
					}
					else
					{
						output = Regex.Replace(output, @"\t(?<timestamp>\w{3}\s\w{3}\s\d{2}\s\d{2}:\d{2}:\d{2}\s\d{4})\r\n", "\r\n");
						output = Regex.Replace(output, @"diff\s.+\r\n", "");
						output = output.Replace(translatedFolder + Path.DirectorySeparatorChar, "");
						output = output.Replace(OutputFolder + Path.DirectorySeparatorChar, "");
						FileSystemUtil.WriteFile(patchFile, output);
					}
				}
			}
			return translatedFolder;
		}

		public string GetGuid(char ch)
		{
			return string.Format("{0}-{1}-{1}-{1}-{2}", RepeatChar(ch, 8), RepeatChar(ch, 4), RepeatChar(ch, 12));
		}

		public string RepeatChar(char ch, int count)
		{
			string str = "";
			for (int i = 0; i < count; i++)
				str += ch;
			return str;
		}

		private void CreateSolutionAndProject()
		{
			progress.Increment("CreatingProjects");
			Solution solution = new Solution(OutputFolder, Solution);
			int projectNumber = 65;
			foreach (Project project in Projects)
			{
				if (project.AssemblyName == null)
					project.AssemblyName = project.Name;
				if (project.Guid == null)
				{
					project.Guid = GetGuid((char) projectNumber);
					projectNumber++;
				}
				solution.Projects.Add(project);
			}

			foreach (Project project in solution.Projects)
			{
				project.Template = GetResourceContents("ProjectTemplate.xml");
				project.Create();
				if (project.Reference != null)
				{
					foreach (string reference in project.Reference.Split(','))
						project.AddProjectReference(solution.GetProject(reference));
				}

				project.OutputFolder = Path.GetFullPath(Path.Combine(OutputFolder, project.OutputFolder));

				IList helpers = new ArrayList();
				string helpersNamespace = "Helpers.";

				foreach (Source source in Sources.Values)
				{
					if (source.OutputFile.StartsWith(project.OutputFolder) && source.CodeFile)
					{
						project.AddFile(source.OutputFile.Replace(project.OutputFolder + Path.DirectorySeparatorChar, ""), true);

						UsageRemoverTransformer uur = new TypeMapper();
						uur.CodeBase = codeBase;
						uur.VisitCompilationUnit(source.CompilationUnit, null);

						foreach (string type in uur.UsedTypes)
						{
							if (type.StartsWith(helpersNamespace) && !helpers.Contains(type))
								helpers.Add(type);
						}
					}
				}

				foreach (string helperType in helpers)
				{
					string file = helperType.Substring(helpersNamespace.Length) + ".cs";
					string filePath = Path.Combine(HelperDirectory, file);
					string helperOutputFolder = Path.Combine(project.OutputFolder, options.GetKey("Projects", "IncludeHelpersInDirectory"));
					string outputFile = Path.Combine(helperOutputFolder, file);
					project.AddFile(outputFile.Replace(project.OutputFolder + Path.DirectorySeparatorChar, ""), true);
					CreateDirectoryForFile(outputFile);
					File.Copy(filePath, outputFile, true);
				}
				if (solution.Folder != Path.Combine(OutputFolder, project.OutputFolder))
					AddAssemblyReferences(project, Path.Combine("..", ReferenceFolder));
				else
					AddAssemblyReferences(project, ReferenceFolder);
				project.Save();
			}
			solution.Save();
		}

		private void AddAssemblyReferences(Project project, string referenceFolder)
		{
			if (Mode == "IKVM")
				project.AddAssemblyReference(referenceFolder + Path.DirectorySeparatorChar + "IKVM.GNU.Classpath.dll");

			IDictionary references = options.GetKeys("References");

			UsingVisitor uv = new UsingVisitor();
			foreach (Source source in Sources.Values)
			{
				if (source.CodeFile && source.OutputFile.StartsWith(project.OutputFolder))
					uv.VisitCompilationUnit(source.CompilationUnit, null);
			}
			IList added = new ArrayList();
			foreach (string key in uv.Usings.Keys)
			{
				foreach (DictionaryEntry entry in references)
				{
					if (key.StartsWith(entry.Key.ToString()))
					{
						string[] assemblies = entry.Value.ToString().Split(',');
						foreach (string assembly in assemblies)
						{
							if (added.Contains(assembly))
								continue;
							project.AddAssemblyReference(referenceFolder + Path.DirectorySeparatorChar + assembly + ".dll");
							added.Add(assembly);
						}
					}
				}
			}
		}

		private void LoadTranslateXml()
		{
			string manifestFile = Path.Combine(InputFolder, "translate.xml");
			if (!File.Exists(manifestFile))
				return;
			XmlDocument manifestDocument = new XmlDocument();
			manifestDocument.Load(manifestFile);

			Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
			string version = assemblyVersion.Major + "." + assemblyVersion.Minor;
			string expectedNamespace = "http://mayaf.org/janett/schema/" + version + "/translate.xsd";

			if (manifestDocument.DocumentElement.Attributes["xmlns"] != null)
			{
				string xmlNamespace = manifestDocument.DocumentElement.Attributes["xmlns"].Value;
				if (xmlNamespace != expectedNamespace)
					throw new ApplicationException(string.Format("Incompatible version of Manifest.xml. Xml namespace should be '{0}'", expectedNamespace));
			}
			else
			{
				XmlAttribute xmlns = manifestDocument.CreateAttribute("xmlns");
				xmlns.Value = expectedNamespace;
				manifestDocument.DocumentElement.Attributes.Prepend(xmlns);
			}

			XmlDocument newDocument = new XmlDocument();
			newDocument.LoadXml(manifestDocument.OuterXml);

			XmlNamespaceManager nsManager = new XmlNamespaceManager(newDocument.NameTable);
			nsManager.AddNamespace("ns", expectedNamespace);

			foreach (XmlNode node in FilterNodes(newDocument.DocumentElement.ChildNodes))
			{
				Deserialize(node, this);
			}
		}

		private void Translate()
		{
			codeBase = new CodeBase(GetLanguage());
			codeBase.Types.LibrariesFolder = Libraries;

			codeBase.Mappings = new Mappings(Mappings);

			parentVisitor = new ParentVisitor();

			typesVisitor = new TypesVisitor();
			typesVisitor.CodeBase = codeBase;

			LoadFiles();

			codeBase.Types.Visitors.Add(parentVisitor);

			BeforeParse();
			progress.SetCount("Parsing", sourceFileCount);
			ParseAndPreVisit();

			BeforeTransformation();
			IDictionary visitors = GetVisitors(typeof(AbstractAstVisitor));
			visitors.Add(typeof(InheritorsVisitor).FullName, typeof(InheritorsVisitor));
			IDictionary transformers = GetVisitors(typeof(AbstractAstTransformer));
			progress.SetCount("Transformation", (visitors.Count + transformers.Count + 1) * sourceFileCount);
			CallVisitors(visitors, "Transformation");
			CallVisitors(transformers, "Transformation");
			CallVisitor(typeof(ReferenceTransformer), "Transformation");

			progress.SetCount("Mapping", 2 * sourceFileCount);
			CallVisitor(typeof(MemberMapper), "Mapping");
			CallVisitor(typeof(TypeMapper), "Mapping");

			BeforeRefactoring();

			int count = 4;
			if (Mode == "IKVM")
				count++;
			if (Namespace != null)
				count++;
			progress.SetCount("Refactoring", sourceFileCount * count);

			Refactor();
			OptimizeUsings();

			GenerateCode();

			SaveFiles();
		}

		protected virtual void BeforeTransformation()
		{
		}

		protected virtual void BeforeRefactoring()
		{
		}

		private void SaveCurrentStatus(Exception ex)
		{
			using (StreamWriter writer = new StreamWriter(diagnosticsFile))
			{
				writer.WriteLine(Diagnostics.GetString());
				if (ex != null)
					writer.WriteLine(ex.ToString());
			}
		}

		public override void Terminate()
		{
			SaveCurrentStatus(null);
		}

		public override void Exception(Exception ex)
		{
			SaveCurrentStatus(ex);
		}

		public virtual SupportedLanguage GetLanguage()
		{
			return SupportedLanguage.CSharp;
		}

		public virtual string ConvertFile(string fileContents)
		{
			return fileContents;
		}

		private void LoadFiles()
		{
			int filesCount = 0;
			foreach (Project project in Projects)
			{
				AddDefaultExcludes(project.FileSet);
				if (project.FileSet.Includes.Count == 0)
					project.FileSet.Includes.Add("**");
				project.FileSet.Load();
				filesCount += project.FileSet.Files.Count;
			}

			progress.SetCount("Loading", filesCount);
			string sourceExt = GetSourceExtension();
			foreach (Project project in Projects)
			{
				foreach (string file in project.FileSet.Files)
				{
					progress.Increment("Loading");
					string extension = Path.GetExtension(file);
					Source source;
					if (extension == sourceExt)
					{
						source = new Source(new FileInfo(file), FileSystemUtil.ReadFile(file));
						source.CodeFile = true;
						sourceFileCount++;
					}
					else
						source = new Source(new FileInfo(file));

					string inputFolder = Path.GetFullPath(InputFolder + Path.DirectorySeparatorChar + project.Folder + Path.DirectorySeparatorChar);
					string outputFile = source.File.Replace(inputFolder, "");
					if (project.OutputFolder == null)
						project.OutputFolder = project.Name;
					string outputFolder = Path.GetFullPath(Path.Combine(OutputFolder, project.OutputFolder));
					if (Package != null)
						outputFile = outputFile.Replace(Package.Replace('.', Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, "");
					outputFile = Path.Combine(outputFolder, outputFile);
					outputFile = outputFile.Replace(".java", ".cs");
					source.OutputFile = outputFile;

					Sources.Add(file, source);
				}
			}
			if (sourceFileCount == 0)
				throw new ApplicationException("There is no source file there");
		}

		private string GetSourceExtension()
		{
			SupportedLanguage language = GetLanguage();
			if (language == SupportedLanguage.Java)
				return ".java";
			else
				return ".cs";
		}

		private void AddDefaultExcludes(FileSet fs)
		{
			KeyValuesDictionary fileOptions = options.GetKeys("Files");
			foreach (string defaultExclude in fileOptions.GetValues("DefaultExcludes"))
			{
				string pattern = @"**" + Path.DirectorySeparatorChar + defaultExclude;
				if (defaultExclude.IndexOf("*") != -1)
					AddExclude(fs, pattern);
				else
					AddExclude(fs, pattern + Path.DirectorySeparatorChar + "**");
			}
			if (fs.Excludes.Count == 0)
				fs.Excludes.Add(@"**\.svn\**");
		}

		private void AddExclude(FileSet fs, string pattern)
		{
			if (!fs.Excludes.Contains(pattern))
				fs.Excludes.Add(pattern);
		}

		private void ParseAndPreVisit()
		{
			foreach (Source entry in Sources.Values)
			{
				if (!entry.CodeFile)
					continue;
				progress.Increment("Parsing");

				string convetedCode = ConvertFile(entry.Code);
				StringReader reader = new StringReader(convetedCode);

				IParser parser = ParserFactory.CreateParser(GetLanguage(), reader);
				parser.ParseMethodBodies = true;
				parser.Parse();
				CompilationUnit compilationUnit = parser.CompilationUnit;
				compilationUnit.Parent = new TypeReference(entry.File);

				parentVisitor.VisitCompilationUnit(compilationUnit, null);
				typesVisitor.VisitCompilationUnit(compilationUnit, null);
				if (MethodExcludes.Contains(entry.File))
				{
					string methods = MethodExcludes[entry.File].ToString();
					MethodExcludeTransformer met = new MethodExcludeTransformer();
					foreach (string method in methods.Split(','))
						met.Methods.Add(method);
					met.VisitCompilationUnit(compilationUnit, null);
				}
				entry.CompilationUnit = compilationUnit;
				entry.Parser = parser;
			}
		}

		private void CallVisitors(IDictionary transformers, string step)
		{
			foreach (Type type in transformers.Values)
			{
				CallVisitor(type, step);
			}
		}

		private IDictionary GetVisitors(Type baseType)
		{
			IDictionary visitors = new SortedList();
			foreach (Type type in Discovery.GetClassInheritedFrom(baseType))
			{
				if (!type.IsAbstract && type.Name != "SemanticVisitor" && type.Assembly != Assembly.GetExecutingAssembly())
				{
					string transoferMode = GetMode(type);
					if ((transoferMode == null || transoferMode == Mode) && GetAttribute(type, typeof(ExplicitAttribute)) == null)
					{
						visitors.Add(type.FullName, type);
					}
				}
			}
			return visitors;
		}

		protected string GetMode(Type type)
		{
			string mode = null;
			ModeAttribute attr = GetAttribute(type, typeof(ModeAttribute)) as ModeAttribute;
			if (attr != null)
				mode = attr.Name;
			return mode;
		}

		private System.Attribute GetAttribute(Type type, Type attributeType)
		{
			object[] attrs = type.GetCustomAttributes(attributeType, true);
			if (attrs.Length > 0)
				return (System.Attribute) attrs[0];
			else
				return null;
		}

		protected void CallVisitor(Type type, string step)
		{
			IAstVisitor visitor = (IAstVisitor) Activator.CreateInstance(type);
			CallVisitor(visitor, step);
		}

		private void CallVisitor(IAstVisitor visitor, string step)
		{
			if (visitor is Transformer)
			{
				Transformer transformer = (Transformer) visitor;
				transformer.CodeBase = codeBase;
				transformer.Mode = Mode;
			}

			foreach (Source entry in Sources.Values)
			{
				if (!entry.CodeFile)
					continue;
				if (step != null)
					progress.Increment(step);
				parentVisitor.VisitCompilationUnit(entry.CompilationUnit, null);
				visitor.VisitCompilationUnit(entry.CompilationUnit, null);
			}
		}

		private void OptimizeUsings()
		{
			if (Mode == GetMode(typeof(ShortenReferencesTransformer)))
				CallVisitor(typeof(ShortenReferencesTransformer), "Refactoring");
		}

		private void Refactor()
		{
			codeBase.References.Clear();
			CallVisitor(typeof(AccessorRefactoring), "Refactoring");
			CallVisitor(typeof(ReferenceTransformer), "Refactoring");
			CallVisitor(typeof(RenameMethodInvocationRefactoring), "Refactoring");
			CallVisitor(typeof(RenameMethodDeclarationRefactoring), "Refactoring");

			if (Package != null && Namespace != null)
			{
				RenameNamespaceRefactoring rns = new RenameNamespaceRefactoring();
				rns.From = Package;
				rns.To = Namespace;
				CallVisitor(rns, "Refactoring");
			}
		}

		private void GenerateCode()
		{
			foreach (Source entry in Sources.Values)
			{
				if (entry.CodeFile)
				{
					CSharpOutputVisitor csharpOutputVisitor = new CSharpOutputVisitor();
					SpecialNodesInserter.Install(entry.Parser.Lexer.SpecialTracker.RetrieveSpecials(), csharpOutputVisitor);
					csharpOutputVisitor.VisitCompilationUnit(entry.CompilationUnit, null);

					entry.Code = csharpOutputVisitor.Text;
				}
			}
		}

		private void SaveFiles()
		{
			progress.SetCount("Saving", Sources.Count);
			foreach (Source entry in Sources.Values)
			{
				progress.Increment("Saving");
				CreateDirectoryForFile(entry.OutputFile);
				if (entry.CodeFile)
					FileSystemUtil.WriteFile(entry.OutputFile, entry.Code);
				else
					File.Copy(entry.File, entry.OutputFile, true);
			}
		}

		private string CallProcess(string executable, string parameters)
		{
			if (DiagnosticsMode)
				FileSystemUtil.WriteFile(Path.Combine(ExecutableDirectory, Path.ChangeExtension(executable, "bat")), executable + " " + parameters);
			ProcessStartInfo psi = new ProcessStartInfo(executable, parameters);
			psi.UseShellExecute = false;
			psi.RedirectStandardOutput = true;
			Process p = Process.Start(psi);
			p.WaitForExit(5000);
			return p.StandardOutput.ReadToEnd().ToString();
		}

		private void Deserialize(XmlNode node, object obj)
		{
			Type type = obj.GetType();
			if (node.Name == "Files")
			{
				Project project;
				if (Projects.Count > 0)
					project = (Project) Projects[0];
				else
				{
					project = GetDefaultProject();
					Projects.Add(project);
				}
				DeserializeFiles(node, project);
			}
			else if (node.Name == "Projects")
			{
				foreach (XmlNode childNode in FilterNodes(node.ChildNodes))
				{
					Project p = new Project();
					p.FileSet = new FileSet(Path.Combine(InputFolder, childNode.Attributes["Folder"].Value));
					foreach (XmlAttribute attribute in childNode.Attributes)
						Deserialize(attribute, p);
					DeserializeFiles(childNode, p);
					Projects.Add(p);
				}
			}
			else
			{
				FieldInfo field = type.GetField(node.Name);
				object fieldValue = field.GetValue(obj);
				if (fieldValue == null || node.Name == "Solution")
					field.SetValue(obj, node.InnerText);
			}
		}

		private Project GetDefaultProject()
		{
			Project project;
			project = new Project();
			project.Name = Solution;
			project.Folder = ".";
			project.OutputFolder = ".";
			project.FileSet = new FileSet(InputFolder);
			return project;
		}

		private void DeserializeFiles(XmlNode node, Project p)
		{
			foreach (XmlNode subNode in FilterNodes(node.ChildNodes))
			{
				string path = subNode.Attributes["Path"].Value;
				if (subNode.Name == "Include")
					p.FileSet.Includes.Add(path);
				else
				{
					int index = path.IndexOf(":");
					if (index != -1)
					{
						string folder = Path.Combine(InputFolder, p.Folder);
						string fileName = path.Substring(0, index);
						string methods = path.Substring(index + 1);
						MethodExcludes.Add(Path.Combine(folder, fileName), methods);
					}
					else
						p.FileSet.Excludes.Add(path);
				}
			}
		}

		private IEnumerable FilterNodes(XmlNodeList nodes)
		{
			IList result = new ArrayList();
			foreach (XmlNode node in nodes)
			{
				if (node is XmlComment)
					continue;
				if (node.Attributes["Mode"] == null || node.Attributes["Mode"].Value == Mode)
					result.Add(node);
			}
			return result;
		}

		private void CopyDirectory(string sourcePath, string destinationPath)
		{
			FileSet fs = new FileSet(sourcePath);
			fs.Includes.Add(@"**\*");
			AddDefaultExcludes(fs);
			fs.Load();

			if (Directory.Exists(destinationPath))
				Directory.Delete(destinationPath, true);

			foreach (string directory in fs.Directories)
			{
				string destDirectory = destinationPath + directory.Replace(sourcePath, "");
				Directory.CreateDirectory(destDirectory);
			}
			foreach (string file in fs.Files)
			{
				string desFile = destinationPath + file.Replace(sourcePath, "");
				File.Copy(file, desFile, true);
			}
		}

		private void CreateDirectoryForFile(string convertedFileName)
		{
			string directoryName = Path.GetDirectoryName(convertedFileName);
			if (!Directory.Exists(directoryName))
				Directory.CreateDirectory(directoryName);
		}

		private string GetResourceContents(string resourceName)
		{
			Stream resource = Discovery.GetResource(resourceName);
			using (StreamReader reader = new StreamReader(resource))
			{
				return reader.ReadToEnd();
			}
		}
	}
}