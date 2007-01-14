namespace Janett.Framework
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text.RegularExpressions;

	using Janett.Commons;

	public class Mappings : Hashtable
	{
		private Regex regex = new Regex(@"(?<name>\w+)\((?<args>\w(,\w)*)?\)");

		public Mappings()
		{
		}

		public Mappings(string folder)
		{
			string baseFolder = folder;
			ReadMappings(folder, baseFolder);
		}

		private void ReadMappings(string folder, string baseFolder)
		{
			ReadMapFiles(folder, baseFolder);

			foreach (string directory in Directory.GetDirectories(folder))
			{
				ReadMappings(directory, baseFolder);
			}
		}

		private void ReadMapFiles(string directory, string baseFolder)
		{
			foreach (string mappingFile in Directory.GetFiles(directory, "*.map"))
			{
				string fileName = Path.GetFileName(mappingFile);

				if (fileName == "Class.map")
				{
					KeyValuePairReader classMapReader = new KeyValuePairReader(mappingFile);

					string package = mappingFile.Substring(baseFolder.Length + 1);
					package = Path.GetDirectoryName(package);
					package = package.Replace('\\', '.');

					foreach (DictionaryEntry entry in classMapReader.GetKeys())
					{
						string key = (string) entry.Key;
						if (!key.StartsWith(package + "."))
							key = package + "." + key;
						TypeMapping typeMapping = new TypeMapping();
						typeMapping.Target = entry.Value.ToString();
						Add(key, typeMapping);
					}
				}
				else
				{
					string mappingFileName = GetMappedType(mappingFile, baseFolder);
					TypeMapping typeMapping = AddClassMapping(mappingFileName);
					KeyValuePairReader mappingReader = new KeyValuePairReader(mappingFile);
					if (typeMapping.Members == null)
						typeMapping.Members = new MembersMapping();
					foreach (DictionaryEntry entry in mappingReader.GetKeys())
					{
						string key = (string) entry.Key;
						string value = (string) entry.Value;

						if (key == "PascalStyle")
							AddPascalStyleMembers(typeMapping, value);
						else
							typeMapping.Members.Add(key, value);
					}
				}
			}
		}

		private void AddPascalStyleMembers(TypeMapping typeMapping, string value)
		{
			foreach (Match member in regex.Matches(value))
			{
				string source = member.Value;
				string target = ToPascalStyle(member.Groups["name"].Value) + "(";
				string args = member.Groups["args"].Value;
				if (args != "")
				{
					string[] argArr = args.Split(',');
					foreach (string argChr in argArr)
					{
						target += "#" + argChr + ",";
					}
					if (argArr.Length > 0)
						target = target.Substring(0, target.Length - 1);
				}
				target += ")";

				typeMapping.Members.Add(source, target);
			}
		}

		private string ToPascalStyle(string input)
		{
			return Char.ToUpper(input[0]) + input.Substring(1);
		}

		public new TypeMapping this[object key]
		{
			get { return base[key] as TypeMapping; }
		}

		public override bool Contains(object key)
		{
			return (this[key] != null);
		}

		public TypeMapping GetCounterpart(string type)
		{
			TypeMapping mapping = this[type];
			if (mapping != null)
				return mapping;
			else
			{
				foreach (TypeMapping classMapping in Values)
				{
					if (classMapping.Target == type)
						return classMapping;
				}
				return null;
			}
		}

		private TypeMapping AddClassMapping(string mappedClasses)
		{
			string javaType = mappedClasses.Substring(0, mappedClasses.IndexOf('-'));
			string csType = mappedClasses.Substring(mappedClasses.IndexOf('-') + 1);

			TypeMapping typeMapping = new TypeMapping();
			typeMapping.Target = csType;
			Add(javaType, typeMapping);
			return typeMapping;
		}

		private string GetMappedType(string mapFile, string baseFolder)
		{
			string mapped = Path.GetFileNameWithoutExtension(mapFile);
			string from = mapped.Substring(0, mapped.IndexOf('-'));
			if (from.IndexOf('.') != -1)
				return mapped;
			else
			{
				mapped = mapFile.Substring(baseFolder.Length + 1);
				mapped = mapped.Replace(".map", "");
				mapped = mapped.Replace('\\', '.');
				return mapped;
			}
		}
	}
}