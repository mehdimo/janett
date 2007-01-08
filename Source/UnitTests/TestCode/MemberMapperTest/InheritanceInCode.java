package Test
public class A
{
	public void Method(ClassifierException e)
	{
		e.printStackTrace();
		string msg = e.getLocalizedMessage();
	}
}
public class ClassifierException extends Exception
{
}