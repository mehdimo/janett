namespace Test
{
	public class A
	{
		public void repeatedName()
		{
			if (true)
			{
				if (true)
				{
					for (;;)
					{
						int i = 3;
						i--;
					}
					int i_Renamed1 = 2;
					i_Renamed1++;
				}
				int i_Renamed2 = 1;
				i_Renamed2++;
			}
			int i_Renamed3 = 0;
			i_Renamed3 = 10 + 2;
			try
			{
				i_Renamed3 = i_Renamed3 * 2;
			}
			catch (Exception ex)
			{
			}
		}
	}
}