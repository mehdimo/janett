namespace Test
{
	public class A : B
	{
		public class Ab : Ac
		{
			public void Method()
			{
				A.m_in_B();
			}
			A A;
			public Ab(A A)
			{
				this.A = A;
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
}