

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace GeoLib
{
    /// <summary>
    /// Class to represent a 2 dimensional arc being a part of a circle.
    /// </summary>
    public class C2DArc : C2DLineBase
    {
        /// <summary>
        /// Contructor.
        /// </summary>
	    public C2DArc() {}

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="Other">Arc to which this will be assigned.</param>
	    public C2DArc(C2DArc Other)
        {
            Set(Other);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
	    ~C2DArc() {}

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="PtFrom">The point the arc is to go from.</param>
        /// <param name="PtTo">The point the arc is to go to.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
	    public C2DArc(C2DPoint PtFrom, C2DPoint PtTo, double dRadius, 
		    bool bCentreOnRight, bool bArcOnRight)
        {
            Line.Set(PtFrom, PtTo);
            Radius = dRadius;
            CentreOnRight = bCentreOnRight;
            ArcOnRight = bArcOnRight;
        }

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="PtFrom">The point the arc is to go from.</param>
        /// <param name="Vector">The vector defining the end point.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
	    public C2DArc(C2DPoint PtFrom, C2DVector Vector, double dRadius, 
		    bool bCentreOnRight, bool bArcOnRight)
        {
            Line.Set(PtFrom, Vector);
            Radius = dRadius;
            CentreOnRight = bCentreOnRight;
            ArcOnRight = bArcOnRight;
        }

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="ArcLine">The line defining the start and end point of the arc.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
        public C2DArc(C2DLine ArcLine, double dRadius, 
		    bool bCentreOnRight, bool bArcOnRight)
        {
            Line.Set(ArcLine);
            Radius = dRadius;
            CentreOnRight = bCentreOnRight;
            ArcOnRight = bArcOnRight;

        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="other">The arc to set this to.</param>
        public void Set(C2DArc other)
        {
            Line.Set(other.Line);
            Radius = other.Radius;
            CentreOnRight = other.CentreOnRight;
            ArcOnRight = other.ArcOnRight;

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
            Line.Set(PtFrom, PtTo);
            Radius = dRadius;
            CentreOnRight = bCentreOnRight;
            ArcOnRight = bArcOnRight;

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
            Line.Set(PtFrom, Vector);
            Radius = dRadius;
            CentreOnRight = bCentreOnRight;
            ArcOnRight = bArcOnRight;
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="ArcLine">The line defining the start and end point of the arc.</param>
        /// <param name="dRadius">The corresponding circles radius.</param>
        /// <param name="bCentreOnRight">Whether the centre is on the right.</param>
        /// <param name="bArcOnRight">Whether the arc is to the right of the line.</param>
        public void Set(C2DLine ArcLine, double dRadius, 
		    bool bCentreOnRight, bool bArcOnRight)
        {
            Line.Set(ArcLine);
            Radius = dRadius;
            CentreOnRight = bCentreOnRight;
            ArcOnRight = bArcOnRight;

        }

        /// <summary>
        /// Assignment given a straight line defining the end points and a point on the arc.
        /// </summary>
        /// <param name="ArcLine">The line defining the start and end point of the arc.</param>
        /// <param name="ptOnArc">A point on the arc.</param>
        public void Set(C2DLine ArcLine, C2DPoint ptOnArc)
        {
            Line.Set(ArcLine);
	        C2DPoint ptTo = new C2DPoint(Line.GetPointTo());

	        C2DCircle Circle = new C2DCircle();
	        Circle.SetCircumscribed( Line.point , ptTo,  ptOnArc) ;
	        Radius = Line.point.Distance( Circle.Centre );
	        ArcOnRight = Line.IsOnRight(ptOnArc);
	        CentreOnRight = Line.IsOnRight(Circle.Centre);
        }


        /// <summary>
        /// Creates a copy of this as a new object.
        /// </summary>
        public override C2DLineBase CreateCopy()
        {
            return new C2DArc(this);
        }

        /// <summary>
        /// Tests to see if the radius is large enough to connect the end points.
        /// </summary>
        public bool IsValid()
        {
	        return (Line.vector.GetLength() <= 2 * Radius);
        }

        /// <summary>
        /// Returns the corresponding circle's centre.
        /// </summary>
        public C2DPoint GetCircleCentre() 
        {
	        if (!IsValid() ) 
		        return new C2DPoint(0, 0);

	        C2DPoint MidPoint = new C2DPoint(Line.GetMidPoint());
	        double dMinToStart = MidPoint.Distance( Line.point);

	        double dMidToCentre = Math.Sqrt( Radius * Radius - dMinToStart * dMinToStart);

	        C2DVector MidToCentre = new C2DVector(Line.vector);
	        if ( CentreOnRight) 
                MidToCentre.TurnRight();
	        else 
                MidToCentre.TurnLeft();

	        MidToCentre.SetLength(dMidToCentre);

	        return (MidPoint.GetPointTo(MidToCentre));
        }

        /// <summary>
        /// Returns the length of the curve.
        /// </summary>
	    public override double GetLength() 
        {
	        if (CentreOnRight ^ ArcOnRight)
	        {
		        return( Radius * GetSegmentAngle());
	        }
	        else
	        {
		        return( Radius * (Constants.conTWOPI - GetSegmentAngle()) );
	        }
        }

        /// <summary>
        ///  Gets the bounding rectangle.	
        /// </summary>
        /// <param name="Rect">The bounding rectangle to recieve the result.</param>	
        public override void GetBoundingRect( C2DRect Rect)
        {
	        if (!IsValid()) 
                return;

	        C2DPoint CentrePoint = new C2DPoint( GetCircleCentre());
	        C2DPoint EndPoint = new C2DPoint( Line.GetPointTo());

	        // First set up the rect that bounds the 2 points then check for if the arc expands this.
	        Line.GetBoundingRect( Rect);
        	
	        // If the arc crosses the y axis..
	        if (  ( (Line.point.x - CentrePoint.x) * (EndPoint.x - CentrePoint.x) ) < 0 )
	        {
		        // if the +ve y axis..
		        if ( Line.GetMidPoint().y > CentrePoint.y)
		        {
			        if (CentreOnRight ^ ArcOnRight) 
			        {
				        Rect.SetTop(CentrePoint.y + Radius);

			        }
			        else
			        {
				        // If the segment is the "Big" bit....
				        Rect.SetBottom(CentrePoint.y - Radius);	
			        }
		        }
		        else // if the -ve y axis...
		        {
			        if (CentreOnRight ^ ArcOnRight)
			        {
				        Rect.SetBottom(CentrePoint.y - Radius);
			        }
			        else
			        {
				        // If the segment is th "Big" bit then...
				        Rect.SetTop(CentrePoint.y + Radius);
			        }
		        }
	        }
	        else if (!(CentreOnRight ^ ArcOnRight))
	        {
		        Rect.SetBottom(CentrePoint.y - Radius);	
		        Rect.SetTop(CentrePoint.y + Radius);
	        }

	        // If the arc crosses the x axis..
	        if (  ( (Line.point.y - CentrePoint.y) * (EndPoint.y - CentrePoint.y) ) < 0 )
	        {
		        // if the +ve x axis..
		        if ( Line.GetMidPoint().x > CentrePoint.x)
		        {
			        if (CentreOnRight ^ ArcOnRight)
			        {
				        Rect.SetRight(CentrePoint.x + Radius);
			        }
			        else
			        {
				        // If the segment is th "Big" bit then...
				        Rect.SetLeft (CentrePoint.x - Radius);
			        }
		        }
		        else // if the -ve x axis...
		        {
			        if (CentreOnRight ^ ArcOnRight)
			        {
				        Rect.SetLeft(CentrePoint.x - Radius);
			        }
			        else
			        {
				        // If the segment is th "Big" bit then...
				        Rect.SetRight(CentrePoint.x + Radius);
			        }
		        }
	        }
	        else if (!(CentreOnRight ^ ArcOnRight))
	        {
		        Rect.SetLeft(CentrePoint.x - Radius);
		        Rect.SetRight(CentrePoint.x + Radius);
	        }

        }

        /// <summary>
        /// Gets the angle of the minimum segment. Always +ve and less than PI. In radians.	
        /// </summary>
        public double GetSegmentAngle() 
        {
	        if (!IsValid()) 
                return 0;

	        return ( 2 * Math.Asin( (Line.vector.GetLength() / 2) / Radius));
        }


        /// <summary>
        /// Returns the first point as a new object.	
        /// </summary>
        public override C2DPoint GetPointFrom() 
        { 
            return new C2DPoint(Line.point) ;
        }

        /// <summary>
        /// Returns the second point as a new object.	
        /// </summary>
        public override C2DPoint GetPointTo() 
        { 
            return new C2DPoint(Line.GetPointTo()) ;
        }

        /// <summary>
        /// True if this crosses the line given as a base class.
        /// </summary>
        /// <param name="Other">The other line as an abstract base class.</param>
        /// <param name="IntersectionPts">The interection point list to recieve the result.</param>
        public override bool Crosses(C2DLineBase Other,  List<C2DPoint> IntersectionPts)
        {

	        if (Other is C2DLine)
	        {
		        return Crosses( Other as C2DLine,  IntersectionPts);
            }
            else if (Other is C2DArc)
            {
		        return Crosses( Other as C2DArc,  IntersectionPts);
            }
            else
            {
                Debug.Assert(false, "Invalid line type");
                return false;
	        }

        }

        /// <summary>
        /// True if this crosses the straight line.
        /// </summary>
        /// <param name="TestLine">The other line.</param>
        /// <param name="IntersectionPts">The interection point list to recieve the result.</param>
        public bool Crosses(C2DLine TestLine,  List<C2DPoint> IntersectionPts)
        {
	        List<C2DPoint> IntPtsTemp = new List<C2DPoint>();
	        C2DCircle TestCircle = new C2DCircle(GetCircleCentre(), Radius);
            if (TestCircle.Crosses(TestLine,  IntPtsTemp))
	        {
		        for (int i = IntPtsTemp.Count - 1 ; i >= 0  ; i--)
		        {
                    if (Line.IsOnRight(IntPtsTemp[i]) ^ ArcOnRight ||
                        IntPtsTemp[i].PointEqualTo( Line.point) ||
                        IntPtsTemp[i].PointEqualTo( Line.GetPointTo()))
			        {
				        IntPtsTemp.RemoveAt(i);
			        }

		        }

		        if (IntPtsTemp.Count == 0)
			        return false;
		        else
		        {
                    IntersectionPts.InsertRange(0,  IntPtsTemp);
                //    .( IntersectionPts );
			  //      IntersectionPts << IntPtsTemp;
			        return true;
		        }
	        }
	        else
	        {
		        return false;
	        }

        }

        /// <summary>
        /// True if this crosses a curved line.
        /// </summary>
        /// <param name="Other">The other arc.</param>
        /// <param name="IntersectionPts">The interection point list to recieve the result.</param>
        public bool Crosses(C2DArc Other,   List<C2DPoint> IntersectionPts) 
        {
	        C2DCircle TestCircleThis = new C2DCircle (GetCircleCentre(), Radius);
	        C2DCircle TestCircleOther= new C2DCircle (Other.GetCircleCentre(), Other.Radius);

	        List<C2DPoint> IntPtsTemp = new List<C2DPoint>();

	        if (TestCircleThis.Crosses(TestCircleOther,  IntPtsTemp))
	        {

		        for (int i = IntPtsTemp.Count - 1; i >= 0 ; i--)
		        {
			        if ((Line.IsOnRight(IntPtsTemp[i]) ^ ArcOnRight) ||
				        Other.Line.IsOnRight(IntPtsTemp[i]) ^ Other.ArcOnRight ||
				        IntPtsTemp[i].PointEqualTo( Line.point) ||
				        IntPtsTemp[i].PointEqualTo( Line.GetPointTo()) ||
				        IntPtsTemp[i].PointEqualTo( Other.GetPointFrom()) ||
				        IntPtsTemp[i].PointEqualTo( Other.GetPointTo()))
			        {
				        IntPtsTemp.RemoveAt(i);
			        }
		        }

		        if (IntPtsTemp.Count == 0)
			        return false;
		        else
		        {

                    IntersectionPts.InsertRange(0, IntPtsTemp);
                //    IntPtsTemp.CopyTo( IntersectionPts );

				//        (*IntersectionPts) << IntPtsTemp;
			        return true;
		        }
	        }
	        else
	        {
		        return false;
	        }


        }

        /// <summary>
        /// True if this crosses the ray given. The ray is an infinite line represented by a line.
        /// The line is assumed no to end.
        /// </summary>
        /// <param name="Ray">The ray.</param>
        /// <param name="IntersectionPts">The interection point list to recieve the result.</param>
        public bool CrossesRay(C2DLine Ray,   List<C2DPoint> IntersectionPts) 
        {
	        double dDist = Ray.point.Distance(GetCircleCentre());
	        C2DLine RayCopy = new C2DLine(Ray);
	        // Ensure the copy line will go through the circle if the ray would.
	        RayCopy.vector.SetLength((dDist + Radius) * 2);

	        return Crosses(RayCopy,  IntersectionPts);

        }

        /// <summary>
        /// Distance between this and the test point.
        /// </summary>
        /// <param name="TestPoint">The test point.</param>
        public override double Distance(C2DPoint TestPoint) 
        {
            C2DPoint pt = new C2DPoint();
	        return Distance(TestPoint,  pt);
        }

        /// <summary>
        /// Distance between this and the test point.
        /// </summary>
        /// <param name="TestPoint">The test point.</param>
        /// <param name="ptOnThis">The closest point on this to the given point as a returned value.</param>
        public override double Distance(C2DPoint TestPoint,  C2DPoint ptOnThis) 
        {
	        C2DPoint ptCen = new C2DPoint( GetCircleCentre());
	        C2DCircle Circle = new C2DCircle(ptCen, Radius);
	        C2DPoint ptOnCircle = new C2DPoint();
	        double dCircleDist = Circle.Distance(TestPoint,  ptOnCircle);

	        if (ArcOnRight ^ Line.IsOnRight(ptOnCircle))
	        {
		        // The closest point on the circle isn't on the curve
		        double d1 = TestPoint.Distance(Line.point);
		        double d2 = TestPoint.Distance(Line.GetPointTo());
        		
		        if (d1 < d2)
		        {
				    ptOnThis.Set(Line.point);
			        return d1;
		        }
		        else
		        {
			        ptOnThis.Set(Line.GetPointTo());
			        return d2;
		        }
	        }
	        else
	        {
		        // The closest point on the circle IS on the curve
		        ptOnThis.Set(ptOnCircle);
		        return Math.Abs(dCircleDist);
	        }


        }

        /// <summary>
        /// The distance between this and another arc.
        /// </summary>
        /// <param name="Other">The test point.</param>
        /// <param name="ptOnThis">The closest point on this to the other as a returned value.</param>
        /// <param name="ptOnOther">The closest point on the other to this as a returned value.</param>     
        public double Distance(C2DArc Other,  C2DPoint ptOnThis,  C2DPoint ptOnOther)
        {
	        List<C2DPoint> IntPts1 = new List<C2DPoint>();
	        List<C2DPoint> IntPts2 = new List<C2DPoint>();

	        C2DPoint ptThisCen = new C2DPoint( GetCircleCentre() );
	        C2DPoint ptOtherCen = new C2DPoint(Other.GetCircleCentre());

	        C2DCircle CircleThis = new C2DCircle( ptThisCen, Radius);
	        C2DCircle CircleOther = new C2DCircle( ptOtherCen, Other.Radius );

	        if (CircleThis.Crosses(  CircleOther ,  IntPts1 ) )
	        {
		        for (int i = 0; i < IntPts1.Count; i++)
		        {
			        if (  (Line.IsOnRight( IntPts1[i] ) == ArcOnRight ) &&
				          (Other.Line.IsOnRight( IntPts1[i] ) == Other.ArcOnRight )     )
			        {
					    ptOnThis.Set(IntPts1[i]);
					    ptOnOther.Set(IntPts1[i]);
				        return 0;
			        }
		        }

		        IntPts1.Clear();
	        }


	        C2DLine LineCenToOther = new C2DLine();
	        LineCenToOther.point = new C2DPoint(ptThisCen);
	        LineCenToOther.vector = new C2DVector(ptThisCen, ptOtherCen);
	        LineCenToOther.GrowFromCentre( Math.Max(Radius, Other.Radius) * 10);

	        double dMinDist = 1.7E308;
	        double dDist = 0;

	        if ( Crosses(LineCenToOther,  IntPts1) && Other.Crosses(LineCenToOther,  IntPts2))
	        {
		        for (int i = 0 ; i < IntPts1.Count; i++)
		        {
			        for (int j = 0 ; j < IntPts2.Count; j++)
			        {
				        dDist = IntPts1[i].Distance(IntPts2[j]);
				        if (dDist < dMinDist)
				        {
						    ptOnThis.Set( IntPts1[i]);
						    ptOnOther.Set( IntPts2[j]);

					        dMinDist = dDist;
				        }
			        }
		        }
	        }

	        C2DPoint ptOnThisTemp = new C2DPoint();
	        dDist = Distance(Other.GetPointFrom(),  ptOnThisTemp);
	        if (dDist < dMinDist)
	        {
			    ptOnThis.Set(ptOnThisTemp);
			    ptOnOther.Set(Other.GetPointFrom());

		        dMinDist = dDist;
	        }

	        dDist = Distance(Other.GetPointTo(),  ptOnThisTemp);
	        if (dDist < dMinDist)
	        {
			    ptOnThis.Set(ptOnThisTemp);
			    ptOnOther.Set(Other.GetPointTo());

		        dMinDist = dDist;
	        }

	        C2DPoint ptOnOtherTemp = new C2DPoint();
	        dDist = Other.Distance(GetPointFrom(),  ptOnOtherTemp);
	        if (dDist < dMinDist)
	        {
			    ptOnThis.Set( GetPointFrom());
			    ptOnOther.Set( ptOnOtherTemp);
		        dMinDist = dDist;
	        }

	        dDist = Other.Distance(GetPointTo(),  ptOnOtherTemp);
	        if (dDist < dMinDist)
	        {
			    ptOnThis.Set( GetPointTo());
			    ptOnOther.Set( ptOnOtherTemp);
		        dMinDist = dDist;
	        }

	        return dMinDist;

        }

        /// <summary>
        /// Distance between this and another straight line.
        /// </summary>
        /// <param name="TestLine">The test line.</param>
        /// <param name="ptOnThis">The closest point on this to the other as a returned value.</param>
        /// <param name="ptOnOther">The closest point on the other to this as a returned value.</param>   
        public double Distance(C2DLine TestLine,  C2DPoint ptOnThis,  C2DPoint ptOnOther) 
        {
	        C2DCircle Circle = new C2DCircle( GetCircleCentre(), Radius);

            double dCircDist = Circle.Distance(TestLine,  ptOnThis,  ptOnOther);
	        double dDist = 0;

            if (TestLine.IsOnRight(ptOnThis) ^ ArcOnRight)
	        {
		        // The point found isn't on this. 
		        // This means the 2 closest points cannot be ON both lines, we must have a end point as one.

                ptOnThis.Set(Line.point);
                dDist = TestLine.Distance(ptOnThis,  ptOnOther);

                C2DPoint ptThisTemp = new C2DPoint(Line.GetPointTo());
		        C2DPoint ptOtherTemp = new C2DPoint();
                double d2 = TestLine.Distance(ptThisTemp,  ptOtherTemp);
		        if (d2 < dDist)
		        {
			        dDist = d2;
                    ptOnThis.Set(ptThisTemp);
                    ptOnOther.Set(ptOtherTemp);
		        }
		        // If the line was outside the circle then stop here as no need to go any further.
		        // This is because the closest point on this must be one of the end points.
		        if (dCircDist < 0)
		        {
                    double d3 = Distance(TestLine.point,  ptThisTemp);
			        if (d3 < dDist)
			        {
				        dDist = d3;
                        ptOnThis.Set(ptThisTemp);
                        ptOnOther.Set(Line.point);
			        }
                    double d4 = Distance(TestLine.GetPointTo(),  ptThisTemp);
			        if (d4 < dDist)
			        {
				        dDist = d4;
                        ptOnThis.Set(ptThisTemp);
                        ptOnOther.Set(Line.GetPointTo());
			        }	
		        }
	        }
	        else
	        {
		        dDist = Math.Abs(dCircDist);
	        }

		//    ptOnThis.Set(ptThis);
		//    ptOnOther.Set(ptOther);

	        return dDist;


        }

        /// <summary>
        /// Returns the minimum distance from the other line to this providing closest points.
        /// </summary>
        /// <param name="Other">The test point.</param>
        /// <param name="ptOnThis">The closest point on this to the other as a returned value.</param>
        /// <param name="ptOnOther">The closest point on the other to this as a returned value.</param> 
	    public override double Distance(C2DLineBase Other,  C2DPoint ptOnThis ,  C2DPoint ptOnOther )
        {
            if (Other is C2DLine)
            {
                return Distance(Other as C2DLine,  ptOnThis,  ptOnOther);
            }
            else if (Other is C2DArc)
            {
                return Distance(Other as C2DArc,  ptOnThis,  ptOnOther);
            }
            else
            {
                Debug.Assert(false, "Invalid line type");
                return 0;
            }



       }

       /// <summary>
       /// Returns the projection of this onto the line provided, given as the interval on
       /// (or off) the line. Interval given as distance from the start of the line.
       /// </summary>
       /// <param name="TestLine">The projection line.</param>
       /// <param name="Interval">The interval to recieve the result.</param>
        public override void Project(C2DLine TestLine,  CInterval Interval) 
        {
	        C2DArc ThisCopy = new C2DArc(this);
            C2DLine LineCopy = new C2DLine(TestLine);

	        double dAng = LineCopy.vector.AngleFromNorth();

	        LineCopy.vector.TurnLeft(dAng);
	        ThisCopy.RotateToRight( -dAng, LineCopy.point);

	        C2DRect rect = new C2DRect();
	        ThisCopy.GetBoundingRect( rect);

            Interval.dMax = rect.GetTop() - LineCopy.point.y;
	        Interval.dMin = Interval.dMax;

            Interval.ExpandToInclude(rect.GetBottom() - LineCopy.point.y);


        }

        /// <summary>
        /// Returns the projection of this onto the vector provided, given as the interval on
        /// (or off) the vector. Interval given as distance from the start of the vector.
        /// The vector is equivalent to a line from (0, 0).
        /// </summary>
        /// <param name="Vector">The projection vector.</param>
        /// <param name="Interval">The interval to recieve the result.</param>
        public override void Project(C2DVector Vector,  CInterval Interval)
        {
	        C2DArc ThisCopy = new C2DArc(this);
	        C2DVector VecCopy = new C2DVector(Vector);

	        double dAng = VecCopy.AngleFromNorth();

	        VecCopy.TurnLeft(dAng);
	        ThisCopy.RotateToRight( -dAng, new C2DPoint(0, 0));

	        C2DRect rect = new C2DRect();
	        ThisCopy.GetBoundingRect( rect);

	        Interval.dMax = rect.GetTop() - VecCopy.j;
	        Interval.dMin = Interval.dMax;

	        Interval.ExpandToInclude( rect.GetBottom() - VecCopy.j );
        }

        /// <summary>
        /// Gets the point half way along the curve as a new object.
        /// </summary>
        public C2DPoint GetMidPoint() 
        {
	        Debug.Assert(IsValid(), "Invalid arc defined, cannot calculate midpoint");
	        // Make a line from the circle centre to the middle of the line
	        C2DPoint ptCentre = new C2DPoint(GetCircleCentre());

	        C2DPoint ptLineCentre = new C2DPoint(Line.GetMidPoint());

	        C2DLine CenToMid = new C2DLine(ptCentre, ptLineCentre);

	        if ( CenToMid.vector.i == 0 && CenToMid.vector.j == 0)
	        {
		        // The centre of the line is the same as the centre of the circle
		        // i.e. this arc is 180 degrees or half a circle.
		        CenToMid.Set(Line);
		        CenToMid.SetPointFrom( ptLineCentre );
		        if ( ArcOnRight )
			        CenToMid.vector.TurnRight();
		        else
			        CenToMid.vector.TurnLeft();
	        }
	        else
	        {
		        // extend it to the edge of the arc
		        CenToMid.SetLength( Radius );
		        // if the arc on the opposite side to the centre then reverse the line.
		        if ( ArcOnRight == CentreOnRight)
		        {
			        CenToMid.vector.Reverse();
		        }
	        }

	        return CenToMid.GetPointTo();

        }

        /// <summary>
        /// Gets the point on the curve determined by the factor as a new object.
        /// </summary>
        public C2DPoint GetPointOn(double dFactorFromStart) 
        {
	        Debug.Assert(IsValid(), "Invalid arc defined, function failure." );
	        // make 2 lines from the centre to the ends of the line
	        C2DPoint ptCentre = new C2DPoint(GetCircleCentre());

	        C2DLine CenToStart = new C2DLine(ptCentre, Line.point);

	        C2DLine CenToEnd = new C2DLine(ptCentre, Line.GetPointTo());

	        if ( !ArcOnRight)	// clockwise
	        {
		        // Find the angle from one to the other and muliply it by the factor
		        // before turning the line's vector by the result.
		        double dAngleToRight = CenToStart.vector.AngleToRight( CenToEnd.vector );
		        double dNewAngle = dAngleToRight* dFactorFromStart;
		        CenToStart.vector.TurnRight( dNewAngle );
		        return CenToStart.GetPointTo();
	        }
	        else	// anticlockwise
	        {
		        double dAngleToLeft = CenToStart.vector.AngleToLeft( CenToEnd.vector );
		        double dNewAngle = dAngleToLeft* dFactorFromStart;
		        CenToStart.vector.TurnLeft( dNewAngle );
		        return CenToStart.GetPointTo();
	        }
        }

        /// <summary>
	    /// Move by the vector given.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public override void Move(C2DVector vector) 
        {
            Line.Move(vector);
        }

        /// <summary>
        /// Rotates this to the right about the origin provided.
        /// </summary>
        /// <param name="dAng">The angle through which to rotate.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
        public override void RotateToRight(double dAng, C2DPoint Origin)  
		{
            Line.RotateToRight(dAng, Origin);
        }

        /// <summary>
	    /// Grow relative to the origin.
        /// </summary>
        /// <param name="dFactor">The factor to grow this by.</param>
        /// <param name="Origin">The origin about which this is to be grown.</param>  
        public override void Grow(double dFactor, C2DPoint Origin)
        {
        	Line.Grow(dFactor, Origin);
	        Radius *= dFactor;
        }

        /// <summary>
	    /// Reflect in the point.
        /// </summary>
        /// <param name="point">The point through which this will be reflected.</param> 
        public override void Reflect(C2DPoint point)
        {
	        Line.Reflect( point);
        }

        /// <summary>
	    /// Reflects throught the line provided.
        /// </summary>
        /// <param name="TestLine">The line through which this will be reflected.</param> 
        public override void Reflect(C2DLine TestLine)
        {
            Line.Reflect(TestLine);
	        ArcOnRight = !ArcOnRight;
	        CentreOnRight = !CentreOnRight;
        }

        /// <summary>
	    /// Reverses the direction.
        /// </summary>
        public override void ReverseDirection()
        {
	        Line.ReverseDirection();
	        ArcOnRight = !ArcOnRight;
	        CentreOnRight = !CentreOnRight;
        }

        /// <summary>
	    /// Returns the lines that go to make this up based on the set of points 
	    /// provided which are assumed to be on the line.
        /// </summary>
        /// <param name="PtsOnLine">The points defining how the line is to be split.</param> 
        /// <param name="LineSet">The line set to recieve the result.</param> 
        public override void GetSubLines(List<C2DPoint> PtsOnLine,  List<C2DLineBase> LineSet) 
        {
            
	        // if there are no points on the line to split on then add a copy of this and return.
	        int usPointsCount = PtsOnLine.Count;
	        if (usPointsCount == 0 )
	        {
		        LineSet.Add(new C2DArc(this));
		        return;
	        }
	        else
	        {
		        // Make a copy of the points for sorting.
		        C2DPointSet TempPts = new C2DPointSet();
		        TempPts.MakeCopy(PtsOnLine);

		        if (usPointsCount > 1) // They need sorting
		        {
			        // Make a line from the mid point of my line to the start
			        C2DLine CenToStart = new C2DLine( Line.GetMidPoint(), Line.point );
			        // Now sort the points according to the order in which they will be encountered
			        if (ArcOnRight)
				        TempPts.SortByAngleToLeft( CenToStart );
			        else
				        TempPts.SortByAngleToRight( CenToStart );
		        }

		        C2DPoint ptCentre = new C2DPoint(GetCircleCentre());

		        // Add the line from the start of this to the first.
		        C2DLine NewLine = new C2DLine( Line.point, TempPts[0] );
                LineSet.Add(new C2DArc(NewLine, Radius,
                                          NewLine.IsOnRight(ptCentre), ArcOnRight));

		        // Add all the sub lines.
		        for (int i = 1; i < usPointsCount; i++)
		        {
                    NewLine.Set(TempPts[i - 1], TempPts[i]);
                    LineSet.Add(new C2DArc(NewLine, Radius,
                                            NewLine.IsOnRight(ptCentre), ArcOnRight));
		        }
		        // Add the line from the last point on this to the end of this.
                NewLine.Set(TempPts[TempPts.Count - 1], Line.GetPointTo());
                LineSet.Add(new C2DArc(NewLine, Radius,
                                           NewLine.IsOnRight(ptCentre), ArcOnRight));
	        }
            
        }

        /// <summary>
        /// Snaps this to the conceptual grid.
        /// </summary>
        /// <param name="grid">The grid object to snap this to.</param> 
        public override void SnapToGrid(CGrid grid)
        {
            Line.SnapToGrid(grid);

            double dLength = Line.vector.GetLength();

            if (dLength > (2 * Radius))
            {
                Radius = dLength / 1.999999999999;	// To ensure errors in the right way.
            }
        }


        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
        public override void Transform(CTransformation pProject)
        {
            this.Line.Transform(pProject);
        }

        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
        public override void InverseTransform(CTransformation pProject)
        {
            this.Line.InverseTransform(pProject);
        }




        /// <summary>
        /// The radius.
        /// </summary>
	    public double Radius;
        /// <summary>
	    /// Whether the associated circle centre is to the right of the line.
        /// </summary>
	    public bool CentreOnRight;
        /// <summary>
	    /// Whether the arc is to the right of the line.
        /// </summary>
	    public bool ArcOnRight;
        /// <summary>
	    /// The straight line used to define the start and end points of the line.
        /// </summary> 
	    protected C2DLine _Line = new C2DLine();
        /// <summary>
        /// The straight line used to define the start and end points of the line.
        /// </summary> 
        public C2DLine Line
        {
            get
            {
                return _Line;
            }
        }

    }
}
