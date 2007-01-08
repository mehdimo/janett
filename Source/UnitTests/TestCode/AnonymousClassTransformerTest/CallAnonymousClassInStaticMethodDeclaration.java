package Test;
public class A
{
	public static void Method1()
	{
		int sum;
		int i = new Calc()
					{
						public int addAll(){ return sum; }
					};
	}
	public void Method2()
	{
		int[] result;
		int i = new Calc()
					{
						public int[] calculate(){ B.StMethod(); return result; }
					};
	}
}
public class B
{
	public static void StMethod() {}
}