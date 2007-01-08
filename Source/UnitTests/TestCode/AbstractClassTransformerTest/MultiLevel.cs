namespace Test
{
	public abstract class A : B
	{
		public void Main(){}
		public abstract int MethodB();
		public abstract void MethodIC();
	}
	public abstract class B : IC
	{
		public abstract int MethodB();
		public abstract void MethodIC();
	}
	public interface IC
	{
		void MethodIC();
	}
}