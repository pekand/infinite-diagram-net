using System.Globalization;

#nullable disable

namespace Diagram
{
	public class Converter //UID8493692592
	{
        /// <summary>
        /// converst string to DateTime</summary>
        public static DateTime ToDateAndTime(string s)
		{
            bool result = DateTime.TryParse(s, out DateTime d);
            if (!result) {
				return DateTime.Now;
			}

			return d;
		}

        /// <summary>
        /// converst string to Date as DateTime</summary>
		public static DateTime ToDate(string s)
		{
            bool result = DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d);
            if (!result) {
				return DateTime.Now;
			}

			return d;
		}

        /// <summary>
        /// converst DateTime to string Date (get only date from datetime as string) </summary>
		public static String DateToString(DateTime d)
		{
			return String.Format("{0:yyyy-MM-dd}", d);
		}



        /// <summary>
        /// converst string to int </summary>
		public static long ToInt(string s)
		{
            bool result = Int64.TryParse(s, out long i);
            if (!result) {
				return 0;
			}

			return i;
		}

        /// <summary>
        /// converst string to decimal </summary>
        public static decimal? StringToDecimal(string text)
        {
            decimal result;

            if (decimal.TryParse(text, NumberStyles.AllowParentheses |
                NumberStyles.AllowLeadingWhite |
                NumberStyles.AllowTrailingWhite |
                NumberStyles.AllowThousands |
                NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowLeadingSign,
                NumberFormatInfo.InvariantInfo, out result))
            {
                return result;
            } 

            return null;
        }

        public static Color ColorFromHexString(string hexColor)
        {

            Color defaultColor = Color.White;

            if (hexColor.StartsWith("#"))
            {
                hexColor = hexColor.Substring(1);
            }

            if (hexColor.Length != 6)
            {
                return defaultColor;
            }

            try
            {
                int r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return Color.FromArgb(r, g, b);
            }
            catch (FormatException)
            {

            }

            return defaultColor;
        }

    }
}

