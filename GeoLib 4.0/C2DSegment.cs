using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{

    /// <summary>
    /// Abstract class representing a 2D entity.
    /// </summary>
    public class C2DSegment : C2DBase
    {

        /// <summary>
        /// Constructor.
        /// </summary>
	    public C2DSegment() {}

        /// <summary>
        /// Destructor.
        /// </summary>
	    ~C2DSegment() {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ArcOther">The other arc.</param>   
	    public C2DSegment(C2DArc ArcOther)
        {
            Arc.Set(ArcOther);
        }

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="PtFrom">The point the arc is to go from.</param>
        /// <param name="PtTo">The point the arc is to go to.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
	    public C2DSegment(C2DPoint PtFrom, C2DPoint PtTo, double dRadius, 
		    bool bCentreOnRight, bool bArcOnRight)
        {
            Arc.Set( PtFrom, PtTo, dRadius, bCentreOnRight, bArcOnRight);
        }


        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="PtFrom">The point the arc is to go from.</param>
        /// <param name="Vector">The vector defining the end point.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
	    public C2DSegment(C2DPoint PtFrom, C2DVector Vector, double dRadius, 
		    bool bCentreOnRight, bool bArcOnRight)
        {
            Arc.Set( PtFrom, Vector, dRadius, bCentreOnRight, bArcOnRight);
        }


        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="Line">The line defining the start and end point of the arc.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
	    public C2DSegment(C2DLine Line, double dRadius, 
		    bool bCentreOnRight, bool bArcOnRight)
        {
            Arc.Set( Line, dRadius, bCentreOnRight, bArcOnRight);
        }


        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="PtFrom">The point the arc is to go from.</param>
        /// <param name="PtTo">The point the arc is to go to.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
	    public void Set(C2DPoint PtFrom, C2DPoint PtTo, double dRadius, 
		    bool bCentreOnRight, bool bArcOnRight)
        {
            Arc.Set( PtFrom, PtTo, dRadius, bCentreOnRight, bArcOnRight);
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="PtFrom">The point the arc is to go from.</param>
        /// <param name="Vector">The vector defining the end point.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
	    public void Set(C2DPoint PtFrom, C2DVector Vector, double dRadius, 
		    bool bCentreOnRight , bool bArcOnRight)
        {
            Arc.Set( PtFrom, Vector, dRadius, bCentreOnRight, bArcOnRight);
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Line">The line defining the start and end point of the arc.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
	    public void Set(C2DLine Line, double dRadius, 
		    bool bCentreOnRight , bool bArcOnRight )
        {
            Arc.Set( Line, dRadius, bCentreOnRight, bArcOnRight);
        }

        /// <summary>
        /// Tests to see if the radius is large enough to connect the end points.
        /// </summary>
	    public bool IsValid()
        {
            return Arc.IsValid();
        }

        /// <summary>
        /// Returns the corresponding circle's centre.
        /// </summary>
	    public C2DPoint GetCircleCentre()
        {
            return Arc.GetCircleCentre();
        }

        /// <summary>
        /// Returns the perimeter of the shape.
        /// </summary>
	    public double GetPerimeter() 
        {
	        return (Arc.Line.vector.GetLength() + Arc.GetLength());
        }

        /// <summary>
        /// Returns the length of the arc.
        /// </summary>
	    public double GetArcLength() 
        {
	        return Arc.GetLength();
        }

        /// <summary>
        /// Returns the bounding rectangle.
        /// </summary>
	    public override void GetBoundingRect( C2DRect Rect) 
        {
            Arc.GetBoundingRect( Rect);
        }

        /// <summary>
        /// Returns the inverse of this i.e. the other part of the circle to this.
        /// </summary>
        /// <param name="Other">The other segment.</param>	
	    public void GetInverse( C2DSegment Other) 
        {
        	Other.Set(Arc.Line, Arc.Radius, 
		    		Arc.CentreOnRight, !Arc.ArcOnRight );
        }

        /// <summary>
        /// Always +ve and LESS than PI. In radians.
        /// </summary>
	    public double GetSegmentAngle() 
        {
	        if (!IsValid()) 
                return 0;

	        return Arc.GetSegmentAngle();
        }

        /// <summary>
	    /// Returns the area.
        /// </summary>
	    public double GetArea() 
        {
	        double dSegAng = Arc.GetSegmentAngle();
	        double dRadius = Arc.Radius;
	        if (Arc.CentreOnRight ^ Arc.ArcOnRight)
	        {
		        return (   dRadius * dRadius * ( dSegAng - Math.Sin(dSegAng)) / 2);

	        }
	        else
	        {
		        // if the curve is the big bit.
		        return   (   dRadius * dRadius * (Constants.conPI - ( dSegAng - Math.Sin(dSegAng)) / 2));
	        }
        }

        /// <summary>
	    /// Returns the area which is positive if anti-clockwise -ve if clockwise
        /// </summary>
	    public double GetAreaSigned() 
        {
	        if (Arc.ArcOnRight)
		        return GetArea();
	        else
		        return -GetArea();
        }

        /// <summary>
	    /// Returns the centroid.
        /// </summary>
	    public C2DPoint GetCentroid()
        {
	        // Find the area first. Do it explicitly as we may need bits of the calc later.
	        double dSegAng = Arc.GetSegmentAngle();
	        bool bBig = Arc.ArcOnRight == Arc.CentreOnRight;

	        double dRadius = Arc.Radius;
	        double dRadiusSquare = dRadius * dRadius;
	        double dCircleArea = dRadiusSquare * Constants.conPI;
	        double dArea = dRadiusSquare * ( (dSegAng - Math.Sin(dSegAng)) / 2);

	        // Find the maximum length of the small segment along the direction of the line.
	        double dLength = Arc.Line.GetLength();
	        // Now find the average height of the segment over that line
	        double dHeight = dArea / dLength;

	        // Find the centre point on the line and the centre of the circle
	        C2DPoint ptLineCen = new C2DPoint(Arc.Line.GetMidPoint());
	        C2DPoint ptCircleCen = new C2DPoint(Arc.GetCircleCentre());

	        // Set up a line from the mid point on the line to the circle centre
	        // then set the length of it to the average height divided by 2. The end
	        // point of the line is then the centroid. If we are using the small bit, 
	        // The line needs to be reversed.
	        C2DLine Line = new C2DLine( ptLineCen, ptCircleCen);

	        Line.vector.Reverse();

	        Line.vector.SetLength(  dHeight / 2 );

	        if (bBig)
	        {
		        C2DPoint ptSmallCen = new C2DPoint(Line.GetPointTo());
		        // Return the weighted average of the 2 centroids.

                ptCircleCen.Multiply(dCircleArea);
                ptSmallCen.Multiply(dArea);
                C2DPoint pRes = ptCircleCen - ptSmallCen;
                pRes.Multiply(1.0 / (dCircleArea - dArea));
                return pRes;
		   //     return ( new C2DPoint(ptCircleCen * dCircleArea - ptSmallCen * dArea) ) / ( dCircleArea - dArea);
	        }
	        else
		        return Line.GetPointTo();
        }

        /// <summary>
        /// Gets the first point on the straight line.
        /// </summary>
	    public C2DPoint GetPointFrom() 
        { 
            return Arc.Line.GetPointFrom() ;
        }

        /// <summary>
        /// Gets the second point on the straight line.
        /// </summary>
	    public C2DPoint GetPointTo()  
        { 
            return Arc.Line.GetPointTo() ;
        }


        /// <summary>
        /// Returns a reference to the line as a new object.
        /// </summary>
	    public C2DLine GetLine() 
        {
            return new C2DLine(Arc.Line);
        }

        /// <summary>
	    /// Returns whether the point is in the shape.
        /// </summary>
	    public bool Contains( C2DPoint TestPoint) 
        {
	        C2DPoint ptCentre = new C2DPoint(GetCircleCentre());
        	
	        if (TestPoint.Distance(ptCentre) > Arc.Radius) 
		        return false;

	        else 
	        {
		        if (Arc.Line.IsOnRight(TestPoint))
		        {
			        return Arc.ArcOnRight;
		        }
		        else
		        {
			        return !Arc.ArcOnRight;
		        }
	        }
        }

        /// <summary>
        /// Moves this point by the vector given.
        /// </summary>
        /// <param name="vector">The vector.</param>
	    public override  void Move(C2DVector vector) 
        {
            Arc.Move(vector);
        }

        /// <summary>
        /// Rotates this to the right about the origin provided.
        /// </summary>
        /// <param name="dAng">The angle through which to rotate.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
	    public override  void RotateToRight(double dAng, C2DPoint Origin)  
        {
            Arc.RotateToRight(dAng, Origin);
        }

        /// <summary>
        /// Grows the segment by the factor from the origin provided.
        /// </summary>
        /// <param name="dFactor">The factor to grow by.</param>
        /// <param name="Origin">The origin about which to grow.</param>
	    public override  void  Grow(double dFactor, C2DPoint Origin) 
        {
            Arc.Grow(dFactor, Origin);
        }

        /// <summary>
        /// Reflects the shape throught the point given.
        /// </summary>
        /// <param name="point">The point to reflect through.</param>
	    public override  void Reflect( C2DPoint point) 
        {
            Arc.Reflect( point);
        }

        /// <summary>
        /// Reflects the in the line given.
        /// </summary>
        /// <param name="Line">The line to reflect through.</param>
	    public  override void Reflect(C2DLine Line) 
        {
            Arc.Reflect( Line);
        }

        /// <summary>
        /// Returns the distance to the point given.
        /// </summary>
        /// <param name="TestPoint">The point.</param>
	    public override double Distance(C2DPoint TestPoint) 
        {
	        if (Contains(TestPoint))
		        return 0;
            double d1 = Arc.Distance(TestPoint);
            double d2 = Arc.Line.Distance(TestPoint);
	        return Math.Min(d1, d2);
        }

        /// <summary>
        /// Projects this onto the line given.
        /// </summary>
        /// <param name="Line">The line.</param>
        /// <param name="Interval">The projection.</param>
	    public override  void Project(C2DLine Line,  CInterval Interval) 
        {
	        Arc.Project(Line, Interval);
	        CInterval LineInterval = new CInterval();
	        Arc.Line.Project(Line,  LineInterval);
	        Interval.ExpandToInclude( LineInterval );
        }

        /// <summary>
        /// Projects this onto the vector given.
        /// </summary>
        /// <param name="Vector">The Vector.</param>
        /// <param name="Interval">The projection.</param>
	    public  override void Project(C2DVector Vector,  CInterval Interval) 
        {
	        Arc.Project(Vector,  Interval);
	        CInterval LineInterval = new CInterval();
	        Arc.Line.Project(Vector,  LineInterval);
	        Interval.ExpandToInclude( LineInterval );
        }

        /// <summary>
        /// Snaps this to the conceptual grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public override void SnapToGrid(CGrid grid)
        {
            Arc.SnapToGrid(grid);

        }
        /// <summary>
        /// The arc.
        /// </summary>
	    protected C2DArc arc = new C2DArc();
        /// <summary>
        /// The arc.
        /// </summary>
        public C2DArc Arc
        {
            get
            {
                return arc;
            }
        }
    }
}
