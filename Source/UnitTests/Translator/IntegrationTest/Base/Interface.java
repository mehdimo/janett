package Test.Integration;
public interface Interface
{
	public String Default = "Default";
    int[] Digits = new int[3];
    static Comparable params = new Comparable()
                    {
                        public int compareTo(Object o)
                        {
                            return 0;
                        }
                    };

	public String InterfaceMethod1(String string);
	public abstract class InterfaceInnerClass
	{
		public InterfaceInnerClass(String name)
		{
		}
		public abstract String InterfaceInnerClassMethod(String string);
	}
}