namespace Test
{
	public interface IShape
	{
		int Length { get; set; }
	}

	public abstract class Shape : IShape
	{
		public abstract int Length{ get; set;}
	}

	public class Rectangle : Shape
	{
		private int length;
		public int Length
		{
			get { return length; }
			set { this.length = value;}
		}
	}
}