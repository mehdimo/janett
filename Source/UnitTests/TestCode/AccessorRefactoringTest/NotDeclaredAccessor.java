package Test;
public abstract class Person
{
	public abstract String getName();
}

public class Staff extends Person
{
	private String name;
	public String getName()
	{
		return name;
	}
}

public class Actor extends Person
{
	private String name;
	public String getName()
	{
		return name;
	}

	public void setName(String newName)
	{
		this.name = newName;
	}
}