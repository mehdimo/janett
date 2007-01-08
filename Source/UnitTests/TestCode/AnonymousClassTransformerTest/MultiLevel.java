package Test;
public abstract class A extends B
{
	public void Main(){}
}
public abstract class B implements IC
{
	public abstract int MethodB();
}
public interface IC
{
	void MethodIC();
}