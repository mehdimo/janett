namespace Test.Integration
{



	[System.SerializableAttribute()]
	public abstract class Class1 : AbstractClass, System.Collections.IComparer
	{
		[System.NonSerializedAttribute()]
		private int ID;
		protected internal int Number;
		protected internal System.Type type;
		private bool _bool;

		static Class1()
		{
			int x = 1;






			int y = 1;
		}
		protected internal Class1()
		{
			type = this.GetType();
			_bool = BoolMethod();
		}
		private bool BoolMethod()
		{
			System.Type c = GetType();
			string name = Name;
			Name = name;
			return base.ExistSimilarFieldAndMethod();
		}
		private sealed class InnerClass1 : System.IComparable
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

		[System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		private bool Method2(int arg)
		{
			System.IO.FileStream stream = new System.IO.FileStream("Test", System.IO.FileMode.Open);
			int count = (int)10f;
			byte[] buffer = new byte[count];
			stream.Read(buffer, 0, buffer.Length);
			System.Reflection.FieldInfo f = null;
			return f.IsNotSerialized;
		}

		public override string InterfaceMethod1(string _string)
		{
			string text = Test.Integration.Interface_Fields.Default + Test.Integration.Interface_Fields._params.ToString();
			text = text.Substring(1).Trim() + typeof(Test.Integration.Interface[]) + typeof(Class1) + this.GetType();
			string _lock = "";
			Test.Integration.InterfaceInnerClass anonymousClass = new AnonymousClassInterface_InterfaceInnerClass1(text, this, _lock);
			System.Collections.IDictionary map = new System.Collections.Hashtable();
			if (map.Contains(text) && ExistSimilarFieldAndMethod())
			{
				lock (text) {
					System.Collections.IEnumerator it = map.Keys.GetEnumerator();
					System.Collections.DictionaryEntry entry = (System.Collections.DictionaryEntry)it.Current;
					object key = entry.Key;
				}
			}
			return text[0] + System.Text.RegularExpressions.Regex.Replace(text, "\\s", "&nbsp");
		}

		public int Compare(object obj1, object obj2)
		{
			return 0;
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
				System.Collections.IList list = null;
				((System.Collections.ArrayList)list).ToArray(typeof(string));
				bool res = enclosingInstance.BoolMethod();
				return _string.Trim() + _lock.Length + res;
			}
			internal Class1 enclosingInstance;
			internal string _lock;
			public Class1 Enclosing_Instance {
				get { return enclosingInstance; }
			}
		}

	}
}
