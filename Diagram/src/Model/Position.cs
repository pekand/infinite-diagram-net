using System;

namespace Diagram
{

    /// <summary>
    /// Point position in canvas</summary>
    public class Position //UID0604640560
    {
        public decimal x = 0;
        public decimal y = 0;

        /*************************************************************************************************************************/
        // CONSTRUCTORS

        /// <summary>
        /// Constructor</summary>
        public Position()
        {
            this.x = 0;
            this.y = 0;
        }

        /// <summary>
        /// Constructor</summary>
        public Position(long x = 0, long y = 0)
        { 
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Constructor</summary>
        public Position(double x = 0, double y = 0)
        {
            this.x = (decimal)x;
            this.y = (decimal)y;
        }

        /// <summary>
        /// Constructor</summary>
        public Position(decimal x = 0, decimal y = 0)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Constructor</summary>
        public Position(Position p)
        {
            if (p != null)
            {
                this.x = p.x;
                this.y = p.y;
            }
            else
            {
                this.x = 0;
                this.y = 0;
            }
        }

        /*************************************************************************************************************************/
        // SETERS AND GETERS

        // <summary>
        /// set </summary>
        public Position Set(long x, long y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        // <summary>
        /// set </summary>
        public Position Set(decimal x, decimal y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        // <summary>
        /// set </summary>
        public Position Set(Position p)
        {
            if (p != null)
            {
                this.x = p.x;
                this.y = p.y;
            }
            else
            {
                this.x = 0;
                this.y = 0;
            }

            return this;
        }


        /// <summary>
        /// Clone</summary>
        public Position Clone()
        {
            return new Position(this);
        }

        // <summary>
        /// Copy position to current position</summary>
        public Position Copy(Position b)
        {
            this.x = b.x;
            this.y = b.y;
            return this;
        }
        
        /*************************************************************************************************************************/
        // ADD OPERATIONS

        // <summary>
        /// add vector</summary>
        public Position Add(Position p)
        {
            this.x += p.x;
            this.y += p.y;
            return this;
        }

        // <summary>
        /// add vector</summary>
        public Position Add(long x, long y)
        {
            this.x += x;
            this.y += y;
            return this;
        }

        public Position Add(long x)
        {
            this.x += x;
            this.y += x;
            return this;
        }

        // <summary>
        /// add vector</summary>
        public Position Add(double a, double b)
        {
            this.x += (decimal)a;
            this.y += (decimal)b;
            return this;
        }

        // <summary>
        /// add vector</summary>
        public Position Add(decimal a, decimal b)
        {
            this.x += a;
            this.y += b;
            return this;
        }

        // <summary>
        /// add node</summary>
        public static Position operator +(Position a, Position b)
        {
            return new Position(a).Add(b);
        }

        /*************************************************************************************************************************/
        // SUBSTRACT OPERATIONS

        // <summary>
        /// subtract vector</summary>
        public Position Subtract(Position p)
        {
            this.x -= p.x;
            this.y -= p.y;
            return this;
        }

        // <summary>
        /// subtract vector</summary>
        public Position Subtract(long a, long b)
        {
            this.x -= a;
            this.y -= b;
            return this;
        }

        // <summary>
        /// subtract vector</summary>
        public Position Subtract(double a, double b)
        {
            this.x -= (decimal)a;
            this.y -= (decimal)b;
            return this;
        }

        // <summary>
        /// subtract vector</summary>
        public Position Subtract(decimal a, decimal b)
        {
            this.x -= a;
            this.y -= b;
            return this;
        }

        // <summary>
        /// subtract constant</summary>
        public Position Subtract(long c)
        {
            this.x -= c;
            this.y -= c;
            return this;
        }

        // <summary>
        /// substact node</summary>        
        public static Position operator -(Position a, Position b)
        {
            return new Position(a).Subtract(b);
        }

        /*************************************************************************************************************************/
        // OPERATIONS

        // <summary>
        /// Count distance between two points</summary>
        public double Distance(Position b)
        {
            return Math.Sqrt((double)((b.x - this.x) * (b.x - this.x) + (b.y - this.y) * (b.y - this.y)));
        }

        // <summary>
        /// Count distance to zero vector</summary>
        public double Size()
        {
            return Math.Sqrt((double)((0 - this.x) * (0 - this.x) + (0 - this.y) * (0 - this.y)));
        }

        // <summary>
        /// subtract vector</summary>
        public Position Invert()
        {
            this.x = -this.x;
            this.y = -this.y;
            return this;
        }

        // <summary>
        /// scale vector</summary>
        public Position Scale(long scale)
        {
            this.x *= (decimal)scale;
            this.y *= (decimal)scale;
            return this;
        }

        // <summary>
        /// scale vector</summary>
        public Position Scale(double scale)
        {
            this.x *= (decimal)scale;
            this.y *= (decimal)scale;
            return this;
        }

        // <summary>
        /// scale vector</summary>
        public Position Scale(decimal scale)
        {
            this.x *= scale;
            this.y *= scale;
            return this;
        }

        // <summary>
        /// zoom vector</summary>
        public Position Split(long scale)
        {
            this.x /= (decimal)scale;
            this.y /= (decimal)scale;
            return this;
        }

        // <summary>
        /// zoom vector</summary>
        public Position Split(double scale)
        {
            this.x /= (decimal)scale;
            this.y /= (decimal)scale;
            return this;
        }

        // <summary>
        /// zoom vector</summary>
        public Position Split(decimal scale)
        {
            this.x /= scale;
            this.y /= scale;
            return this;
        }

        // <summary>
        /// scale vector by constant</summary>
        public static Position operator *(Position a, decimal c)
        {
            return new Position(a).Scale(c);
        }

        // <summary>
        /// scale vector by constant</summary>
        public static Position operator /(Position a, decimal c)
        {
            return new Position(a).Split(c);
        }

        /*************************************************************************************************************************/
        // CONVERSION

        // <summary>
        /// Convert position to cartesian coordinate</summary>
        public Position ConvertToStandard()
        {
            return new Position(this.x, -this.y);
        }

        // <summary>
        /// Convert position to string</summary>
        public override string ToString()
        {
            return "[" + x.ToString() +  "," + y.ToString() + "]";
        }

        // <summary>
        /// Convert position to cartesian coordinates</summary>
        public Position ToCartesian()
        {
            this.y = -this.y;
            return this;
        }

        // <summary>
        /// Convert position to pc view coordinates</summary>
        public Position ToView()
        {
            this.y = -this.y;
            return this;
        }

    }
}
