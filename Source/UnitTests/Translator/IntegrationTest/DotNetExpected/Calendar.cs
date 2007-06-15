namespace Test.Integration
{
	public class Calendar : Helpers.ThreadHelper
	{
		protected internal const int WeekDays = 7;
		protected static internal string[] Months;
		protected internal int holyDay;
		protected internal int run_Field = 0;

		public void run()
		{
			string str = (10).ToString();
			HolyDay_Property = 5;
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.WaitForExit();
			run_Field = proc.ExitCode;
		}
		public int HolyDay_Property {

			set { holyDay = value; }
		}

		public class HolyDay : System.ICloneable
		{
			public object Clone()
			{
				HolyDay hd = (HolyDay)base.MemberwiseClone();
				return hd;
			}
		}
	}
}
