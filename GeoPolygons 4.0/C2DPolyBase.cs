using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace GeoLib
{
    /// <summary>
    /// Class to represent a shape bounded by lines. Lines currently supported are 
    /// Straight and Arced lines. No holed permitted and should not self intersect.
    /// </summary>
    public class C2DPolyBase : C2DBase
    {

        /// <summary>
	    /// Constructor
        /// </summary> 
	    public C2DPolyBase() {}

        /// <summary>
	    /// Constructor sets from another
        /// </summary> 
        /// <param name="Other">The other polygon.</param> 
	    public C2DPolyBase(C2DPolyBase Other)
        {
        	Set(Other);
        }

        /// <summary>
	    /// Destructor
        /// </summary> 
	    ~C2DPolyBase() {}


        /// <summary>
        /// Assigment sets from another
        /// </summary> 
        /// <param name="Other">The other polygon.</param> 
        public void Set(C2DPolyBase Other)
        {
	        Clear();

	        Lines.MakeValueCopy(Other.Lines);

	        BoundingRect.Set(Other.BoundingRect);

	        for (int i = 0 ; i < Other.LineRects.Count ; i ++)
	        {
		        LineRects.Add(new C2DRect( Other.LineRects[i]) );
	        }
        }

        /// <summary>
        /// Creates directly from a set of lines by extracting them from the set provided.
        /// </summary> 
        /// <param name="NewLines">The line set.</param> 
	    public void CreateDirect( C2DLineBaseSet NewLines)
        {
            Lines.Clear();
            Lines.ExtractAllOf(NewLines);

            this.MakeLineRects();
            this.MakeBoundingRect();
        }

        /// <summary>
        /// Creates from the set of lines using copies.
        /// </summary> 
        /// <param name="Lines">The line set.</param> 
	    public void Create(C2DLineBaseSet Lines)
        {
            Lines.MakeValueCopy(Lines);

            this.MakeLineRects();
            this.MakeBoundingRect();
        }

        /// <summary>
        /// True if the point is in the shape.
        /// </summary> 
        /// <param name="pt">The point to test set.</param> 
	    public bool Contains(C2DPoint pt)
        {
	        if (!BoundingRect.Contains(pt))
		        return false;

	        C2DPointSet IntersectedPts = new C2DPointSet ();

	        C2DLine Ray = new C2DLine(pt, new C2DVector(BoundingRect.Width(), 0.000001)); // Make sure to leave

	        if (!this.Crosses(Ray,  IntersectedPts))
		        return false;
	        else
	        {
		        IntersectedPts.SortByDistance(Ray.point);
		        if ( IntersectedPts[0].PointEqualTo(pt))
		        {
			        // For integers, the pt can start On a line, meaning it's INSIDE, but the ray could cross again
			        // so just return true. Because the equality test is really a test for proximity, this leads to the
			        // possibility that a point could lie just outside the shape but be considered to be inside. This would
			        // only be a problem with very small shapes that are a very long way from the origin. E.g. a 1m2 object
			        // 1 million metres from the origin and a point 0.1mm away from the edge would give rise to a relative 
			        // difference of 0.0001 / 1000000 = 0.000000001 which would just be consider to be inside.
			        return true;
		        }
		        else
		        {
			        // Return true if the ray 
			        return (IntersectedPts.Count & (int)1) > 0;
		        }
	        }

        }


        /// <summary>
        /// True if it entirely contains the other.
        /// </summary> 
        /// <param name="Other">The other polygon.</param> 
        public bool Contains(C2DPolyBase Other)
        {
            if (Other.Lines.Count == 0)
                return false;

            if (!BoundingRect.Contains(Other.BoundingRect))
                return false;

            if (!Contains(Other.Lines[0].GetPointFrom()))
                return false;

            return !this.Crosses(Other);
        }


        /// <summary>
        /// True if it entirely contains the line.
        /// </summary> 
        /// <param name="Line">The line to test.</param> 
        public bool Contains(C2DLineBase Line)
        {
            if (!Contains(Line.GetPointFrom()))
                return false;

            C2DPointSet Pts = new C2DPointSet();

            return !Crosses(Line, Pts);
        }

        /// <summary>
        /// True if any of the lines cross.
        /// </summary> 
        public bool HasCrossingLines()
        {
            return Lines.HasCrossingLines();
        }

        /// <summary>
        /// Distance of the point from the shape. Returns -ve if inside.
        /// </summary> 
        /// <param name="pt">The point to test.</param> 
        public override double Distance(C2DPoint pt)
        {
	        if (Lines.Count == 0)
		        return 0;

	        double dResult = Lines[0].Distance(pt);
	        for (int i = 1; i < Lines.Count; i++)
	        {
		        double dDist = Lines[i].Distance(pt);
		        if (dDist < dResult)
			        dResult = dDist;
	        }

	        if (Contains(pt))
		        return -dResult;
	        else
		        return dResult;
        }

        /// <summary>
        /// Distance to the line.
        /// </summary> 
        /// <param name="Line">The line to test.</param> 
        public double Distance(C2DLineBase Line)
        {
	        if (Lines.Count == 0)
		        return 0;

            C2DPoint pt1 = new C2DPoint();
            C2DPoint pt2 = new C2DPoint();

            double dMin = Lines[0].Distance(Line, pt1, pt2);
	        double dDist;

	        for (int i = 1 ; i < Lines.Count; i++)
	        {
		        dDist = Lines[i].Distance(Line, pt1, pt2);
		        if (dDist == 0 )
			        return 0;
		        if (dDist < dMin)
			        dMin = dDist;
	        }

	        if ( Contains(Line.GetPointFrom()))
		        return -dMin;
	        else
		        return dMin;
        }

        /// <summary>
        /// Distance of the poly from the shape. 
        /// </summary> 
        /// <param name="Other">The other polygon test.</param> 
        /// <param name="ptOnThis">Output. The closest point on this.</param> 
        /// <param name="ptOnOther">The closest point on the other.</param> 
        public double Distance(C2DPolyBase Other, C2DPoint ptOnThis, C2DPoint ptOnOther)
        {
	        if (Lines.Count == 0)
		        return 0;

	        if (Other.Lines.Count == 0)
		        return 0;

	        if (Other.LineRects.Count != Other.Lines.Count)
		        return 0;

	        if (Lines.Count != LineRects.Count)
		        return 0;

	        // First we find the closest line rect to the other's bounding rectangle.
	        int usThisClosestLineGuess = 0;
	        C2DRect OtherBoundingRect = Other.BoundingRect;
	        double dClosestDist = LineRects[0].Distance(OtherBoundingRect);
	        for (int i = 1; i < LineRects.Count; i++)
	        {
		        double dDist = LineRects[i].Distance(OtherBoundingRect);
		        if (dDist < dClosestDist)
		        {
			        dClosestDist = dDist;
			        usThisClosestLineGuess = i;
		        }
	        }
	        // Now cycle through all the other poly's line rects to find the closest to the
	        // guessed at closest line on this.
	        int usOtherClosestLineGuess = 0;
	        dClosestDist = Other.LineRects[0].Distance(LineRects[usThisClosestLineGuess]);
	        for (int j = 1; j < Other.LineRects.Count; j++)
	        {
		        double dDist = Other.LineRects[j].Distance(LineRects[usThisClosestLineGuess]);
		        if (dDist < dClosestDist)
		        {
			        dClosestDist = dDist;
			        usOtherClosestLineGuess = j;
		        }
	        }

	        // Now we have a guess at the 2 closest lines.
	        double dMinDistGuess = Lines[usThisClosestLineGuess].Distance(
							        Other.Lines[usOtherClosestLineGuess],
							        ptOnThis,
							        ptOnOther);
	        // If its 0 then return 0.
	        if (dMinDistGuess == 0)
		        return 0;

	        C2DPoint ptOnThisTemp = new C2DPoint();
	        C2DPoint ptOnOtherTemp = new C2DPoint();

	        // Now go through all of our line rects and only check further if they are closer
	        // to the other's bounding rect than the min guess.
	        for (int i = 0; i < Lines.Count; i++)
	        {
		        if (LineRects[i].Distance( OtherBoundingRect ) <  dMinDistGuess)
		        {
			        for (  int j = 0 ; j < Other.Lines.Count ; j++)
			        {
				        double dDist = Lines[i].Distance(Other.Lines[j],
													        ptOnThisTemp,
													        ptOnOtherTemp);
        				
				        if (dDist < dMinDistGuess)
				        {	
						    ptOnThis.Set(ptOnThisTemp);
					        ptOnOther.Set(ptOnOtherTemp);

					        if (dDist == 0)
						        return 0;

					        dMinDistGuess = dDist; 
				        }
			        }
		        }
	        }

	        // if we are here, there is no intersection but the other could be inside this or vice-versa
	        if ( BoundingRect.Contains(Other.BoundingRect)
		        && Contains(ptOnOtherTemp)  )
	        {
		        dMinDistGuess *= -1.0;
	        }
	        else if ( Other.BoundingRect.Contains(BoundingRect)
		        && Other.Contains( ptOnThisTemp ))
	        {
		        dMinDistGuess *= -1.0;
	        }

	        return dMinDistGuess;
        }


        /// <summary>
        /// True if the point is with the range given to the shape or inside.
        /// </summary> 
        /// <param name="pt">The point to test.</param> 
        /// <param name="dRange">The range to test against.</param> 
        public bool IsWithinDistance(C2DPoint pt, double dRange)
        {
	        C2DRect RectTemp = new C2DRect(BoundingRect);
	        RectTemp.Expand(dRange);

	        if (!RectTemp.Contains(pt))
		        return false;

	        if (Lines.Count == 0)
		        return false;

	        if (Lines[0].GetPointFrom().Distance(pt) < dRange)
		        return true;

	        if (this.Contains(pt))
		        return true;

	        for (int i = 1; i < Lines.Count; i++)
	        {
		        if(Lines[i].Distance(pt) < dRange)
			        return true;
	        }

	        return false;
        }

        /// <summary>
        /// Returns the bounding rectangle. Can access directly as well.
        /// </summary> 
        /// <param name="Rect">Output. The bounding rectangle.</param> 
        public override void GetBoundingRect(C2DRect Rect) 
        {
            Rect.Set(BoundingRect); 
        }

        /// <summary>
        /// Calculates the perimeter.
        /// </summary> 
        public double GetPerimeter()
        {
	        double dResult = 0;
	        for (int i = 0; i < this.Lines.Count; i++)
	        {
		        dResult += Lines[i].GetLength();
	        }
	        return dResult;
        }

        /// <summary>
        /// Clears all.
        /// </summary> 
	    public void Clear()
        {
	        BoundingRect.Clear();
	        Lines.Clear();
	        LineRects.Clear();
        }

        /// <summary>
        /// Moves this point by the vector given.
        /// </summary>
        /// <param name="vector">The vector.</param>
	    public override void Move(C2DVector vector)
        {
	        Debug.Assert(Lines.Count == LineRects.Count);

	        if(Lines.Count != LineRects.Count)
		        return;

            for (int i = 0; i < Lines.Count; i++)
	        {
		        Lines[i].Move(vector);
		        LineRects[i].Move(vector);
	        }

	        BoundingRect.Move(vector);
        }
        /// <summary>
        /// Rotates this to the right about the origin provided.
        /// </summary>
        /// <param name="dAng">The angle in radians through which to rotate.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
        public override void RotateToRight(double dAng, C2DPoint Origin)
        {
            Debug.Assert(Lines.Count == LineRects.Count);

	        if(Lines.Count != LineRects.Count)
		        return;

	        for (int i = 0; i < Lines.Count; i++)
	        {
		        Lines[i].RotateToRight(dAng, Origin);
		        Lines[i].GetBoundingRect(LineRects[i]);		
	        }

	        MakeBoundingRect();

        }

        /// <summary>
        /// Grows the polygon around the origin.
        /// </summary>
        /// <param name="dFactor">The factor to grow by.</param>
        /// <param name="Origin">The origin about which to grow.</param>
        public override void Grow(double dFactor, C2DPoint Origin)
        {
	        Debug.Assert(Lines.Count == LineRects.Count);

	        if(Lines.Count != LineRects.Count)
		        return;

            for (int i = 0; i < Lines.Count; i++)
	        {
		        Lines[i].Grow(dFactor, Origin);
		        Lines[i].GetBoundingRect(LineRects[i]);		
	        }

	        BoundingRect.Grow(dFactor, Origin);

        }

        /// <summary>
        /// Reflects the area about the point.
        /// </summary>
        /// <param name="point">The point to reflect through.</param>
        public override void Reflect(C2DPoint point)
        {
	        Debug.Assert(Lines.Count == LineRects.Count);

	        if(Lines.Count != LineRects.Count)
		        return;

            for (int i = 0; i < Lines.Count; i++)
	        {
		        Lines[i].Reflect(point);
	        }
	        ReverseDirection(); // ALSO MAKES THE LINES AGAIN.

	        BoundingRect.Reflect(point);

        }

        /// <summary>
        /// Reflects throught the line provided.
        /// </summary>
        /// <param name="Line">The line to reflect through.</param>
        public override void Reflect(C2DLine Line)
        {
	        Debug.Assert(Lines.Count == LineRects.Count);

	        if(Lines.Count != LineRects.Count)
		        return;

            for (int i = 0; i < Lines.Count; i++)
	        {
		        Lines[i].Reflect(Line);
	        }
	        ReverseDirection(); // ALSO MAKES THE LINES AGAIN.

	        BoundingRect.Reflect(Line);


        }


        /// <summary>
        /// True if it crosses the other.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        public bool Crosses(C2DPolyBase Other)
        {
	        if (!BoundingRect.Overlaps(Other.BoundingRect))
		        return false;

            List<C2DPoint> Temp = new List<C2DPoint>();
	        for (int i = 0; i < Lines.Count; i++)
	        {
                if (Other.Crosses(Lines[i], Temp))
			        return true;
	        }
	        return false;

        }

        /// <summary>
        /// Intersection with another.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        /// <param name="IntersectionPts">Output. The intersection points.</param>
        public bool Crosses(C2DPolyBase Other, List<C2DPoint> IntersectionPts)
        {
            if (!BoundingRect.Overlaps(Other.BoundingRect))
                return false;

            List<C2DPoint> IntPtsTemp = new List<C2DPoint>();
            List<int> Index1 = new List<int>();
            List<int> Index2 = new List<int>();

            Lines.GetIntersections(Other.Lines, IntPtsTemp, Index1, Index2,
                                    BoundingRect, Other.BoundingRect);

            bool bResult = IntPtsTemp.Count > 0;

            IntersectionPts.InsertRange(0, IntPtsTemp);

            return bResult;
        }

        /// <summary>
        /// Intersection with a line base.
        /// </summary>
        /// <param name="Line">The other line.</param>
        public bool Crosses(C2DLineBase Line)
        {
	        C2DRect LineRect = new C2DRect ();
	        Line.GetBoundingRect(LineRect);

            List<C2DPoint> Temp = new List<C2DPoint>();

	        for (int i = 0; i < this.Lines.Count; i++)
	        {
		        if (LineRects[i].Overlaps( LineRect ) &&  Lines[i].Crosses(Line, Temp))
			        return true;
	        }
	        return false;

        }

        /// <summary>
        /// True if it crosses the line. Provides the intersection points.
        /// </summary>
        /// <param name="Line">The other line.</param>
        /// <param name="IntersectionPts">Output. The intersection points.</param>
        public bool Crosses(C2DLineBase Line, List<C2DPoint> IntersectionPts)
        {
	        C2DRect LineRect = new C2DRect();
	        Line.GetBoundingRect( LineRect);

	        if (!BoundingRect.Overlaps(LineRect))
		        return false;

	        Debug.Assert(Lines.Count == LineRects.Count);

	        if(Lines.Count != LineRects.Count)
		        return false;

	        C2DPointSet IntersectionTemp = new C2DPointSet();

	        bool bResult = false;

            for (int i = 0; i < this.Lines.Count; i++)
	        {
		        if (LineRects[i].Overlaps(LineRect) &&
			        Lines[i].Crosses(Line, IntersectionTemp as List<C2DPoint>))
		        {
			        bResult = true;
		        }
	        }

	        IntersectionPts.InsertRange(0, IntersectionTemp);

	        return bResult;
        }

        /// <summary>
        /// True if it crosses the ray. Provides the intersection points.
        /// </summary>
        /// <param name="Ray">The infinite line.</param>
        /// <param name="IntersectionPts">Output. The intersection points.</param>
        public bool CrossesRay(C2DLine Ray, C2DPointSet IntersectionPts)
        {
            double dDist = Ray.point.Distance(BoundingRect.GetCentre());

            C2DLine LineTemp = new C2DLine(Ray);

            LineTemp.vector.SetLength(dDist + BoundingRect.Width() + BoundingRect.Height());

            return Crosses(LineTemp, IntersectionPts);

        }

        /// <summary>
        /// True if this overlaps the other.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        public bool Overlaps(C2DPolyBase Other)
        {
            if (Lines.Count == 0 || Other.Lines.Count == 0)
                return false;

            if (Other.Contains(Lines[0].GetPointTo()))
                return true;

            if (Crosses(Other))
                return true;

            return (this.Contains(Other.Lines[0].GetPointTo()));
        }

        /// <summary>
        /// True if this overlaps the other.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        public bool Overlaps(C2DHoledPolyBase Other)
        {
            return Other.Overlaps(this);
        }

        /// <summary>
        /// True if it is a closed shape which it should be.
        /// </summary>
        public bool IsClosed()
        {
            return Lines.IsClosed(false);
        }

        /// <summary>
        /// Snap to the conceptual grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public override void SnapToGrid(CGrid grid)
        {
            for (int i = 0; i < _Lines.Count; i++)
            {
                _Lines[i].SnapToGrid(grid);
            }
            for(int i = 0; i < _LineRects.Count; i++)
            {
                _LineRects[i].SnapToGrid(grid);
            }
            BoundingRect.SnapToGrid(grid);
        }

        /// <summary>
        /// Returns the non-overlaps of this with another.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        /// <param name="Polygons">The output polygons.</param>
        /// <param name="grid">The degenerate settings.</param>
        public void GetNonOverlaps(C2DPolyBase Other, List<C2DHoledPolyBase> Polygons,
                                            CGrid grid)
        {
            GetBoolean(Other, Polygons, false, true, grid);
        }

        /// <summary>
        /// Returns the union of this with another.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        /// <param name="Polygons">The output polygons.</param>
        /// <param name="grid">The degenerate settings.</param>
        public void GetUnion(C2DPolyBase Other, List<C2DHoledPolyBase> Polygons,
                                        CGrid grid)
        {
            GetBoolean(Other, Polygons, false, false, grid);
        }

        /// <summary>
        /// Returns the overlaps of this with another.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        /// <param name="Polygons">The output polygons.</param>
        /// <param name="grid">The degenerate settings.</param>
        public void GetOverlaps(C2DPolyBase Other, List<C2DHoledPolyBase> Polygons,
										    CGrid grid)
        {
            GetBoolean(Other, Polygons, true, true, grid);
        }

        /// <summary>
        /// Returns the routes (collection of lines and sublines) either inside or outside another
        /// Given the intersection points.
        /// </summary>
        /// <param name="IntPts">The intersection points of this with the other polygon.</param>
        /// <param name="IntIndexes">The corresponding line indexes.</param>
        /// <param name="Routes">Output. The routes to get the result.</param>
        /// <param name="bStartInside">True if this polygon starts inside the other.</param>
        /// <param name="bRoutesInside">True if we require routes of this polygon inside the other.</param>    
        public void GetRoutes(C2DPointSet IntPts, List<int> IntIndexes,
            C2DLineBaseSetSet Routes, bool bStartInside, bool bRoutesInside)
        {
            
	        // Make sure the intersection indexes and points are the same size.
	        if (IntIndexes.Count != IntPts.Count )
	        {
		        Debug.Assert(false);
		        return;
	        }
	        // Set up a new collection of routes.
	        C2DLineBaseSetSet NewRoutes = new C2DLineBaseSetSet();
	        // If the polygon has no points then return.
	        if ( _Lines.Count < 1) 
		        return;
	        // Sort the intersections by index so we can go through them in order.
            IntPts.SortByIndex( IntIndexes );   
            
            // Set the inside / outside flag to the same as the start inside / outside flag.
	        bool bInside = bStartInside;
	        // If we are inside and want route inside or outside and want routes outside then add a new route.
	        if (bInside == bRoutesInside)
	        {
		        NewRoutes.Add(new C2DLineBaseSet());
	        }

	        // The current index of the intersects.
	        int usCurrentIntIndex = 0;

	        // cycle through the lines on the polygon.
	        for (int i = 0 ; i < Lines.Count ; i++)
	        {
		        // Set up a list of intersection points on this line only.
		        C2DPointSet IntsOnLine = new C2DPointSet();
		        // Cycle through all intersections on this line (leaving the usCurrentIntIndex at the next intersected line).
		        while ( usCurrentIntIndex < IntIndexes.Count && IntIndexes[usCurrentIntIndex] == i)
		        {
			        // Add a copy of the points on this line that are intersections
			        IntsOnLine.AddCopy( IntPts[ usCurrentIntIndex ] );
			        usCurrentIntIndex++;
		        }

		        // If the line in question intersects the other poly then we have left / entered.
		        if ( IntsOnLine.Count > 0 )
		        {
			        C2DLineBaseSet SubLines = new C2DLineBaseSet();
			        Lines[i].GetSubLines( IntsOnLine, SubLines );

			        while (SubLines.Count > 1)
			        {
				        if (bInside == bRoutesInside)
				        {
					        // We have 1. Left and want route in. OR 2. Entered and want routes out.
					        NewRoutes[NewRoutes.Count - 1].Add( SubLines.ExtractAt(0) );
					        bInside = true ^ bRoutesInside;
				        }
				        else
				        {
					        NewRoutes.Add(new C2DLineBaseSet());
					        bInside = false ^ bRoutesInside;
					        SubLines.RemoveAt(0);
				        }
			        }
			        if (bInside == bRoutesInside)
				        NewRoutes[NewRoutes.Count - 1].Add( SubLines.ExtractAt(SubLines.Count - 1 ) );
			        else
				        SubLines.RemoveAt(SubLines.Count - 1);
		        }
		        // Otherwise, if we are e.g. inside and want routes in the keep adding the end poitn of the line.
		        else if (bInside == bRoutesInside)
		        {
			        NewRoutes[NewRoutes.Count - 1].AddCopy(  Lines[i] );
		        }

	        }
	        // Put all the new routes into the provided collection.
	        Routes.ExtractAllOf(NewRoutes);
        }


        /// <summary>
        /// Gets the boolean operation with the other. e.g. union / intersection.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        /// <param name="HoledPolys">The set to recieve the result.</param>
        /// <param name="bThisInside">The flag to indicate routes inside.</param>
        /// <param name="bOtherInside">The flag to indicate routes inside for the other.</param>
        /// <param name="grid">The degenerate settings.</param>
        public void GetBoolean(C2DPolyBase Other, List<C2DHoledPolyBase> HoledPolys,
                            bool bThisInside, bool bOtherInside,
                            CGrid grid)
        {
            
	        if (BoundingRect.Overlaps(Other.BoundingRect ))
	        {
		        switch (grid.DegenerateHandling)
		        {
		        case CGrid.eDegenerateHandling.None:
			        {
				        C2DLineBaseSetSet Routes1 = new C2DLineBaseSetSet();
                        C2DLineBaseSetSet Routes2 = new C2DLineBaseSetSet();
				        C2DPolyBase.GetRoutes( this, bThisInside, Other, bOtherInside, Routes1, Routes2);
				        Routes1.ExtractAllOf(Routes2);

				        if (Routes1.Count > 0)
				        {
					        // Add all the joining routes together to form closed routes
					        Routes1.MergeJoining();
					        // Set up some temporary polygons.
					        List<C2DPolyBase> Polygons = new List<C2DPolyBase>();
					        // Turn the routes into polygons.
					        for (int i = Routes1.Count - 1; i >= 0; i--)
					        {

						        if (Routes1[i].IsClosed(true) && Routes1[i].Count > 2)
						        {
							        Polygons.Add(new C2DPolyBase());
							        Polygons[Polygons.Count - 1].CreateDirect( Routes1[i]);
						        }
						        else
						        {
							     //   Debug.Assert(false);
							        grid.LogDegenerateError();
						        }	
					        }
                            

					        // Set up some temporary holed polygons
					        C2DHoledPolyBaseSet NewComPolys = new C2DHoledPolyBaseSet();
					        // Turn the set of polygons into holed polygons. Not needed for intersection.
					        if (!(bThisInside && bOtherInside))
					        {
						        C2DHoledPolyBase.PolygonsToHoledPolygons(NewComPolys, Polygons);
						        if (NewComPolys.Count != 1)
						        {
							     //   Debug.Assert(false);
							        grid.LogDegenerateError();
						        }
					        }
					        else
					        {
                                for (int i = 0; i < Polygons.Count; i++)
						            HoledPolys.Add(new C2DHoledPolyBase(Polygons[i]));
					        }

					        // Now add them all to the provided set.
                            for (int i = 0 ; i < NewComPolys.Count; i++)
					            HoledPolys.Add(NewComPolys[i]);
				        }
			        }
			        break;
		        case CGrid.eDegenerateHandling.RandomPerturbation:
			        {
				        C2DPolyBase OtherCopy = new C2DPolyBase(Other);
				        OtherCopy.RandomPerturb();
                        grid.DegenerateHandling = CGrid.eDegenerateHandling.None;
				        GetBoolean( OtherCopy, HoledPolys, bThisInside, bOtherInside, grid);
                        grid.DegenerateHandling = CGrid.eDegenerateHandling.RandomPerturbation;
			        }
			        break;
		        case CGrid.eDegenerateHandling.DynamicGrid:
			        {
				        C2DRect Rect = new C2DRect(); 
				        if (this.BoundingRect.Overlaps(Other.BoundingRect, Rect))
				        {
					        double dOldGrid = grid.GridSize;
					        grid.SetToMinGridSize(Rect, false);
                            grid.DegenerateHandling = CGrid.eDegenerateHandling.PreDefinedGrid;
					        GetBoolean( Other, HoledPolys, bThisInside, bOtherInside, grid);
                            grid.DegenerateHandling = CGrid.eDegenerateHandling.DynamicGrid;
				        }
			        }
			        break;
		        case CGrid.eDegenerateHandling.PreDefinedGrid:
			        {
				        C2DPolyBase P1 = new C2DPolyBase(this);
                        C2DPolyBase P2 = new C2DPolyBase(Other);
				        P1.SnapToGrid(grid);
                        P2.SnapToGrid(grid);
				        C2DVector V1 = new C2DVector( P1.BoundingRect.TopLeft,  P2.BoundingRect.TopLeft);
				        double dPerturbation = grid.GridSize; // ensure it snaps back to original grid positions.
				        if(V1.i > 0) 
                            V1.i = dPerturbation;
                        else
                           V1.i = -dPerturbation;	// move away slightly if possible
				        if(V1.j > 0) 
                            V1.j = dPerturbation;
                        else
                            V1.j = -dPerturbation; // move away slightly if possible
				        V1.i *= 0.411923;// ensure it snaps back to original grid positions.
				        V1.j *= 0.313131;// ensure it snaps back to original grid positions.

				        P2.Move( V1 );
                        grid.DegenerateHandling = CGrid.eDegenerateHandling.None;
				        P1.GetBoolean( P2, HoledPolys, bThisInside, bOtherInside, grid);

                        for (int i = 0; i < HoledPolys.Count; i++)
                            HoledPolys[i].SnapToGrid(grid);

	                    grid.DegenerateHandling = CGrid.eDegenerateHandling.PreDefinedGrid;
			        }
			        break;
		        case CGrid.eDegenerateHandling.PreDefinedGridPreSnapped:
			        {
				        C2DPolyBase P2 = new C2DPolyBase(Other);
				        C2DVector V1 = new C2DVector( this.BoundingRect.TopLeft,  P2.BoundingRect.TopLeft);
				        double dPerturbation = grid.GridSize; // ensure it snaps back to original grid positions.
				        if (V1.i > 0)
                            V1.i = dPerturbation;
                        else
                            V1.i = -dPerturbation; // move away slightly if possible
				        if (V1.j > 0)
                            V1.j = dPerturbation;
                        else
                            V1.j = -dPerturbation; // move away slightly if possible
				        V1.i *= 0.411923;// ensure it snaps back to original grid positions.
				        V1.j *= 0.313131;// ensure it snaps back to original grid positions.

				        P2.Move( V1 );
                        grid.DegenerateHandling = CGrid.eDegenerateHandling.None;
				        GetBoolean( P2, HoledPolys, bThisInside, bOtherInside, grid);

                        for (int i = 0; i < HoledPolys.Count; i++)
                            HoledPolys[i].SnapToGrid(grid);
                        grid.DegenerateHandling = CGrid.eDegenerateHandling.PreDefinedGridPreSnapped;
			        }
			        break;
		        }
                       
            }
        }

        /// <summary>
        /// Projection onto the line.
        /// </summary>
        /// <param name="Line">The line.</param>
        /// <param name="Interval">Output. The interval.</param>
        public override void Project(C2DLine Line, CInterval Interval)
        {
	        if(Lines.Count == 0)
		        return;

	        Lines[0].Project(Line, Interval);

            for (int i = 1; i < Lines.Count; i++)
	        {
		        CInterval LineInt = new CInterval();
		        Lines[i].Project(Line, LineInt);
		        Interval.ExpandToInclude( LineInt );
	        }
        }

        /// <summary>
        /// Projection onto the vector.
        /// </summary>
        /// <param name="Vector">The vector.</param>
        /// <param name="Interval">Output. The interval.</param>
        public override void Project(C2DVector Vector, CInterval Interval)
        {
	        if(Lines.Count == 0)
		        return;

	        Lines[0].Project(Vector, Interval);

	        for (int i = 1; i < Lines.Count; i++)
	        {
		        CInterval LineInt = new CInterval();
		        Lines[i].Project(Vector, LineInt);
		        Interval.ExpandToInclude( LineInt );
	        }
        }

        /// <summary>
        /// Moves this by a tiny random amount.
        /// </summary>
	    public void RandomPerturb()
        {
	        C2DPoint pt = BoundingRect.GetPointFurthestFromOrigin();
	        double dMinEq = Math.Max(pt.x, pt.y) * Constants.conEqualityTolerance;
	        CRandomNumber rn = new CRandomNumber (dMinEq * 10, dMinEq * 100);

	        C2DVector cVector = new C2DVector( rn.Get(), rn.Get() );
	        if (rn.GetBool())
		        cVector.i = - cVector.i ;
	        if (rn.GetBool())
		        cVector.j = - cVector.j ;
	        Move(cVector);

        }

        /// <summary>
        /// Returns the routes (multiple lines or part polygons) either inside or
        /// outside the polygons provided. These are based on the intersections
        /// of the 2 polygons e.g. the routes / part polygons of one inside or
        /// outside the other.
        /// </summary>
        /// <param name="Poly1">The first polygon.</param> 
        /// <param name="bP1RoutesInside">True if routes inside the second polygon are 
        /// required for the first polygon.</param> 
        /// <param name="Poly2">The second polygon.</param> 
        /// <param name="bP2RoutesInside">True if routes inside the first polygon are 
        /// required for the second polygon.</param> 
        /// <param name="Routes1">Output. Set of lines for the first polygon.</param> 
        /// <param name="Routes2">Output. Set of lines for the second polygon.</param> 
	    public static void GetRoutes(C2DPolyBase Poly1, bool bP1RoutesInside, 
				    C2DPolyBase Poly2, bool bP2RoutesInside, 
				    C2DLineBaseSetSet Routes1, C2DLineBaseSetSet Routes2)
        {
            // Set up a collection of intersected points, and corresponding indexes.
            C2DPointSet IntPoints = new C2DPointSet();
            List<int> Indexes1 = new List<int>();
            List<int> Indexes2 = new List<int>();
            // Use the line collections in each shape to find the intersections between them.
            Poly1.Lines.GetIntersections(Poly2.Lines, IntPoints,
                                                Indexes1, Indexes2,
                                        Poly1.BoundingRect, Poly2.BoundingRect);
            // Make a copy of the point set because this will be sorted by line index in the 
            // Get routes function later. We need an unsorted set for each polygon.
            C2DPointSet IntPointsCopy = new C2DPointSet();
            IntPointsCopy.MakeCopy(IntPoints);

            // Find out whether the first poly starts inside the second.
            bool bP1StartInside = Poly2.Contains(Poly1.Lines[0].GetPointFrom());
            // Find out if poly 2 starts inside poly 1.
            bool bP2StartInside = Poly1.Contains(Poly2.Lines[0].GetPointFrom());

            if (IntPoints.Count == 0 && !bP1StartInside && !bP2StartInside)
                return;	// No interaction between the 2.

            // Get the routes of poly 1 inside / outside the other, passing the unsorted
            // intersection points and polygon1 intersection indexes.
            Poly1.GetRoutes(IntPoints, Indexes1, Routes1, bP1StartInside, bP1RoutesInside);
            // Do the same for poly 2 but pass it the unsorted copy of the intersection points
            // So that they correspond to the indexes.
            Poly2.GetRoutes(IntPointsCopy, Indexes2, Routes2, bP2StartInside, bP2RoutesInside);
        }

        /// <summary>
        /// Forms the bounding rectangle.
        /// </summary>
        protected void MakeBoundingRect()
        {
	        if ( LineRects.Count == 0)
	        {
		        BoundingRect.Clear();
		        return;
	        }
	        else
	        {
		        BoundingRect.Set(LineRects[0]);

		        for (int i = 1 ; i  < LineRects.Count; i++)
		        {
			        BoundingRect.ExpandToInclude(LineRects[i]);
		        }
	        }
        }

        /// <summary>
        /// Forms the bounding rectangles for all the lines.
        /// </summary>
        protected void MakeLineRects()
        {
	        LineRects.Clear();

	        for (int i = 0 ; i  < Lines.Count; i++)
	        {
		        C2DRect pRect = new C2DRect();
		        Lines[i].GetBoundingRect( pRect);
		        LineRects.Add( pRect );
	        }

        }

        /// <summary>
        /// Reverses the direction of the lines.
        /// </summary>
        protected void ReverseDirection()
        {
	        Lines.ReverseDirection();

	        MakeLineRects();
        }

        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
        public void Transform(CTransformation pProject)
        {
	        for (int i = 0; i < this._Lines.Count; i++)	
	        {
                C2DLineBase pLine = _Lines[i];
		        pLine.Transform(pProject);
		        pLine.GetBoundingRect(_LineRects[i]);
	        }

	        this.MakeBoundingRect();
        }

        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
        public void InverseTransform(CTransformation pProject)
        {
            for (int i = 0; i < this._Lines.Count; i++)
            {
                C2DLineBase pLine = _Lines[i];
                pLine.InverseTransform(pProject);
                pLine.GetBoundingRect(_LineRects[i]);
            }

            this.MakeBoundingRect();

        }



        /// <summary>
        /// The lines.
        /// </summary>
	    protected C2DLineBaseSet _Lines = new C2DLineBaseSet();

        /// <summary>
        /// The lines.
        /// </summary>
        public C2DLineBaseSet Lines
        {
            get
            {
                return _Lines;
            }
        }

        /// <summary>
        /// The bounding rectangle.
        /// </summary>
        protected C2DRect _BoundingRect = new C2DRect();

        /// <summary>
        /// The bounding rectangle.
        /// </summary>
        public C2DRect BoundingRect
        {
            get
            {
                return _BoundingRect;
            }
        }

        /// <summary>
        /// The line bounding rectangles.
        /// </summary>
        protected List<C2DRect> _LineRects = new List<C2DRect>();

        /// <summary>
        /// The line bounding rectangles.
        /// </summary>
        public List<C2DRect> LineRects
        {
            get
            {
                return _LineRects;
            }
        }
    }
}
