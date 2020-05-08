using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{

    /// <summary>
    /// Class representing a 2D vector.
    /// </summary>
    public class C2DVector
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DVector() {}

        /// <summary>
        /// Destructor.
        /// </summary>
        ~C2DVector() {}

        /// <summary>
        /// Constructor with assignment.
        /// </summary>
        /// <param name="di">i.</param>
        /// <param name="dj">j.</param>
        public C2DVector(double di, double dj) 
        { 
            i = di; 
            j = dj; 
        }

        /// <summary>
        /// Constructor with assignment.
        /// </summary>
        /// <param name="Other">other vector.</param>
        public C2DVector(C2DVector Other) 
        {
            i = Other.i;
            j = Other.j;
        }

        /// <summary>
        /// Constructor provides 2 points, this being the vector from the first to the second.
        /// </summary>
        /// <param name="PointFrom">Point 1.</param>
        /// <param name="PointTo">Point 2.</param>
        public C2DVector(C2DPoint PointFrom, C2DPoint PointTo)
        {
            i = PointTo.x - PointFrom.x;
            j = PointTo.y - PointFrom.y;
        }

        /// <summary>
        /// Constructor converts a point to the vector (a point can be interpreted as a point and vice versa)
        /// </summary>
        /// <param name="Point">Point to assign to.</param>
        public C2DVector(C2DPoint Point)
        {
            i = Point.x;
            j = Point.y;
        }

        /// <summary>
        /// Constructor converts a point to the vector (a point can be interpreted as a point and vice versa)
        /// </summary>
        /// <param name="di">i.</param>
        /// <param name="dj">j.</param>
        public void Set(double di, double dj) 
        { 
            i = di; 
            j = dj; 
        }

        /// <summary>
        /// Sets it to be the vector from the 1st to the second.
        /// </summary>
        /// <param name="PointFrom">Point 1.</param>
        /// <param name="PointTo">Point 2.</param>
        public void Set(C2DPoint PointFrom, C2DPoint PointTo)
        {
            i = PointTo.x - PointFrom.x;
            j = PointTo.y - PointFrom.y;
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Other">Other vector.</param>
        public void Set(C2DVector Other)
        {
            i = Other.i;
            j = Other.j;
        }

        /// <summary>
        /// Reverses the direction of the vector.
        /// </summary>
        public void Reverse()
        {
            i = -i;
            j = -j;
        }

        /// <summary>
        /// Turns right 90 degrees.
        /// </summary>
        public void TurnRight()
        {
            double tempi = i;
            i = j;
            j = -tempi;
        }

        /// <summary>
        /// Turns right by the angle given in radians.
        /// </summary>
        /// <param name="dAng">Angle to turn through.</param>
        public void TurnRight(double dAng)
        {
            TurnLeft(Constants.conTWOPI - dAng);
        }

        /// <summary>
        /// Turns left by 90 degrees.
        /// </summary>
        public void TurnLeft()
        {
            double tempi = i;
            i = -j;
            j = tempi;
        }

        /// <summary>
        /// Turns left by the angle given in radians.
        /// </summary>
        /// <param name="dAng">Angle to turn through.</param>
        public void TurnLeft(double dAng)
        {
            double temp = i;

            i = Math.Cos(dAng) * i - Math.Sin(dAng) * j;
            j = Math.Sin(dAng) * temp + Math.Cos(dAng) * j;
        }

        /// <summary>
        /// Returns the length of the vector.
        /// </summary>
        public double GetLength() 
        {
            return Math.Sqrt(i * i + j * j);
        }

        /// <summary>
        /// Sets the length of the vector.
        /// </summary>
        /// <param name="dDistance">The new length.</param>
        public void SetLength(double dDistance)
        {
            MakeUnit();
            i = i * dDistance;
            j = j * dDistance;
        }

        /// <summary>
        /// Makes the vector unit.
        /// </summary>
        public void MakeUnit()
        {
            double dDistance = GetLength();
            if (dDistance == 0) 
                return;
            i = i / dDistance;
            j = j / dDistance;
        }

        /// <summary>
        /// Addition.
        /// </summary>
        public static C2DVector operator+(C2DVector V1, C2DVector V2)
        {
            C2DVector V3 = new C2DVector(V1.i + V2.i, V1.j + V2.j);
            return V3;
        }

        /// <summary>
        /// Subtraction.
        /// </summary>
        public static C2DVector operator-(C2DVector V1, C2DVector V2)
        {
            C2DVector V3 = new C2DVector(V1.i - V2.i, V1.j - V2.j);
            return V3;
        }

        /// <summary>
        /// Multiplication.
        /// </summary>
	    public void Multiply(double dFactor)
        {
            i *= dFactor;
            j *= dFactor;
        }

        /// <summary>
	    /// Dot product.
        /// </summary>
        public double Dot(C2DVector Other)
        {
            return i * Other.i + j * Other.j;
        }

        /// <summary>
	    /// Cross product.
        /// </summary>
	    public double Cross(C2DVector Other)
        {
            return i * Other.i - j * Other.j;
        }

        /// <summary>
	    /// Assignment to a point.
        /// </summary>
        public void Set(C2DPoint Other)
        {
            i = Other.x;
            j = Other.y;
        }

        /// <summary>
	    /// Equality test.
        /// </summary>
	    public bool VectorEqualTo(  C2DVector Other)
        {
            bool biClose;
            bool bjClose;

            if (i == 0)
                biClose = Other.i == 0;
            else
                biClose = Math.Abs((Other.i - i) / i) < Constants.conEqualityTolerance;
            if (j == 0)
                bjClose = Other.j == 0;
            else
                bjClose = Math.Abs((Other.j - j) / j) < Constants.conEqualityTolerance;

            return (biClose && bjClose);
        }

        /// <summary>
	    /// Returns the angle from north in radians.
        /// </summary>
	    public double AngleFromNorth()
        {
            if (j == 0)
            {
                if (i > 0)
                    return Constants.conHALFPI;
                else
                    return 3 * Constants.conHALFPI;
            }
            if (i == 0)
            {
                if (j > 0)
                    return 0;
                else
                    return Constants.conPI;
            }

            double ang = Math.Atan(i / j);

            if (j < 0) ang += Constants.conPI;
            else if (i < 0) ang += 2 * Constants.conPI;

            return ang;
        }

        /// <summary>
        /// Returns the angle to the right to another vector.
        /// </summary>
	    public double AngleToRight(C2DVector Other)
        {
            double Result = Other.AngleFromNorth() - AngleFromNorth();
            if (Result < 0) Result += Constants.conTWOPI;

            return Result;
        }

        /// <summary>
        /// Returns the angle to the left to another vector.
        /// </summary>
	    public double AngleToLeft(C2DVector Other)
        {
            return (Constants.conTWOPI - AngleToRight(Other));
        }

        /// <summary>
        /// Returns the shortest angle between 2 vectors i.e. the dot product of the norms.
        /// </summary>
	    public double AngleBetween(C2DVector Other)
        {
            double dDot = this.Dot(Other);
            dDot /= (this.GetLength() * Other.GetLength());
            return Math.Acos(dDot);
        }

        /// <summary>
        /// The value in the x axis.
        /// </summary>
        public double i;

        /// <summary>
        /// The value in the y axis.
        /// </summary>
        public double j;
    }
}
