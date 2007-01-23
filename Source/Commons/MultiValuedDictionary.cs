namespace Janett.Commons
{
	using System.Collections;

	public class MultiValuedDictionary : Hashtable
	{
		public override void Add(object key, object value)
		{
			if (base.Contains(key))
			{
				IList list = (IList) base[key];
				list.Add(value);
			}
			else
			{
				IList list = new ArrayList();
				list.Add(value);
				base.Add(key, list);
			}
		}

		public IList this[string id]
		{
			get
			{
				if (base.Contains(id))
					return (IList) base[id];
				else
					return new ArrayList();
			}
		}
	}
}