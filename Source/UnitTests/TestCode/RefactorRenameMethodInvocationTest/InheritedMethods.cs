namespace Test
{
	public class C
	{
		B bf;
		public void Method()
		{
			B b;
			b.M_in_B();
			b.M_in_A();
			this.bf.M_in_A();
		}
	}
	public interface A
	{
		void m_in_A();
	}
	public interface B : A
	{
		void m_in_B();
	}
}