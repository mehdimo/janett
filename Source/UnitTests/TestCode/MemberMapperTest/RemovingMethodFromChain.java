package Test;
import java.lang.reflect.Modifier;
import java.lang.reflect.Field;

public class Test
{
	public void Method(Field f)
	{
		Modifier.isStatic(f.getModifiers())
	}
}