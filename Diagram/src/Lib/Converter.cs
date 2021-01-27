using System;
using System.Globalization;

namespace Diagram
{
	public class Converter //UID8493692592
	{
		public static DateTime ToDateAndTime(string s)
		{
            bool result = DateTime.TryParse(s, out DateTime d);
            if (!result) {
				return DateTime.Now;
			}

			return d;
		}

		public static DateTime ToDate(string s)
		{
            bool result = DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d);
            if (!result) {
				return DateTime.Now;
			}

			return d;
		}

		public static String DateToString(DateTime d)
		{
			return String.Format("{0:yyyy-MM-dd}", d);
		}



		public static long ToInt(string s)
		{
            bool result = Int64.TryParse(s, out long i);
            if (!result) {
				return 0;
			}

			return i;
		}

	}
}

