namespace Test.Integration
{
	public interface Interface
	{

		string InterfaceMethod1(string _string);
	}
	public class Interface_Fields
	{
		public const string Default = "Default";
		public static int[] Digits = new int[3];
		public static System.IComparable _params = new AnonymousClassComparable1();
	}
	public abstract class InterfaceInnerClass
	{
		public InterfaceInnerClass(string name)
		{
		}
		public abstract string InterfaceInnerClassMethod(string _string);
	}
	public class AnonymousClassComparable1 : System.IComparable
	{
		public AnonymousClassComparable1()
		{
		}
		public int CompareTo(object o)
		{
			return 0;
		}
	}
}