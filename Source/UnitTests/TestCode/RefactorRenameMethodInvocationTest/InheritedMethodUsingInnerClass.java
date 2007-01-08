package Test;
public class A extends B
{
	public class Ab
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