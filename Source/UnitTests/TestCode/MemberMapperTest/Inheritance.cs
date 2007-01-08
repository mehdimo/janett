namespace Test
{
	using java.lang.UnsupportedOperationException; 
	
	public class Class : UnsupportedOperationException 
	{
		public void Method() 
		{ 
			try {}
			catch (UnsupportedOperationException e)
			{
				System.Console.WriteLine(e.StackTrace);
			}
		}
	}
}