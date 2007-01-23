package Test;

public interface IResource
{
	String getResourceName();
}

public abstract class AbstractFileResource implements IResource
{
	public abstract String getResourceName();
}

public abstract class AbstractDBResource implements IResource
{
	public abstract String getResourceName();
}

public class NetworkResource implements IResource
{
	public String getResourceName()
	{
		return getUrl();
	}
	public Strign getUrl()
	{
		return "http://www.mayaf.org/Janett/";
	}
}

public class TextFileResource extends AbstractFileResource
{
	String resourceName;
	public String getResourceName()
	{
		return resourceName;
	}
}

public class BinaryFileResource extends AbstractFileResource
{
	public String getResourceName()
	{
		return "Binary file";
	}
}