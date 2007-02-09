namespace Helpers
{
	using System;
	using System.Collections;

	public class Collections
	{
		public static object max(ICollection collection)
		{
			object max = null;
			foreach (IComparable obj in collection)
			{
				if (obj.CompareTo(max) > 0 || max == null)
					max = obj;
			}
			return max;
		}

		public static void sort(IList list)
		{
			((ArrayList) list).Sort();
		}

		public static void sort(IList list, IComparer comparer)
		{
			((ArrayList) list).Sort(comparer);
		}

		public static IList singleton(object o)
		{
			ArrayList list = new ArrayList();
			list.Add(o);
			return list;
		}

		public static IList subList(IList list, int from, int to)
		{
			IList newList = new ArrayList();
			for (int i = from; i < to; i++)
				newList.Add(list[i]);
			return newList;
		}

		public static void putAll(IDictionary dest, IDictionary source)
		{
			foreach (DictionaryEntry entry in source)
			{
				dest.Add(entry.Key, entry.Value);
			}
		}

		public static IList synchronizedSet(ArrayList list)
		{
			return list;
		}

		public static bool containsAll(IList set, ICollection coll)
		{
			foreach (object item in coll)
			{
				if (!set.Contains(item))
					return false;
			}
			return true;
		}


		public static void removeAll(IList set, ICollection coll)
		{
			foreach (object item in coll)
			{
				set.Remove(item);
			}
		}

		public static IList EMPTY_LIST
		{
			get { return new ArrayList(); }
		}
	}
}