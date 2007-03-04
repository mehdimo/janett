namespace Test
{
	public class A
	{
		int x; 
		int y;  

		public A(int arg)
		{
			InitA(); 
			int z = arg;
		} 

		public A(String arg) 
		{
			InitA(); 
			String s = arg;
		} 

		private void InitA() 
		{
			x = 0; 
			y = 1;
		}	
	}
}