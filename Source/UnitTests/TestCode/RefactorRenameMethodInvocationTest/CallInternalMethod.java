package Test;
public class Person
{
}
public class PersonTest
{
	public void test()
	{
		Person person = new Person();
		Helpers.ReflectionHelper.CallInternalMethod("setName", person, new object[]{"Mary"});
	}
}
