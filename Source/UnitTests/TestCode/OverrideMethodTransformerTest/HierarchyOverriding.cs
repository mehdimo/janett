namespace Test
{
	public class C : B
	{
		public override void MethodA()
		{
		}
	}
	public class B : A
	{
		public override void MethodA()
		{
		}
	}
	public abstract class A
	{
		public abstract void MethodA();
	}
}