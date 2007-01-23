package Test;
public class A extends B
{
	public class Ab extends Ac
	{
		public void Method()
		{
			m_in_B();
		}
	}
}
public class B
{
	public void m_in_B(){}
}
public class Ac
{
	public void M_in_Ac() {}
}