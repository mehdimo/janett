namespace Test
{
	public class A : B
	{
		public class Ab
		{
			public void Method()
			{
				M_in_B();
			}
		}
	}
	public class B
	{
		public void m_in_B(){}
	}
}