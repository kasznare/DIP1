using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace GeoLib
{
    /// <summary>
    /// Class to represent a 2D line as a point and a vector frm that point to a
    /// secnd point.
    /// </summary>
    public class C2DLine : C2DLineBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DLine() {}

        /// <summary>
        /// Destructor.
        /// </summary>
        ~C2DLine() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dPt1x">The x value of the first point.</param>
        /// <param name="dPt1y">The y value of the first point.</param>
        /// <param name="dPt2x">The x value of the second point.</param>
        /// <param name="dPt2y">The y value of the second point.</param>
        public C2DLine(double dPt1x, double dPt1y, double dPt2x, double dPt2y)
        {
            point.Set(dPt1x, dPt1y);
            vector.Set(dPt2x - dPt1x, dPt2y - dPt1y);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="PointFrom">The point from.</param>
        /// <param name="VectorTo">The vector defining the second point.</param>
	    public C2DLine(C2DPoint PointFrom, C2DVector VectorTo)
        {
            point.Set(PointFrom);
            vector.Set(VectorTo);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="PointFrom">The point from.</param>
        /// <param name="PointTo">The point to.</param>
	    public C2DLine(C2DPoint PointFrom, C2DPoint PointTo)
        {
            point.Set(PointFrom);
            vector.Set(PointFrom, PointTo);

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">The other line.</param>
	    public C2DLine(C2DLine Other)
        {
            point.Set(Other.point);
            vector.Set(Other.vector);
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Other">The other line.</param>
        public void Set(C2DLine Other)
        {
            point.Set(Other.point);
            vector.Set(Other.vector);
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="PointFrom">The point from.</param>
        /// <param name="PointTo">The point to.</param>
	    public void Set(C2DPoint PointFrom, C2DPoint PointTo)
        {
            point.Set(PointFrom);
            vector.Set(PointFrom, PointTo);
        }
        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="PointFrom">The point from.</param>
        /// <param name="VectorTo">The vector defining the second point.</param>
	    public void Set(C2DPoint PointFrom, C2DVector VectorTo)
        {
            point.Set(PointFrom);
            vector.Set(VectorTo);
        }

        /// <summary>
        /// Sets the point that this is going to.
        /// </summary>
        /// <param name="PointTo">The point to go to.</param>
	    public void SetPointTo(C2DPoint PointTo)
        {
	        vector.Set(PointTo - point);
        }

        /// <summary>
        /// Sets the point that this is going to. The second point is unchanged.
        /// </summary>
        /// <param name="PointFrom">The point to go from.</param>
	    public void SetPointFrom( C2DPoint PointFrom)
        {
	        // Get point to.
	        C2DPoint PointTo = new C2DPoint(point.x + vector.i, point.y + vector.j);	
	        // Set the point from 
	        point.Set(PointFrom);
	        // Rest the point to.
	        vector.Set(PointTo - point);
        }

        /// <summary>
        /// Sets the length. The first point is unchanged.
        /// </summary>
        /// <param name="dLength">The new length.</param>
	    public void SetLength (double dLength) 
        {
            vector.SetLength(dLength);
        }

        /// <summary>
        /// Creates a copy of this.
        /// </summary>
        public override C2DLineBase CreateCopy()
        {
            return new C2DLine(this);
        }

        /// <summary>
        /// Reverses the direction of the line.
        /// </summary>
        public override void ReverseDirection()
        {
	        point.Set(GetPointTo());
	        vector.Reverse();
        }

        /// <summary>
        /// True if the point is to the right of the line.
        /// </summary>
        /// <param name="OtherPoint">The new point to test.</param>
	    public bool IsOnRight(C2DPoint OtherPoint)
        {
	        return ( C2DTriangle.GetAreaSigned( point, GetPointTo(), OtherPoint) < 0);
        }

        /// <summary>
        /// Returns the second point as a new object.
        /// </summary>
        public override C2DPoint GetPointTo()
        {
	        C2DPoint PointTo = new C2DPoint(point.x + vector.i, point.y + vector.j);
	        return PointTo;
        }

        /// <summary>
        /// Returns the first point as a new object.
        /// </summary>
	    public override C2DPoint GetPointFrom() 
        {
            return new C2DPoint(point);
        }

        /// <summary>
        /// True if this line would cross the other if this were infinite.
        /// </summary>
        /// <param name="Other">The other line to test.</param>
	    public bool WouldCross(C2DLine Other)
        {
	        bool bPointOnRight = IsOnRight(Other.point);
	        bool bPointToOnRight = IsOnRight(Other.GetPointTo());

	        return (bPointOnRight^bPointToOnRight);
        }

        /// <summary>
        /// True if this line crosses the other line, returns the intersection pt.
        /// </summary>
        /// <param name="Other">The other line to test.</param>
        /// <param name="IntersectionPts">Output. The intersection points.</param>
	    public override bool Crosses(C2DLineBase Other,  List<C2DPoint> IntersectionPts)
        {
	        if (Other is C2DLine)
	        {
		        return Crosses( Other as C2DLine,  IntersectionPts);
            }
            else if (Other is C2DArc)
            {
                C2DArc Arc = Other as C2DArc;
                return Arc.Crosses(this,  IntersectionPts);
            }
            else
            {
                Debug.Assert(false, "Invalid Line type");
                return false;
            }
        }

        /// <summary>
        /// True if this line crosses the other line, returns the intersection pt.
        /// </summary>
        /// <param name="Other">The other line to test.</param>
        /// <param name="IntersectionPts">Output. The intersection points.</param>
        public bool Crosses(C2DLine Other,  List<C2DPoint> IntersectionPts)
        {
            bool bOnThis = true;
            bool bOnOther = true;

            return Crosses(Other,  IntersectionPts, ref bOnThis, ref bOnOther, false);
        }

	    
        /// <summary>
        /// True if this line crosses the other. Returns the point is a collection is provided.
        /// Returns whether it would cross on this or on the other. Can opt to get the point 
        /// where the cross would occur (if not parallel) even if they don't cross.
        /// </summary>
        /// <param name="Other">The other line</param>
        /// <param name="IntersectionPts">To recieve the result</param>
        /// <param name="bOnThis">Output. True is the intersection would be on this line.</param>
        /// <param name="bOnOther">Output. True is the intersection would be on the other line.</param>
        /// <param name="bAddPtIfFalse">Input. True to add the intersection point even if there is no intersection.</param>
        /// <returns></returns>
        public bool Crosses(C2DLine Other,  List<C2DPoint> IntersectionPts , 
		    ref bool bOnThis, ref bool bOnOther, bool bAddPtIfFalse)
        {
	        bOnThis = false;
	        bOnOther = false;

	        C2DPoint p1 = point;
	        C2DPoint p2 = GetPointTo();

	        C2DPoint p3 = Other.point;
	        C2DPoint p4 = Other.GetPointTo();

	        double Ua = (p4.x - p3.x)*(p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x);
	        double Ub = (p2.x - p1.x)*(p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x);

	        double dDenominator = (p4.y - p3.y)*(p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

	        if (dDenominator == 0) 
                return false;

	        Ua = Ua / dDenominator;
	        Ub = Ub / dDenominator;

	        bOnThis = (Ua >= 0 && Ua < 1);		// For ints we need the line to be the point set [a,b);
	        bOnOther = (Ub >= 0 && Ub < 1);		// For ints we need the line to be the point set [a,b);
	        bool bResult  = bOnThis && bOnOther;

	        if (bAddPtIfFalse || bResult)
	        {
		        IntersectionPts.Add(new C2DPoint(p1.x + Ua*(p2.x - p1.x) , p1.y + Ua*(p2.y - p1.y)));
	        }

	        return (bResult); 

        }


        /// <summary>
        /// Function to join the 2 lines at the point where they do / would intersect. If they do then
        /// the lines are clipped to remove the smallest part of the line. Returns false if they
        /// cannot be joined.
        /// </summary>
        /// <param name="Other">The other line</param>
        public bool Join(C2DLine Other)
        {
            C2DPoint p1 = point;
            C2DPoint p2 = GetPointTo();

            C2DPoint p3 = Other.point;
            C2DPoint p4 = Other.GetPointTo();

            double Ua = (p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x);
            double Ub = (p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x);

            double dDenominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

            if (dDenominator == 0)
                return false;

            Ua = Ua / dDenominator;
            Ub = Ub / dDenominator;

            C2DPoint IntPt = new C2DPoint(p1.x + Ua * (p2.x - p1.x), p1.y + Ua * (p2.y - p1.y));
            if ( Ua >=0.5)
                this.SetPointTo( IntPt );
            else
                this.SetPointFrom( IntPt );

            if ( Ub >=0.5)
                Other.SetPointTo( IntPt );
            else
                Other.SetPointFrom( IntPt );

            return true;
        }

        /// <summary>
        /// True if the ray provided (infinite line starting from the first point) crosses this. 
        /// </summary>
        /// <param name="Ray">The other line to test.</param>
        /// <param name="IntersectionPts">Output. The intersection points.</param>
	    public bool CrossesRay(C2DLine Ray,  List<C2DPoint> IntersectionPts)
        {
	        C2DPoint p1 = point;
	        C2DPoint p2 = GetPointTo();

	        C2DPoint p3 = Ray.point;
	        C2DPoint p4 = Ray.GetPointTo();

	        double Ua = (p4.x - p3.x)*(p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x);
	        double Ub = (p2.x - p1.x)*(p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x);

	        double dDenominator = (p4.y - p3.y)*(p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

	        if (dDenominator == 0) 
                return false;

	        Ua = Ua / dDenominator;
	        Ub = Ub / dDenominator;

	        bool bResult = (Ua >= 0 && Ua <= 1) && (Ub >= 0);

	        if (bResult)
	        {
		        IntersectionPts.Add(new C2DPoint(p1.x + Ua*(p2.x - p1.x) , p1.y + Ua*(p2.y - p1.y)));
	        }

	        return bResult;
        }

        /// <summary>
        /// Returns the distance from this to the point. 
        /// </summary>
        /// <param name="TestPoint">The test point.</param>
	    public override double Distance(C2DPoint TestPoint)
        {
            C2DPoint P1 = new C2DPoint();
            return Distance(TestPoint,  P1);
        }

        /// <summary>
        /// Returns the distance from this to the point. 
        /// </summary>
        /// <param name="TestPoint">The test pointt.</param>
        /// <param name="ptOnThis">Output. The closest point on this.</param>
        public override double Distance(C2DPoint TestPoint,  C2DPoint ptOnThis)
        {
	        C2DVector vP1ToPoint = new C2DVector(point, TestPoint);
	        double dLength = GetLength();
	        double dProjLength = vP1ToPoint.Dot(vector);

	        if (dProjLength < 0)
	        {
			    ptOnThis.Set(point);

		        return TestPoint.Distance(point);
	        }
	        else
	        {
		        dProjLength = dProjLength / dLength;
		        if (dProjLength < dLength)
		        {
			        // The projection is on the line
			        double dFactorOnLine = dProjLength / dLength;
			        C2DPoint PtOnLine = new C2DPoint(point.x + vector.i * dFactorOnLine, 
							          point.y + vector.j * dFactorOnLine);
			        ptOnThis.Set(PtOnLine);
			        return TestPoint.Distance(PtOnLine);
		        }
		        else
		        {
			        ptOnThis .Set(GetPointTo());

			        return TestPoint.Distance( GetPointTo() );
		        }
	        }
        }

        /// <summary>
        /// Returns the distance from this to the other line.
        /// </summary>
        /// <param name="Other">The Other line.</param>
        /// <param name="ptOnThis">Output. The closest point on this.</param>
        /// <param name="ptOnOther">Output. The closest point on the other line.</param>
	    public double Distance(C2DLine Other,  C2DPoint ptOnThis ,  C2DPoint ptOnOther) 
        {
            // First, project the other line onto this and if it falls entirely below it or
	        // above it then 1. There is no intersection, 2. This is closest to one end on this line.
	        C2DPoint ptOtherP2 = new C2DPoint(Other.GetPointTo());
	        C2DVector vThisP1OtherP1 = new C2DVector(point, Other.point);
	        C2DVector vThisP1OtherP2 = new C2DVector(point, ptOtherP2);
	        C2DPoint ptThisP2 = new C2DPoint(GetPointTo());

	        double dOtherP1Proj = vThisP1OtherP1.Dot(vector);
	        double dOtherP2Proj = vThisP1OtherP2.Dot(vector);
	        // If they are both less than 0 then the projection falls below the line.
	        if (dOtherP1Proj <= 0 && dOtherP2Proj <= 0)
	        {
			    ptOnThis.Set(point);
		        return Other.Distance(point,  ptOnOther);
	        }
	        // Now modify the projection so it is the length along this line.
	        double dThisLength = GetLength();
	        dOtherP1Proj = dOtherP1Proj / dThisLength;
	        dOtherP2Proj = dOtherP2Proj / dThisLength;
	        // If the projections are both above the line then the second point is closest
	        if (dOtherP1Proj >= dThisLength && dOtherP2Proj >= dThisLength)
	        {
		        ptOnThis.Set(ptThisP2);
		        return Other.Distance( ptThisP2,   ptOnOther);
	        }

	        // This hasn't worked so try the same on the other line.
	        C2DVector vOtherP1ThisP1 = new C2DVector (Other.point, point);
	        C2DVector vOtherP1ThisP2 = new C2DVector(Other.point, ptThisP2);

	        double dThisP1Proj = vOtherP1ThisP1.Dot(Other.vector);
	        double dThisP2Proj = vOtherP1ThisP2.Dot(Other.vector);
	        // If they are both less than 0 then the projection falls below the line.
	        if (dThisP1Proj <= 0 && dThisP2Proj <= 0)
	        {
			    ptOnOther.Set( Other.point);
		        return Distance(Other.point,  ptOnThis);
	        }
	        // Now modify the projection so it is the length along this line.
	        double dOtherLength = Other.GetLength();
	        dThisP1Proj = dThisP1Proj / dOtherLength;
	        dThisP2Proj = dThisP2Proj / dOtherLength;
	        // If the projections are both above the line then the second point is closest
	        if (dThisP1Proj >= dOtherLength && dThisP2Proj >= dOtherLength)
	        {
			    ptOnOther.Set(ptOtherP2);
		        return Distance( ptOtherP2,   ptOnThis);
	        }
	        // Now test for an intersection.
            List<C2DPoint> IntPoint = new List<C2DPoint>();
            bool B1 = true, B2 = true;
            if (this.Crosses(Other,  IntPoint,ref  B1, ref B2, false))
	        {
			    ptOnOther.Set(IntPoint[0]);
			    ptOnThis.Set(IntPoint[0]);
		        return 0;
	        }
	        // Otherwise, there MUST be a point projection on one of the lines otherwise both
	        // lines project on either side of each other which is impossible. 
	        // So find the distances to all these projections and take the minimum.
	        double dDist = 0;
	        double dMinDist = 0;
	        bool bSet = false;


            C2DPoint ptOnThisTemp = new C2DPoint();
            C2DPoint ptOnOtherTemp = new C2DPoint(); 
	        // Is the other lines first point projected on this?
	        if (dOtherP1Proj >= 0 && dOtherP1Proj <= dThisLength)
	        {
		        // If so find the point on this line and get distance to it.
		        double dFactor = dOtherP1Proj / dThisLength;
                ptOnThisTemp.Set(new C2DPoint(point.x + vector.i * dFactor,
								           point.y + vector.j * dFactor) );

                dMinDist = Other.point.Distance(ptOnThisTemp);
		        bSet = true;

			    ptOnOther.Set(Other.point);
                ptOnThis.Set(ptOnThisTemp);
	        }
            // Is the other lines second point projected onto this?
	        if (dOtherP2Proj >= 0 && dOtherP2Proj <= dThisLength)
	        {
		        // If so find the point on this and then the distance. Is it less?
		        double dFactor = dOtherP2Proj / dThisLength;
		        ptOnThisTemp.Set( new C2DPoint(point.x + vector.i * dFactor,
											           point.y + vector.j * dFactor) );

                dDist = ptOtherP2.Distance(ptOnThisTemp);
		        if (!bSet || dDist < dMinDist)
		        {
				    ptOnOther.Set(ptOtherP2);
                    ptOnThis.Set(ptOnThisTemp);
			        dMinDist = dDist;

			        bSet = true;
		        }
	        }
	        // Is the first point on this projected onto the other line?
	        if (dThisP1Proj >= 0 && dThisP1Proj <= dOtherLength)
	        {
		        // If so find the point and the distance. Is it less?
		        double dFactor = dThisP1Proj / dOtherLength;
		        ptOnOtherTemp.Set( new C2DPoint(Other.point.x + Other.vector.i * dFactor,
								         Other.point.y + Other.vector.j * dFactor));

                dDist = point.Distance(ptOnOtherTemp);
		        if (!bSet || dDist < dMinDist)
		        {
				    ptOnThis.Set(point);
                    ptOnOther.Set(ptOnOtherTemp);
			        dMinDist = dDist;

			        bSet = true;
		        }

	        }

	        // Is the second point on this projected onto the other line?
	        if (dThisP2Proj >= 0 && dThisP2Proj <= dOtherLength)
	        {
		        // If so find the point and the distance. Is it less?
		        double dFactor = dThisP2Proj / dOtherLength;

		        ptOnOtherTemp.Set( new C2DPoint(Other.point.x + Other.vector.i * dFactor,
											         Other.point.y + Other.vector.j * dFactor));

                dDist = ptThisP2.Distance(ptOnOtherTemp);
		        if (!bSet || dDist < dMinDist)
		        {
				    ptOnThis.Set(ptThisP2);
                    ptOnOther.Set(ptOnOtherTemp);
			        dMinDist = dDist;

			        bSet = true;
		        }
	        }

	        Debug.Assert( bSet );
	        // Now return the minimum distance
	        return dMinDist;

        }


        /// <summary>
        /// Returns the distance from this to the other line.
        /// </summary>
        /// <param name="Other">The other line.</param>
        /// <param name="ptOnThis">Output. The closest point on this.</param>
        /// <param name="ptOnOther">Output. The closest point on the other line.</param>
	    public override double Distance(C2DLineBase Other,  C2DPoint ptOnThis ,  C2DPoint ptOnOther)
        {
      	    if (Other is C2DLine)
            {
		        return Distance( Other as C2DLine,  ptOnThis,  ptOnOther);	
            }
      	    else if (Other is C2DArc)
            {
                C2DArc Arc = Other as C2DArc;

                return Arc.Distance(this,  ptOnThis,  ptOnOther);
	        }
            else
            {
                Debug.Assert(false, "Invalid Hole type");
                return 0;
            }
        }

        /// <summary>
        /// Gets the mid point on the line.
        /// </summary>
	    public C2DPoint GetMidPoint()
        {
	        C2DPoint Result = new C2DPoint(point.x + vector.i / 2, point.y + vector.j / 2);
	        return Result;
        }

        /// <summary>
        /// Gets the point on the line given by the factor. e.g. 0.5 = mid point.
        /// </summary>
        /// <param name="dFactorFromStart">The factor from the start.</param>
	    public C2DPoint GetPointOn(double dFactorFromStart)
        {
	        C2DVector vNew = new C2DVector(vector);
            vNew.Multiply( dFactorFromStart);

	        C2DPoint Result = new C2DPoint(point.x + vNew.i, point.y + vNew.j );
	        return Result;
        }

        /// <summary>
        /// Returns the bounding rectangle.
        /// </summary>
        /// <param name="Rect">Output. The bounding rectangle.</param>
        public override void GetBoundingRect( C2DRect Rect) 
        {
	        Rect.Set(point);
	        Rect.ExpandToInclude(GetPointTo());
        }

        /// <summary>
        /// Returns the length of the line.
        /// </summary>
        public override double GetLength() 
        {
            return vector.GetLength();
        }

        /// <summary>
        /// Moves this point by the vector given.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public override void Move(C2DVector vector) 
        {
            point.Move(vector);
        }
        /// <summary>
        /// Rotates this to the right about the origin provided.
        /// </summary>
        /// <param name="dAng">The angle through which to rotate.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
        public override void RotateToRight(double dAng, C2DPoint Origin)
        {
        	point.RotateToRight(dAng, Origin);
	        vector.TurnRight(dAng);
        }

        /// <summary>
        /// Grows the line about the origin.
        /// </summary>
        /// <param name="dFactor">The factor to grow by.</param>
        /// <param name="Origin">The origin about which to grow.</param>
        public override void Grow(double dFactor, C2DPoint Origin)
        {
	        C2DPoint pointTo = new C2DPoint(GetPointTo());
	        point.Grow(dFactor, Origin);
	        pointTo.Grow(dFactor, Origin);
	        SetPointTo(pointTo);
        }

        /// <summary>
        /// Point reflection.
        /// </summary>
        /// <param name="Point">The point through which to reflect this.</param>
        public override void Reflect(C2DPoint Point)
        {
	        C2DPoint pointTo = new C2DPoint(GetPointTo());
	        point.Reflect(Point);
	        pointTo.Reflect(Point);
	        SetPointTo(pointTo);
        }

        /// <summary>
        /// Point reflection.
        /// </summary>
        /// <param name="Line">The line through which to reflect this.</param>
        public override void Reflect(C2DLine Line)
        {
	        C2DPoint pointTo = new C2DPoint(GetPointTo());
	        point.Reflect(Line);
	        pointTo.Reflect(Line);
	        SetPointTo(pointTo);
        }

        /// <summary>
        /// Grows from the centre by the amount.
        /// </summary>
        /// <param name="dFactor">The factor to grow this by.</param>
        public void GrowFromCentre(double dFactor)
        {
	        point.x -= ( vector.i * dFactor - vector.i ) / 2;
	        point.y -= ( vector.j * dFactor - vector.j ) / 2;

	        this.vector.Multiply(dFactor);
        }

        /// <summary>
        /// Projection onto a line.
        /// </summary>
        /// <param name="TestLine">The line to project this on.</param>
        /// <param name="Interval">Output. The interval.</param>
        public override void Project(C2DLine TestLine,  CInterval Interval)
        {
            double dP1 = point.Project(TestLine);
	        Interval.dMax = dP1;
	        Interval.dMin = dP1;
            Interval.ExpandToInclude(GetPointTo().Project(TestLine));
        }

        /// <summary>
        /// Projection onto a vector.
        /// </summary>
        /// <param name="Vector">The vector to project this on.</param>
        /// <param name="Interval">Output. The interval.</param>
        public override void Project(C2DVector Vector,  CInterval Interval) 
        {
	        double dP1 = point.Project(Vector);
	        Interval.dMax = dP1;
	        Interval.dMin = dP1;
	        Interval.ExpandToInclude( GetPointTo().Project( Vector) );
        }

        /// <summary>
        /// Returns the lines that make up this defined by the points which are assumed
        /// to be on this line. i.e. splits the line up.
        /// </summary>
        /// <param name="PtsOnLine">The point set defining how this is to be broken up.</param>
        /// <param name="LineSet">Output. The sub lines.</param>
        public override void GetSubLines(List<C2DPoint> PtsOnLine,  List<C2DLineBase> LineSet)
        {
	        // if there are no points on the line to split on then add a copy of this and return.
	        int usPointsCount = PtsOnLine.Count;
	        if (usPointsCount == 0 )
	        {
		        LineSet.Add(new C2DLine(this));
	        }
	        else
	        {
		        C2DPointSet TempPts = new C2DPointSet();
		        TempPts.MakeCopy(PtsOnLine);

		        if (usPointsCount > 1) // They need sorting
		        {
			        // Now sort the points according to the order in which they will be encountered
			        TempPts.SortByDistance(point);
		        }

		        // Add the line from the start of this to the first.
		        LineSet.Add(new C2DLine(point, TempPts[0]  ));

		        // Add all the sub lines.
		        for (int i = 1; i < usPointsCount; i++)
			        LineSet.Add(new C2DLine(TempPts[i - 1], TempPts[i]));

		        // Add the line from the last point on this to the end of this.
                LineSet.Add(new C2DLine(TempPts[TempPts.Count - 1], GetPointTo()));
	        }

            Debug.Assert(LineSet.Count == (PtsOnLine.Count + 1));

        }

        /// <summary>
        /// Snaps this to the conceptual grid.
        /// </summary>
        /// <param name="grid">The grid to snap to.</param>
        public override void SnapToGrid(CGrid grid)
        {
            C2DPoint pt = GetPointTo();

            pt.SnapToGrid(grid);

            point.SnapToGrid(grid);
            SetPointTo(pt);
        }



        /// <summary>
        /// Given x, this returns y assuming the line is infinite.
        /// </summary>
        /// <param name="dx"></param>
        /// <returns></returns>
        public double GetY(double dx)
        {
	        if ( vector.i == 0)
	        {
		        Debug.Assert(false);
		        return 0;
	        }
	        double m = vector.j / vector.i;
	        return m * dx + point.y - m * point.x;
        }


        /// <summary>
        /// True if part of this line is above or below the other. Returns the point 
        /// on this and on the other.
        /// </summary>
        /// <param name="Other"></param>
        /// <param name="dVerticalDistance"></param>
        /// <param name="ptOnThis"></param>
        /// <param name="ptOnOther"></param>
        /// <returns></returns>
        public bool OverlapsVertically(C2DLine Other, ref double dVerticalDistance, 
									        C2DPoint ptOnThis, C2DPoint ptOnOther)
        {
	        // Get the 2 points for both lines
	        C2DPoint OtherTo = new C2DPoint(Other.point.x + Other.vector.i, Other.point.y + Other.vector.j);
	        C2DPoint ThisTo = new C2DPoint(point.x + vector.i, point.y + vector.j);
	        // Make an interval for both in the x plane
	        CInterval iThis = new CInterval( point.x, point.x);
	        iThis.ExpandToInclude( ThisTo.x );

	        CInterval iOther = new CInterval( Other.point.x, Other.point.x);
            iOther.ExpandToInclude(OtherTo.x);
	        // This is an interval for the overlap between the 2
	        CInterval iOverlap = new CInterval();
	        // If there is an overlap...
	        if (iThis.Overlaps(iOther, iOverlap))
	        {
		        double dThisYMin;
		        double dThisYMax;

		        double dOtherYMin;
		        double dOtherYMax;
		        // If the line is vertical then y at the x min / max can be set to the ends of the line.
		        if (vector.i == 0)
		        {
			        dThisYMin = point.y;
			        dThisYMax = ThisTo.y;
		        }
		        else	// otherwise, caluclate the y values at the interval ends
		        {
			        dThisYMin = GetY(iOverlap.dMin);
			        dThisYMax = GetY(iOverlap.dMax);
		        }
		        // Now do the same for the other line
		        if (Other.vector.i == 0)
		        {
			        dOtherYMin = Other.point.y;
			        dOtherYMax = OtherTo.y;
		        }
		        else
		        {
			        dOtherYMin = Other.GetY(iOverlap.dMin);
			        dOtherYMax = Other.GetY(iOverlap.dMax);
		        }
		        // Now find the distance between the 2 at the ends
		        double dDistMin = dOtherYMin - dThisYMin;
		        double dDistMax = dOtherYMax - dThisYMax;
		        // If they are both same sign then there is no intersection
		        if ( (dDistMin * dDistMax) > 0)
		        {
			        dDistMin = Math.Abs( dDistMin);
                    dDistMax = Math.Abs(dDistMax);
			        // find which one is smallest
			        if ( dDistMin > dDistMax)
			        {
				        dVerticalDistance = dDistMax;// distance at the max is smallest
				        ptOnThis.x = iOverlap.dMax;
				        ptOnThis.y = dThisYMax;
				        ptOnOther.x = iOverlap.dMax;
				        ptOnOther.y = dOtherYMax;
			        }
			        else
			        {
				        dVerticalDistance = dDistMin;// distance at the min is smallest
				        ptOnThis.x = iOverlap.dMin;
				        ptOnThis.y = dThisYMin;
				        ptOnOther.x = iOverlap.dMin;
				        ptOnOther.y = dOtherYMin;
			        }
			        return true;
		        }
		        else
		        {
			        // find the intersection.
			        dVerticalDistance = 0;
			        C2DPointSet pts = new C2DPointSet();
			        if(this.Crosses(Other, pts))
			        {
				        ptOnThis = pts[0];
				        ptOnOther = ptOnThis;
			        }
			        else
			        {
				        Debug.Assert(false);
			        }
		        }
        		
		        return true;
	        }
	        else
	        {
		        return false;
	        }

        }

        /// <summary>
        /// True if part of this line is above the other. Returns the point 
        /// on this and on the other.
        /// </summary>
        /// <param name="Other"></param>
        /// <param name="dVerticalDistance"></param>
        /// <param name="ptOnThis"></param>
        /// <param name="ptOnOther"></param>
        /// <returns></returns>
        public bool OverlapsAbove(C2DLine Other, ref double dVerticalDistance, 
									        C2DPoint ptOnThis, C2DPoint ptOnOther)
        {
	        // Get the 2 points for both lines
	        C2DPoint OtherTo = new C2DPoint(Other.point.x + Other.vector.i, Other.point.y + Other.vector.j);
	        C2DPoint ThisTo = new C2DPoint(point.x + vector.i, point.y + vector.j);
	        // Make an interval for both in the x plane
	        CInterval iThis = new CInterval( point.x, point.x);
	        iThis.ExpandToInclude( ThisTo.x );

	        CInterval iOther = new CInterval( Other.point.x, Other.point.x);
	        iOther.ExpandToInclude( OtherTo.x );
	        // This is an interval for the overlap between the 2
	        CInterval iOverlap = new CInterval();
	        // If there is an overlap...
	        if (iThis.Overlaps(iOther, iOverlap))
	        {
		        double dThisYMin;
		        double dThisYMax;

		        double dOtherYMin;
		        double dOtherYMax;
		        // If the line is vertical then y at the x min / max can be set to the ends of the line.
		        if (vector.i == 0)
		        {
			        dThisYMin = point.y;
			        dThisYMax = ThisTo.y;
		        }
		        else	// otherwise, caluclate the y values at the interval ends
		        {
			        dThisYMin = GetY(iOverlap.dMin);
			        dThisYMax = GetY(iOverlap.dMax);
		        }
		        // Now do the same for the other line
		        if (Other.vector.i == 0)
		        {
			        dOtherYMin = Other.point.y;
			        dOtherYMax = OtherTo.y;
		        }
		        else
		        {
			        dOtherYMin = Other.GetY(iOverlap.dMin);
			        dOtherYMax = Other.GetY(iOverlap.dMax);
		        }
        		
		        // Now find the distance between the 2 at the ends
		        double dDistMin = dThisYMin - dOtherYMin;
		        double dDistMax = dThisYMax - dOtherYMax;
		        // If they are both > 0 then no intersection
		        if ( (dDistMin > 0) && (dDistMax > 0))
		        {
			        dDistMin = Math.Abs( dDistMin);
                    dDistMax = Math.Abs(dDistMax);
			        // find which one is smallest
			        if ( dDistMin > dDistMax)
			        {	
				        dVerticalDistance = dDistMax;	// distance at the max is smallest
				        ptOnThis.x = iOverlap.dMax;
				        ptOnThis.y = dThisYMax;
				        ptOnOther.x = iOverlap.dMax;
				        ptOnOther.y = dOtherYMax;
			        }
			        else
			        {
				        dVerticalDistance = dDistMin;  // distance at the min is smallest
				        ptOnThis.x = iOverlap.dMin;
				        ptOnThis.y = dThisYMin;
				        ptOnOther.x = iOverlap.dMin;
				        ptOnOther.y = dOtherYMin;
			        }

			        return true;
		        }	
		        else if ( (dDistMin < 0) && (dDistMax < 0)) // This is below.
		        {
			        return false;
		        }
		        else
		        {
			        // find the intersection.
			        dVerticalDistance = 0;
			        C2DPointSet pts = new C2DPointSet();
			        if(this.Crosses(Other, pts))
			        {
				        ptOnThis = pts[0];
				        ptOnOther = ptOnThis;
			        }
			        else
			        {
				        Debug.Assert(false);
			        }
		        }
        	
		        return true;
	        }
	        else
	        {
		        return false;
	        }

        }

        /// <summary>
        /// Returns the distance from this to the point with this as a ray.
        /// </summary>
        /// <param name="TestPoint"></param>
        /// <returns></returns>
        public double DistanceAsRay(C2DPoint TestPoint)
        {
            C2DPoint ptOnThis = new C2DPoint();
            return DistanceAsRay(TestPoint, ptOnThis);
        }


        /// <summary>
        /// Returns the distance from this to the point with this as a ray.
        /// </summary>
        /// <param name="TestPoint"></param>
        /// <param name="ptOnThis"></param>
        /// <returns></returns>
        public double DistanceAsRay(C2DPoint TestPoint,  C2DPoint ptOnThis)
        {
	        C2DVector vP1ToPoint = new C2DVector(point, TestPoint);

	        // The projection is on the line
	        double dFactorOnLine = vP1ToPoint.Dot(vector) / (vector.i * vector.i + vector.j * vector.j);

            ptOnThis.Set(point.x + vector.i * dFactorOnLine,
                              point.y + vector.j * dFactorOnLine);
            return TestPoint.Distance(ptOnThis);
        }


        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
        public override void Transform(CTransformation pProject)
        {
	        C2DPoint pt2 = new C2DPoint(point.x + vector.i, point.y + vector.j);

	        pProject.Transform(point.x, point.y);
	        pProject.Transform(pt2.x, pt2.y);

	        vector.i = pt2.x - point.x;
	        vector.j = pt2.y - point.y;
        }

        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
        public override void InverseTransform(CTransformation pProject)
        {
	        C2DPoint pt2 = new C2DPoint(point.x + vector.i, point.y + vector.j);

	        pProject.InverseTransform(point.x, point.y);
	        pProject.InverseTransform(pt2.x, pt2.y);

	        vector.i = pt2.x - point.x;
	        vector.j = pt2.y - point.y;
        }


        /// The first point.
        public C2DPoint point = new C2DPoint();
        /// The vector to the second point.
        public C2DVector vector = new C2DVector();
    }
}
