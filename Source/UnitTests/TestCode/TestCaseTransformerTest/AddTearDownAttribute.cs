namespace Test
{
	using junit.framework.TestCase; 
	
	[NUnit.Framework.TestFixture()] 
	public class B : TestCase 
	{
		[NUnit.Framework.TearDown()] 
		public void tearDown() 
		{
		} 
	}
}