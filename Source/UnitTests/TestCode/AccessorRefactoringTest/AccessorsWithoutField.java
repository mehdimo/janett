package Test;
public interface IShape
{
	Class getType();
}

public abstract class Shape implements IShape
{
	public Class getType()
	{
		return getClass();
	}
}

public class Rectangle extends Shape
{
	public Class getType()
	{
		return getClass();
	}
}