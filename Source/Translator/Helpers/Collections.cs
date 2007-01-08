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
	}
}