using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{
    /// <summary>
    /// Class to represent a 2 dimensional point in cartesian co-ordinates.
    /// </summary>
    public class C2DPoint : C2DBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DPoint() {}

        /// <summary>
        /// Destructor.
        /// </summary>
	    ~C2DPoint() {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">Point to which this will be assigned.</param>
        public C2DPoint(C2DPoint Other) 
        { 
            x = Other.x; 
            y = Other.y; 
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dx">The x value of the point.</param>
        /// <param name="dy">The y value of the point.</param>
	    public C2DPoint(double dx, double dy) 
        { 
            x = dx; 
            y = dy; 
        }

        /// <summary>
        /// Sets the x and y values of the point.
        /// </summary>
        /// <param name="dx">The x value of the point.</param>
        /// <param name="dy">The y value of the point.</param>
	    public void Set(double dx, double dy) 
        { 
            x = dx; 
            y = dy;
        }

        /// <summary>
        /// Assignment to another point.
        /// </summary>
        /// <param name="pt">The point to assign to.</param>
        public void Set(C2DPoint pt)
        {
            x = pt.x;
            y = pt.y;
        }

        /// <summary>
        /// Construction from a vector which can be thought of as a point (and vice versa).
        /// </summary>
        /// <param name="Vector">Vector to assign to.</param>
        public C2DPoint( C2DVector Vector) 
        { 
            x = Vector.i; 
            y = Vector.j; 
        }

        /// <summary>
        /// Returns the mid point between this and the other as a new object.
        /// </summary>
        /// <param name="Other">Another point.</param>
	    public C2DPoint GetMidPoint(C2DPoint Other)
        {
            return new C2DPoint((x + Other.x) / 2, (y + Other.y) / 2);
        }

        /// <summary>
        /// Projects the point on the vector given returning a distance along the vector.
        /// </summary>
        /// <param name="Vector">The vector to project this on.</param>
	    public double Project(C2DVector Vector) 
        {
	        C2DVector vecthis = new C2DVector(x, y);

	        return (vecthis.Dot(Vector)) / Vector.GetLength();
        }

        /// <summary>
        /// Projects the point on the line given returning a distance along the line from the start.
        /// </summary>
        /// <param name="Line">The line to project this on.</param>
        public double Project(C2DLine Line)
        {
	        C2DVector vecthis = new C2DVector(Line.point, this );

	        return (vecthis.Dot(Line.vector)) / Line.vector.GetLength();
        }

        /// <summary>
        /// Projects the point on the vector given returning a distance along the vector.
        /// </summary>
        /// <param name="Vector">The vector to project this on.</param>
        /// <param name="Interval">The interval to recieve the result. 
        /// Both the min and max of the interval will be set to the result.</param>
        public override void Project(C2DVector Vector,  CInterval Interval)
        {
            Interval.dMax = Project(Vector);
            Interval.dMin = Interval.dMax;
        }

        /// <summary>
        /// Projects the point on the vector given returning a distance along the vector.
        /// </summary>
        /// <param name="Line">The vector to project this on.</param>
        /// <param name="Interval">The interval to recieve the result. 
        /// Both the min and max of the interval will be set to the result.</param>
	    public override void Project(C2DLine Line,  CInterval Interval)
        {
            Interval.dMax = Project(Line);
            Interval.dMin = Interval.dMax;
        }

        /// <summary>
        /// True if the point projects onto the line given and returns the point on the line.
        /// Also returns whether the line projects above or below the line if relevant.
        /// </summary>
        /// <param name="Line">The line to project this on.</param>
        /// <param name="ptOnLine">The point to recieve the result.</param>
        /// <param name="bAbove">The flag to indicate whether it projects above or below.</param>
	    public bool ProjectsOnLine(C2DLine Line,  C2DPoint ptOnLine , 
		    ref bool bAbove)
        {
            C2DVector vecthis = new C2DVector(x - Line.point.x, y - Line.point.y);
	        double dProj = vecthis.Dot(Line.vector);

	        if (dProj < 0)
	        {
		        bAbove = false;
		        return false;
	        }

	        double dLength = Line.vector.GetLength();

	        dProj /= dLength;

	        if (dProj > dLength)
	        {
		        bAbove = true;
		        return false;
	        }

	        double dFactor = dProj / dLength;
    		
	        C2DVector vProj = new C2DVector(Line.vector);
	        vProj.Multiply(dFactor);
	        ptOnLine.Set(Line.point.x + vProj.i, Line.point.y + vProj.j);
	        return true;

        }

        /// <summary>
        /// Returns the distance between this and the other point.
        /// </summary>
        /// <param name="Other">The point to return the distance to.</param>
        public override double Distance(C2DPoint Other)
        {
	        double dXD = x - Other.x;
	        double dYD = y - Other.y;

            return Math.Sqrt( dXD * dXD + dYD * dYD );
        }

        /// <summary>
        /// Returns a vector from this to the other point as a new object.
        /// </summary>
        /// <param name="PointTo">The point that vector is to go to.</param>
	    public C2DVector MakeVector(C2DPoint PointTo)
        {
            return new C2DVector(PointTo.x - x, PointTo.y - y);
        }


        /// <summary>
        /// Returns the point that the vector supplied would take this point to as a new object.
        /// </summary>
        /// <param name="V1">The vector from this to the new point.</param>
	    public C2DPoint GetPointTo(C2DVector V1)
        {
            return new C2DPoint(x + V1.i, y + V1.j);
        }

        /// <summary>
        /// Adds 2 points together.
        /// </summary>
        /// <param name="P1">The first point.</param>
        /// <param name="P2">The second point.</param>
        public static C2DPoint operator +(C2DPoint P1, C2DPoint P2) 
        {
            C2DPoint Result = new C2DPoint(P1.x + P2.x, P1.y + P2.y);
            return Result;
        }

        /// <summary>
        /// Takes 1 point from the other.
        /// </summary>
        /// <param name="P1">The first point.</param>
        /// <param name="P2">The second point.</param>
        public static C2DPoint operator -(C2DPoint P1, C2DPoint P2) 
        {
            C2DPoint Result = new C2DPoint(P1.x - P2.x, P1.y - P2.y);
            return Result;
        }

        /// <summary>
        /// Multiplies the point by the factor.
        /// </summary>
        /// <param name="dFactor">The multiplication factor.</param>
	    public void Multiply(double dFactor)
        {
            x *= dFactor;
            y *= dFactor;
        }

        /// <summary>
        /// Divides the point by the factor.
        /// </summary>
        /// <param name="dFactor">The divisor.</param>
	    public void Divide(double dFactor)
        {
            x /= dFactor;
            y /= dFactor;
        }

        /// <summary>
        /// Equality test which really tests for point proximity.
        /// <seealso cref="Constants.conEqualityTolerance"/> 
        /// </summary>
        /// <param name="Other">The other point.</param>
	    public bool PointEqualTo( C2DPoint Other)
        {
	        bool bxClose;
	        bool byClose;

	        if ( x == 0 )
                bxClose = Other.x == 0;
	        else
                bxClose = Math.Abs((Other.x - x) / x) < Constants.conEqualityTolerance;

	        if (!bxClose)
		        return false;		// Get out early if we can.

	        if ( y == 0 )
                byClose = Other.y == 0;
	        else
                byClose = Math.Abs((Other.y - y) / y) < Constants.conEqualityTolerance;

	        return ( byClose );		// We know x is close.
        }

        /// <summary>
        /// Moves this point by the vector given.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public override void Move(C2DVector vector)
        {
            x += vector.i;
            y += vector.j;
        }

        /// <summary>
        /// Rotates this to the right about the origin provided.
        /// </summary>
        /// <param name="dAng">The angle through which to rotate.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
        public override void RotateToRight(double dAng, C2DPoint Origin)
        {
	        C2DVector vector = new C2DVector(Origin, this);
	        vector.TurnRight(dAng);
	        x = Origin.x + vector.i;
	        y = Origin.y + vector.j;
        }

        /// <summary>
        /// Grows this about the origin provided. 
        /// In the case of a point this will just move it away (or closer) 
        /// to the origin as there is no shape to grow.
        /// </summary>
        /// <param name="dFactor">The factor to grow this by.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
        public override void Grow(double dFactor, C2DPoint Origin)
        {
	        C2DVector vector = new C2DVector(Origin, this);
	        vector.Multiply(dFactor);
	        x = Origin.x + vector.i;
	        y = Origin.y + vector.j;

        }

        /// <summary>
        /// Reflects this through the point given.
        /// </summary>
        /// <param name="Other">The point to reflect this through.</param>
        public override void Reflect(C2DPoint Other)
        {
	        // Set up a vector from this to the other
	        C2DVector vec = new C2DVector(this, Other);
	        // Now use the vector to find the reflection by continuing along it from the point given.
	        this.Set(  Other.GetPointTo(vec) );
        }

        /// <summary>
        /// Reflects this through the line given.
        /// </summary>
        /// <param name="Line">The line to reflect this through.</param>
	    public override  void Reflect(C2DLine Line)
        {
	        // First find the point along the line that this projects onto.
	        // Make a vector from the point on the line given to this point.
	        C2DVector vecthis = new C2DVector(Line.point, this );
	        // Find the length of the line given.
	        double dLength = Line.vector.GetLength();
	        // Now make the projection of this point on the line.
	        double dProj = vecthis.Dot(Line.vector);
	        dProj /= dLength;
	        // Find the factor along the line that the projection is.
	        double dFactor = dProj / dLength;
	        // Now set up a copy of the vector of the line given.
	        C2DVector vProj = new C2DVector(Line.vector);
	        // Multiply that by that factor calculated.
	        vProj.Multiply(dFactor);
	        // Use the vector to find the point on the line.
	        C2DPoint ptOnLine = new C2DPoint(Line.point.GetPointTo(vProj) );
	        // Noe simply reflect this in the point.
	        this.Reflect(ptOnLine);
        }

        /// <summary>
        /// Reflects through the y axis.
        /// </summary>
	    public void  ReflectY()
        {
	        x = -x;
        }
        /// <summary>
        /// Reflects through the x axis.
        /// </summary>
	    public void  ReflectX()
        {
            y = -y;
        }

        /// <summary>
        /// Returns the bounding rectangle which will be set to this point.
        /// </summary>
        /// <param name="Rect">The rectangle to recieve the result.</param>
        public override void GetBoundingRect( C2DRect Rect) 
        {
            Rect.Set(this);
        }


        /// <summary>
        /// Snaps this to the grid. The x and y values can only be multiples or the grid size.
        /// </summary>
        /// <param name="grid">The grid to snap this to.</param>
        public override void SnapToGrid(CGrid grid)
        {
            double dxMultiple = Math.Abs(x / grid.GridSize) + 0.5;

            dxMultiple = Math.Floor(dxMultiple);

            if (x < 0)
                x = -dxMultiple * grid.GridSize;
            else
                x = dxMultiple * grid.GridSize;

            double dyMultiple = Math.Abs(y / grid.GridSize) + 0.5;

	        dyMultiple = Math.Floor(dyMultiple);

            if (y < 0) 
                y = -dyMultiple * grid.GridSize;
            else
                y = dyMultiple * grid.GridSize;
        }

        /// <summary>
        /// True if the point projects onto the line given and returns the 
        /// point on the line.
        /// </summary>
        /// <param name="Line"></param>
        public void ProjectOnRay(C2DLine Line)
        {
	        C2DVector vecthis = new C2DVector(Line.point, this );
	        double dProj = vecthis.Dot(Line.vector);

	        double dFactor = dProj / (Line.vector.i * Line.vector.i + Line.vector.j * Line.vector.j );
        	
	        C2DVector vProj = new C2DVector(Line.vector);
	        vProj.i *= dFactor;
            vProj.j *= dFactor;
            this.Set(Line.point.x + vProj.i, Line.point.y + vProj.j);
        }


        /// <summary>
        /// The x component of the point.
        /// </summary>
        public double x;
        /// <summary>
        /// The y component of the point.
        /// </summary>
        public double y;

    }
}
