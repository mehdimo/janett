package Test;
import RandomAccessFile = java.io.RandomAccessFile;

public class A
{
	public class InnerA extends RandomAccessFile
	{
	}
	InnerA file = null;
	public void Method()
	{
		file.seek(1);
	}
}