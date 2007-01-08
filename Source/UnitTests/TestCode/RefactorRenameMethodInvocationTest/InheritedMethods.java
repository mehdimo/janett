package Test;
public class C
{
	B bf;
	public void Method()
	{
		B b;
		b.m_in_B();
		b.m_in_A();
		this.bf.m_in_A();
	}
}
public interface A
{
	void m_in_A();
}
public interface B extends A
{
	void m_in_B();
}