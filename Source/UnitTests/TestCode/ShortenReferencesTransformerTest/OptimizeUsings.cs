namespace Test
{
	using IComparable = System.IComparable;
	using NonSerializedAttribute = System.NonSerializedAttribute;
	using MethodImplAttribute = System.Runtime.CompilerServices.MethodImplAttribute;
	using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptions;

	public class A : IComparable 
	{ 
		[NonSerializedAttribute()]
		IComparable com;

		[MethodImplAttribute(MethodImplOptions.Synchronized)]
		private void Method()
		{
		}
	}
}