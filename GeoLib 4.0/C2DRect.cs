using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{

    /// <summary>
    /// Class to represent a 2D rectangle.
    /// </summary>
    public class C2DRect : C2DBase
    {

        /// <summary>
        /// Constructor.
        /// </summary>
	    public C2DRect() {}

        /// <summary>
        /// Destructor.
        /// </summary>
	    ~C2DRect() {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">The other rect.</param>   
        public C2DRect(C2DRect Other)
        {
            TopLeft.Set(Other.TopLeft);
            BottomRight.Set(Other.BottomRight);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ptTopLeft">The top left point.</param>  
        /// <param name="ptBottomRight">The bottom right point.</param>  
	    public C2DRect(C2DPoint ptTopLeft, C2DPoint ptBottomRight)
        {
            TopLeft.Set(ptTopLeft);
            BottomRight.Set(ptBottomRight);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dLeft">Left.</param>  
        /// <param name="dTop">Top.</param>  
        /// <param name="dRight">Right.</param>  
        /// <param name="dBottom">Bottom.</param>  
	    public C2DRect(double dLeft, double dTop, double dRight, double dBottom)
        {
            TopLeft.x = dLeft;
            TopLeft.y = dTop;

            BottomRight.x = dRight;
            BottomRight.y = dBottom;
        }

        /// <summary>
        /// Constructor sets both the top left and bottom right to equal the rect.
        /// </summary>
        /// <param name="pt">Point.</param>  
	    public C2DRect(C2DPoint pt )
        {
            TopLeft.Set(pt);
            BottomRight.Set(pt);
        }

        /// <summary>
        /// Sets both the top left and bottom right to equal the rect.
        /// </summary>
        /// <param name="pt">Point.</param>  
	    public void Set( C2DPoint pt)
        {
            TopLeft.Set(pt);
            BottomRight.Set(pt);
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="ptTopLeft">The top left point.</param>  
        /// <param name="ptBottomRight">The bottom right point.</param>  
        public void Set(C2DPoint ptTopLeft, C2DPoint ptBottomRight)
        {
            TopLeft.Set(ptTopLeft);
            BottomRight.Set(ptBottomRight);
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="dLeft">Left.</param>  
        /// <param name="dTop">Top.</param>  
        /// <param name="dRight">Right.</param>  
        /// <param name="dBottom">Bottom.</param>  
        public void Set(double dLeft, double dTop, double dRight, double dBottom)
        {
            TopLeft.x = dLeft;
            TopLeft.y = dTop;

            BottomRight.x = dRight;
            BottomRight.y = dBottom;
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="dTop">Top.</param>  
	    public void SetTop(double dTop) 
        {
            TopLeft.y = dTop; 
        }


        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="dLeft">Left.</param>  
	    public void SetLeft(double dLeft) 
        {
            TopLeft.x = dLeft; 
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="dBottom">Bottom.</param>  
	    public void SetBottom(double dBottom) 
        {
            BottomRight.y = dBottom; 
        }


        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="dRight">Right.</param>  
	    public void SetRight(double dRight) 
        {
            BottomRight.x = dRight; 
        }

        /// <summary>
        /// Clears the rectangle.
        /// </summary>
        public void Clear()
        {
            TopLeft.x = 0;
            TopLeft.y = 0;
            BottomRight.x = 0;
            BottomRight.y = 0;
        }

        /// <summary>
        /// Expands to include the point.
        /// </summary>
        /// <param name="NewPt">Point.</param> 
	    public void ExpandToInclude(C2DPoint NewPt)
        {
            if (NewPt.x > BottomRight.x) 
                BottomRight.x = NewPt.x;
            else if (NewPt.x < TopLeft.x) 
                TopLeft.x = NewPt.x;
            if (NewPt.y > TopLeft.y) 
                TopLeft.y = NewPt.y;
            else if (NewPt.y < BottomRight.y) 
                BottomRight.y = NewPt.y;
        }

        /// <summary>
        /// Expands to include the rectangle.
        /// </summary>
        /// <param name="Other">Rectangle.</param> 
	    public void ExpandToInclude(C2DRect Other)
        {
            ExpandToInclude(Other.TopLeft);
            ExpandToInclude(Other.BottomRight);
        }

        /// <summary>
        /// True if there is an overlap, returns the overlap.
        /// </summary>
        /// <param name="Other">Rectangle.</param> 
        /// <param name="Overlap">Output. The overlap.</param> 
        public bool Overlaps(C2DRect Other, C2DRect Overlap)
        {
            C2DPoint ptOvTL = new C2DPoint();
            C2DPoint ptOvBR = new C2DPoint();

            ptOvTL.y = Math.Min(TopLeft.y, Other.TopLeft.y);
            ptOvBR.y = Math.Max(BottomRight.y, Other.BottomRight.y);

            ptOvTL.x = Math.Max(TopLeft.x, Other.TopLeft.x);
            ptOvBR.x = Math.Min(BottomRight.x, Other.BottomRight.x);

            Overlap.Set(ptOvTL, ptOvBR);

            return Overlap.IsValid();
        }

        /// <summary>
        /// True if the point is within the rectangle.
        /// </summary>
        /// <param name="Pt">Point.</param> 
	    public bool Contains(C2DPoint Pt)
        {
            return (Pt.x >= TopLeft.x && Pt.x <= BottomRight.x &&
                     Pt.y <= TopLeft.y && Pt.y >= BottomRight.y);
        }


        /// <summary>
        /// True if the entire other rectangle is within.
        /// </summary>
        /// <param name="Other">Other rectangle.</param> 
	    public bool Contains(C2DRect Other)
        {
            return (Other.GetLeft() > TopLeft.x &&
                      Other.GetRight() < BottomRight.x &&
                      Other.GetBottom() > BottomRight.y &&
                      Other.GetTop() < TopLeft.y);
        }

        /// <summary>
        /// True if there is an overlap.
        /// </summary>
        /// <param name="Other">Other rectangle.</param> 
	    public bool Overlaps(C2DRect Other)
        {
            bool bOvX = !(Other.GetLeft() >= BottomRight.x ||
                          Other.GetRight() <= TopLeft.x);

            bool bOvY = !(Other.GetBottom() >= TopLeft.y ||
                          Other.GetTop() <= BottomRight.y);

            return bOvX && bOvY;
        }

        /// <summary>
        /// If the area is positive e.g. the top is greater than the bottom.
        /// </summary>
	    public bool IsValid()
        {
            return ((TopLeft.x < BottomRight.x) && (TopLeft.y > BottomRight.y));
        }

        /// <summary>
        /// Returns the area.
        /// </summary>
        public double GetArea()
        {
            return ((TopLeft.y - BottomRight.y) * (BottomRight.x - TopLeft.x));
        }

        /// <summary>
        /// Returns the width.
        /// </summary>
	    public double Width()
        {
            return (BottomRight.x - TopLeft.x);
        }

        /// <summary>
        /// Returns the height.
        /// </summary>
        public double Height()
        {
            return (TopLeft.y - BottomRight.y);
        }

        /// <summary>
        /// Returns the top.
        /// </summary>
	    public double GetTop( ) 
        {
            return  TopLeft.y;
        }

        /// <summary>
        /// Returns the left.
        /// </summary>
	    public double GetLeft( )   
        {
            return  TopLeft.x ;
        }


        /// <summary>
        /// Returns the bottom.
        /// </summary>
	    public double GetBottom( )  
        {
            return BottomRight.y;
        }

        /// <summary>
        /// Returns the right.
        /// </summary>
	    public double GetRight( )  
        {
            return BottomRight.x ;
        }

        /// <summary>
	    /// Assignment.
        /// </summary>
        /// <param name="Other">Other rectangle.</param> 
	    public void Set(C2DRect Other)
        {
            TopLeft.x = Other.TopLeft.x;
            TopLeft.y = Other.TopLeft.y;
            BottomRight.x = Other.BottomRight.x;
            BottomRight.y = Other.BottomRight.y;
        }

        /// <summary>
        /// Grows it from its centre.
        /// </summary>
        /// <param name="dFactor">Factor to grow by.</param> 
        public void Grow(double dFactor)
        {
            C2DPoint ptCentre = new C2DPoint(GetCentre());

            BottomRight.x = (BottomRight.x - ptCentre.x) * dFactor + ptCentre.x;
            BottomRight.y = (BottomRight.y - ptCentre.y) * dFactor + ptCentre.y;

            TopLeft.x = (TopLeft.x - ptCentre.x) * dFactor + ptCentre.x;
            TopLeft.y = (TopLeft.y - ptCentre.y) * dFactor + ptCentre.y;

        }

        /// <summary>
        /// Grow the height it from its centre.
        /// </summary>
        /// <param name="dFactor">Factor to grow by.</param> 
        public void GrowHeight(double dFactor)
        {
            C2DPoint ptCentre = new C2DPoint(GetCentre());
            BottomRight.y = (BottomRight.y - ptCentre.y) * dFactor + ptCentre.y;
            TopLeft.y = (TopLeft.y - ptCentre.y) * dFactor + ptCentre.y;

        }

        /// <summary>
        /// Grows the width from its centre.
        /// </summary>
        /// <param name="dFactor">Factor to grow by.</param> 
        public void GrowWidth(double dFactor)
        {
            C2DPoint ptCentre = new C2DPoint(GetCentre());
            BottomRight.x = (BottomRight.x - ptCentre.x) * dFactor + ptCentre.x;
            TopLeft.x = (TopLeft.x - ptCentre.x) * dFactor + ptCentre.x;

        }

        /// <summary>
        /// Expands from the centre by the fixed amount given.
        /// </summary>
        /// <param name="dRange">Amount to expand by.</param> 
        public void Expand(double dRange)
        {
            BottomRight.x += dRange;
            BottomRight.y -= dRange;

            TopLeft.x -= dRange;
            TopLeft.y += dRange;
        }

        /// <summary>
        /// Grows it from the given point.
        /// </summary>
        /// <param name="dFactor">Factor to grow by.</param> 
        /// <param name="Origin">The origin.</param> 
        public override void Grow(double dFactor, C2DPoint Origin)
        {
            BottomRight.Grow(dFactor, Origin);
            TopLeft.Grow(dFactor, Origin);
        }

        /// <summary>
        /// Moves this point by the vector given.
        /// </summary>
        /// <param name="Vector">The vector.</param>
        public override void Move(C2DVector Vector)
        {
            TopLeft.Move(Vector);
            BottomRight.Move(Vector);
        }

        /// <summary>
        /// Reflect throught the point given. 
        /// Switches Top Left and Bottom Right to maintain validity.
        /// </summary>
        /// <param name="Point">Reflection point.</param> 
        public override void Reflect(C2DPoint Point)
        {
            TopLeft.Reflect(Point);
            BottomRight.Reflect(Point);

            double x = TopLeft.x;
            double y = TopLeft.y;

            TopLeft.Set( BottomRight);
            BottomRight.x = x;
            BottomRight.y = y;
        }

        /// <summary>
        /// Reflect throught the line by reflecting the centre of the 
        /// rect and keeping the validity.
        /// </summary>
        /// <param name="Line">Reflection Line.</param> 
        public override void Reflect(C2DLine Line)
        {
	        C2DPoint ptCen = new C2DPoint(this.GetCentre());
	        C2DPoint ptNewCen = new C2DPoint(ptCen);
	        ptNewCen.Reflect(Line);
	        C2DVector vec = new C2DVector(ptCen, ptNewCen);
	        Move(vec);
        }

        /// <summary>
        /// Rotates this to the right about the origin provided.
        /// Note that as the horizontal/vertical line property will be
        /// preserved. If you rotate an object and its bounding box, the box may not still
        /// bound the object.
        /// </summary>
        /// <param name="dAng">The angle through which to rotate.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
        public override void RotateToRight(double dAng, C2DPoint Origin)
        {
            double dHalfWidth = Width() / 2;
            double dHalfHeight = Height() / 2;

            C2DPoint ptCen = new C2DPoint(GetCentre());
            ptCen.RotateToRight(dAng, Origin);

            TopLeft.x = ptCen.x - dHalfWidth;
            TopLeft.y = ptCen.y + dHalfHeight;
            BottomRight.x = ptCen.x + dHalfWidth;
            BottomRight.y = ptCen.y - dHalfHeight;
        }

        /// <summary>
        /// Returns the distance from this to the point. 0 if the point inside.
        /// </summary>
        /// <param name="TestPoint">Test Point.</param> 
        public override double Distance(C2DPoint TestPoint)
        {
            if (TestPoint.x > BottomRight.x) // To the east half
            {
                if (TestPoint.y > TopLeft.y)			// To the north east
                    return TestPoint.Distance(new C2DPoint(BottomRight.x, TopLeft.y));
                else if (TestPoint.y < BottomRight.y)		// To the south east
                    return TestPoint.Distance(BottomRight);
                else
                    return (TestPoint.x - BottomRight.x);	// To the east
            }
            else if (TestPoint.x < TopLeft.x)	// To the west half
            {
                if (TestPoint.y > TopLeft.y)			// To the north west
                    return TestPoint.Distance(TopLeft);
                else if (TestPoint.y < BottomRight.y)		// To the south west
                    return TestPoint.Distance(new C2DPoint(TopLeft.x, BottomRight.y));
                else
                    return (TopLeft.x - TestPoint.x);	// To the west
            }
            else
            {
                if (TestPoint.y > TopLeft.y)		//To the north
                    return (TestPoint.y - TopLeft.y);
                else if (TestPoint.y < BottomRight.y)	// To the south
                    return (BottomRight.y - TestPoint.y);
            }

          //  assert(Contains(TestPoint));
            return 0;	// Inside
        }

        /// <summary>
        /// Returns the distance from this to the other rect. 0 if there is an overlap.
        /// </summary>
        /// <param name="Other">Other rectangle.</param> 
       public double Distance(C2DRect Other)
       {
	        if (this.Overlaps(Other))
		        return 0;

	        if (Other.GetLeft() > this.BottomRight.x)
	        {
		        // Other is to the right
		        if (Other.GetBottom() > this.TopLeft.y)
		        {
			        // Other is to the top right
			        C2DPoint ptTopRight = new C2DPoint(BottomRight.x,  TopLeft.y);
			        return ptTopRight.Distance(new C2DPoint(Other.GetLeft(), Other.GetBottom()));
		        }
		        else if (Other.GetTop() < this.BottomRight.y)
		        {
			        // Other to the bottom right
			        return BottomRight.Distance( Other.TopLeft );
		        }
		        else
		        {
			        // to the right
			        return Other.GetLeft() - this.BottomRight.x;
		        }
	        }
	        else if ( Other.GetRight() < this.TopLeft.x)
	        {
		        // Other to the left
		        if (Other.GetBottom() > this.TopLeft.y)
		        {
			        // Other is to the top left
			        return  TopLeft.Distance(Other.BottomRight);
		        }
		        else if (Other.GetTop() < this.BottomRight.y)
		        {
			        // Other to the bottom left
			        C2DPoint ptBottomLeft = new C2DPoint(TopLeft.x, BottomRight.y);
			        return ptBottomLeft.Distance ( new C2DPoint( Other.GetRight(), Other.GetTop()));
		        }
		        else
		        {
			        //Just to the left
			        return (this.TopLeft.x - Other.GetRight());
		        }
	        }
	        else
	        {
		        // There is horizontal overlap;
		        if (Other.GetBottom() >  TopLeft.y)
			        return Other.GetBottom() -  TopLeft.y;
		        else
			        return BottomRight.y - Other.GetTop();
	        }		

        }

        /// <summary>
        /// Returns the bounding rectangle. (Required for virtual base class).
        /// </summary>
        /// <param name="Rect">Ouput. Bounding rectangle.</param> 
        public override void GetBoundingRect(C2DRect Rect) 
        { 
            Rect.Set(this);
        }

        /// <summary>
        /// Scales the rectangle accordingly.
        /// </summary>
	    public void Scale(C2DPoint ptScale) 
        {
            TopLeft.x =  TopLeft.x * ptScale.x;
            TopLeft.y = TopLeft.y * ptScale.y; 

		    BottomRight.x = BottomRight.x * ptScale.x;
            BottomRight.y = BottomRight.y * ptScale.y;
        }

        /// <summary>
        /// Returns the centre.
        /// </summary>
        public C2DPoint GetCentre()
        {
            return BottomRight.GetMidPoint(TopLeft);
        }

        /// <summary>
        /// Returns the point which is closest to the origin (0,0).
        /// </summary>
        public C2DPoint GetPointClosestToOrigin()
        {
            C2DPoint ptResult = new C2DPoint();
            if (Math.Abs(TopLeft.x) < Math.Abs(BottomRight.x))
            {
                // Left is closest to the origin.
                ptResult.x = TopLeft.x;
            }
            else
            {
                // Right is closest to the origin
                ptResult.x = BottomRight.x;
            }

            if (Math.Abs(TopLeft.y) < Math.Abs(BottomRight.y))
            {
                // Top is closest to the origin.
                ptResult.y = TopLeft.y;
            }
            else
            {
                // Bottom is closest to the origin
                ptResult.y = BottomRight.y;
            }

            return ptResult;
        }

        /// <summary>
        /// Returns the point which is furthest from the origin (0,0).
        /// </summary>
        public C2DPoint GetPointFurthestFromOrigin()
        {
            C2DPoint ptResult = new C2DPoint();
            if (Math.Abs(TopLeft.x) > Math.Abs(BottomRight.x))
            {
                // Left is furthest to the origin.
                ptResult.x = TopLeft.x;
            }
            else
            {
                // Right is furthest to the origin
                ptResult.x = BottomRight.x;
            }

            if (Math.Abs(TopLeft.y) > Math.Abs(BottomRight.y))
            {
                // Top is furthest to the origin.
                ptResult.y = TopLeft.y;
            }
            else
            {
                // Bottom is furthest to the origin
                ptResult.y = BottomRight.y;
            }

            return ptResult;
        }

        /// <summary>
        /// Projection onto the line
        /// </summary>
        /// <param name="Line">Line to project on.</param> 
        /// <param name="Interval">Ouput. Projection.</param> 
        public override void Project(C2DLine Line,  CInterval Interval)
        {
	        this.TopLeft.Project( Line,  Interval);
	        Interval.ExpandToInclude( BottomRight.Project( Line));
	        C2DPoint TR = new C2DPoint( BottomRight.x,   TopLeft.y);
            C2DPoint BL = new C2DPoint( TopLeft.x, BottomRight.y);
	        Interval.ExpandToInclude( TR.Project( Line));
	        Interval.ExpandToInclude( BL.Project( Line));

        }

        /// <summary>
        /// Projection onto the Vector.
        /// </summary>
        /// <param name="Vector">Vector to project on.</param> 
        /// <param name="Interval">Ouput. Projection.</param> 
        public override void Project(C2DVector Vector,  CInterval Interval)
        {
	        this.TopLeft.Project( Vector,  Interval);
	        Interval.ExpandToInclude( BottomRight.Project( Vector));
	        C2DPoint TR = new C2DPoint( BottomRight.x,   TopLeft.y);
            C2DPoint BL = new C2DPoint(TopLeft.x, BottomRight.y);
	        Interval.ExpandToInclude( TR.Project( Vector));
	        Interval.ExpandToInclude( BL.Project( Vector));

        }

        /// <summary>
        /// Snaps this to the conceptual grid.
        /// </summary>
        /// <param name="grid">Grid to snap to.</param> 
        public override void SnapToGrid(CGrid grid)
        {
            TopLeft.SnapToGrid(grid);
            BottomRight.SnapToGrid(grid);

        }



        /// <summary>
        /// True if this is above or below the other
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public bool OverlapsVertically( C2DRect Other)
        {
	        return !(Other.GetLeft() >= BottomRight.x ||
				          Other.GetRight() <=  TopLeft.x);
        }


        /// <summary>
        /// True if this is above the other.
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public bool OverlapsAbove( C2DRect Other)
        {
	        if (Other.GetLeft() >= BottomRight.x ||
				          Other.GetRight() <=  TopLeft.x)
	        {
		        return false;
	        }
	        else 
	        {
		        return TopLeft.y > Other.GetBottom();
	        }
        }


        /// <summary>
        /// True if this is below the other.
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public bool OverlapsBelow( C2DRect Other)
        {
	        if (Other.GetLeft() >= BottomRight.x ||
				          Other.GetRight() <=  TopLeft.x)
	        {
		        return false;
	        }
	        else 
	        {
		        return BottomRight.y < Other.GetTop();
	        }
        }


        /// <summary>
        /// Top left.
        /// </summary>
        private C2DPoint topLeft = new C2DPoint();
        /// <summary>
        /// Top left.
        /// </summary>
        public C2DPoint TopLeft
        {
            get
            {
                return topLeft;
            }

        }
        /// <summary>
        /// Bottom right.
        /// </summary>
        private C2DPoint bottomRight = new C2DPoint();
        /// <summary>
        /// Bottom right.
        /// </summary>
        public C2DPoint BottomRight
        {
            get
            {
                return bottomRight;
            }
        }
    }
}
