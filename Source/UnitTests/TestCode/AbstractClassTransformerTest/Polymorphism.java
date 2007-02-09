package Test;
public interface Comparator
{
	int equals(Object arg);
}
public class Object
{
	public int equals(Object obj)
	{
		return 0;
	}
}
public abstract class BinaryCode extends Object
{
}
public abstract class BCD extends BinaryCode implements Comparator
{
}