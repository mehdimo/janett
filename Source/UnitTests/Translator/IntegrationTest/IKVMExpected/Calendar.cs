namespace Test.Integration
{
	using java.lang;
	using NumberFormat = java.text.NumberFormat;
	public class Calendar : java.lang.Thread
	{
		protected internal const int WeekDays = 7;
		protected static internal string[] Months;
		protected internal int holyDay;
		protected internal int run_Field = 0;

		public override void run()
		{
			NumberFormat nf = NumberFormat.getNumberInstance();
			string str = nf.format(10);
			HolyDay_Property = 5;
			Process proc = new Process();
			run_Field = proc.waitFor();
		}
		public int HolyDay_Property {

			set { holyDay = value; }
		}

		public class HolyDay : java.lang.Object, Cloneable.__Interface
		{
			public object clone()
			{
				HolyDay hd = (HolyDay)java.lang.Object.instancehelper_clone(this);
				return hd;
			}
		}
	}
}
