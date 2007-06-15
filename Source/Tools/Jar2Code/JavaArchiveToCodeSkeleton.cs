namespace Janett.Tools
{
	using System;
	using System.Collections;
	using System.IO;

	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.NRefactory.PrettyPrinter;
	using ICSharpCode.SharpZipLib.Zip;

	using Janett.Commons;

	[Named("jar2code")]
	public class JavaArchiveToCodeSkeleton : Commandlet
	{
		private IDictionary classes = new SortedList();
		private SignatureParser sigParser = new SignatureParser();
		private IList compilationUnits = new ArrayList();

		[CommandLineValue("JarFile", 1)]
		public string JarFile;

		[CommandLineValue("out", 2)]
		public string CodeFolder;

		[CommandLineValue("inc", 3, Optional = true)]
		public IList Includes = new ArrayList();

		[CommandLineValue("exc", 4, Optional = true)]
		public IList Excludes = new ArrayList();

		private bool IncludeParamters = true;

		public override void Execute()
		{
			CreateTypeDeclarations(JarFile);
			SaveCode(CodeFolder);
		}

		private void CreateTypeDeclarations(string jarFile)
		{
			LoadClasses(Path.GetFullPath(jarFile));

			foreach (string package in classes.Keys)
			{
				if (!IsIncluded(package))
					continue;
				Console.WriteLine(package);
				IDictionary types = new Hashtable();
				IList packageClasses = (IList) classes[package];
				foreach (ClassFile cf in packageClasses)
				{
					if (cf.IsPublic)
					{
						TypeDeclaration type = GetTypeDeclaration(cf);
						types.Add(type.Name, type);
					}
				}

				foreach (TypeDeclaration type in types.Values)
				{
					if (type.Name.IndexOf("$") == -1)
					{
						NamespaceDeclaration nameSpace = new NamespaceDeclaration(package);
						type.Parent = nameSpace;
						nameSpace.AddChild(type);

						CompilationUnit cu = new CompilationUnit();
						cu.Children.Add(nameSpace);
						compilationUnits.Add(cu);
					}
					else
					{
						string parentName = type.Name.Substring(0, type.Name.IndexOf("$"));
						TypeDeclaration parent = (TypeDeclaration) types[GetTypeName(parentName)];
						if (parent != null)
						{
							type.Name = GetTypeName(type.Name);
							parent.AddChild(type);
						}
					}
				}
			}
		}

		public void SaveCode(string folder)
		{
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);
			foreach (CompilationUnit cu in compilationUnits)
			{
				NamespaceDeclaration ns = (NamespaceDeclaration) cu.Children[0];
				TypeDeclaration type = (TypeDeclaration) ns.Children[0];
				IOutputAstVisitor vis = new JavaOutputVisitor();
				vis.VisitCompilationUnit(cu, null);
				string packageFolder = Path.Combine(folder, ns.Name);
				packageFolder = packageFolder.Replace('.', '\\');
				if (!Directory.Exists(packageFolder))
					Directory.CreateDirectory(packageFolder);
				FileSystemUtil.WriteFile(Path.Combine(packageFolder, type.Name + ".java"), vis.Text);
			}
		}

		private TypeDeclaration GetTypeDeclaration(ClassFile clazz)
		{
			TypeDeclaration typeDeclaration = new TypeDeclaration(Modifiers.None, null);
			if (clazz.IsInterface)
				typeDeclaration.Type = ClassType.Interface;
			else
				typeDeclaration.Modifier = Modifiers.Abstract;

			typeDeclaration.Name = clazz.Name;
			if (clazz.Name.IndexOf("$") == -1)
				typeDeclaration.Name = GetTypeName(clazz.Name);
			foreach (ClassFile.Method method in clazz.Methods)
			{
				if (!(method.IsPublic || method.IsProtected))
					continue;
				ParametrizedNode methodDeclaration;
				if (method.Name == "<init>")
					methodDeclaration = new ConstructorDeclaration(typeDeclaration.Name, Modifiers.Public, null, null);
				else
				{
					TypeReference returnType = sigParser.GetReturnType(method.Signature);
					methodDeclaration = new MethodDeclaration(method.Name, Modifiers.None, returnType, null, null);
					if (method.IsAbstract)
						methodDeclaration.Modifier |= Modifiers.Abstract;
					methodDeclaration.Modifier |= Modifiers.Public;
				}
				methodDeclaration.Parent = typeDeclaration;

				if (IncludeParamters)
				{
					TypeReference[] arguments = sigParser.GetArgumentTypes(method.Signature);
					int i = 1;
					foreach (TypeReference typeRef in arguments)
					{
						ParameterDeclarationExpression param = new ParameterDeclarationExpression(typeRef, "parameter" + i++.ToString());
						methodDeclaration.Parameters.Add(param);
					}
				}
				typeDeclaration.Children.Add(methodDeclaration);
			}
			foreach (ClassFile.Field field in clazz.Fields)
			{
				if (!field.IsPublic)
					continue;
				TypeReference type = sigParser.GetFieldType(field.Signature);
				FieldDeclaration fieldDeclaration = new FieldDeclaration(null, type, Modifiers.None);
				fieldDeclaration.Fields.Add(new VariableDeclaration(field.Name));
				fieldDeclaration.Parent = typeDeclaration;
				typeDeclaration.Children.Add(fieldDeclaration);
			}
			if (clazz.Name != "java.lang.Object" && clazz.SuperClass != null && clazz.SuperClass != "java.lang.Object")
			{
				TypeReference baseType = new TypeReference(clazz.SuperClass.Replace("$", "."));
				baseType.Kind = TypeReferenceKind.Extends;
				typeDeclaration.BaseTypes.Add(baseType);
			}
			foreach (ClassFile.ConstantPoolItemClass interfaceType in clazz.Interfaces)
			{
				TypeReference interf = new TypeReference(interfaceType.Name.Replace("$", "."));
				interf.Kind = TypeReferenceKind.Implements;
				typeDeclaration.BaseTypes.Add(interf);
			}
			return typeDeclaration;
		}

		private string GetTypeName(string fullName)
		{
			if (fullName.IndexOf("$") != -1)
				return fullName.Substring(fullName.IndexOf('$') + 1);
			else
				return fullName.Substring(fullName.LastIndexOf('.') + 1);
		}

		private string GetNamespace(string fullName)
		{
			int lastDotIndex = fullName.LastIndexOf('.');
			if (lastDotIndex != -1)
				return fullName.Substring(0, lastDotIndex);
			return null;
		}

		private void LoadClasses(string file)
		{
			ZipFile zf = new ZipFile(file);
			try
			{
				foreach (ZipEntry ze in zf)
				{
					if (!ze.Name.ToLower().EndsWith(".class"))
						continue;
					byte[] buffer = ReadZipEntry(zf, ze);
					ClassFile classFile = new ClassFile(buffer, 0, buffer.Length, "Test", true);
					string package = GetNamespace(classFile.Name);
					if (!classes.Contains(package))
						classes.Add(package, new ArrayList());
					IList packageClasses = (IList) classes[package];
					packageClasses.Add(classFile);
				}
			}
			finally
			{
				zf.Close();
			}
		}

		private byte[] ReadZipEntry(ZipFile zf, ZipEntry ze)
		{
			byte[] buf = new byte[ze.Size];
			int pos = 0;
			Stream s = zf.GetInputStream(ze);
			while (pos < buf.Length)
				pos += s.Read(buf, pos, buf.Length - pos);
			return buf;
		}

		private bool IsIncluded(string package)
		{
			bool included = false;
			foreach (string include in Includes)
			{
				if (package == include || package.StartsWith(include + "."))
					included = true;
			}
			if (included)
			{
				foreach (string exclude in Excludes)
					if (package == exclude || package.StartsWith(exclude + "."))
						included = false;
			}
			return included;
		}
	}
}