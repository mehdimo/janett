namespace Test
{
	public abstract class Person
	{
		public abstract String Name
		{
			get;
			set;
		}
	}

	public class Staff : Person
	{
		private String name;
		public String Name
		{
			get{return name;}
			set{ throw new System.NotSupportedException();}
		}
	}

	public class Actor : Person
	{
		private String name;
		public String Name
		{
			get{return name;}
			set{ this.name = value;}
		}
	}
}