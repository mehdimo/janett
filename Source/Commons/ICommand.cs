namespace Janett.Commons
{
	using System.Collections;

	public interface ICommand
	{
		IList Outputs { get; set; }
		string Result { get; set; }
		void Execute();
	}
}