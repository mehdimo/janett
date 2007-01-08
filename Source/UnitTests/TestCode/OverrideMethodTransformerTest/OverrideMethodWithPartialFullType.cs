namespace Test
{
	public class Test : A
	{
		public override int MethodA(A.B ab)
		{
			return 0;
		}
	}
	public abstract class A
	{
		public class B
		{
		}
		public abstract int MethodA(B b);
	}
}