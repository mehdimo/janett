namespace Janett.Commons
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;

	using NUnit.Framework;

	[TestFixture]
	public class DiscoveryTest
	{
		private Discovery dis;

		[SetUp]
		public void Initialize()
		{
			dis = new Discovery();
			dis.AddAssembly(Assembly.GetExecutingAssembly());
		}

		[Test]
		public void RemoveAssembly()
		{
			Discovery dis = new Discovery();
			dis.AddAssembly("UnitTest.dll");
			Assert.AreEqual(1, dis.Assemblies.Count);
			dis.RemoveAssembly("UnitTest.dll");
			Assert.AreEqual(0, dis.Assemblies.Count);
		}

		[Test]
		public void GetClass()
		{
			Type t = dis.GetClass(typeof(DiscoveryTest).Namespace + ".TestClass");
			Assert.IsNotNull(t);

			t = dis.GetClass("TestClass");
			Assert.IsNotNull(t);
		}

		[Test]
		public void GetClassWithAttribute()
		{
			Type t = dis.GetClass(typeof(MyAttribute), new MyAttribute("Test2"));
			Assert.IsNotNull(t);
			Assert.AreEqual("TestClass2WithAttribute", t.Name);
		}

		[Test]
		public void GetClassWithAttributeFromTwoTypes()
		{
			Type t = dis.GetClass(typeof(MyAttribute), new MyAttribute("Test"));
			Assert.IsNotNull(t);
			Assert.AreEqual("TestClassWithAttribute", t.Name);

			t = dis.GetClass(typeof(OtherAttribute), new OtherAttribute("Test"));
			Assert.IsNotNull(t);
			Assert.AreEqual("TestClassWithAttribute", t.Name);
		}

		[Test]
		public void GetClassWithMultipleAttribute()
		{
			Type t = dis.GetClass(typeof(MyAttribute), new MyAttribute("TestName1"));
			Assert.IsNotNull(t);
			Assert.AreEqual("TestClassWithMultiAttribute", t.Name);

			t = dis.GetClass(typeof(MyAttribute), new MyAttribute("TestName2"));
			Assert.IsNotNull(t);
			Assert.AreEqual("TestClassWithMultiAttribute", t.Name);
		}

		[Test]
		public void GetClassWithMultipleAttributeNonComma()
		{
			Type t = dis.GetClass(typeof(MyAttribute), new MyAttribute("TestName4"));
			Assert.IsNotNull(t);
			Assert.AreEqual("TestClassWithMultiAttributeNonComma", t.Name);

			t = dis.GetClass(typeof(MyAttribute), new MyAttribute("TestName5"));
			Assert.IsNotNull(t);
			Assert.AreEqual("TestClassWithMultiAttributeNonComma", t.Name);
		}

		[Test]
		public void GetClassesWithAttribute()
		{
			Type t = dis.GetClasses(typeof(MyAttribute))[0];
			Assert.IsNotNull(t);
			Assert.AreEqual("TestClassWithAttribute", t.Name);
		}

		[Test]
		public void GetMemberByAttributeTypeValue()
		{
			Discovery discovery = new Discovery();
			MemberInfo property = discovery.GetMemberInType(typeof(TestClassWithMethods), new MyAttribute("Property"));
			MemberInfo field = discovery.GetMemberInType(typeof(TestClassWithMethods), new MyAttribute("Field"));
			Assert.AreEqual("Property", property.Name);
			Assert.AreEqual("Field", field.Name);
		}

		[Test]
		public void Inheritence()
		{
			Discovery discovery = new Discovery();
			MemberInfo[] properties = discovery.GetMembersInType(typeof(TestClassInherited), (typeof(MyAttribute)));
			Assert.AreEqual(2, properties.Length);
		}

		[Test]
		public void Resources()
		{
			Stream st = dis.GetResource("Decision.ico");
			Assert.IsNotNull(st);
			Assert.IsTrue(st.CanRead);

			ArrayList resources = dis.GetResources("Decision.ico");
			Assert.AreEqual(1, resources.Count);
			st = (Stream) resources[0];
			Assert.IsNotNull(st);
			Assert.IsTrue(st.CanRead);
		}

		[Test]
		public void Interface()
		{
			Type[] types = dis.GetClassWithInterface(typeof(TestInterface));
			Assert.AreEqual(2, types.Length);
			Assert.AreEqual("TestInterfaceClass1", types[0].Name);
			Assert.AreEqual("TestInterfaceClass2", types[1].Name);
		}

		[Test]
		public void Cache()
		{
			Type type = dis.GetClass(typeof(OtherAttribute), new OtherAttribute("CacheTest"));
			Assert.AreEqual("CacheTest1", type.Name);

			type = dis.GetClass(typeof(MyAttribute), new MyAttribute("CacheTest"));
			Assert.AreEqual("CacheTest2", type.Name);

			type = dis.GetClass(typeof(OtherAttribute), new OtherAttribute("CacheTest", "Test"));
			Assert.AreEqual("CacheTest3", type.Name);
		}

		[Test]
		public void GetMethod()
		{
			MemberInfo member = dis.GetMember(new MyAttribute("Test"), new MyAttribute("TestMethod"));
			Assert.AreEqual("TestMethod", member.Name);
			Assert.AreEqual("TestClassWithAttribute", member.DeclaringType.Name);

			member = dis.GetMember(new MyAttribute("Test"), new MyAttribute("TestMethod"));
			Assert.AreEqual("TestMethod", member.Name);
		}

		[Test]
		public void GetMembers()
		{
			MemberInfo[] members = dis.GetMembers(typeof(MyAttribute), typeof(MyAttribute));
			Assert.IsTrue(members.Length >= 6);
			MemberInfo member = members[0];
			Assert.AreEqual("TestMethod", member.Name);
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class OtherAttribute : NamedAttribute
	{
		public string Arguments;

		public OtherAttribute(string name) : base(name)
		{
		}

		public OtherAttribute(string name, string arguments) : base(name)
		{
			Arguments = arguments;
		}

		public override bool Equals(Object attribute)
		{
			return base.Equals(attribute) && ((OtherAttribute) attribute).Arguments == this.Arguments;
		}

		public override int GetHashCode()
		{
			if (Arguments != null)
				return 17 * Arguments.GetHashCode() + base.GetHashCode();
			else
				return base.GetHashCode();
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field,
		AllowMultiple = true)]
	public class MyAttribute : NamedAttribute
	{
		public MyAttribute(string name) : base(name)
		{
		}
	}

	public class TestClass
	{
	}

	[OtherAttribute("Test")]
	[MyAttribute("Test")]
	public class TestClassWithAttribute
	{
		[MyAttribute("TestMethod")]
		public void TestMethod()
		{
		}
	}

	[MyAttribute("Other")]
	public class OtherTestClassWithAttribute
	{
		[MyAttribute("TestMethod")]
		public void TestMethod()
		{
		}
	}

	[MyAttribute("Test1")]
	public class TestClass1WithAttribute
	{
	}

	[MyAttribute("Test2")]
	public class TestClass2WithAttribute
	{
	}

	[MyAttribute("TestName1,TestName2,TestName3")]
	public class TestClassWithMultiAttribute
	{
	}

	[MyAttribute("TestName4")]
	[MyAttribute("TestName5")]
	public class TestClassWithMultiAttributeNonComma
	{
	}

	public class TestClassBase
	{
		[MyAttribute("Property1")]
		public static int Property1
		{
			get { return 1; }
		}
	}

	[OtherAttribute("CacheTest")]
	public class CacheTest1
	{
	}

	[MyAttribute("CacheTest")]
	public class CacheTest2
	{
	}

	[OtherAttribute("CacheTest", "Test")]
	public class CacheTest3
	{
	}

	public class TestClassInherited : TestClassBase
	{
		[MyAttribute("Property2")]
		public static int Property2
		{
			get { return 1; }
		}
	}

	public interface TestInterface
	{
		void Test();
	}

	public class TestInterfaceClass1 : TestInterface
	{
		public void Test()
		{
		}
	}

	public class TestInterfaceClass2 : TestInterface
	{
		public void Test()
		{
		}
	}

	[MyAttribute("Test3")]
	public class TestClassWithMethods
	{
		[MyAttribute("Property")]
		public static int Property
		{
			get { return 1; }
		}

		[MyAttribute("Field")]
		public static int Field;

		public static int Method2WithAttribute()
		{
			return 2;
		}

		[MyAttribute("Method")]
		public static int Method3WithoutAttribute()
		{
			return 3;
		}

		[MyAttribute("Method")]
		public static int Method4WithAttribute()
		{
			return 4;
		}
	}
}