namespace Helpers
{
	public class ComponentOrientation
	{
		public static ComponentOrientation UNKNOWN = new ComponentOrientation();

		public bool isLeftToRight()
		{
			return false;
		}

		public bool isHorizontal()
		{
			return true;
		}
	}
}