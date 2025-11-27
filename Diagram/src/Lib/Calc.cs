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

        public static bool IsPointInsideRotatedRectangle(
        decimal rectangleX, decimal rectangleY, decimal rectangleWidth, decimal rectangleHeight,
        decimal rotationVectorX, decimal rotationVectorY,
        decimal mouseX, decimal mouseY)
        {
            decimal centerX = rectangleX + (rectangleWidth / 2.0m);
            decimal centerY = rectangleY + (rectangleHeight / 2.0m);

            decimal rotationAngleRadians = (decimal)Math.Atan2((double)rotationVectorY, (double)rotationVectorX);

            decimal translatedMouseX = mouseX - centerX;
            decimal translatedMouseY = mouseY - centerY;

            decimal cosTheta = (decimal)Math.Cos((double)(-rotationAngleRadians));
            decimal sinTheta = (decimal)Math.Sin((double)(-rotationAngleRadians));

            decimal rotatedMouseX = (translatedMouseX * cosTheta) - (translatedMouseY * sinTheta);
            decimal rotatedMouseY = (translatedMouseX * sinTheta) + (translatedMouseY * cosTheta);

            decimal halfWidth = rectangleWidth / 2.0m;
            decimal halfHeight = rectangleHeight / 2.0m;

            bool hitOnWidthAxis = (rotatedMouseX >= -halfWidth) && (rotatedMouseX <= halfWidth);
            bool hitOnHeightAxis = (rotatedMouseY >= -halfHeight) && (rotatedMouseY <= halfHeight);

            return hitOnWidthAxis && hitOnHeightAxis;
        }
    }
}
