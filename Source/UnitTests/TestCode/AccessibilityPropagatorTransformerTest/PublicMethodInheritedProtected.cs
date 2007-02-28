namespace Test
{
	public abstract class Shape
	{
		public abstract void FillColor();
	}
	public abstract class SimpleShape : Shape
	{
	}
	public class Rectangle : SimpleShape 
	{
		public void FillColor()
		{
		}
	}

	public class PolygonShape : Shape 
	{
		public void FillColor()
		{
		}
	}
	public class ComplexShape : PolygonShape
	{
		public void FillColor()
		{
		}
	}
}