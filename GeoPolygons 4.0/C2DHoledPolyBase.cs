using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace GeoLib
{
    /// <summary>
    /// Class representing a polygon with (or can be without) holes. The outer Rim is
    /// a reference to a polygon (C2DPolyBase) with the holes stored in an array of
    /// references to other polygons. Functions generally assume that the holes do
    /// not overlap and are fully contained within the Rim. Each polygon should not 
    /// self intersect.
    /// </summary>
    public class C2DHoledPolyBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
	    public C2DHoledPolyBase() {}
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
	    public C2DHoledPolyBase(C2DHoledPolyBase Other)
        {
	        _Rim =  new C2DPolyBase( Other.Rim);

	        for (int i = 0 ; i < Other.HoleCount; i++)
	        {
		        _Holes.Add(new C2DPolyBase(Other.GetHole(i)));
	        }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public C2DHoledPolyBase(C2DPolyBase Other)
        {
            _Rim = new C2DPolyBase(Other);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
	    ~C2DHoledPolyBase(){}

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
	    public void Set(C2DHoledPolyBase Other)
        {
            _Rim.Set(Other.Rim);
            _Holes.Clear();
	        for (int i = 0 ; i < Other.HoleCount; i++)
	        {
		        _Holes.Add(new C2DPolyBase(Other.GetHole(i)));
	        }
        }

        /// <summary>
        /// Return the number of lines.
        /// </summary>
	    public int GetLineCount()
        {
	        int nResult = 0;

            nResult += _Rim.Lines.Count;

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        nResult +=	_Holes[i].Lines.Count;
	        }

	        return nResult;
        }

        /// <summary>
        /// Clears the shape.
        /// </summary>
        public void Clear()
        {
            _Rim.Clear();

	        _Holes.Clear();
        }

        /// <summary>
        /// Validity check. True if the holes are contained and non-intersecting.
        /// </summary>
        public bool IsValid()
        {
	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
                if (!_Rim.Contains(_Holes[i]))
			        return false;
	        }

	        int h = 0;
	        while (h < _Holes.Count)
	        {
		        int r = h + 1;
		        while (r < _Holes.Count)
		        {
			        if (_Holes[h].Overlaps(_Holes[r]))
				        return false;
                    r++;
		        }
                h++;
	        }
	        return true;
        }

        /// <summary>
        /// Rotates to the right by the angle around the origin.
        /// </summary>
        /// <param name="dAng">Angle in radians to rotate by.</param> 
        /// <param name="Origin">The origin.</param> 
        public void RotateToRight(double dAng, C2DPoint Origin)
        {
            _Rim.RotateToRight(dAng, Origin);

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        _Holes[i].RotateToRight(dAng, Origin);
	        }
        }

        /// <summary>
        /// Moves the polygon.
        /// </summary>
        /// <param name="Vector">Vector to move by.</param> 
        public void Move(C2DVector Vector)
        {
            _Rim.Move(Vector);

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        _Holes[i].Move(Vector);
	        }
        }

        /// <summary>
        /// Grows around the origin.
        /// </summary>
        /// <param name="dFactor">Factor to grow by.</param> 
        /// <param name="Origin">Origin to grow this around.</param> 
        public void Grow(double dFactor, C2DPoint Origin)
        {
            _Rim.Grow(dFactor, Origin);

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        _Holes[i].Grow(dFactor, Origin);
	        }

        }

        /// <summary>
        /// Point reflection.
        /// </summary>
        /// <param name="Point">Point through which to reflect this.</param> 
        public void Reflect(C2DPoint Point)
        {
            _Rim.Reflect(Point);

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        _Holes[i].Reflect(Point);
	        }
        }

        /// <summary>
        /// Reflects throught the line provided.
        /// </summary>
        /// <param name="Line">Line through which to reflect this.</param> 
        public void Reflect(C2DLine Line)
        {
            _Rim.Reflect(Line);

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        _Holes[i].Reflect(Line);
	        }
        }

        /// <summary>
        /// Distance from the point.
        /// </summary>
        /// <param name="TestPoint">Point to find the distance to.</param> 
        public double Distance(C2DPoint TestPoint)
        {
            double dResult = _Rim.Distance(TestPoint);
	        bool bInside = dResult < 0;
	        dResult = Math.Abs(dResult);

	        for (int i = 0; i < _Holes.Count; i++)
	        {
		        double dDist = _Holes[i].Distance(TestPoint); 
		        if (dDist < 0)
			        bInside = false;
		        if (Math.Abs(dDist) < dResult)
			        dResult = Math.Abs(dDist);
	        }

	        if (bInside)
		        return dResult;
	        else
		        return - dResult;

        }

        /// <summary>
        /// Distance from the line provided.
        /// </summary>
        /// <param name="Line">Line to find the distance to.</param> 
        public double Distance(C2DLineBase Line) 
        {
            double dResult = _Rim.Distance(Line);
	        if (dResult == 0)
		        return 0;

	        bool bInside = dResult < 0;
	        dResult = Math.Abs(dResult);

	        for ( int i = 0; i < _Holes.Count; i++)
	        {
		        double dDist = _Holes[i].Distance(Line); 
		        if (dDist == 0)
			        return 0;
        		

		        if (dDist < 0)
			        bInside = false;
		        if (Math.Abs(dDist) < dResult)
			        dResult = Math.Abs(dDist);
	        }

	        if (bInside)
		        return dResult;
	        else
		        return - dResult;

        }

        /// <summary>
        /// Distance from the polygon provided.
        /// </summary>
        /// <param name="Poly">Polygon to find the distance to.</param> 
        /// <param name="ptOnThis">Closest point on this to recieve the result.</param> 
        /// <param name="ptOnOther">Closest point on the other to recieve the result.</param> 
        public double Distance(C2DPolyBase Poly, C2DPoint ptOnThis, C2DPoint ptOnOther) 
        {
	        C2DPoint ptOnThisResult = new C2DPoint();
	        C2DPoint ptOnOtherResult = new C2DPoint();


            double dResult = _Rim.Distance(Poly, ptOnThis, ptOnOther);

	        if (dResult == 0)
		        return 0;

		    ptOnThisResult.Set(ptOnThis); 
		    ptOnOtherResult.Set(ptOnOther); 

	        bool bInside = dResult < 0;
	        dResult = Math.Abs(dResult);

	        for (int i = 0; i < _Holes.Count; i++)
	        {
		        double dDist = _Holes[i].Distance(Poly, ptOnThis, ptOnOther); 
		        if (dDist == 0)
			        return 0;
        		

		        if (dDist < 0)
			        bInside = false;
		        if (Math.Abs(dDist) < dResult)
		        {
				    ptOnThisResult.Set(ptOnThis); 
				    ptOnOtherResult.Set(ptOnOther); 

			        dResult = Math.Abs(dDist);
		        }
	        }

		    ptOnThis.Set(ptOnThisResult); 
		    ptOnOther.Set(ptOnOtherResult); 

	        if (bInside)
		        return dResult;
	        else
		        return - dResult;
        }
        /// <summary>
        /// Proximity test.
        /// </summary>
        /// <param name="TestPoint">Point to test against.</param> 
        /// <param name="dDist">Distance threshold.</param> 
        public bool IsWithinDistance(C2DPoint TestPoint, double dDist) 
        {
            if (_Rim.IsWithinDistance(TestPoint, dDist))
		        return true;

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        if (_Holes[i].IsWithinDistance( TestPoint,  dDist))
			        return true;
	        }

	        return false;
        }
        /// <summary>
        /// Perimeter.
        /// </summary>
	    public double GetPerimeter() 
        {

            double dResult = _Rim.GetPerimeter();

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        dResult += _Holes[i].GetPerimeter();
	        }

	        return dResult;

        }

        /// <summary>
        /// Projection onto the line.
        /// </summary>
        /// <param name="Line">Line to project this on.</param> 
        /// <param name="Interval">Interval to recieve the result.</param> 
        public void Project(C2DLine Line, CInterval Interval) 
        {
            _Rim.Project(Line, Interval);
        }

        /// <summary>
        /// Projection onto the vector.
        /// </summary>
        /// <param name="Vector">Vector to project this on.</param> 
        /// <param name="Interval">Interval to recieve the result.</param> 
        public void Project(C2DVector Vector, CInterval Interval) 
        {
            _Rim.Project(Vector, Interval);
        }

        /// <summary>
        /// Returns true if there are crossing lines.
        /// </summary>
        public bool HasCrossingLines()
        {
	        C2DLineBaseSet Lines = new C2DLineBaseSet();

            Lines.InsertRange(0, _Rim.Lines);

	        for (int i = 0; i < _Holes.Count; i++)
	        {
		        Lines.InsertRange( 0, _Holes[i].Lines);
	        }

	        return Lines.HasCrossingLines();
        }

        /// <summary>
        /// Returns the bounding rectangle.
        /// </summary>
        /// <param name="Rect">Rectangle to recieve the result.</param> 
        public void GetBoundingRect(C2DRect Rect) 
        {
            Rect.Set(_Rim.BoundingRect);

	        for (int i = 0; i < _Holes.Count; i++)
	        {
		        Rect.ExpandToInclude( _Holes[i].BoundingRect);
	        }
        }

        /// <summary>
        /// Point inside test.
        /// </summary>
        /// <param name="pt">Point to test for.</param> 
	    public bool Contains(C2DPoint pt) 
        {
            if (!_Rim.Contains(pt))
		        return false;

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        if (_Holes[i].Contains(pt))
			        return false;
	        }

	        return true;
        }

        /// <summary>
        /// Line entirely inside test.
        /// </summary>
        /// <param name="Line">Line to test for.</param> 
        public bool Contains(C2DLineBase Line) 
        {
            if (!_Rim.Contains(Line))
		        return false;

	        for ( int i = 0 ; i < _Holes.Count; i++)
	        {
		        if (_Holes[i].Crosses(Line) || _Holes[i].Contains( Line.GetPointFrom()))
			        return false;
	        }

	        return true;

        }

        /// <summary>
        /// Polygon entirely inside test.
        /// </summary>
        /// <param name="Polygon">Polygon to test for.</param> 
        public bool Contains(C2DPolyBase Polygon) 
        {
            if (!_Rim.Contains(Polygon))
		        return false;

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        if (_Holes[i].Overlaps(Polygon))
			        return false;
	        }

	        return true;
        }
        
        /// <summary>
        /// Polygon entirely inside test.
        /// </summary>
        /// <param name="Polygon">Polygon to test for.</param> 
        public bool Contains(C2DHoledPolyBase Polygon) 
        {
	        if (! Contains( Polygon.Rim))
		        return false;
        	
	        for (int i = 0 ; i < Polygon.HoleCount; i++)
	        {
		        if (! Contains(  Polygon.GetHole(i)) )
			        return false;
	        }
	        return true;
        }

        /// <summary>
        /// True if this crosses the line
        /// </summary>
        /// <param name="Line">Line to test for.</param> 
        public bool Crosses(C2DLineBase Line)
        {
            if (_Rim.Crosses(Line))
		        return true;

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        if (_Holes[i].Crosses(Line))
			        return true;
	        }
	        return false;
        }

        /// <summary>
        /// True if this crosses the line.
        /// </summary>
        /// <param name="Line">Line to test for.</param> 
        /// <param name="IntersectionPts">Point set to recieve the intersections.</param> 
        public bool Crosses(C2DLineBase Line, C2DPointSet IntersectionPts) 
        {
	        C2DPointSet IntPts = new C2DPointSet();

            _Rim.Crosses(Line, IntPts);

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        _Holes[i].Crosses(Line, IntPts);
	        }
	        bool bResult = (IntPts.Count != 0);
		    IntersectionPts.ExtractAllOf(IntPts);

	        return (bResult);

        }


        /// <summary>
        /// True if this crosses the other polygon.
        /// </summary>
        /// <param name="Poly">Polygon to test for.</param> 
        public bool Crosses(C2DPolyBase Poly) 
        {
            if (_Rim.Crosses(Poly))
		        return true;

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        if(_Holes[i].Crosses(Poly))
			        return true;
	        }
	        return false;
        }

        /// <summary>
        /// True if this crosses the ray, returns the intersection points.
        /// </summary>
        /// <param name="Ray">Ray to test for.</param> 
        /// <param name="IntersectionPts">Intersection points.</param> 
        public bool CrossesRay(C2DLine Ray, C2DPointSet IntersectionPts) 
        {
	        C2DPointSet IntPts = new C2DPointSet();

            _Rim.CrossesRay(Ray, IntPts);

	        IntersectionPts.ExtractAllOf(IntPts);

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        if (_Holes[i].CrossesRay(Ray, IntPts))
		        {
			        double dDist = Ray.point.Distance(IntPts[0]);
			        int nInsert = 0;

			        while (nInsert < IntersectionPts.Count && 
				        Ray.point.Distance( IntersectionPts[nInsert]) < dDist )
			        {
				        nInsert++;
			        }

			        IntersectionPts.InsertRange(nInsert, IntPts);
		        }
	        }

	        return (IntersectionPts.Count > 0);

        }

        /// <summary>
        /// True if this overlaps the other.
        /// </summary>
        /// <param name="Other">Other polygon to test for.</param> 
        public bool Overlaps(C2DHoledPolyBase Other) 
        {
	        if (!Overlaps( Other.Rim ))
		        return false;
	        for (int i = 0 ; i < Other.HoleCount; i++)
	        {
                if (Other.GetHole(i).Contains(_Rim))
			        return false;
	        }
	        return true;
        }

        /// <summary>
        /// True if this overlaps the other.
        /// </summary>
        /// <param name="Other">Other polygon to test for.</param> 
        public bool Overlaps(C2DPolyBase Other) 
        {
            if (!_Rim.Overlaps(Other))
		        return false;

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        if (_Holes[i].Contains(Other))
			        return false;
	        }
	        return true;
        }

        /// <summary>
        /// Function to convert polygons to complex polygons. Assigning holes to those that are contained.
        /// The set of holed polygons will be filled from the set of simple polygons.
        /// </summary>
        /// <param name="HoledPolys">Holed polygon set.</param> 
        /// <param name="Polygons">Simple polygon set.</param> 
        public static void PolygonsToHoledPolygons(List<C2DHoledPolyBase> HoledPolys,
				    List<C2DPolyBase> Polygons)
        {
	        List<C2DPolyBase> Unmatched = new List<C2DPolyBase>();
	        List<C2DHoledPolyBase> NewHoledPolys = new List<C2DHoledPolyBase>();

	        for (int i = Polygons.Count - 1 ; i >= 0  ; i--)
	        {
		        bool bMatched = false;

		        C2DPolyBase pPoly = Polygons[i];
                Polygons.RemoveAt(i);

		        // Cycle through the newly created polygons to see if it's a hole.
		        for ( int p = 0; p < NewHoledPolys.Count; p++)
		        {
			        if (  NewHoledPolys[p].Rim.Contains(  pPoly.Lines[0].GetPointFrom() ))
			        {
				        NewHoledPolys[p].AddHole(pPoly);
				        bMatched = true;
				        break;
			        }
		        }
		        // If its not then compare it to all the other unknowns.
		        if (!bMatched)
		        {
			        int u = 0; 

			        bool bKnownRim = false;

			        while (u < Unmatched.Count)
			        {
				        if ( !bKnownRim && Unmatched[u].Contains(  pPoly.Lines[0].GetPointFrom() ))
				        {
					        // This is a hole.
					        NewHoledPolys.Add(new C2DHoledPolyBase());
					        NewHoledPolys[ NewHoledPolys.Count -1 ].Rim = Unmatched[u];
                            Unmatched.RemoveAt(u);
					        NewHoledPolys[ NewHoledPolys.Count -1 ].AddHole(pPoly);
					        bMatched = true;
					        break;

				        }
				        else if ( pPoly.Contains(  Unmatched[u].Lines[0].GetPointFrom() ))
				        {
				        //	int nCount = OverlapPolygons->GetCount();
					        // This is a rim.
					        if (!bKnownRim)
					        {
						        // If we haven't alreay worked this out then record that its a rim
						        // and set up the new polygon.
						        bKnownRim = true;
						        NewHoledPolys.Add(new C2DHoledPolyBase());
						        NewHoledPolys[ NewHoledPolys.Count -1 ].Rim = pPoly;
						        NewHoledPolys[ NewHoledPolys.Count -1 ].AddHole(  Unmatched[u] );
                                Unmatched.RemoveAt(u);
					        }
					        else
					        {
						        // We already worked out this was a rim so it must be the last polygon.
						        NewHoledPolys[ NewHoledPolys.Count -1 ].AddHole(  Unmatched[u] );
                                Unmatched.RemoveAt(u);
					        }
					        // Record that its been matched.
					        bMatched = true;
				        }
				        else
				        {
					        // Only if there was no match do we increment the counter.
					        u++;
				        }
			        }		
		        }

		        if (!bMatched)
		        {
			        Unmatched.Add(pPoly);
		        }
	        }

            for (int i = 0; i < Unmatched.Count; i++)
            {
                C2DHoledPolyBase NewHoled = new C2DHoledPolyBase();
                NewHoled.Rim = Unmatched[i];
                HoledPolys.Add(NewHoled);
            }
        	
	        HoledPolys.AddRange(NewHoledPolys);
        }

        /// <summary>
        /// Returns the overlaps between this and the other complex polygon.
        /// </summary>
        /// <param name="Other">Other polygon.</param> 
        /// <param name="HoledPolys">Set to receieve all the resulting polygons.</param> 
        /// <param name="grid">Grid containing the degenerate handling settings.</param> 
        public void GetOverlaps(C2DHoledPolyBase Other, List<C2DHoledPolyBase> HoledPolys, 
							    CGrid grid)
        {
            GetBoolean(Other, HoledPolys, true, true, grid);
        }

        /// <summary>
        /// Returns the difference between this and the other polygon.
        /// </summary>
        /// <param name="Other">Other polygon.</param> 
        /// <param name="HoledPolys">Set to receieve all the resulting polygons.</param> 
        /// <param name="grid">Grid containing the degenerate handling settings.</param> 
        public void GetNonOverlaps(C2DHoledPolyBase Other, List<C2DHoledPolyBase> HoledPolys, 
							    CGrid grid) 
        {
        	GetBoolean(Other, HoledPolys, false, true, grid);

        }
        /// <summary>
        /// Returns the union of this and the other.
        /// </summary>
        /// <param name="Other">Other polygon.</param> 
        /// <param name="HoledPolys">Set to receieve all the resulting polygons.</param> 
        /// <param name="grid">Grid containing the degenerate handling settings.</param> 
        public void GetUnion(C2DHoledPolyBase Other, List<C2DHoledPolyBase> HoledPolys,
						    CGrid grid) 
        {
        	GetBoolean(Other, HoledPolys,false , false, grid);
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
        /// <param name="CompleteHoles1">Output. Complete holes for the first polygon.</param> 
        /// <param name="CompleteHoles2">Output. Complete holes for the second polygon.</param> 
        /// <param name="grid">Contains the degenerate handling settings.</param> 
        public static void GetRoutes(C2DHoledPolyBase Poly1, bool bP1RoutesInside, 
				    C2DHoledPolyBase Poly2, bool bP2RoutesInside, 
				    C2DLineBaseSetSet Routes1, C2DLineBaseSetSet Routes2, 
				    List<C2DPolyBase> CompleteHoles1, List<C2DPolyBase> CompleteHoles2,
                    CGrid grid)
        {

		    if (Poly1.Rim.Lines.Count == 0 || Poly2.Rim.Lines.Count == 0)
		    {
			    Debug.Assert(false, "Polygon with no lines" );
			    return;
		    }

		    C2DPointSet IntPointsTemp = new C2DPointSet();
            C2DPointSet IntPointsRim1 = new C2DPointSet();
            C2DPointSet IntPointsRim2 = new C2DPointSet();
		    List<int>	IndexesRim1 = new List<int>();
            List<int>	IndexesRim2 = new List<int>();


            List<C2DPointSet> IntPoints1AllHoles = new List<C2DPointSet>();
            List<C2DPointSet> IntPoints2AllHoles = new List<C2DPointSet>();
            List<List<int>> Indexes1AllHoles = new List<List<int>>();
            List<List<int>> Indexes2AllHoles = new List<List<int>>();
		//    std::vector<C2DPointSet* > IntPoints1AllHoles, IntPoints2AllHoles;
		 //   std::vector<CIndexSet*> Indexes1AllHoles, Indexes2AllHoles;

		    int usP1Holes = Poly1.HoleCount;
		    int usP2Holes = Poly2.HoleCount;

		    // *** Rim Rim Intersections
		    Poly1.Rim.Lines.GetIntersections(  Poly2.Rim.Lines,
			    IntPointsTemp, IndexesRim1, IndexesRim2, 
			    Poly1.Rim.BoundingRect, Poly2.Rim.BoundingRect );

		    IntPointsRim1.AddCopy( IntPointsTemp );
		    IntPointsRim2.ExtractAllOf(IntPointsTemp);

		    // *** Rim Hole Intersections
		    for ( int i = 0 ; i < usP2Holes; i++)
		    {
			    Debug.Assert(IntPointsTemp.Count == 0);

			    IntPoints2AllHoles.Add(new C2DPointSet());
			    Indexes2AllHoles.Add( new List<int>() );

			    if (Poly1.Rim.BoundingRect.Overlaps( Poly2.GetHole(i).BoundingRect ))
			    {
				    Poly1.Rim.Lines.GetIntersections(  Poly2.GetHole(i).Lines,
						    IntPointsTemp, IndexesRim1, Indexes2AllHoles[i],
						    Poly1.Rim.BoundingRect, Poly2.GetHole(i).BoundingRect);
    				
				    IntPointsRim1.AddCopy( IntPointsTemp);
				    IntPoints2AllHoles[i].ExtractAllOf(IntPointsTemp);
			    }
		    }
		    // *** Rim Hole Intersections
		    for ( int j = 0 ; j < usP1Holes; j++)
		    {
			    Debug.Assert(IntPointsTemp.Count == 0);

			    IntPoints1AllHoles.Add( new C2DPointSet());
			    Indexes1AllHoles.Add( new List<int>());

                if (Poly2.Rim.BoundingRect.Overlaps(Poly1.GetHole(j).BoundingRect))
			    {
				    Poly2.Rim.Lines.GetIntersections(  Poly1.GetHole(j).Lines,
						    IntPointsTemp, IndexesRim2, Indexes1AllHoles[j],
                            Poly2.Rim.BoundingRect, Poly1.GetHole(j).BoundingRect);
    				
				    IntPointsRim2.AddCopy( IntPointsTemp);
				    IntPoints1AllHoles[j].ExtractAllOf(IntPointsTemp);
			    }

		    }

		    // *** Quick Escape
		    bool bRim1StartInPoly2 = Poly2.Contains( Poly1.Rim.Lines[0].GetPointFrom() );
		    bool bRim2StartInPoly1 = Poly1.Contains( Poly2.Rim.Lines[0].GetPointFrom() );

		    if (IntPointsRim1.Count != 0 || IntPointsRim2.Count != 0 ||
					    bRim1StartInPoly2 || bRim2StartInPoly1			)	
			    // pos no interaction
		    {	
			    // *** Rim Routes
			    Poly1.Rim.GetRoutes( IntPointsRim1, IndexesRim1, Routes1, 
										    bRim1StartInPoly2, bP1RoutesInside);
			    Poly2.Rim.GetRoutes( IntPointsRim2, IndexesRim2, Routes2,
										    bRim2StartInPoly1, bP2RoutesInside);

			    if( IntPointsRim1.Count % 2 != 0)	// Must be even
			    {
				    grid.LogDegenerateError();
				  //  Debug.Assert(false);
			    }

			    if( IntPointsRim2.Count % 2 != 0)	// Must be even
			    {
				    grid.LogDegenerateError();
				 //   Debug.Assert(false);
			    }

			    // *** Hole Hole Intersections
			    for (int h = 0 ; h < usP1Holes; h++)
			    {
				    for ( int k = 0 ; k < usP2Holes; k++)
				    {
					    Debug.Assert(IntPointsTemp.Count == 0);	
					    C2DPolyBase pHole1 = Poly1.GetHole(h);
                        C2DPolyBase pHole2 = Poly2.GetHole(k);

					    if ( pHole1.BoundingRect.Overlaps( pHole2.BoundingRect) )
					    {
						    pHole1.Lines.GetIntersections( pHole2.Lines, 
							    IntPointsTemp, Indexes1AllHoles[h], Indexes2AllHoles[k],
							    pHole1.BoundingRect, pHole2.BoundingRect);

						    IntPoints1AllHoles[h].AddCopy( IntPointsTemp);
						    IntPoints2AllHoles[k].ExtractAllOf(IntPointsTemp);
					    }
				    }
			    }


			    // *** Hole Routes
			    for (int a = 0 ; a < usP1Holes; a++)
			    {
				    C2DPolyBase pHole = Poly1.GetHole(a);
    				
				    if ( IntPoints1AllHoles[a].Count % 2 != 0)	// Must be even
				    {
				          grid.LogDegenerateError();
				       //   Debug.Assert(false);
				    }

				    if (pHole.Lines.Count != 0)
				    {
					    bool bHole1StartInside = Poly2.Contains( pHole.Lines[0].GetPointFrom() );
					    if ( IntPoints1AllHoles[a].Count == 0)
					    {
						    if ( bHole1StartInside == bP1RoutesInside)
							    CompleteHoles1.Add( new C2DPolyBase(pHole) );
					    }
					    else
					    {
						    pHole.GetRoutes( IntPoints1AllHoles[a], Indexes1AllHoles[a], Routes1, 
										    bHole1StartInside, bP1RoutesInside);
					    }
				    }
			    }
			    // *** Hole Routes	
			    for (int b = 0 ; b < usP2Holes; b++)
			    {
				    C2DPolyBase pHole = Poly2.GetHole(b);

				    if ( IntPoints2AllHoles[b].Count % 2 != 0)	// Must be even
				    {
				          grid.LogDegenerateError();
				      //    Debug.Assert(false);
				    }

				    if (pHole.Lines.Count != 0)
				    {
					    bool bHole2StartInside = Poly1.Contains( pHole.Lines[0].GetPointFrom() );
					    if ( IntPoints2AllHoles[b].Count == 0)
					    {
						    if ( bHole2StartInside == bP2RoutesInside)
							    CompleteHoles2.Add( new C2DPolyBase( pHole) );
					    }
					    else
					    {
						    pHole.GetRoutes( IntPoints2AllHoles[b], Indexes2AllHoles[b], Routes2, 
											    bHole2StartInside, bP2RoutesInside);
					    }
				    }
			    }	
		    }


            //for (unsigned int i = 0 ; i < IntPoints1AllHoles.size(); i++)
            //    delete IntPoints1AllHoles[i];
            //for (unsigned int i = 0 ; i < IntPoints2AllHoles.size(); i++)
            //    delete IntPoints2AllHoles[i];
            //for (unsigned int i = 0 ; i < Indexes1AllHoles.size(); i++)
            //    delete Indexes1AllHoles[i];
            //for (unsigned int i = 0 ; i < Indexes2AllHoles.size(); i++)
            //    delete Indexes2AllHoles[i];

        }

        /// <summary>
        /// Moves this by a small random amount.
        /// </summary>
        public void RandomPerturb()
        {
            C2DPoint pt = _Rim.BoundingRect.GetPointFurthestFromOrigin();
	        double dMinEq = Math.Max(pt.x, pt.y) * Constants.conEqualityTolerance;
	        CRandomNumber rn = new CRandomNumber(dMinEq * 10, dMinEq * 100);

	        C2DVector cVector = new C2DVector( rn.Get(), rn.Get() );
	        if (rn.GetBool())
		        cVector.i = - cVector.i ;
	        if (rn.GetBool())
		        cVector.j = - cVector.j ;

	        Move( cVector );

        }

        /// <summary>
        /// Snaps this to the conceptual grip.
        /// </summary>
        /// <param name="grid">The grid to snap to.</param> 
        public void SnapToGrid(CGrid grid)
        {
            _Rim.SnapToGrid(grid);

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
		        GetHole(i).SnapToGrid(grid);
	        }
        }

        /// <summary>
        /// Returns the boolean result (e.g. union) of 2 shapes. Boolean Operation defined by 
        /// the inside / outside flags.
        /// </summary>
        /// <param name="Other">Other polygon.</param> 
        /// <param name="HoledPolys">Set of polygons to recieve the result.</param> 
        /// <param name="bThisInside">Does the operation require elements of this INSIDE the other.</param> 
        /// <param name="bOtherInside">Does the operation require elements of the other INSIDE this.</param> 
        /// <param name="grid">The grid with the degenerate settings.</param> 
        public void GetBoolean(C2DHoledPolyBase Other, List<C2DHoledPolyBase> HoledPolys,
                            bool bThisInside, bool bOtherInside,
                            CGrid grid)
        {
            if (_Rim.Lines.Count == 0 || Other.Rim.Lines.Count == 0)
		        return;

            if (_Rim.BoundingRect.Overlaps(Other.Rim.BoundingRect))
	        {
		        switch (grid.DegenerateHandling)
		        {
		        case CGrid.eDegenerateHandling.None:
			        {
				        List<C2DPolyBase> CompleteHoles1 = new List<C2DPolyBase>();
				        List<C2DPolyBase> CompleteHoles2 = new List<C2DPolyBase>();
				        C2DLineBaseSetSet Routes1 = new C2DLineBaseSetSet(); 
				        C2DLineBaseSetSet Routes2 = new C2DLineBaseSetSet(); 
				        GetRoutes( this, bThisInside, Other, bOtherInside, Routes1, Routes2,
								        CompleteHoles1, CompleteHoles2, grid);

				        Routes1.ExtractAllOf(Routes2);

				        if (Routes1.Count > 0)
				        {
					        Routes1.MergeJoining();

					        List<C2DPolyBase> Polygons = new List<C2DPolyBase>();

					        for (int i = Routes1.Count - 1; i >= 0; i--)
					        {
						        C2DLineBaseSet pRoute = Routes1[i];
						        if (pRoute.IsClosed(true) )
						        {
							        Polygons.Add(new C2DPolyBase());
							        Polygons[Polygons.Count - 1].CreateDirect(pRoute);
						        }
						        else
						        {
							     //   Debug.Assert(false);
							        grid.LogDegenerateError();
						        }	
					        }

                            C2DHoledPolyBaseSet NewComPolys = new C2DHoledPolyBaseSet();
        					
					        PolygonsToHoledPolygons(NewComPolys, Polygons);

					        NewComPolys.AddKnownHoles( CompleteHoles1 );

					        NewComPolys.AddKnownHoles( CompleteHoles2 );

					        if ( !bThisInside && !bOtherInside && NewComPolys.Count != 1)
					        {
							  //  Debug.Assert(false);
							    grid.LogDegenerateError();
					        }


					        HoledPolys.AddRange(NewComPolys);

                            NewComPolys.Clear();
				        }
			        }
			        break;
		        case CGrid.eDegenerateHandling.RandomPerturbation:
			        {
				        C2DHoledPolyBase OtherCopy = new C2DHoledPolyBase(Other);
				        OtherCopy.RandomPerturb();
                        grid.DegenerateHandling = CGrid.eDegenerateHandling.None;
				        GetBoolean( OtherCopy, HoledPolys, bThisInside, bOtherInside , grid );
                        grid.DegenerateHandling = CGrid.eDegenerateHandling.RandomPerturbation;
			        }
			        break;
		        case CGrid.eDegenerateHandling.DynamicGrid:
			        {
				        C2DRect Rect = new C2DRect();
                        if (_Rim.BoundingRect.Overlaps(Other.Rim.BoundingRect, Rect))
				        {
					        //double dOldGrid = CGrid::GetGridSize();
					        grid.SetToMinGridSize(Rect, false);
                            grid.DegenerateHandling = CGrid.eDegenerateHandling.PreDefinedGrid;
					        GetBoolean( Other, HoledPolys, bThisInside, bOtherInside , grid );
                            grid.DegenerateHandling = CGrid.eDegenerateHandling.DynamicGrid;
				        }
			        }
			        break;
		        case CGrid.eDegenerateHandling.PreDefinedGrid:
			        {
				        C2DHoledPolyBase P1 = new C2DHoledPolyBase(this);
                        C2DHoledPolyBase P2 = new C2DHoledPolyBase(Other);
				        P1.SnapToGrid(grid);
				        P2.SnapToGrid(grid);
				        C2DVector V1 = new C2DVector( P1.Rim.BoundingRect.TopLeft,  P2.Rim.BoundingRect.TopLeft);
				        double dPerturbation = grid.GridSize; // ensure it snaps back to original grid positions.
				        if (V1.i > 0)
                            V1.i = dPerturbation;
                        else
                            V1.i = -dPerturbation;	// move away slightly if possible
				        if (V1.j > 0)
                            V1.j = dPerturbation;
                        else 
                            V1.j = -dPerturbation; // move away slightly if possible
				        V1.i *= 0.411923;// ensure it snaps back to original grid positions.
				        V1.j *= 0.313131;// ensure it snaps back to original grid positions.
        				
				        P2.Move( V1 );
                        grid.DegenerateHandling = CGrid.eDegenerateHandling.None;
				        P1.GetBoolean( P2, HoledPolys, bThisInside, bOtherInside , grid );

                        for (int i = 0 ; i < HoledPolys.Count ; i++)
				            HoledPolys[i].SnapToGrid(grid);

                        grid.DegenerateHandling = CGrid.eDegenerateHandling.PreDefinedGrid;

			        }
			        break;
		        case CGrid.eDegenerateHandling.PreDefinedGridPreSnapped:
			        {
				        C2DHoledPolyBase P2 = new C2DHoledPolyBase(Other);
                        C2DVector V1 = new C2DVector(_Rim.BoundingRect.TopLeft, P2.Rim.BoundingRect.TopLeft);
				        double dPerturbation = grid.GridSize;
				        if (V1.i > 0) 
                            V1.i = dPerturbation; 
                        else
                            V1.i = -dPerturbation; // move away slightly if possible
				        if (V1.j > 0) 
                            V1.j = dPerturbation;
                        else
                            V1.j = -dPerturbation; // move away slightly if possible
				        V1.i *= 0.411923; // ensure it snaps back to original grid positions.
				        V1.j *= 0.313131;// ensure it snaps back to original grid positions.
				        P2.Move( V1 );

                        grid.DegenerateHandling = CGrid.eDegenerateHandling.None;
                        GetBoolean(P2, HoledPolys, bThisInside, bOtherInside, grid);

                        for (int i = 0; i < HoledPolys.Count; i++)
                            HoledPolys[i].SnapToGrid(grid);

                        grid.DegenerateHandling = CGrid.eDegenerateHandling.PreDefinedGridPreSnapped;
			        }
			        break;
		        }// switch
	        }
        }

        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
        public void Transform(CTransformation pProject)
        {
            if (_Rim != null)
	            _Rim.Transform(pProject);

            for (int i = 0; i < _Holes.Count; i++)
            {
	            _Holes[i].Transform(pProject);
            }
        }

        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
        public void InverseTransform(CTransformation pProject)
        {
            if (_Rim != null)
                _Rim.InverseTransform(pProject);

            for (int i = 0; i < _Holes.Count; i++)
            {
                _Holes[i].Transform(pProject);
            }
        }




        /// <summary>
        /// The outer rim.
        /// </summary>
        protected C2DPolyBase _Rim = null;

        /// <summary>
        /// Rim access. 
        /// </summary>
        public C2DPolyBase Rim
        {
            get
            {
                return _Rim;
            }
            set
            {
                _Rim = value;
            }
        }



        /// <summary>
        /// Holes.
        /// </summary>
        protected List<C2DPolyBase> _Holes = new List<C2DPolyBase>();

        /// <summary>
        /// Hole count.
        /// </summary>
        public int HoleCount
        {
            get
            {
                return _Holes.Count;
            }
        }

        /// <summary>
        /// Hole access.
        /// </summary>
        public C2DPolyBase GetHole(int i)
        {
           return _Holes[i];
        }
        /// <summary>
        /// Hole assignment.
        /// </summary>
        public void SetHole(int i, C2DPolyBase Poly)
        {
            _Holes[i] = Poly;
        }
        /// <summary>
        /// Hole addition.
        /// </summary>
        public void AddHole(C2DPolyBase Poly)
        {
            _Holes.Add(Poly);
        }
        /// <summary>
        /// Hole removal.
        /// </summary>
        public void RemoveHole(int i)
        {
            _Holes.RemoveAt(i);
        }

    }
}
