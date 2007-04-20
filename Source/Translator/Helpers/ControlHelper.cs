namespace Helpers
{
	using System;
	using System.Collections;
	using System.Drawing;
	using System.Windows.Forms;

	public class ControlHelper
	{
		private static IDictionary displayedMnemonicIndex = new Hashtable();
		private static IDictionary displayedMnemonic = new Hashtable();
		private static IDictionary layoutManagers = new Hashtable();
		private static IDictionary horizentalAlignment = new Hashtable();

		public static object getTreeLock(Control control)
		{
			return control;
		}

		public static Insets getInsets(Control control)
		{
			return new Insets(0, 0, 0, 0);
		}

		public static Size getMinimumSize(Control component)
		{
			if (component is JComponent)
				return ((JComponent) component).MinimumSize;
			else
				return component.Size;
		}

		public static FontMetrics getFontMetrics(Control control, Font font)
		{
			return new FontMetrics(control, font);
		}

		public static Size getPreferredSize(Control component)
		{
			if (component is JComponent)
				return ((JComponent) component).PreferredSize;
			else
				return getMinimumSize(component);
		}

		public static Toolkit getToolkit(Control control)
		{
			throw new NotImplementedException();
		}

		public static void putClientProperty(Control component, string key, object value)
		{
			throw new NotImplementedException();
		}

		public static ComponentOrientation getComponentOrientation(Control control)
		{
			return ComponentOrientation.UNKNOWN;
		}

		public static void setLayout(Control container, LayoutManager2 layoutManager)
		{
			layoutManagers[container.GetHashCode()] = layoutManager;
		}

		public static void setOpaque(Control control, bool opaque)
		{
		}

		public static void setBorder(Control panel, Border border)
		{
		}

		public static void setVerticalAlignment(Label label, object alignment)
		{
		}

		public static void setHorizontalAlignment(Label label, int alignment)
		{
			horizentalAlignment[label.GetHashCode()] = alignment;
		}

		public static int getHorizontalAlignment(Label label)
		{
			return (int) horizentalAlignment[label.GetHashCode()];
		}

		public static void setDisplayedMnemonic(Label label, char c)
		{
			displayedMnemonic.Add(label.GetHashCode(), Char.ToUpper(c));
		}

		public static void setDisplayedMnemonicIndex(Label label, int index)
		{
			displayedMnemonicIndex.Add(label.GetHashCode(), index);
		}

		public static void setLabelFor(Label label, Control control)
		{
			throw new NotImplementedException();
		}

		public static void add(Control container, Control control, object constraints)
		{
			container.Controls.Add(control);
			LayoutManager layoutManager = (LayoutManager) layoutManagers[container.GetHashCode()];
			if (layoutManager is LayoutManager2)
				((LayoutManager2) layoutManager).AddLayoutComponent(control, constraints);
		}

		public static Font deriveFont(Font font, bool bold)
		{
			throw new NotImplementedException();
		}

		public static Font deriveFont(Font font, FontStyle style)
		{
			throw new NotImplementedException();
		}

		public static int getDisplayedMnemonic(Label label)
		{
			if (displayedMnemonic.Contains(label.GetHashCode()))
				return (char) displayedMnemonic[label.GetHashCode()];
			else
				return 0;
		}

		public static double getDisplayedMnemonicIndex(Label label)
		{
			if (displayedMnemonicIndex.Contains(label.GetHashCode()))
				return (int) displayedMnemonicIndex[label.GetHashCode()];
			else
				return -1;
		}

		public static void doLayout(Control panel)
		{
			LayoutManager layoutManager = (LayoutManager) layoutManagers[panel.GetHashCode()];
			layoutManager.LayoutContainer(panel);
		}

		public static Panel CreatePanel(LayoutManager layoutManager)
		{
			Panel p = new Panel();
			layoutManagers.Add(p.GetHashCode(), layoutManager);
			return p;
		}
	}

	public abstract class JComponent : Control
	{
		public abstract Size PreferredSize { get; }
		public abstract Size MinimumSize { get; }
	}
}