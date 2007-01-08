namespace Janett.Commons
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;

	public class Discovery
	{
		public ArrayList Assemblies;

		private Hashtable cache;

		public Discovery()
		{
			Assemblies = new ArrayList();
			cache = new Hashtable();
		}

		public MemberInfo[] GetMembers(Type type)
		{
			Stack stack = new Stack();
			while (type != null)
			{
				stack.Push(type);
				type = type.BaseType;
			}
			IDictionary members = new ListDictionary();
			foreach (Type htype in stack)
			{
				foreach (MemberInfo member in htype.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
				{
					if (members.Contains(member.Name))
						members.Remove(member.Name);
					members.Add(member.Name, member);
				}
			}
			return (MemberInfo[]) new ArrayList(members.Values).ToArray(typeof(MemberInfo));
		}

		public object GetAttribute(MemberInfo member, Type attributeType)
		{
			object[] attributes = member.GetCustomAttributes(attributeType, true);
			return GetAttributeOfType(attributes, attributeType);
		}

		public object GetAttribute(Type type, Type attributeType)
		{
			object[] attributes = type.GetCustomAttributes(attributeType, true);
			return GetAttributeOfType(attributes, attributeType);
		}

		private object GetAttributeOfType(object[] attributes, Type attributeType)
		{
			foreach (object attribute in attributes)
			{
				if (attributeType.IsAssignableFrom(attribute.GetType()))
					return attribute;
			}
			return null;
		}

		public void AddAssembly(string address)
		{
			Assemblies.Add(Assembly.LoadFrom(address));
		}

		public void AddAssembly(Assembly assembly)
		{
			Assemblies.Add(assembly);
		}

		public void RemoveAssembly(string address)
		{
			Assembly assemblyToRemove = null;
			foreach (Assembly assembly in Assemblies)
			{
				if (assembly.CodeBase.ToLower().EndsWith(address.ToLower()))
					assemblyToRemove = assembly;
			}
			if (assemblyToRemove != null)
				Assemblies.Remove(assemblyToRemove);
		}

		public Type GetClass(string className)
		{
			foreach (Assembly a in Assemblies)
			{
				Type type = a.GetType(className);
				if (type != null)
					return type;
				else
				{
					foreach (Type t in a.GetTypes())
					{
						if (t.Name == className)
							return t;
					}
				}
			}
			return null;
		}

		public Type[] GetClasses()
		{
			ArrayList output = new ArrayList();
			foreach (Assembly asm in Assemblies)
				foreach (Type type in asm.GetTypes())
					output.Add(type);
			return (Type[]) output.ToArray(typeof(Type));
		}

		public Type[] GetClasses(Type attributeType)
		{
			ArrayList result = new ArrayList();
			foreach (Assembly a in Assemblies)
			{
				foreach (Type type in a.GetTypes())
				{
					if (type.GetCustomAttributes(attributeType, true).Length != 0)
					{
						result.Add(type);
					}
				}
			}
			return (Type[]) result.ToArray(typeof(Type));
		}

		public Type GetClass(object attributeValue)
		{
			return GetClass(attributeValue.GetType(), attributeValue);
		}

		public Type GetClass(Type attributeType, object attributeValue)
		{
			string attributeKey = attributeType.ToString() + "-" + attributeValue.GetHashCode().ToString();
			if (cache.Contains(attributeKey))
				return cache[attributeKey] as Type;
			foreach (Assembly a in Assemblies)
			{
				foreach (Type t in a.GetTypes())
				{
					object[] attrs = t.GetCustomAttributes(attributeType, true);
					foreach (object attr in attrs)
					{
						if (attributeValue.Equals(attr))
						{
							cache.Add(attributeKey, t);
							return t;
						}
					}
				}
			}
			cache.Add(attributeKey, null);
			return null;
		}

		public MemberInfo[] GetMembers(object classAttributeValue, Type methodAttributeType)
		{
			ArrayList methods = new ArrayList();
			Type type = GetClass(classAttributeValue);
			foreach (MemberInfo mi in type.GetMembers())
			{
				object[] attrs = mi.GetCustomAttributes(methodAttributeType, true);
				if (attrs.Length > 0)
					methods.Add(mi);
			}
			return (MemberInfo[]) methods.ToArray(typeof(MethodInfo));
		}

		public MemberInfo[] GetMembers(Type classAttributeType, Type memberAttributeType)
		{
			ArrayList members = new ArrayList();
			Type[] types = GetClasses(classAttributeType);
			foreach (Type type in types)
				members.AddRange(GetMembersInType(type, memberAttributeType));
			return (MemberInfo[]) members.ToArray(typeof(MemberInfo));
		}

		public MemberInfo GetMember(object classAttributeValue, object memberAttributeValue)
		{
			string attributeKey = classAttributeValue.GetType().ToString() + "-" +
			                      classAttributeValue.GetHashCode().ToString() + "-" +
			                      memberAttributeValue.GetType().ToString() + "-" +
			                      memberAttributeValue.GetHashCode().ToString();
			if (cache.Contains(attributeKey))
				return cache[attributeKey] as MemberInfo;
			Type type = GetClass(classAttributeValue);
			foreach (MemberInfo mi in type.GetMembers())
			{
				object[] attrs = mi.GetCustomAttributes(memberAttributeValue.GetType(), true);
				foreach (object attr in attrs)
				{
					if (attr.Equals(memberAttributeValue))
					{
						cache.Add(attributeKey, mi);
						return mi;
					}
				}
			}
			cache.Add(attributeKey, null);
			return null;
		}

		public MemberInfo GetMemberInType(Type targetType, object memberAttributeValue)
		{
			foreach (MemberInfo mi in targetType.GetMembers())
			{
				object[] attrs = mi.GetCustomAttributes(memberAttributeValue.GetType(), true);
				foreach (object attr in attrs)
				{
					if (attr.Equals(memberAttributeValue))
						return mi;
				}
			}
			return null;
		}

		public MemberInfo[] GetMembersInType(Type targetType, Type attributeType)
		{
			ArrayList members = new ArrayList();
			while (targetType != typeof(Object))
			{
				foreach (MemberInfo mi in targetType.GetMembers())
				{
					object[] attrs = mi.GetCustomAttributes(attributeType, true);
					if (GetAttributeOfType(attrs, attributeType) != null)
						members.Add(mi);
				}
				targetType = targetType.BaseType;
			}
			return (MemberInfo[]) (members.ToArray(typeof(MemberInfo)));
		}

		public Stream GetResource(string name)
		{
			foreach (Assembly asm in Assemblies)
			{
				foreach (string resourceName in asm.GetManifestResourceNames())
				{
					if (resourceName.ToUpper().EndsWith("." + name.ToUpper()))
						return asm.GetManifestResourceStream(resourceName);
				}
			}
			return null;
		}

		public ArrayList GetResources(string name)
		{
			ArrayList resources = new ArrayList();
			foreach (Assembly asm in Assemblies)
			{
				foreach (string resourceName in asm.GetManifestResourceNames())
				{
					if (resourceName.ToUpper().EndsWith(name.ToUpper()))
						resources.Add(asm.GetManifestResourceStream(resourceName));
				}
			}
			return resources;
		}

		public Type[] GetClassInheritedFrom(Type baseType)
		{
			ArrayList classes = new ArrayList();
			foreach (Assembly asm in Assemblies)
			{
				foreach (Type type in asm.GetTypes())
				{
					Type parentType = type;
					while (parentType.BaseType != null && parentType.BaseType != typeof(object))
					{
						parentType = parentType.BaseType;
						if (parentType == baseType)
						{
							classes.Add(type);
							break;
						}
					}
				}
			}
			return (Type[]) classes.ToArray(typeof(Type));
		}

		public Type[] GetClassWithInterface(Type interfaceType)
		{
			ArrayList classes = new ArrayList();
			foreach (Assembly asm in Assemblies)
				foreach (Type type in asm.GetTypes())
				{
					TypeFilter tf = new TypeFilter(InterfaceFilter);
					Type[] types = type.FindInterfaces(tf, interfaceType.ToString());
					if (types.Length > 0)
						classes.Add(type);
				}
			return (Type[]) classes.ToArray(typeof(Type));
		}

		public static bool InterfaceFilter(Type typeObj, Object criteriaObj)
		{
			if (typeObj.ToString() == criteriaObj.ToString())
				return true;
			else
				return false;
		}
	}
}