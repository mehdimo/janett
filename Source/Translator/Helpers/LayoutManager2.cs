namespace Helpers
{
	using System.Windows.Forms;

	public interface LayoutManager2 : LayoutManager
	{
		void AddLayoutComponent(Control control, object constraints);
	}
}