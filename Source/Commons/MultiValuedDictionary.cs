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

		public void Add(object key)
		{
			if (!base.Contains(key))
			{
				IList list = new ArrayList();
				base.Add(key, list);
			}
		}

		public bool ContainsValue(object key, object value)
		{
			IList list = (IList) base[key];
			if (list == null)
				return false;
			else
				return list.Contains(value);
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
			set
			{
				if (base.Contains(id))
					base[id] = value;
				else
				{
					Add(id);
					base[id] = value;
				}
			}
		}
	}
}