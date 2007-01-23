namespace Test
{
	public interface IShape
	{
		Class getType();
	}

	public abstract class Shape : IShape
	{
		public Class getType()
		{
			return getClass();
		}
	}

	public class Rectangle : Shape
	{
		public Class getType()
		{
			return getClass();
		}
	}
}