namespace Janett.Framework
{
	public class VariableRenamer : IRenamer
	{
		private static int counter;

		public void Reset()
		{
			counter = 0;
		}

		public string GetNewName(string name)
		{
			counter++;
			return name + "_Renamed" + counter.ToString();
		}
	}
}