package net.host;

public interface Document
{
	public String getContent(Page page);
	public interface Page
	{
		public int getPageCount();
	}
}
