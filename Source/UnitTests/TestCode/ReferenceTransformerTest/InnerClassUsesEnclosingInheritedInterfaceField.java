package Test;
public interface IDocument
{
}
public class IDocument_Fields
{
	public static string TITLE;
}

public class Book extends IDocument
{
	public class Page
	{
		public void getTitle()
		{
			string title = TITLE;
		}
	}
}