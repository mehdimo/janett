namespace Test.Integration
{
	using SerializableAttribute = System.SerializableAttribute;
	using NonSerializedAttribute = System.NonSerializedAttribute;
	using IComparable = System.IComparable;
	using MethodImplAttribute = System.Runtime.CompilerServices.MethodImplAttribute;
	using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptions;
	using java.lang;

	using java.util;
	using List = java.util.List;
	using Map = java.util.Map;
	using java.io;
	using Modifier = java.lang.reflect.Modifier;


	[SerializableAttribute()]
	public abstract class Class1 : AbstractClass, Comparator
	{
		[NonSerializedAttribute()]
		private int ID;
		protected internal int Number;
		protected internal Class type;
		private bool _bool;

		static Class1()
		{
			int x = 1;






			int y = 1;
		}
		protected internal Class1()
		{
			type = java.lang.Object.instancehelper_getClass(this);
			_bool = BoolMethod();
		}
		private bool BoolMethod()
		{
			string name = Name;
			Name = name;
			return base.ExistSimilarFieldAndMethod();
		}
		private sealed class InnerClass1 : IComparable
		{
			internal InnerClass1(int arg, Class1 Class1) : this(Class1)
			{
				this.Class1 = Class1;
			}
			protected internal InnerClass1(int arg1, int arg2, Class1 Class1) : this(Class1)
			{
				this.Class1 = Class1;
			}
			internal int ID;
			protected internal int MethodForTestConstArgs(bool b, char c)
			{
				return 0;
			}
			public int CompareTo(object _object)
			{
				if (true) return 0; 				else return -1; 
			}
			internal void InnerMembers()
			{
				bool b = Class1.Method2(ID + Class1.Number);
			}
			public InnerClass1(Class1 Class1)
			{
				this.Class1 = Class1;
				ID = MethodForTestConstArgs(true, 'a');
			}
			Class1 Class1;
		}

		[MethodImplAttribute(MethodImplOptions.Synchronized)]
		private bool Method2(int arg)
		{
			FileInputStream stream = new FileInputStream("Test");
			int count = Float.floatToIntBits(10f);
			byte[] buffer = new byte[count];
			stream.read(buffer);
			java.lang.reflect.Field f = null;
			return Modifier.isTransient(f.getModifiers());
		}

		public override string InterfaceMethod1(string _string)
		{
			string text = Test.Integration.Interface_Fields.Default + java.lang.Object.instancehelper_toString(Test.Integration.Interface_Fields._params);
			text = java.lang.String.instancehelper_trim(java.lang.String.instancehelper_substring(text, 1)) + java.lang.Class.forName(typeof(Test.Integration.Interface[]).AssemblyQualifiedName) + java.lang.Class.forName(typeof(Class1).AssemblyQualifiedName) + java.lang.Object.instancehelper_getClass(this);
			string _lock = "";
			Test.Integration.InterfaceInnerClass anonymousClass = new AnonymousClassInterface_InterfaceInnerClass1(text, this, _lock);
			Map map = new HashMap();
			if (map.containsKey(text) && ExistSimilarFieldAndMethod())
			{
				lock (text) {
					Iterator it = map.keySet().iterator();
					object obj = it.next();
				}
			}
			return java.lang.String.instancehelper_charAt(text, 0) + java.lang.String.instancehelper_replaceAll(text, "\\s", "&nbsp");
		}

		public int compare(object obj1, object obj2)
		{
			return 0;
		}
		public override abstract void AbstractClassMethod();
		public bool equals(object parameter1)
		{
			return java.lang.Object.instancehelper_equals(this, parameter1);
		}
		private class AnonymousClassInterface_InterfaceInnerClass1 : Test.Integration.InterfaceInnerClass
		{
			public AnonymousClassInterface_InterfaceInnerClass1(string name, Class1 enclosingInstance, string _lock) : base(name)
			{
				this.enclosingInstance = enclosingInstance;
				this._lock = _lock;
			}
			public override string InterfaceInnerClassMethod(string _string)
			{
				List list = null;
				list.toArray(new string[5]);
				bool res = enclosingInstance.BoolMethod();
				return java.lang.String.instancehelper_trim(_string) + java.lang.String.instancehelper_length(_lock) + res;
			}
			internal Class1 enclosingInstance;
			internal string _lock;
			public Class1 Enclosing_Instance {
				get { return enclosingInstance; }
			}
		}
	}
}
