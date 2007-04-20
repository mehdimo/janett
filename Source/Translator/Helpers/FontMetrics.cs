namespace Helpers
{
	using System.Drawing;
	using System.Windows.Forms;

	public class FontMetrics
	{
		private Control control;
		private Font font;

		public FontMetrics(Control control, Font font)
		{
			this.control = control;
			this.font = font;
		}

		public int getAscent()
		{
			return (int) font.Size;
		}

		public Font getFont()
		{
			return font;
		}

		public int stringWidth(string s)
		{
			return (int) Graphics.FromHwnd(control.Handle).MeasureString(s, font).Width;
		}

		public int getMaxAscent()
		{
			return getAscent();
		}
	}
}