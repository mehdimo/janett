package Test.Integration;

import java.util.*;
import java.util.List;
import java.util.Map;
import java.util.Calendar;
import java.io.*;
import java.lang.reflect.Modifier;

import Test.Integration.Interface;

public abstract class Class1 extends AbstractClass implements Serializable, Comparator
{
    private transient int ID;
    int Number;
	Class type = this.getClass();
    private boolean bool = boolMethod();

    static
    {
        int x = 1;
    }

    protected Class1()
    {
    }

    private boolean boolMethod()
    {
		Class c = getClass();
		String name = getName();
		setName(name);
		Calendar cal = Calendar.getInstance();
		int min = cal.get(Calendar.MINUTE);
		cal.set(Calendar.MONTH, java.util.Calendar.AUGUST);
		cal.add(Calendar.HOUR, 12);
        return super.existSimilarFieldAndMethod();
    }

    private final class InnerClass1 implements Comparable
	{
        private InnerClass1(int arg)
        {
        }
        InnerClass1(int arg1, int arg2)
        {
            this(arg1);
        }

        private int ID = MethodForTestConstArgs(true, 'a');
        int MethodForTestConstArgs(boolean b, char c){return 0;}

        public int compareTo(Object object)
		{
            if (true)
                return 0;
            else return -1;
        }
		private final void InnerMembers() throws java.lang.UnsupportedOperationException, Exception
		{
			boolean b = method2(ID + Number);
		}
	}

    static
    {
        int y = 1;
    }

    private synchronized boolean method2(int arg) throws java.lang.UnsupportedOperationException, Exception
	{
		if (arg == 0)
		{
			int count = 10;
		}
        FileInputStream stream = new FileInputStream("Test");
        int count = Float.floatToIntBits(10.0f);
        byte buffer[] = new byte[count];
        stream.read(buffer);
        java.lang.reflect.Field f = null;
        return Modifier.isTransient(f.getModifiers());
	}

	public String InterfaceMethod1(String string)
	{
		String text = Interface.Default + Interface.params.toString();
		text = text.substring(1).trim() + Test.Integration.Interface[].class + Class1.class + this.getClass();
        final String lock = "";
        Interface.InterfaceInnerClass anonymousClass = new Interface.InterfaceInnerClass(text)
								{
                                    public String InterfaceInnerClassMethod(String string)
									{
                                        List list = null;
                                        list.toArray(new String[5]);
                                        boolean res = boolMethod();
                                        return string.trim() + lock.length() + res;
									}
								};
		Map map = new HashMap();
		if (map.containsKey(text) && existSimilarFieldAndMethod())
		{
			synchronized (text)
			{
				Iterator it = map.keySet().iterator();
				Map.Entry entry = (Map.Entry) it.next();
				Object key = entry.getKey();
			}
		}
        return text.charAt(0) + text.replaceAll("\\s", "&nbsp");
	}

    public int compare(Object obj1, Object obj2)
    {
        methodToExclude();
        return 0;
    }

	public void methodToExclude() 
	{
	}
}
