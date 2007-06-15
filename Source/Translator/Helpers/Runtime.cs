namespace Helpers
{
	public class Runtime
	{
		public static Runtime getRuntime()
		{
			return new Runtime();
		}

		public void addShutdownHook(ThreadHelper thread)
		{
			throw new System.NotImplementedException();
		}

		public System.Diagnostics.Process exec(string[] args)
		{
			return System.Diagnostics.Process.Start(args[0]);
		}
	}
}