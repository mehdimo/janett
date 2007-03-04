namespace Test
{
	public interface IDocument
	{
	}
	public class IDocument_Fields
	{
		public static string TITLE;
	}

	public class Book : IDocument
	{
		public class Page
		{
			public void getTitle()
			{
				string title = Test.IDocument_Fields.TITLE;
			}
		}
	}
}