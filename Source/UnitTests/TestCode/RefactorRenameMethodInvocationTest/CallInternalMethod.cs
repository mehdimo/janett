namespace Test
{
	public class Person
	{
		public String Name
		{
			set{}
		}
	}
	public class PersonTest
	{
		public void test()
		{
			Person person = new Person();
			Helpers.ReflectionHelper.CallInternalMethod("set_Name", person, new object[]{"Mary"});
		}
	}
}
