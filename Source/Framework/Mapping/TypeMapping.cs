namespace Janett.Framework
{
	public class TypeMapping
	{
		public string Target;
		public MembersMapping Members = new MembersMapping();

		public TypeMapping(string target)
		{
			Target = target;
		}

		public TypeMapping()
		{
		}
	}
}