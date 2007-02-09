package Test;
import java.util.Map;
public class A extends Map
{
	public boolean containsKey(Object obj)
	{
		return true;
	}
}
public class B
{
	public void Method()
	{
		A obj = new A();
		Object key = null;
		boolean b = obj.containsKey(key);
	}
}