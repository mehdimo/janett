package Test;
import java.lang.UnsupportedOperationException;

public class Class extends UnsupportedOperationException
{
	public void Method()
	{
		try {}
		catch (UnsupportedOperationException e)
		{
			e.printStackTrace();
		}
	}
}