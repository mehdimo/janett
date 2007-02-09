namespace Test
{
	public class B : IT
	{
	}
	public class A : B
	{
		public void Method()
		{
			int index = Test.IT_Fields.Alpha;
		}
	}
	public interface IT
	{
	}
	public class IT_Fields
	{
		public int Alpha = 1;
	}
}
