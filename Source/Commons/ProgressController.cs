namespace Janett.Commons
{
	using System;
	using System.Collections;

	public class ProgressController
	{
		private IDictionary counts = new Hashtable();
		private IDictionary all = new Hashtable();

		public void SetCount(string step, int count)
		{
			float milestone = ((float) count / 30);
			all.Add(step, milestone);
		}

		public void Increment(string step)
		{
			if (counts.Contains(step))
			{
				int count = (int) counts[step];
				if (!all.Contains(step))
					return;
				float milestone = (float) all[step];
				counts[step] = ++count;
				float rem = count % milestone;
				if (rem < 1)
					Console.Write(".");
			}
			else
			{
				Console.WriteLine();
				Console.Write(step.PadRight(20));
				counts.Add(step, 0);
			}
		}
	}
}