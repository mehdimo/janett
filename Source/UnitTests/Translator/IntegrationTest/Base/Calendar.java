package Test.Integration
import java.text.NumberFormat;
public class Calendar extends java.lang.Thread
{
	static int WeekDays = 7;
	static String[] Months;
	int holyDay;
	int run = 0;
	
	public void run()
	{
		NumberFormat nf = NumberFormat.getNumberInstance();
		String str = nf.format(10);
		setHolyDay(5);
		Process proc = new Process();
		run = proc.waitFor();
	}
	
	public void setHolyDay(int day)
	{
		holyDay = day;
	}
	
	public class HolyDay implements Cloneable
	{
		public Object clone()
		{
			HolyDay hd = (HolyDay)super.clone();
			return hd;
		}
	}
}