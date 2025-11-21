#nullable disable

namespace Diagram
{
    public class Calc
    {
        public static decimal GetScale(decimal scale)
        {
            return (decimal)Math.Pow(2, (double)scale);  // 2^scale
        }

        public static float GetAngleInDegrees(Point point1, Point point2)
        {
            double deltaX = point2.X - point1.X;
            double deltaY = point2.Y - point1.Y;

            if (deltaX == 0 && deltaY == 0)
            {
                return 0;
            }

            double radians = Math.Atan2(deltaY, deltaX);

            double degrees = radians * (180.0 / Math.PI);

            return (float)((degrees + 360.0) % 360.0);
        }
    }
}
