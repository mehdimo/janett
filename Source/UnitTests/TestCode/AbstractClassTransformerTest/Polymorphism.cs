namespace Test
{
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
	public abstract class BinaryCode : Object
	{
	}
	public abstract class BCD : BinaryCode, Comparator
		{
	}
}