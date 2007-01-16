namespace Test
{
	using IComparable = System.IComparable;
	using NonSerializedAttribute = System.NonSerializedAttribute;

	public class A : IComparable 
	{ 
		[NonSerializedAttribute()]
		IComparable com;
	}
}