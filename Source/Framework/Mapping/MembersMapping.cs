namespace Janett.Framework
{
	using System.Collections;

	public class MembersMapping : Hashtable
	{
		public override bool Contains(object key)
		{
			return Contains(key.ToString());
		}

		public bool Contains(string key)
		{
			foreach (string id in Keys)
			{
				if (id == key.ToString())
					return true;
			}
			return false;
		}

		public new string this[object key]
		{
			get
			{
				foreach (string id in Keys)
				{
					if (id == key.ToString() || id.StartsWith(key.ToString()))
						return (string) base[id];
				}
				return null;
			}
		}
	}
}