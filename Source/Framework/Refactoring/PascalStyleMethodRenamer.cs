namespace Janett.Framework
{
	public class PascalStyleMethodRenamer : IRenamer
	{
		public string GetNewName(string name)
		{
			return name[0].ToString().ToUpper() + name.Substring(1);
		}
	}
}