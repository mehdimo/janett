namespace Janett.Commons
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;

	public class CommandLineParser
	{
		private Type argumentsType;
		private string[] arguments;

		public CommandLineParser(Type type, string[] args)
		{
			argumentsType = type;
			this.arguments = args;
		}

		public string Usage
		{
			get
			{
				string usage = string.Format("Usage: {0}", Path.GetFileNameWithoutExtension(Assembly.GetCallingAssembly().CodeBase));
				string description = "";

				FieldInfo[] fields = argumentsType.GetFields();
				foreach (FieldInfo field in fields)
				{
					object[] attributes = field.GetCustomAttributes(typeof(CommandLineAttribute), true);
					if (attributes.Length > 0)
					{
						object attrObject = attributes[0];
						CommandLineAttribute attr = (CommandLineAttribute) attrObject;
						usage += GetUsage(attr);
						description += GetDescription(attr);
					}
				}
				return usage + "\r\n" + description + "\r\n";
			}
		}

		private string GetDescription(CommandLineAttribute attr)
		{
			string description = "\r\n";
			if (attr is CommandLineValueAttribute)
			{
				CommandLineValueAttribute cattr = (CommandLineValueAttribute) attr;
				if (cattr.Place != -1)
				{
					description = "\r\n<" + attr.Name + ">";
					return description.PadRight(30) + attr.Description;
				}
			}
			description += "/" + attr.Name;
			if (attr is CommandLineValueAttribute)
				description += " <value>";
			description = description.PadRight(30);
			description += attr.Description;
			return description;
		}

		private string GetUsage(CommandLineAttribute attr)
		{
			string usage = " ";
			if (attr is CommandLineValueAttribute)
			{
				CommandLineValueAttribute cattr = (CommandLineValueAttribute) attr;
				if (cattr.Place != -1)
					return " <" + attr.Name + ">";
			}
			if (attr.Optional)
				usage += "[";
			usage += "/" + attr.Name;
			if (attr.AlternateName != null)
				usage += "|/" + attr.AlternateName;
			if (attr is CommandLineValueAttribute)
				usage += " <value>";
			if (attr.Optional)
				usage += "]";
			return usage;
		}

		public object Parse()
		{
			FieldInfo[] fields = argumentsType.GetFields();
			object argumentObject = Activator.CreateInstance(argumentsType);
			foreach (FieldInfo field in fields)
			{
				object[] attributes = field.GetCustomAttributes(typeof(CommandLineAttribute), true);
				if (attributes.Length > 0)
				{
					object attrObject = attributes[0];
					CommandLineAttribute attr = (CommandLineAttribute) attrObject;
					object argumentValue = GetArgument(attrObject, attr.Name, true);
					if (argumentValue != null)
						SetValue(field, argumentObject, argumentValue);
					else if (!attr.Optional)
						throw new CommandLineArgumentException(string.Format("Expected argument '{0}'", attr.Name));
				}
			}
			return argumentObject;
		}

		private void SetValue(FieldInfo field, object argumentObject, object argumentValue)
		{
			if (field.FieldType == typeof(IList) || field.FieldType.GetInterface("IList", true) != null)
			{
				string[] argumentValues = argumentValue.ToString().Split(',');
				field.SetValue(argumentObject, new ArrayList(argumentValues));
			}
			else
				field.SetValue(argumentObject, argumentValue);
		}

		private object GetArgument(object attrObject, string name, bool alternate)
		{
			object argumentValue = null;
			CommandLineAttribute attr = (CommandLineAttribute) attrObject;
			if (attrObject is CommandLineValueAttribute)
			{
				CommandLineValueAttribute cattr = (CommandLineValueAttribute) attr;
				if (cattr.Place != -1)
				{
					if (cattr.Place <= arguments.Length && !arguments[cattr.Place - 1].StartsWith("/") && !arguments[cattr.Place - 1].StartsWith("-"))
						return arguments[cattr.Place - 1];
					else
					{
						if (!cattr.Optional)
							throw new CommandLineArgumentException("Missing parameters");
					}
				}
				else
					argumentValue = GetArgumentValue(name);
			}
			else
			{
				int index = GetArgumentIndex(name);
				if (attrObject is CommandLineFlagAttribute && index != -1)
					argumentValue = !arguments[index].EndsWith("-");
			}
			if (argumentValue == null && alternate && attr.AlternateName != null)
				return GetArgument(attrObject, attr.AlternateName, false);
			else
				return argumentValue;
		}

		public string GetArgumentValue(string argumentName)
		{
			for (int i = 0; i < arguments.Length; i++)
			{
				if (arguments[i].StartsWith("-") || arguments[i].StartsWith("/"))
				{
					string argName = arguments[i].TrimStart('-', '/');
					if (argName.ToLower() == argumentName.ToLower())
						if (i + 1 == arguments.Length)
							throw new CommandLineArgumentException(string.Format("Expected value for argument '{0}'", argumentName));
						else if (arguments[i + 1].StartsWith("/") || arguments[i + 1].StartsWith("-"))
							throw new CommandLineArgumentException(string.Format("Expected value for argument '{0}'", argumentName));
						else
							return arguments[i + 1];
					else if (argName.ToLower().StartsWith(argumentName.ToLower() + ":"))
						return argName.Substring(argName.IndexOf(':') + 1);
				}
			}
			return null;
		}

		private int GetArgumentIndex(string argumentName)
		{
			for (int i = 0; i < arguments.Length; i++)
			{
				if (arguments[i].StartsWith("/" + argumentName) || arguments[i].StartsWith("-" + argumentName))
					return i;
			}
			return -1;
		}

		public bool ArgumentExists(string argumentName)
		{
			return (GetArgumentIndex(argumentName) != -1);
		}
	}
}