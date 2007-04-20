namespace Helpers
{
	using System;
	using System.Drawing;

	internal class UIManager
	{
		public static void addPropertyChangeListener(PropertyChangeListener handler)
		{
		}

		public static LookAndFeel getLookAndFeel()
		{
			throw new NotImplementedException();
		}

		public static void setLookAndFeel(string lookAndFeel)
		{
		}

		public static Font getFont(string s)
		{
			return new Font("Dialog", 12);
		}

		public static Color getColor(string colorName)
		{
			throw new NotImplementedException();
		}
	}
}