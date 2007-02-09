namespace Test
{
	using java.util.Map;
	public class A : Map
	{
		public override boolean Contains(Object obj)
		{
			return true;
		}
	}
	public class B
	{
		public void Method()
		{
			A obj = new A();
			Object key = null;
			boolean b = obj.Contains(key);
		}
	}
}