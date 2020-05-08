using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{

    /// <summary>
    /// Class to represent a 2D polygon with optional curved lines.
    /// </summary>
    public class C2DPolyArc : C2DPolyBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DPolyArc() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">The other polygon.</param> 
	    public C2DPolyArc(C2DPolyBase Other) : base(Other)
        {
            
        }
        /// <summary>
        /// Constructor.
        /// </summary>
	    public C2DPolyArc(C2DPolyArc Other): base(Other)
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
	    ~C2DPolyArc() {}
        /// <summary>
	    /// Sets the starting point.
        /// </summary>
        /// <param name="Point">The start point.</param> 
	    public void SetStartPoint(C2DPoint Point)
        {
	        Clear();

	        Lines.Add(new C2DLine(Point, Point));
        }

        /// <summary>
        /// Arc to a new point.
        /// </summary>
        /// <param name="Point">The point to go to.</param> 
        /// <param name="dRadius">The radius.</param> 
        /// <param name="bCentreOnRight">Indicates whether the centre of the arc is to the right of the line.</param> 
        /// <param name="bArcOnRight">indicates whether the curve is to the right i.e. anti-clockwise.</param> 
        public void LineTo(C2DPoint Point, double dRadius,
		                        bool bCentreOnRight, bool bArcOnRight)
        {
	        if (Lines.Count == 0)
		        return;

	        C2DArc pLine = new C2DArc( Lines[Lines.Count - 1].GetPointTo(), Point, 
								        dRadius, bCentreOnRight, bArcOnRight);

	        if (Lines.Count == 1 && Lines[0] is C2DLine &&
                Lines[0].GetPointTo().PointEqualTo(Lines[0].GetPointFrom()))  // CR 19-1-09
	        {
                Lines[0] = pLine;
	        }
	        else
	        {
                Lines.Add(pLine);
	        }
        }

        /// <summary>
        /// Adds a point which is a striaght line from the previous.
        /// </summary>
        /// <param name="Point">The point to go to.</param> 
	    public void LineTo(C2DPoint Point)
        {
	        if (Lines.Count == 0)
		        return;

	        C2DLine pLine = new C2DLine( Lines[Lines.Count - 1].GetPointTo(), Point );

	        if (Lines.Count == 1 && Lines[0] is C2DLine &&
                Lines[0].GetPointTo().PointEqualTo(Lines[0].GetPointFrom()))  // CR 19-1-09
	        {
                Lines[0] = pLine;
	        }
	        else
	        {
                Lines.Add(pLine);
	        }
        }

        /// <summary>
        /// Close with a curved line back to the first point.
        /// </summary>
        /// <param name="dRadius">The radius.</param> 
        /// <param name="bCentreOnRight">Indicates whether the centre of the arc is to the right of the line.</param> 
        /// <param name="bArcOnRight">indicates whether the curve is to the right i.e. anti-clockwise.</param> 
	    public void Close(double dRadius, bool bCentreOnRight, bool bArcOnRight)
        {
	        if (Lines.Count == 0)
		        return;

	        Lines.Add( new C2DArc( Lines[Lines.Count - 1].GetPointTo(), Lines[0].GetPointFrom(), 
							        dRadius, bCentreOnRight, bArcOnRight));

	        MakeLineRects();
	        MakeBoundingRect();

        }

        /// <summary>
        /// Close with a straight line back to the first point.
        /// </summary>
	    public void Close()
        {
	        if (Lines.Count == 0)
		        return;

	        Lines.Add( new C2DLine( Lines[Lines.Count - 1].GetPointTo(), Lines[0].GetPointFrom() ));

	        MakeLineRects();
	        MakeBoundingRect();

        }
        /// <summary>
        /// Creates a random shape.
        /// </summary>
        /// <param name="cBoundary">The boundary.</param> 
        /// <param name="nMinPoints">The minimum points.</param> 
        /// <param name="nMaxPoints">The maximum points.</param> 
	    public bool CreateRandom(C2DRect cBoundary, int nMinPoints, int nMaxPoints)
        {
	        C2DPolygon Poly = new C2DPolygon();
	        if (!Poly.CreateRandom(cBoundary, nMinPoints, nMaxPoints))
		        return false;

	        CRandomNumber rCenOnRight = new CRandomNumber(0, 1);

	        this.Set( Poly );

	        for (int i = 0 ; i < Lines.Count; i ++)
	        {
		        C2DLineBase pLine = Lines[i];

		        bool bCenOnRight = (rCenOnRight.GetInt() > 0 );
		        double dLength = pLine.GetLength();
		        CRandomNumber Radius = new CRandomNumber(dLength , dLength * 3);


		        C2DArc pNew = new C2DArc( pLine.GetPointFrom(), pLine.GetPointTo(), 
							        Radius.Get(), bCenOnRight, !bCenOnRight);

		        if (!this.Crosses( pNew ))
		        {
                    Lines[i] = pNew;
                    pNew.GetBoundingRect(LineRects[i]);
                    BoundingRect.ExpandToInclude(LineRects[i]);
		        }
	        }

	   //     this.MakeLineRects();
	  //      this.MakeBoundingRect();

	        return true;
        }


        /// <summary>
        /// Gets the non overlaps i.e. the parts of this that aren't in the other.
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetNonOverlaps(C2DPolyArc Other, List<C2DHoledPolyArc> Polygons, 
										    CGrid grid) 
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetNonOverlaps(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolyArc(NewPolys[i]));
        }

        /// <summary>
        /// Gets the union of the 2 shapes.
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetUnion(C2DPolyArc Other, List<C2DHoledPolyArc> Polygons,
										    CGrid grid) 
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetUnion(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolyArc(NewPolys[i]));
        }


        /// <summary>
        /// Gets the overlaps of the 2 shapes.	
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetOverlaps(C2DPolyArc Other, List<C2DHoledPolyArc> Polygons,
										    CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetOverlaps(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolyArc(NewPolys[i]));
        }

        /// <summary>
        /// Gets the area.
        /// </summary>
	    public double GetArea() 
        {
	        double dArea = 0;

	        for (int i = 0; i < Lines.Count; i++)
	        {
                C2DPoint pt1 = _Lines[i].GetPointFrom();
                C2DPoint pt2 = _Lines[i].GetPointTo();

                dArea += pt1.x * pt2.y - pt2.x * pt1.y;
	        }
	        dArea = dArea / 2.0;

	        for (int i = 0; i < Lines.Count; i++)	
	        {
		        if (_Lines[i] is C2DArc)
		        {
			        C2DArc Arc = _Lines[i] as C2DArc;

			        C2DSegment Seg = new C2DSegment( Arc );

			        dArea += Seg.GetAreaSigned();
		        }
	        }
        	
	        return Math.Abs(dArea);

        }

        /// <summary>
        /// Returns the centroid.
        /// </summary>
	    public C2DPoint GetCentroid() 
        {
	        // Find the centroid and area of the straight line polygon.
	        C2DPoint Centroid = new C2DPoint(0, 0);
	     //   C2DPoint pti = new C2DPoint();
	     //   C2DPoint ptii;
	        double dArea = 0;

	        for (int i = 0; i < Lines.Count; i++)
	        {
		        C2DPoint pti = Lines[i].GetPointFrom();
		        C2DPoint ptii = Lines[i].GetPointTo();

		        Centroid.x += (pti.x + ptii.x) * (pti.x * ptii.y - ptii.x * pti.y);
		        Centroid.y += (pti.y + ptii.y) * (pti.x * ptii.y - ptii.x * pti.y);

		        dArea += pti.x * ptii.y - ptii.x * pti.y;
	        }
	        dArea = dArea / 2.0;

	        Centroid.x = Centroid.x / (6.0 * dArea);
	        Centroid.y = Centroid.y / (6.0 * dArea);

	        List<double> dSegAreas = new List<double>();
	        double dTotalArea = dArea;
	        List<C2DPoint> SegCentroids = new List<C2DPoint>();

	        for (int i = 0; i < Lines.Count; i++)
	        {
		        if (Lines[i] is C2DArc)
		        {
			        C2DSegment Seg = new C2DSegment( Lines[i] as C2DArc );
			        double dSegArea = Seg.GetAreaSigned();
			        dTotalArea += dSegArea;
			        dSegAreas.Add( dSegArea );
			        SegCentroids.Add( Seg.GetCentroid() );
		        }
	        }

	        Centroid.Multiply( dArea);

	        for (int i = 0; i < dSegAreas.Count; i++)
	        {
		        Centroid.x += SegCentroids[i].x * dSegAreas[i];
                Centroid.y += SegCentroids[i].y * dSegAreas[i];
	        }

            Centroid.Multiply( 1/ dTotalArea);
	        return Centroid;

        }
        
	    /// Rotates the polygon to the right around the centroid.
        /// 
        /// <summary>
        /// Rotates the shape to the right about the centroid.	
        /// </summary>
        /// <param name="dAng">The angle to rotate by.</param> 
        public void RotateToRight(double dAng)
        {
	        RotateToRight( dAng, GetCentroid());
        }


    }
}
