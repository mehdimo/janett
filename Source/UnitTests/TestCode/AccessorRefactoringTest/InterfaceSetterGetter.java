package Test;
public interface IShape
{
	int getLength();
	void setLength(int length);
}

public abstract class Shape implements IShape
{
	public abstract int getLength();
	public abstract void setLength(int length);
}

public class Rectangle extends Shape
{
	private int length;
	public int getLength()
	{
		return length;
	}
	public void setLength(int length)
	{
		this.length = length;
	}
}