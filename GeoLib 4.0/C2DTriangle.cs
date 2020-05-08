using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GeoLib
{
    /// <summary>
    /// Class representing a 2D triangle.
    /// </summary>
    public class C2DTriangle : C2DBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
	    public C2DTriangle(){}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pt1">Point 1.</param>
        /// <param name="pt2">Point 2.</param>
        /// <param name="pt3">Point 3.</param>
	    public C2DTriangle(C2DPoint pt1, C2DPoint pt2, C2DPoint pt3)
        {
            P1.Set(pt1);
            P2.Set(pt2);
            P3.Set(pt3);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
	    ~C2DTriangle() {}

        /// <summary>
        /// Assignement.
        /// </summary>
        /// <param name="pt1">Point 1.</param>
        /// <param name="pt2">Point 2.</param>
        /// <param name="pt3">Point 3.</param>
	    public void Set(C2DPoint pt1, C2DPoint pt2, C2DPoint pt3)
        {
            P1.Set(pt1);
            P2.Set(pt2);
            P3.Set(pt3);
        }

        /// <summary>
        /// True if the 3 are collinear.
        /// </summary>
	    public bool Collinear()
        {
	        return (GetAreaSigned() == 0);
        }

        /// <summary>
        /// Returns the area.
        /// </summary>
	    public double GetArea()
        {
	        return Math.Abs( GetAreaSigned(P1, P2, P3));
        }

        /// <summary>
        /// Returns the area signed (indicating weather clockwise or not).
        /// </summary>
	    public double GetAreaSigned()
        {
            return GetAreaSigned(P1, P2, P3);
        }

        /// <summary>
        /// True if clockwise.
        /// </summary>
	    public bool IsClockwise() 
        {
            return GetAreaSigned() < 0;
        }

        /// <summary>
        /// Returns the circumcentre.
        /// </summary>
	    public C2DPoint GetCircumCentre()
        {
	        return C2DTriangle.GetCircumCentre(P1, P2, P3);
        }

        /// <summary>
        /// Returns the Fermat point (also known as the Torricelli point).
        /// </summary>
        public C2DPoint GetFermatPoint()
        {
            return C2DTriangle.GetFermatPoint(P1, P2, P3);
        }

        /// <summary>
        /// InCentre function.
        /// </summary>
	    public C2DPoint GetInCentre()
        {
            return C2DTriangle.GetInCentre(  P1, P2, P3);
        }

        /// <summary>
        /// Returns the perimeter.
        /// </summary>
	    public double GetPerimeter()
        {
      	    return P1.Distance(P2) + P2.Distance(P3) + P3.Distance(P1);
        }

        /// <summary>
        /// Returns true if the point is contained.
        /// </summary>
	    public bool Contains(C2DPoint ptTest)
        {
	        bool bClockwise = GetAreaSigned() < 0;

	        if ( (GetAreaSigned(P1, P2, ptTest) < 0) ^ bClockwise)
		        return false;

	        if ( (GetAreaSigned(P2, P3, ptTest) < 0) ^ bClockwise)
		        return false;
        	
	        if ( (GetAreaSigned(P3, P1, ptTest) < 0) ^ bClockwise)
		        return false;

	        return true;
        }

        /// <summary>
        /// Moves this point by the vector given.
        /// </summary>
        /// <param name="Vector">The vector.</param>
	    public override void Move(C2DVector Vector)
        {
            P1.Move(Vector);
            P2.Move(Vector);
            P3.Move(Vector);
        }

        /// <summary>
        /// Rotates this to the right about the origin provided.
        /// </summary>
        /// <param name="dAng">The angle through which to rotate.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
        public override void RotateToRight(double dAng, C2DPoint Origin)
        {
            P1.RotateToRight(dAng, Origin);
            P2.RotateToRight(dAng, Origin);
            P3.RotateToRight(dAng, Origin);
        }

        /// <summary>
        /// Grows the triangle.
        /// </summary>
        /// <param name="dFactor">The factor to grow by.</param>
        /// <param name="Origin">The origin through which to grow.</param>
        public override void Grow(double dFactor, C2DPoint Origin)
        {
	        P1.Grow(dFactor, Origin);
    	    P2.Grow(dFactor, Origin);
    	    P3.Grow(dFactor, Origin);
        }

        /// <summary>
        /// Reflects the triangle.
        /// </summary>
        /// <param name="Point">The point to reflect through.</param>
        public override void Reflect(C2DPoint Point)
        {
            P1.Reflect(Point);
            P2.Reflect(Point);
            P3.Reflect(Point);
        }

        /// <summary>
        /// Reflects the in the line given.
        /// </summary>
        /// <param name="Line">The line to reflect through.</param>
        public override void Reflect(C2DLine Line)
        {
    	    P1.Reflect(Line);
	        P2.Reflect(Line);
    	    P3.Reflect(Line);
        }

        /// <summary>
        /// Distance to a point.
        /// </summary>
        /// <param name="ptTest">The test point.</param>
        public override double Distance(C2DPoint ptTest)
        {
            C2DPoint P1 = new C2DPoint();
	        return Distance(ptTest,  P1);
        }

        /// <summary>
        /// Distance to a point.
        /// </summary>
        /// <param name="ptTest">The test point.</param>
        /// <param name="ptOnThis">Output. The closest point on the triangle.</param>
	    public double Distance(C2DPoint ptTest,  C2DPoint ptOnThis)
        {
	        double dArea = GetAreaSigned();
            bool BTemp = true;
	        // Construct the lines.
	        C2DLine Line12 = new C2DLine(P1, P2);
	        C2DLine Line23 = new C2DLine(P2, P3);
            C2DLine Line31 = new C2DLine(P3, P1);

	        if (dArea == 0)
	        {
		        // Colinear so find the biggest line and return the distance from that
		        double d1 = Line12.GetLength();
		        double d2 = Line23.GetLength();
		        double d3 = Line31.GetLength();
		        if (d1 > d2 && d1 > d3)
			        return Line12.Distance(ptTest,  ptOnThis);
		        else if (d2 > d3)
			        return Line23.Distance(ptTest,  ptOnThis);
		        else
			        return Line31.Distance(ptTest,  ptOnThis);
	        }
	        // Find out it the triangle is clockwise or not.
	        bool bClockwise = dArea < 0;

	        // Set up some pointers to record the lines that the point is "above", "above" meaning that the
	        // point is on the opposite side of the line to the rest of the triangle
	        C2DLine LineAbove1 = null;
            C2DLine LineAbove2 = null;

	        // Find out which Lines have the point above.
	        if (  Line12.IsOnRight( ptTest ) ^ bClockwise  )  // if the pt is on the opposite side to the triangle
		        LineAbove1 = Line12;
	        if ( Line23.IsOnRight( ptTest ) ^ bClockwise)
	        {
		        if (LineAbove1 != null)
			        LineAbove2 = Line23;
		        else
			        LineAbove1 = Line23;
	        }
	        if ( Line31.IsOnRight( ptTest ) ^ bClockwise)
	        {
		        if (LineAbove1 != null)
		        {
			        // We can't have all the lines with the point above.
			        Debug.Assert(LineAbove2 != null);
			        LineAbove2 = Line31;
		        }
		        else
			        LineAbove1 = Line31;
	        }

	        // Check for containment (if there isn't a single line that its above then it must be inside)
	        if (LineAbove1 == null)
	        {
		        // Pt inside so project onto all the lines and find the closest projection (there must be one).
        	
		        // Set up a record of the point projection on the lines.
		        C2DPoint ptOnLine = new C2DPoint();
		        bool bSet = false;
		        double dMinDist = 0;

                if (ptTest.ProjectsOnLine(Line12,  ptOnLine,  ref BTemp))
		        {
			        dMinDist = ptTest.Distance(ptOnLine);
				    ptOnThis.Set(ptOnLine);
			        bSet = true;
		        }
                if (ptTest.ProjectsOnLine(Line23,  ptOnLine, ref BTemp))
		        {
			        double dDist = ptTest.Distance(ptOnLine);
			        if (!bSet || dDist < dMinDist)
			        {
				        dMinDist = dDist;
					    ptOnThis.Set(ptOnLine);
				        bSet = true;
			        }
		        }
                if (ptTest.ProjectsOnLine(Line31,  ptOnLine, ref BTemp))
		        {
			        double dDist = ptTest.Distance(ptOnLine);
			        if (!bSet || dDist < dMinDist)
			        {
				        dMinDist = dDist;
					    ptOnThis.Set(ptOnLine);
				        bSet = true;
			        }
		        }
		        Debug.Assert(bSet);
		        return -dMinDist; //-ve if inside
	        }
	        else if (LineAbove2 == null)
	        {
		        // it is only above 1 of the lines so simply return the distance to that line
		        return LineAbove1.Distance(ptTest,  ptOnThis);
	        }
	        else
	        {
		        // It's above 2 lines so first check them both for projection. Can only be projected on 1.
		        // If the point can be projected onto the line then that's the closest point.
		        C2DPoint ptOnLine = new C2DPoint();
                if (ptTest.ProjectsOnLine(LineAbove1,  ptOnLine, ref BTemp))
		        {
			        ptOnThis = ptOnLine;
			        return ptOnLine.Distance(ptTest);
		        }
                else if (ptTest.ProjectsOnLine(LineAbove2,  ptOnLine, ref BTemp))
		        {
				    ptOnThis = ptOnLine;
			        return ptOnLine.Distance(ptTest);
		        }
		        else
		        {
			        // The point doesn't project onto either line so find the closest point
			        if (LineAbove1 == Line12)
			        {
				        if (LineAbove2 == Line23)
				        {
					        ptOnThis = P2;
					        return ptTest.Distance(P2);
				        }
				        else
				        {
						    ptOnThis = P1;
					        return ptTest.Distance(P1);
				        }
			        }
			        else
			        {
					    ptOnThis = P3;
				        return ptTest.Distance(P3);
			        }
		        }
	        }
        }

        /// <summary>
        /// Distance to a another.
        /// </summary>
        /// <param name="Other">The other triangle.</param>
        /// <param name="ptOnThis">Output. The closest point on the triangle.</param>
        /// <param name="ptOnOther">Output. The closest point on the other triangle.</param>
        public double Distance(C2DTriangle Other,  C2DPoint ptOnThis,  C2DPoint ptOnOther)
        {
            C2DPoint ptTemp = new C2DPoint();
            double dMinDist = Distance(Other.P1,  ptOnThis);
            ptOnOther.Set( Other.P1 );

            double dDist = Distance(Other.P2,  ptTemp);
            if (dDist < dMinDist)
            {
                ptOnOther.Set(Other.P2);
                ptOnThis.Set(ptTemp);

                dMinDist = dDist;
            }

            dDist = Distance(Other.P3,  ptTemp);
            if (dDist < dMinDist)
            {
                ptOnOther.Set( Other.P3);
                ptOnThis.Set(ptTemp);
                dMinDist = dDist;
            }

            dDist = Other.Distance(P1,  ptTemp);
            if (dDist < dMinDist)
            {
                ptOnOther.Set(ptTemp);
                ptOnThis.Set(P1);
                dMinDist = dDist;
            }

            dDist = Other.Distance(P2,  ptTemp);
            if (dDist < dMinDist)
            {
                ptOnOther.Set(ptTemp);
                ptOnThis.Set(P2);
                dMinDist = dDist;
            }

            dDist = Other.Distance(P3,  ptTemp);
            if (dDist < dMinDist)
            {
                ptOnOther.Set(ptTemp);
                ptOnThis.Set(P3);
                dMinDist = dDist;
            }

            return dMinDist;
        }

        /// <summary>
        /// Returns the bounding rect.
        /// </summary>
        /// <param name="Rect">Output. The bounding rectangle.</param>
        public override void GetBoundingRect(C2DRect Rect)
        {
	        Rect.Set(P1);
    	    Rect.ExpandToInclude(P2);
	        Rect.ExpandToInclude(P3);

        }

        /// <summary>
        /// Static version of area signed function.
        /// </summary>
	    public static double GetAreaSigned(C2DPoint pt1, C2DPoint pt2, C2DPoint pt3)
        {
	        double dArea = pt1.x * pt2.y - pt2.x * pt1.y +
				           pt2.x * pt3.y - pt3.x * pt2.y +
				           pt3.x * pt1.y - pt1.x * pt3.y;

	        dArea /= 2.0;

	        return dArea;

        }

        /// <summary>
        /// Returns whether the triangle is clockwise.
        /// </summary>
	    public static bool IsClockwise(C2DPoint pt1, C2DPoint pt2, C2DPoint pt3)
        {
	        return GetAreaSigned(pt1, pt2, pt3) < 0;
        }

        /// <summary>
        /// Static version of circumcentre function.
        /// </summary>
        public static C2DPoint GetCircumCentre(C2DPoint pt1, C2DPoint pt2, C2DPoint pt3)
        {
            
	        C2DLine Line12 = new C2DLine (pt1, pt2);
	        C2DLine Line23 = new C2DLine (pt2, pt3);
        	
	        // Move the lines to start from the midpoint on them
	        Line12.point.Set( Line12.GetMidPoint());
	        Line23.point.Set( Line23.GetMidPoint());
	        // Turn them right (left would work as well)
	        Line12.vector.TurnRight();
	        Line23.vector.TurnRight();
	        // Find the intersection between them taking the intersect point even if they don't 
	        // intersect directly (i.e. where they would intersect because we may have turned them
	        // the wrong way).
	        List<C2DPoint> IntPt = new List<C2DPoint>();
            bool B1 = true , B2 = true;
	        Line12.Crosses(Line23,  IntPt,ref B1, ref B2, true);

	        C2DPoint ptResult = new C2DPoint(0, 0);

	        if (IntPt.Count == 1)
	        {
		        ptResult = IntPt[0];
	        }
	        else
	        {
		        // co-linear so fail.
                Debug.Assert(false, "Colinnear triangle. Cannot calculate Circum Centre");
	        }

	        return ptResult;

        }

        /// <summary>
        /// Static version of Fermat point function.
        /// </summary>
        public static C2DPoint GetFermatPoint(C2DPoint pt1, C2DPoint pt2, C2DPoint pt3)
        {
            C2DLine Line12 = new C2DLine(pt1, pt2);
            C2DLine Line23 = new C2DLine(pt2, pt3);
            C2DLine Line31 = new C2DLine(pt3, pt1);

            double dAng2 = Constants.conPI - Line12.vector.AngleBetween(Line23.vector);
            if (dAng2 >= Constants.conTWOTHIRDPI) // greater than 120 degrees
            {
	            return new C2DPoint(pt2);
            }
            else if (dAng2 < Constants.conTHIRDPI)  // if less than 60 then 1 of the other 2 could be greater than 120
            {
                double dAng3 = Constants.conPI - Line23.vector.AngleBetween(Line31.vector);
	            if (dAng3 >= Constants.conTWOTHIRDPI) // greater than 120 degrees
	            {
		            return new C2DPoint(pt3);
	            }
                else if ((Constants.conPI - dAng2 - dAng3) >= Constants.conTWOTHIRDPI) // if least angle is greater than 120
	            {
		            return new C2DPoint(pt1);
	            }
            }

            bool bClockwise = Line12.IsOnRight(pt3);

            if (bClockwise)
            {
	            Line12.vector.TurnLeft(Constants.conTHIRDPI);		// 60 degrees
	            Line23.vector.TurnLeft(Constants.conTHIRDPI);		// 60 degrees
            }
            else
            {
	            Line12.vector.TurnRight(Constants.conTHIRDPI);	// 60 degrees
	            Line23.vector.TurnRight(Constants.conTHIRDPI);	// 60 degrees
            }
        	
            Line12.SetPointFrom(pt3);
            Line23.SetPointFrom(pt1);	
        	
            List<C2DPoint> IntPt = new List<C2DPoint>();
            bool B1 = true, B2 = true;
            if (Line12.Crosses(Line23,  IntPt, ref B1, ref B2, false))
            {
	            return IntPt[0];
            }
            else
            {
	            Debug.Assert(false);
            }	

            return 	new C2DPoint(0, 0);
        }

        /// <summary>
        /// Static version of InCentre function.
        /// </summary>
        public static C2DPoint GetInCentre(C2DPoint pt1, C2DPoint pt2, C2DPoint pt3)
        {
	        // Set up a line to bisect the lines from 1 to 2 and 1 to 3
	        C2DLine Line1 = new C2DLine(pt1, pt2);
	        C2DLine Line2 = new C2DLine(pt1, pt3);
	        Line1.SetLength( Line2.GetLength() );
	        C2DLine Line12Bisect = new C2DLine(  pt1, pt3.GetMidPoint( Line1.GetPointTo()));

	        // Set up a line to bisect the lines from 2 to 1 and 2 to 3
	        C2DLine Line3 = new C2DLine(pt2, pt1);
	        C2DLine Line4 = new C2DLine(pt2, pt3);
	        Line3.SetLength( Line4.GetLength() );
            C2DLine Line34Bisect = new C2DLine(pt2, pt3.GetMidPoint(Line3.GetPointTo()));

	        // Now intersect the 2 lines and find the point.
	        List<C2DPoint> Int = new List<C2DPoint>();

	        // Add the intersection even if there isn't one (i.e. infinite lines)
            bool B1 = true, B2 = true;
	        Line12Bisect.Crosses(Line34Bisect,  Int, ref B1, ref B2, true);

	        Debug.Assert (Int.Count == 1);

	        return Int[0];
        }

        /// <summary>
        /// Static collinear.
        /// </summary>
	    public static bool Collinear(C2DPoint pt1, C2DPoint pt2, C2DPoint pt3)
        {
	        return ( C2DTriangle.GetAreaSigned(pt1, pt2, pt3) == 0);
        }

        /// <summary>
        /// Projects this onto the line given.
        /// </summary>
        /// <param name="Line">Line to project on.</param> 
        /// <param name="Interval">Ouput. Projection.</param> 
        public override void Project(C2DLine Line, CInterval Interval)
        {
            P1.Project(Line,  Interval);
            Interval.ExpandToInclude(P2.Project(Line));
            Interval.ExpandToInclude(P3.Project(Line));
        }

        /// <summary>
        /// Projection onto the Vector.
        /// </summary>
        /// <param name="Vector">Vector to project on.</param> 
        /// <param name="Interval">Ouput. Projection.</param> 
        public override void Project(C2DVector Vector, CInterval Interval)
        {
            P1.Project(Vector,  Interval);
            Interval.ExpandToInclude(P2.Project(Vector));
            Interval.ExpandToInclude(P3.Project(Vector));
        }


        /// <summary>
        /// Snaps this to the conceptual grid.
        /// </summary>
        /// <param name="grid">Grid to snap to.</param> 
        public override void SnapToGrid(CGrid grid)
        {
            P1.SnapToGrid(grid);
            P2.SnapToGrid(grid);
            P3.SnapToGrid(grid);
        }

        /// <summary>
	    /// Point 1.
        /// </summary>
	    public C2DPoint p1 = new C2DPoint();

        /// <summary>
        /// Point 1.
        /// </summary>
        public C2DPoint P1
        {
            get
            {
                return p1;
            }

        }

	    /// Point 2.
	    public C2DPoint p2 = new C2DPoint();

        /// <summary>
        /// Point 2.
        /// </summary>
        public C2DPoint P2
        {
            get
            {
                return p2;
            }
        }

	    /// Point 3.
	    public C2DPoint p3 = new C2DPoint();

        /// <summary>
        /// Point 3.
        /// </summary>
        public C2DPoint P3
        {
            get
            {
                return p3;
            }

        }
    }
}
