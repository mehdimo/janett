namespace Janett.Commons
{
	using System;
	using System.Collections;

	public abstract class Commandlet : ICommand
	{
		private IList outputs;
		private string result;

		public IList Outputs
		{
			get { return outputs; }
			set { outputs = value; }
		}

		public string Result
		{
			get { return result; }
			set { result = value; }
		}

		public string ExecutableDirectory;
		public string ExecutableName;

		public abstract void Execute();

		public virtual void Terminate()
		{
		}

		public virtual void Exception(Exception ex)
		{
		}
	}
}