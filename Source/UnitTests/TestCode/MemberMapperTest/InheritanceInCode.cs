namespace Test
{
	public class A
	{
		public void Method(ClassifierException e)
		{
			System.Console.WriteLine(e.StackTrace);
			string msg = e.Message;
		}
	}
	public class ClassifierException : Exception { }
}