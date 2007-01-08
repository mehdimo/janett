namespace Test.Integration
{



	[System.SerializableAttribute()]
	public abstract class Class1 : AbstractClass, System.Collections.IComparer
	{
		[System.NonSerializedAttribute()]
		private int ID;
		protected internal int Number;
		private bool _bool;

		static Class1()
		{
			int x = 1;






			int y = 1;
		}
		protected internal Class1()
		{
			_bool = BoolMethod();
		}
		private bool BoolMethod()
		{
			string name = Name;
			Name = name;
			return base.ExistSimilarFieldAndMethod();
		}
		private sealed class InnerClass1 : System.IComparable
		{
			private InnerClass1(int arg) : this()
			{
			}
			protected internal InnerClass1(int arg1, int arg2) : this()
			{
			}
			private int ID;
			protected internal int MethodForTestConstArgs(bool b, char c)
			{
				return 0;
			}
			public int CompareTo(object _object)
			{
				if (true) return 0; 				else return -1; 
			}
			private void InnerMembers()
			{
				bool b = Method2(ID + Number);
			}
			public InnerClass1()
			{
				ID = MethodForTestConstArgs(true, 'a');
			}
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
					object obj = it.Current;
				}
			}
			return text[0] + System.Text.RegularExpressions.Regex.Replace(text, "\\s", "&nbsp");
		}

		public int Compare(object obj1, object obj2)
		{
			return 0;
		}
		public override abstract void AbstractClassMethod();
		public abstract bool Equals(object parameter1);
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
			private Class1 enclosingInstance;
			private string _lock;
			public Class1 Enclosing_Instance {
				get { return enclosingInstance; }
			}
		}
	}
}
