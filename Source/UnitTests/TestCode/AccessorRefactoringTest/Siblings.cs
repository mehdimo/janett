namespace Test
{

	public interface IResource
	{
		String ResourceName{get;}
	}

	public abstract class AbstractFileResource : IResource
	{
		public abstract String ResourceName
		{
			get;
		}
	}

	public abstract class AbstractDBResource : IResource
	{
		public abstract String ResourceName
		{
			get;
		}
	}

	public class NetworkResource : IResource
	{
		public String ResourceName
		{
			get {return getUrl();}
		}
		public Strign getUrl()
		{
			return "http://www.mayaf.org/Janett/";
		}
	}

	public class TextFileResource : AbstractFileResource
	{
		String resourceName;
		public String ResourceName
		{
			get { return resourceName; }
		}
	}

	public class BinaryFileResource : AbstractFileResource
	{
		public String ResourceName
		{
			get { return "Binary file"; }
		}
	}
}