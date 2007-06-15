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

		public static bool removeAll(ICollection set, ICollection coll)
		{
			foreach (object item in coll)
			{
				((IList) set).Remove(item);
			}
			return true;
		}

		public static ICollection unmodifiableCollection(ICollection coll)
		{
			return coll;
		}

		public static IList unmodifiableList(IList list)
		{
			return list;
		}

		public static ArrayList unmodifiableSortedSet(ArrayList list)
		{
			return list;
		}

		public static IDictionary singletonMap(object key, object value)
		{
			IDictionary dictionary = new Hashtable();
			dictionary.Add(key, value);
			return dictionary;
		}

		public static IList singletonList(object obj)
		{
			IList list = new ArrayList();
			list.Add(obj);
			return list;
		}

		public static bool retainAll(ICollection col, ICollection c)
		{
			throw new NotImplementedException();
		}

		public static IList EMPTY_LIST
		{
			get { return new ArrayList(); }
		}

		public static bool remove(IList collection, object value)
		{
			bool b = collection.Contains(value);
			if (b)
				collection.Remove(value);
			return b;
		}

		public static bool add(ICollection list, object value)
		{
			if (!((IList) list).Contains(value))
			{
				((IList) list).Add(value);
				return true;
			}
			else
				return false;
		}

		public static bool equals(Array arrayA, Array arrayB)
		{
			if (arrayA.Length != arrayB.Length)
				return false;
			for (int i = 0; i < arrayA.Length; i++)
			{
				if (arrayB.GetValue(i) != arrayA.GetValue(i))
					return false;
			}
			return true;
		}

		public static ArrayList tailSet(IList list, int from)
		{
			ArrayList tailList = new ArrayList();
			for (int i = from; i < list.Count; i++)
				tailList.Add(list[i]);

			return tailList;
		}
	}
}