namespace Janett.Framework
{
	using System.IO;

	public class ProjectTextWriter : StringWriter
	{
		public int Depth;
		private char prevChar;

		public override void Write(char value)
		{
			if (prevChar == '"' && value == '>')
			{
				base.WriteLine();
				for (int i = 0; i < 4 * (Depth - 2); i++)
					base.Write(' ');
				base.Write(">");
			}
			else
				base.Write(value);
			prevChar = value;
		}
	}
}