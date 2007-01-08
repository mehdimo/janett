namespace Janett.Framework
{
	using System.IO;

	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;

	public class Source
	{
		public bool CodeFile;

		public string File;
		public string OutputFile;
		public string Code;
		public CompilationUnit CompilationUnit;
		public IParser Parser;

		public Source(FileInfo fileInfo, string contents)
		{
			File = fileInfo.FullName;
			Code = contents;
		}

		public Source(FileInfo fileInfo)
		{
			File = fileInfo.FullName;
		}
	}
}