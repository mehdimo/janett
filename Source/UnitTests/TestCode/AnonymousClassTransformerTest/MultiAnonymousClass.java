package Test;
public class Multi
{
	public void Main()
	{
		outPut(str, new ArrayConstructor() {
						public object Create (int length)
						{
							return new bool[length];
						}} );

		outPut (bit, new ArrayConstructor() {
						public object create (int length)
						{
							return new byte[length];
						}} );
	}
}