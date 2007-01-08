package Test.Integration;

import junit.framework.TestCase;

public class TestClass extends TestCase
{
    public TestClass(int arg1, String arg2)
    {
    }

    public void setUp()
    {
    }

    public void tearDown()
    {
    }

    public void testMethod()
    {
		AbstractClass abc = null;
		String testName = abc.getName();
		abc.setName(testName);
		Integer a = new Integer(0);
		int i = a.intValue();
        AbstractClass.InnerAbstractClass inf = new AbstractClass.InnerAbstractClass(i)
                                                    {
                                                        public void InnerAbstractClassMethod(int num)
                                                        {
                                                        }
                                                    };
        Integer b[] = null;
        boolean ab = (a == null) || (b == null);
		int j = 0;
        assertEquals("message :", i, j);
    }

    public void main()
    {
    }
}
