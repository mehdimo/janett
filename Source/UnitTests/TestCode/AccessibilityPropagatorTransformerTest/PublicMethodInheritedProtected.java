package Test;
public abstract class Shape
{
	protected abstract void FillColor();
}
public abstract class SimpleShape extends Shape
{
}
public class Rectangle extends SimpleShape 
{
	public void FillColor()
	{
	}
}

public class PolygonShape extends Shape 
{
	protected void FillColor()
	{
	}
}
public class ComplexShape extends PolygonShape
{
	protected void FillColor()
	{
	}
}