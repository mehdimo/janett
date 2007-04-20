namespace Helpers
{
	using System;

	public class Insets : ICloneable
	{
		private int _top;
		private int _left;
		private int _bottom;
		private int _right;

		public Insets(int _top, int _left, int _bottom, int _right)
		{
			this._top = _top;
			this._left = _left;
			this._bottom = _bottom;
			this._right = _right;
		}

		public int top
		{
			get { return _top; }
		}

		public int left
		{
			get { return _left; }
		}

		public int bottom
		{
			get { return _bottom; }
		}

		public int right
		{
			get { return _right; }
		}

		public object Clone()
		{
			return new Insets(_top, _left, _bottom, _right);
		}
	}
}