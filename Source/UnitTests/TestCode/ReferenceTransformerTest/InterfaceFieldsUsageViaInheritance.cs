namespace Test
{
	public class A : IT
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