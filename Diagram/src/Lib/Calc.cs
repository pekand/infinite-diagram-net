#nullable disable

namespace Diagram
{
    public class Calc
    {
        public static decimal GetScale(decimal scale)
        {
            return (decimal)Math.Pow(2, (double)scale);  // 2^scale
        }
    }
}
