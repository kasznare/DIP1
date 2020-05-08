using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace GeoLib
{

    /// <summary>
    /// Class representing a polygon with straight lines. Inherits from C2DPolyBase
    /// but only allows straight lines to be added.
    /// </summary>
    public class C2DPolygon : C2DPolyBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DPolygon() 
        { 
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Points">The points to create from.</param>
        /// <param name="bReorderIfNeeded">True if reordering is required.</param>
	    public C2DPolygon(List<C2DPoint> Points, bool bReorderIfNeeded)
        {
	        subArea1 = null;
	        subArea2 = null;

	        Create(Points, bReorderIfNeeded);

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
	    public C2DPolygon(C2DPolygon Other) 
        {
            Set(Other);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
	    public C2DPolygon(C2DPolyBase Other)
        {
            Clear();

            for (int i = 0; i < Other.Lines.Count; i++)
            {
                if (Other.Lines[i] is C2DLine)
                {
                    Lines.Add(new C2DLine(Other.Lines[i] as C2DLine));
                }
                else
                {
                    Debug.Assert(false, "C2DPolygon creation with none straight line");
                }
            }

            MakeLineRects();
            MakeBoundingRect();

        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Other">The other to set to.</param>
        public new void Set(C2DPolyBase Other)
        {
            Clear();

            for (int i = 0; i < Other.Lines.Count; i++)
            {
                if (Other.Lines[i] is C2DLine)
                {
                    Lines.Add(new C2DLine(Other.Lines[i] as C2DLine));
                }
                else
                {
                    Debug.Assert(false, "C2DPolygon creation with none straight line");
                }
            }

            MakeLineRects();
            MakeBoundingRect();
        }


        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Other">The other to set to.</param>
        public void Set(C2DPolygon Other)
        {
            Clear();

            base.Set(Other);

            if (Other.SubArea1 != null)
            {
                subArea1 = new C2DPolygon();
                subArea1.Set(Other.SubArea1);
            }
            if (Other.SubArea2 != null)
            {
                subArea2 = new C2DPolygon();
                subArea2.Set(Other.SubArea1);
            }
        }


        /// <summary>
        /// Destructor.
        /// </summary>
        ~C2DPolygon() { }

        /// <summary>
        /// Creates the polygon with optional reordering of points.
        /// </summary>
        /// <param name="Points">The points to create from.</param>
        /// <param name="bReorderIfNeeded">True if reordering is required.</param>
        public bool Create(List<C2DPoint> Points, bool bReorderIfNeeded )
        {
		    subArea1 = null;
            subArea2 = null;

	        if (Points.Count < 3)
		        return false;

	        MakeLines(Points);

	        MakeLineRects();

	        if (!IsClockwise())
		        ReverseDirection();

	        MakeBoundingRect();

	        if (bReorderIfNeeded && HasCrossingLines())
		        return Reorder();

	        return true;
        }

        /// <summary>
        /// Creates a regular polygon.
        /// </summary>
        /// <param name="Centre">The centre.</param>
        /// <param name="dDistanceToPoints">The distance to each point.</param>
        /// <param name="nNumberSides">True number of sides.</param>
        public bool CreateRegular(C2DPoint Centre, double dDistanceToPoints, int nNumberSides)
        {
	        Clear();

	        if (dDistanceToPoints == 0 || nNumberSides < 3) 
                return false;

	        double dAngle =  Constants.conTWOPI / nNumberSides ;
	        C2DVector Vector = new C2DVector( 0 , dDistanceToPoints);
	        C2DLine LineToEachPt = new C2DLine(Centre, Vector);

	        C2DPointSet Points = new C2DPointSet();

	        for (int i = 0 ; i < nNumberSides; i ++)
	        {
		        C2DPoint pNewPt = new C2DPoint();
		        pNewPt.Set(LineToEachPt.GetPointTo());
		        Points.Add(pNewPt);
		        LineToEachPt.vector.TurnRight(dAngle);
	        }

	        return Create(Points, false);
        }

        /// <summary>
        /// Creates a convex hull from another polygon. Uses Graham's algorithm.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        public bool CreateConvexHull(C2DPolygon Other)
        {
            Clear();

            C2DPointSet Points = new C2DPointSet();
            Other.GetPointsCopy(Points);

            C2DPointSet Hull = new C2DPointSet();
            Hull.ExtractConvexHull(Points);

            return Create(Hull, false);

        }

        /// <summary>
        /// Creates a randon polygon.
        /// </summary>
        /// <param name="cBoundary">The boundary of the random shape.</param>
        /// <param name="nMinPoints">The minimum number of points.</param>
        /// <param name="nMaxPoints">The maximum number of points.</param>
	    public bool CreateRandom(C2DRect cBoundary, int nMinPoints, int nMaxPoints)
        {
	        Clear();

	        Debug.Assert(nMinPoints <= nMaxPoints);

	        if (nMinPoints < 3) 
		        return false;
	        if (nMinPoints > nMaxPoints)
		        return false;

            CRandomNumber rnNumber = new CRandomNumber(nMinPoints, nMaxPoints);

	        int nNumber = rnNumber.GetInt();

	        C2DPoint pt = new C2DPoint();
	        CRandomNumber rnX = new CRandomNumber(cBoundary.TopLeft.x, cBoundary.BottomRight.x);
	        CRandomNumber rnY = new CRandomNumber(cBoundary.BottomRight.y, cBoundary.TopLeft.y);

	        C2DPointSet Points = new C2DPointSet();

	        for (int i = 0 ; i < nNumber; i++)
	        {
		        pt.x = rnX.Get();
		        pt.y = rnY.Get();

		        Points.AddCopy(pt);
	        }

	        return Create(Points, true);

        }

        /// <summary>
        /// Mophs this polygon into another by the factor given.
        /// </summary>
        /// <param name="OtherFrom">The other shape from.</param>
        /// <param name="OtherTo">The other shape to.</param>
        /// <param name="dFactor">The factor between the 2 polygons.</param>
        public bool CreateMorph(C2DPolygon OtherFrom, C2DPolygon OtherTo, double dFactor)
        {
	        int nOtherFromCount = OtherFrom._Lines.Count;
	        int nOtherToCount = OtherTo._Lines.Count;

	        if (nOtherToCount < 3 || nOtherFromCount < 3)
		        return false;

	        if (nOtherFromCount > nOtherToCount)
	        {
		        return CreateMorph(OtherTo, OtherFrom, 1 - dFactor);
	        }
	        else
	        {
		        // Going from poly with less points to poly with more.
		        C2DPointSet Points = new C2DPointSet();

		        int nOtherFromLeft = OtherFrom.GetLeftMostPoint();
		        // Add the OtherFroms points starting from the left most.
		        for (int i = 0; i < OtherFrom.Lines.Count; i++)
		        {
			        C2DPoint pNewPoint = new C2DPoint();
			        pNewPoint.Set(  OtherFrom.Lines[(i + nOtherFromLeft)% OtherFrom.Lines.Count].GetPointFrom());
			        Points.Add(pNewPoint);
		        }

		        int nPointsToAdd = nOtherToCount - nOtherFromCount; // we know this is positive.

		        int nPointsAdded = 0;
		        int nLine = 0;
        		
		        // Add points to the list so that it is the same size as OtherTo.
		        while (nPointsAdded < nPointsToAdd)
		        {
			        C2DLine TempLine = new C2DLine( Points[nLine], Points[nLine + 1]);

			        C2DPoint pNewPoint = new C2DPoint();
			        pNewPoint = TempLine.GetMidPoint();
			        Points.Insert(nLine + 1, pNewPoint);
			        nLine +=2;
			        nPointsAdded ++;
			        if (nLine > Points.Count -2 )
				        nLine = 0;
		        }

		        int nOtherToLeft = OtherTo.GetLeftMostPoint();

		        Debug.Assert(Points.Count == nOtherToCount);

		        for ( int i = 0 ; i < nOtherToCount ; i++ )
		        {
			        C2DVector vMove = new C2DVector(Points[i] , OtherTo.Lines[(nOtherToLeft + i) % OtherTo.Lines.Count].GetPointFrom());
			        vMove.Multiply( dFactor);

			        Points[i].Move(vMove);
		        }

		        return Create(Points, false);
	        }
        }

        /// <summary>
        /// Creates convex sub areas of the current polygon. These can then be extracted. 
        /// This function is also useful when obtaining minimum translation vectors. 
        /// </summary>
        public bool CreateConvexSubAreas()
        {
	        subArea1 = null;
	        subArea2 = null;

	        int nLineCount = Lines.Count;

	        if ( nLineCount < 4 )
		        return true;

	        bool bInflection = false;
	        for (int nStart = 0 ; nStart < nLineCount; nStart++)
	        {
		        if (IsPointInflected(nStart))
		        {
			        bInflection = true;

			        int nEnd = nStart + 2;
			        bool bContinue = true;
			        while (bContinue)
			        {
				        if (C2DTriangle.IsClockwise(GetPoint(nEnd - 2),GetPoint(nEnd - 1),GetPoint(nEnd))
					        && C2DTriangle.IsClockwise( GetPoint(nEnd - 1), GetPoint(nEnd), GetPoint(nStart))
					        && C2DTriangle.IsClockwise( GetPoint(nEnd), GetPoint(nStart), GetPoint(nStart + 1)) 
					        && CanPointsBeJoined(nStart, nEnd))
				        {
					        nEnd++;
				        }
				        else
				        {
					        nEnd--;
					        bContinue = false;
				        }
			        }
			        if (nEnd >= nStart + 2)
			        {
                        bool bRes = CreateSubAreas(nStart, nEnd, ref subArea1,ref subArea2);
                        bRes &= SubArea1.CreateConvexSubAreas();
                        bRes &= SubArea2.CreateConvexSubAreas();
				        return bRes;
			        }
		        }
	        }

	        if (!bInflection)
		        return true;

	        for (int nStart = 2 * nLineCount - 1 ; nStart >= nLineCount; nStart--)
	        {
		        if (IsPointInflected(nStart))
		        {
			        bInflection = true;

			        int nEnd = nStart - 2;
			        bool bContinue = true;
			        while (bContinue)
			        {
				        if (!C2DTriangle.IsClockwise(GetPoint(nEnd + 2),GetPoint(nEnd + 1),GetPoint(nEnd)) 
					        && !C2DTriangle.IsClockwise( GetPoint(nEnd + 1), GetPoint(nEnd), GetPoint(nStart)) 
					        && !C2DTriangle.IsClockwise( GetPoint(nEnd), GetPoint(nStart), GetPoint(nStart - 1)) 
					        && CanPointsBeJoined(nStart, nEnd))
				        {
					        nEnd--;
				        }
				        else
				        {
					        nEnd++;
					        bContinue = false;
				        }
			        }
			        if (nEnd <= nStart - 2)
			        {
				        bool bRes = CreateSubAreas(nStart, nEnd, ref subArea1, ref subArea2);
				        bRes &= SubArea1.CreateConvexSubAreas();
				        bRes &= SubArea2.CreateConvexSubAreas();
				        return bRes;
			        }
		        }
	        }


	        Debug.Assert(false);
	        return false;


        }

        /// <summary>
        /// Removes the convex sub areas.
        /// </summary>
	    public void ClearConvexSubAreas()
        {
             subArea1 = null;
             subArea2 = null;
        }

        /// <summary>
        /// True if the polygon is convex.
        /// </summary>
	    public bool IsConvex()
        {
	        if (Lines.Count < 4) 
                    return true;
	        int nTemp = 0;
	        return !FindFirstInflection(ref nTemp);
        }

        /// <summary>
        /// Clears all.
        /// </summary>
        public new void Clear()
        {
            this.ClearConvexSubAreas();
            base.Clear();
        }

        /// <summary>
        /// Rotates the polygon to the right around the centroid.
        /// </summary>
        /// <param name="dAng">The angle to rotate by.</param>
        public void RotateToRight(double dAng)
        {
            RotateToRight(dAng, GetCentroid());
        }

        /// <summary>
        /// Grows the polygon from the centre.
        /// </summary>
        /// <param name="dFactor">The factor to grow by.</param>
        public void Grow(double dFactor)
        {
            Grow(dFactor, GetCentroid());
        }

        /// <summary>
        /// Moves this point by the vector given.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public override void Move(C2DVector vector)
        {
            base.Move(vector);
            if (subArea1 != null)
                subArea1.Move(vector);
            if (subArea2 != null)
                subArea2.Move(vector);
        }
        /// <summary>
        /// Rotates this to the right about the origin provided.
        /// </summary>
        /// <param name="dAng">The angle in radians through which to rotate.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
        public override void RotateToRight(double dAng, C2DPoint Origin)
        {
            base.RotateToRight(dAng, Origin);
            if (subArea1 != null)
                subArea1.RotateToRight(dAng, Origin);
            if (subArea2 != null)
                subArea2.RotateToRight(dAng, Origin);

        }

        /// <summary>
        /// Grows the polygon around the origin.
        /// </summary>
        /// <param name="dFactor">The factor to grow by.</param>
        /// <param name="Origin">The origin about which to grow.</param>
        public override void Grow(double dFactor, C2DPoint Origin)
        {
            base.Grow(dFactor, Origin);
            if (subArea1 != null)
                subArea1.Grow(dFactor, Origin);
            if (subArea2 != null)
                subArea2.Grow(dFactor, Origin);

        }

        /// <summary>
        /// Reflects the area about the point.
        /// </summary>
        /// <param name="point">The point to reflect through.</param>
        public override void Reflect(C2DPoint point)
        {
            base.Reflect(point);
            if (subArea1 != null)
                subArea1.Reflect(point);
            if (subArea2 != null)
                subArea2.Reflect(point);
        }

        /// <summary>
        /// Reflects throught the line provided.
        /// </summary>
        /// <param name="Line">The line to reflect through.</param>
        public override void Reflect(C2DLine Line)
        {
            base.Reflect(Line);
            if (subArea1 != null)
                subArea1.Reflect(Line);
            if (subArea2 != null)
                subArea2.Reflect(Line);

        }




         /// <summary>
         /// True if there are repeated points.
         /// </summary>
	    bool HasRepeatedPoints()
        {
            for ( int i = 0 ; i < Lines.Count; i++)
	        {
                for (int r = i + 1; r < Lines.Count; r++)
		        {
			        if (Lines[i].GetPointFrom().PointEqualTo(  Lines[r].GetPointFrom()  )) 
				        return true;
		        }
	        }
	        return false;
        }

        /// <summary>
        /// True if clockwise.
        /// </summary>
	    public bool IsClockwise()
        {
	        Debug.Assert(Lines.Count> 2);

	        return (GetAreaSigned() < 0);

        }

        /// <summary>
        /// Returns the convex sub areas if created.
        /// </summary>
        /// <param name="SubAreas">Ouput. The sub areas.</param>
	    public void GetConvexSubAreas(List< C2DPolygon> SubAreas)
        {
            if (SubArea1 != null && SubArea2 != null)
            {
                SubArea1.GetConvexSubAreas(SubAreas);
                SubArea2.GetConvexSubAreas(SubAreas);
            }
            else
            {
                SubAreas.Add(this);
            }
        }

        /// <summary>
	    /// True if this overlaps another and returns the translation vector required to move
	    /// this apart. Exact if this is convex, approximate if concave. Better approximation
	    /// if convex sub areas have been created.
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        /// <param name="MinimumTranslationVector">Ouput. The vector to move this by to move it away from the other.</param>
        public bool Overlaps(C2DPolygon Other, C2DVector MinimumTranslationVector)
        {
	        if (Lines.Count < 2 || Other.Lines.Count < 2) 
                return false;

	        if (!_BoundingRect.Overlaps(Other.BoundingRect))
		        return false;

	        if (SubArea1 != null && SubArea2 != null)
	        {

		        C2DVector v1 = new C2DVector();
		        C2DVector v2 = new C2DVector();
		        bool b1 = SubArea1.Overlaps(Other, v1);
		        bool b2 = SubArea2.Overlaps(Other, v2);
		        if (b1 && b2) 
                    MinimumTranslationVector.Set( v1 + v2);
		        else if (b1) 
                    MinimumTranslationVector.Set(v1);
		        else if (b2) 
                    MinimumTranslationVector.Set(v2);
		        return b1 || b2;
	        }
	        else if (Other.SubArea1 != null && Other.SubArea2 != null)
	        {
		        bool bRes = Other.Overlaps(this, MinimumTranslationVector);
		        if (bRes) 
                    MinimumTranslationVector.Reverse();
		        return bRes;
	        }
	        else
	        {
		        CInterval ThisProj = new CInterval();
		        CInterval OtherProj = new CInterval();

		        C2DLine ProjLine = new C2DLine();

		        bool bVecFound = false;

		        for (int i = 0 ; i < Lines.Count + Other.Lines.Count; i++)
		        {
                    if (i < Lines.Count)
				        ProjLine.Set(GetPoint(i),GetPoint(i+1));
			        else
                        ProjLine.Set(Other.GetPoint(i - Lines.Count),
                            Other.GetPoint(i - Lines.Count + 1));

			        ProjLine.vector.TurnRight();
			        ProjLine.vector.MakeUnit();

			        this.Project(ProjLine.vector, ThisProj );
			        Other.Project(ProjLine.vector, OtherProj );

			        if (ThisProj.dMin < OtherProj.dMax && ThisProj.dMax > OtherProj.dMax)
			        {
				        if (!bVecFound || 
					        (OtherProj.dMax - ThisProj.dMin) < MinimumTranslationVector.GetLength())
				        {
					        MinimumTranslationVector.Set( ProjLine.vector);
					        MinimumTranslationVector.SetLength(OtherProj.dMax - ThisProj.dMin);
					        MinimumTranslationVector.Multiply(1.001);

					        bVecFound = true;
				        }
			        }
			        else if (OtherProj.dMin <ThisProj.dMax && OtherProj.dMax > ThisProj.dMax )
			        {
				        if (!bVecFound || 
					        (ThisProj.dMax - OtherProj.dMin) < MinimumTranslationVector.GetLength())
				        {
					        MinimumTranslationVector.Set( ProjLine.vector);
					        MinimumTranslationVector.SetLength(ThisProj.dMax - OtherProj.dMin);
					        MinimumTranslationVector.Reverse();
                            MinimumTranslationVector.Multiply(1.001);
					        bVecFound = true;
				        }
			        }
			        else
			        {
				        return false;
			        }
		        }
	        }

	        return true;

        }

        /// <summary>
        /// Moves this away from the other by the minimal amount
        /// </summary>
        /// <param name="Other">The other polygon.</param>
        public void Avoid(C2DPolygon Other)
        {
            C2DVector vTrans = new C2DVector();

            if (this.Overlaps(Other, vTrans))
                this.Move(vTrans);

        }

        /// <summary>
        /// Returns the centroid.
        /// </summary>
        public C2DPoint GetCentroid() 
        {
	        C2DPoint Centroid = new C2DPoint(0, 0);
	        double dArea = 0;

	        for ( int i = 0; i < Lines.Count; i++)
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

	        return Centroid;

        }

        /// <summary>
        /// Returns the area.
        /// </summary>
        public double GetArea()
        {
            return Math.Abs(GetAreaSigned());
        }

        /// <summary>
        /// Returns the area signed (-ve if clockwise).
        /// </summary>
        public double GetAreaSigned()
        {
	        double dArea = 0;

	        for (int i = 0; i < Lines.Count; i++)
	        {
                C2DPoint pt1 = Lines[i].GetPointFrom();
                C2DPoint pt2 = Lines[i].GetPointTo();
                dArea += pt1.x * pt2.y - pt2.x * pt1.y;
	        }
	        dArea = dArea / 2.0;

	        return dArea;

        }

        /// <summary>
        /// Gets the point at the specified index. Index is cyclic.
        /// </summary>
        /// <param name="nPointIndex">The index of the point.</param>
	    C2DPoint GetPoint(int nPointIndex)
        {
	        if (Lines.Count == 0)
		        return new C2DPoint();

	        return Lines[nPointIndex  %  Lines.Count].GetPointFrom();

        }

        /// <summary>
        /// Copies the points into the set object provided.
        /// </summary>
        /// <param name="PointCopy">The point set to recieve the result.</param>
	    public void GetPointsCopy(List<C2DPoint> PointCopy)
        {
	        for (int i = 0; i < Lines.Count; i++)
	        {
		        PointCopy.Add(Lines[i].GetPointFrom());
	        }
        }


        /// <summary>
        /// Returns the left most point.
        /// </summary>
        public int GetLeftMostPoint()
        {
	        if (Lines.Count < 2) 
                return 0;
        	
	        int nRes = 0;

	        for ( int i = 1 ; i < Lines.Count; i++)
	        {
		        if (Lines[i].GetPointFrom().x < Lines[nRes].GetPointFrom().x)
			        nRes = i;
	        }

	        return nRes;
        }

        /// <summary>
        /// Smooths the polygon. 
        /// </summary>
        /// <param name="dMinAngleDegrees">The minimum angle between points. Default is 144 degrees.</param>
        /// <param name="dCropFactor">The factor to crop "sharp" lines by. Default is 0.8.</param>
	    public void Smooth(double dMinAngleDegrees, double dCropFactor)
        {
            double dMinAngle = dMinAngleDegrees * Constants.conRadiansPerDegree;

	        int i = 0;

	        if (Lines.Count < 3)
		        return;

	        Debug.Assert(IsClockwise());

	        int nCount = Lines.Count;
	        int nIt = 0;

	        while (nIt < nCount )
	        {
		        C2DLineBase LineBase1 = Lines[i % Lines.Count] ;
                C2DLineBase LineBase2 = Lines[(i + 1) % Lines.Count];

                C2DLine Line1 = new C2DLine(LineBase1.GetPointFrom(), LineBase1.GetPointTo());
                C2DLine Line2 = new C2DLine(LineBase2.GetPointFrom(), LineBase2.GetPointTo());

		        C2DVector Vec = new C2DVector(Line1.vector);

		        Vec.Reverse();

		        double dAng = Line2.vector.AngleToRight(Vec) ;
		        if(dAng <  dMinAngle || dAng > (Constants.conTWOPI - dMinAngle))
		        {
			        SetPoint( Line1.GetPointOn(dCropFactor),  (i + 1));
			        InsertPoint(i +2, Line2.GetPointOn(1 - dCropFactor));
			        nCount++;
			        i += 2;
			        nIt = 0;
		        }
		        else
		        {
			        i++;
			        nIt++;
		        }
		        if (i >= nCount)
			        i = 0;
	        }

	        MakeBoundingRect();

	        if (SubArea1 != null && SubArea2 != null)
		        CreateConvexSubAreas();

        }

        /// <summary>
        /// Smooths the polygon using default smoothing i.e. 144 degrees and 0.8 crop factor. 
        /// </summary>
	    public void Smooth()
        {
            Smooth(Constants.conPI * 0.8 * Constants.conDegreesPerRadian, 0.8 );
        }

        /// <summary>
        /// Get the minimum bounding circle.
        /// </summary>
        /// <param name="Circle">Output. The bounding circle.</param>
        public void GetBoundingCircle(C2DCircle Circle)
        {
            C2DPointSet Points = new C2DPointSet();
            GetPointsCopy(Points);

            Points.GetBoundingCircle(Circle);
        }

        /// <summary>
        /// Finds the index of the first inflected point if there is one.
        /// </summary>
        /// <param name="nFirstInflection">Output. Inflected point.</param>
        private bool FindFirstInflection(ref int nFirstInflection)
        {
	        for ( int i = 0; i < Lines.Count; i++)
	        {
		        if(IsPointInflected(i))
		        {
			        nFirstInflection = i;
			        return true;
		        }
	        }
	        return false;

        }

        /// <summary>
        /// True if the point is inflected.
        /// </summary>
        /// <param name="nIndex">The point index.</param>
        public bool IsPointInflected(int nIndex)
        {
	        int usBefore;
	        if (nIndex == 0)
		        usBefore = Lines.Count - 1;
	        else
		        usBefore = nIndex - 1;

	        C2DLine TestLine = new C2DLine(GetPoint(usBefore), GetPoint(nIndex));

	        Debug.Assert(IsClockwise());

	        return !TestLine.IsOnRight(GetPoint(nIndex + 1 ) );

        }

        /// <summary>
        /// True if the points can be joined with no resulting crossing lines.
        /// </summary>
        /// <param name="nStart">The first point index.</param>
        /// <param name="nEnd">The second point index.</param>
	    public bool CanPointsBeJoined(int nStart, int nEnd)
        {
	        int usBefore = 0;
	        if (nStart == 0)
		        usBefore = Lines.Count - 1;
	        else
		        usBefore = nStart - 1;

	        C2DVector VecBefore = new C2DVector(GetPoint(nStart), GetPoint(usBefore)); 
	        C2DVector VecAfter = new C2DVector(GetPoint(nStart), GetPoint(nStart + 1)); 

	        C2DLine TestLine = new C2DLine( GetPoint(nStart), GetPoint(nEnd));

	        Debug.Assert(IsClockwise());

	        if( VecAfter.AngleToRight(TestLine.vector) < 
	                VecAfter.AngleToRight(VecBefore))
	        {
		        TestLine.GrowFromCentre(0.99999);
		        if (!this.Crosses(TestLine)) 
                    return true;
	        }

	        return false;

        }


        /// <summary>
        /// Creates sub areas given 2 point indexes and pointers to the new areas.
        /// </summary>
        /// <param name="nPt1">The first point index.</param>
        /// <param name="nPt2">The second point index.</param>
        /// <param name="pNewArea1">The first subarea.</param>
        /// <param name="pNewArea2">The second subarea.</param>
	    private bool CreateSubAreas( int nPt1,  int nPt2, ref C2DPolygon pNewArea1, ref C2DPolygon pNewArea2)
        {
	        pNewArea1 = new C2DPolygon();
	        pNewArea2 = new C2DPolygon();

	        while (nPt2 < nPt1) 
                nPt2 += Lines.Count;

	        C2DPointSet Points1 = new C2DPointSet();
	        for (int i = nPt1; i <= nPt2; i++)
	        {
		        Points1.Add(GetPoint(i));
	        }
	        bool bRes1 = pNewArea1.Create(Points1, false);

	        while (nPt1 < nPt2) 
                nPt1 += Lines.Count;

	        C2DPointSet Points2 = new C2DPointSet();
	        for ( int j = nPt2; j <= nPt1; j++)
	        {
		        Points2.Add(GetPoint(j));
	        }
            bool bRes2 = pNewArea2.Create(Points2, false);

	        return bRes1 && bRes2;

        }

        /// <summary>
        /// Reorders the points to minimise perimeter.
        /// </summary>
        public bool Reorder()
        {
            if (Lines.Count < 4)
                return true;
            // Get a copy of the points.
            C2DPointSet Points = new C2DPointSet();
            GetPointsCopy(Points);

            // Make a convex hull from them.
            C2DPointSet Hull = new C2DPointSet();
            Hull.ExtractConvexHull(Points);
            // Close the hull.
            Hull.AddCopy(Hull[0]);
            // Get the bounding rect for the hull and sort the rest by the distance from it.
            C2DRect Rect = new C2DRect();
            Hull.GetBoundingRect(Rect);
            Points.SortByDistance(Rect.GetCentre());
            Points.Reverse();

            // Set up the travelling saleman and give him the route i.e. the now closed hull.
            CTravellingSalesman TS = new CTravellingSalesman();
            TS.SetPointsDirect(Hull);

            // Insert the rest starting with the closest (hopefully).
            while (Points.Count > 0)
                TS.InsertOptimally(Points.ExtractAt(Points.Count - 1));

            // Refine the TS.
            TS.Refine();

            // Get the points back
            TS.ExtractPoints(Points);

            // Remove the closure.
            Points.RemoveAt(Points.Count - 1);

            // Make the lines again.
            MakeLines(Points);
            // Make the rectangles again.
            MakeLineRects();
            // Reverse direction if needed.
            if (!IsClockwise())
                ReverseDirection();
            // Remake the bounding rectangle.
            MakeBoundingRect();
            // Eliminate crossing lines.
            if (!EliminateCrossingLines())
                return false;

            return true;
        }

        /// <summary>
        /// Reorders to eliminate crossing lines.
        /// </summary>
        private bool EliminateCrossingLines()
        {
	        bool bRepeat = true;
	        int nIt = 0;

            List<C2DPoint> Temp = new List<C2DPoint>();

	        while (bRepeat && nIt < 30)
	        {
		        nIt++;
		        bRepeat = false;
		        for ( int nCross1 = 0; nCross1 < Lines.Count ; nCross1++)
		        {
			        for (int nCross2 = nCross1 + 2; nCross2 < Lines.Count ; nCross2++)
			        {
				        if ( (nCross1 == 0) && (nCross2 == (Lines.Count - 1)) ) 
                                continue;

				        if (this.LineRects[nCross1].Overlaps(LineRects[nCross2]) &&
                            Lines[nCross1].Crosses(Lines[nCross2], Temp))
				        {
					        int nSwapStart = nCross1 + 1; // end of first line
					        int nSwapEnd = nCross2;
					        Debug.Assert(nSwapEnd > nSwapStart);
					        int nHalfway =	(nSwapEnd - nSwapStart) / 2;

					        for (int nPoint = 0; nPoint <= nHalfway; nPoint++)
					        {
						        SwapPositions( nSwapStart + nPoint, nSwapEnd - nPoint);
					        }
					  //      bReordered = true;	
					        bRepeat = true;
				        }
 			        }
		        }
	        }

	        MakeBoundingRect();

	        return (!bRepeat);

        }

        /// <summary>
        /// Swaps the two points.
        /// </summary>
        /// <param name="Pos1">The first point index.</param>
        /// <param name="Pos2">The second point index.</param>
        private void SwapPositions(int Pos1, int Pos2)
        {
            C2DPoint temp = GetPoint(Pos1);

            SetPoint(GetPoint(Pos2), Pos1);
            SetPoint(temp, Pos2);
        }

        /// <summary>
        /// Sets the point to the point provided.
        /// </summary>
        /// <param name="Point">The point to be set to.</param>
        /// <param name="nPointIndex">The point index.</param>
        private void SetPoint(C2DPoint Point, int nPointIndex)
        {
	        if (nPointIndex >=  Lines.Count )
		        nPointIndex -= Lines.Count;

	        int nPointIndexBefore;
	        if(nPointIndex == 0) 
                nPointIndexBefore = Lines.Count - 1 ;
            else
                nPointIndexBefore = nPointIndex - 1;

	        int nPointIndexAfter;
	        if (nPointIndex == Lines.Count - 1) 
                nPointIndexAfter = 0;
            else
                nPointIndexAfter = nPointIndex + 1;


	        C2DLineBase pLineBase = Lines[nPointIndex];
	        if (pLineBase is C2DLine)
	        {
		        C2DLine pLine = pLineBase as C2DLine;
		        pLine.Set(Point, Lines[nPointIndexAfter].GetPointFrom());
		        pLine.GetBoundingRect(LineRects[nPointIndex]);

		        C2DLineBase pLineBaseBefore = Lines[nPointIndexBefore];
		        if (pLineBaseBefore is C2DLine)
		        {
                    C2DLine pLineBefore = pLineBaseBefore as C2DLine;
			        pLineBefore.SetPointTo(Point);
			        pLineBefore.GetBoundingRect(LineRects[nPointIndexBefore]);
		        }
	        }


        }

        /// <summary>
        /// Inserts a point.
        /// </summary>
        /// <param name="nPointIndex">The point index.</param>   
        /// <param name="Point">The point to be set to.</param>
	    private void InsertPoint( int nPointIndex, C2DPoint Point)
        {
	        nPointIndex  = nPointIndex % Lines.Count;

	        int nPointIndexBefore = 0;
	        if (nPointIndex == 0) 
                nPointIndexBefore = Lines.Count - 1 ;
            else
                nPointIndexBefore = nPointIndex - 1;

	        int nPointIndexAfter = 0;
	        if (nPointIndex == (Lines.Count - 1))
                nPointIndexAfter = 0 ;
            else
                nPointIndexAfter = nPointIndex + 1;

	        C2DLine pInsert = new C2DLine(Point, Lines[nPointIndex].GetPointFrom());
	        C2DRect pInsertRect = new C2DRect();

	        pInsert.GetBoundingRect(pInsertRect);

        	
	        C2DLineBase pLineBase = Lines[nPointIndexBefore];
            if (pLineBase is C2DLine)
	        {
		        C2DLine pLineBefore = pLineBase as C2DLine;
		        pLineBefore.SetPointTo(Point);
		        pLineBefore.GetBoundingRect(LineRects[nPointIndexBefore]);

		        Lines.Insert(nPointIndex, pInsert);

		        LineRects.Insert(nPointIndex, pInsertRect);
	        }
        }


        /// <summary>
        /// Simple buffer around the polygon at a fixed amount. No attemp to ensure validity
        /// as intended for small buffer amounts.
        /// </summary>
        /// <param name="dBuffer">The buffer amount.</param>   
        public void SimpleBuffer(double dBuffer)
        {
            ClearConvexSubAreas();

            bool bClockwise = this.IsClockwise();

            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i] is C2DLine)
                {
                    C2DLine Line = Lines[i] as C2DLine;
                    C2DVector v = new C2DVector(Line.vector);
                    if (bClockwise)
                        v.TurnLeft();
                    else
                        v.TurnRight();
                    v.SetLength(dBuffer);
                    Line.Move(v);
                }
                else
                {
                    Debug.Assert(false);
                    return;
                }
            }

            for (int i = 1; i < Lines.Count; i++)
            {
                C2DLine Line1 = Lines[i - 1] as C2DLine;
                C2DLine Line2 = Lines[i] as C2DLine;

                Line1.Join(Line2);
            }

            (Lines[Lines.Count - 1] as C2DLine).Join(  Lines[0] as C2DLine  );

            MakeLineRects();
            MakeBoundingRect();

        }

        /// <summary>
        /// Removes null areas, will return true if the shape is no longer valid.
        /// </summary>
        /// <param name="dTolerance"></param>
        /// <returns>True if the polygon is no longer valid i.e. completely null.</returns>
        public bool RemoveNullAreas(double dTolerance)
        {
            ClearConvexSubAreas();

            int i = 0;
            bool bChanged = false;
            while (i < Lines.Count && Lines.Count > 2)
            {
                int nNext = (i + 1) % Lines.Count;
                double dArea = C2DTriangle.GetAreaSigned(Lines[i].GetPointFrom(),
                                                            Lines[i].GetPointTo(),
                                                            Lines[nNext].GetPointTo());

                if (Math.Abs(dArea) < dTolerance)
                {
                    if (Lines[i] is C2DLine)
                    {
                        (Lines[i] as C2DLine).SetPointTo(Lines[(i + 1) % Lines.Count].GetPointTo());
                        Lines.RemoveAt(nNext);
                        bChanged = true;
                    }
                    else
                    {
                        Debug.Assert(false);
                        return true;
                    }
                }
                else
                {
                    i++;
                }
            }

            if (Lines.Count <= 2)
            {
                Clear();
                return true;
            }

            if (bChanged)
            {
                MakeLineRects();
                MakeBoundingRect();
            }

            return false;
        }


        /// <summary>
        /// Make the lines related to the points for the base class to work on.
        /// </summary>
        /// <param name="Points">The point set.</param>   
        public void MakeLines(List<C2DPoint> Points)
        {
	        Lines.Clear();

	        for (int i = 0 ; i < Points.Count; i++)
	        {
		        int nNext = i + 1;
                if (nNext == Points.Count)
			        nNext = 0;
		        Lines.Add(new C2DLine(Points[i], Points[nNext]));
	        }
        }


        /// <summary>
        /// Gets the non overlaps i.e. the parts of this that aren't in the other.
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetNonOverlaps(C2DPolygon Other, List<C2DHoledPolygon> Polygons,
                                            CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetNonOverlaps(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolygon(NewPolys[i]));
        }

        /// <summary>
        /// Gets the union of the 2 shapes.
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetUnion(C2DPolygon Other, List<C2DHoledPolygon> Polygons,
                                            CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetUnion(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolygon(NewPolys[i]));
        }


        /// <summary>
        /// Gets the overlaps of the 2 shapes.	
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetOverlaps(C2DPolygon Other, List<C2DHoledPolygon> Polygons,
                                            CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetOverlaps(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolygon(NewPolys[i]));
        }



        /// <summary>
        /// Class to hold a line reference and bounding rect for OverlapsAbove algorithm.
        /// </summary>
        public class CLineBaseRect
        {
            /// <summary>
            /// Line reference
            /// </summary>
	        public C2DLine Line = null;
            /// <summary>
            /// Line bounding rect
            /// </summary>
            public C2DRect Rect = null;
            /// <summary>
            /// Set flag
            /// </summary>
            public bool bSetFlag = true;
        };


        /// <summary>
        /// True if this polygon is above the other. Returns the vertical distance 
        /// and the points on both polygons.
        /// </summary>
        /// <param name="Other"></param>
        /// <param name="dVerticalDistance"></param>
        /// <param name="ptOnThis"></param>
        /// <param name="ptOnOther"></param>
        /// <returns></returns>
        public bool OverlapsAbove( C2DPolygon Other, ref double dVerticalDistance,
										        C2DPoint ptOnThis, C2DPoint ptOnOther)
        {
	        C2DRect OtherBoundingRect = Other.BoundingRect;

	        if ( !BoundingRect.OverlapsAbove( OtherBoundingRect)  )
		        return false;

	        int nLineCount = Lines.Count;

	        if ( nLineCount != LineRects.Count)
		        return false;

	        int nOtherLineCount = Other.Lines.Count;

	        if ( nOtherLineCount != Other.LineRects.Count)
            {
		        return false;
            }


            List<CLineBaseRect> LineSet= new List<CLineBaseRect>();

	        for (int i = 0 ; i < nLineCount; i++)
	        {
		        if ( LineRects[i].OverlapsAbove( OtherBoundingRect ) )
		        {
			        CLineBaseRect pNewLine = new CLineBaseRect();
			        pNewLine.Line = Lines[i] as C2DLine;
			        pNewLine.Rect = LineRects[i];
			        pNewLine.bSetFlag = true;
                    LineSet.Add(pNewLine);
		        }
	        }

	        for (int i = 0 ; i < nOtherLineCount; i++)
	        {
		        if ( Other.LineRects[i].OverlapsBelow( this.BoundingRect ) )
		        {
			        CLineBaseRect pNewLine = new CLineBaseRect();
			        pNewLine.Line = Other.Lines[i] as C2DLine;
			        pNewLine.Rect = Other.LineRects[i];
			        pNewLine.bSetFlag = false;
                    LineSet.Add(pNewLine);
		        }
	        }
            CLineBaseRectLeftToRight Comparitor = new CLineBaseRectLeftToRight();
            LineSet.Sort(Comparitor);

	        bool bResult = false;

	        int j = 0;
            while (j < LineSet.Count)
	        {
		        int r = j + 1;

                double dXLimit = LineSet[j].Rect.GetRight();

                while (r < LineSet.Count &&
                       LineSet[r].Rect.GetLeft() < dXLimit)
		        {
			        double dDistTemp = 0;
			        C2DPoint ptOnThisTemp =  new C2DPoint();
			        C2DPoint ptOnOtherTemp =  new C2DPoint();
			        bool bOverlap = false;
                    if (LineSet[j].bSetFlag)
			        {
                        if (!LineSet[r].bSetFlag &&
                            LineSet[j].Line.OverlapsAbove(LineSet[r].Line, ref dDistTemp, 
													        ptOnThisTemp,  ptOnOtherTemp ))
				        {
					        bOverlap = true;
				        }
			        }
			        else 
			        {
                        if (LineSet[r].bSetFlag &&
                            LineSet[r].Line.OverlapsAbove(LineSet[j].Line, ref dDistTemp, 
													        ptOnThisTemp,  ptOnOtherTemp ))
				        {
					        bOverlap = true;
				        }
			        }

			        if ( bOverlap && (dDistTemp < dVerticalDistance || !bResult) )
			        {
				        bResult = true;
				        dVerticalDistance = dDistTemp;
				        ptOnThis.Set(ptOnThisTemp);
				        ptOnOther.Set(ptOnOtherTemp);
				        if ( dDistTemp == 0)
				        {
					        j += Lines.Count; // escape;
					        r += Lines.Count; // escape;
				        }
			        }

			        r++;
		        }

		        j++;
	        }


	        return bResult;
        }


        
        /// <summary>
        /// True if this polygon is above or below the other. Returns the vertical distance 
        /// and the points on both polygons.
        /// </summary>
        /// <param name="Other"></param>
        /// <param name="dVerticalDistance"></param>
        /// <param name="ptOnThis"></param>
        /// <param name="ptOnOther"></param>
        /// <returns></returns>
        public bool OverlapsVertically( C2DPolygon Other, ref double dVerticalDistance,
										        C2DPoint ptOnThis, C2DPoint ptOnOther)
        {
	        C2DRect OtherBoundingRect = Other.BoundingRect;

	        if ( !BoundingRect.OverlapsVertically( OtherBoundingRect)  )
		        return false;

	        int nLineCount = Lines.Count;

	        if ( nLineCount != LineRects.Count)
		        return false;

	        int nOtherLineCount = Other.Lines.Count;

	        if ( nOtherLineCount != Other.LineRects.Count)
		        return false;


            List<CLineBaseRect> LineSet = new List<CLineBaseRect>();

	        for (int i = 0 ; i < nLineCount; i++)
	        {
		        if ( LineRects[i].OverlapsVertically( OtherBoundingRect ) )
		        {
			        CLineBaseRect pNewLine = new CLineBaseRect();
			        pNewLine.Line = Lines[i] as C2DLine;
			        pNewLine.Rect = LineRects[i];
			        pNewLine.bSetFlag = true;
                    LineSet.Add(pNewLine);
		        }
	        }

	        for (int i = 0 ; i < nOtherLineCount; i++)
	        {
		        if ( Other.LineRects[i].OverlapsVertically( this.BoundingRect ) )
		        {
			        CLineBaseRect pNewLine = new CLineBaseRect();
			        pNewLine.Line = Other.Lines[i] as C2DLine;
			        pNewLine.Rect = Other.LineRects[i];
			        pNewLine.bSetFlag = false;
                    LineSet.Add(pNewLine);
		        }
	        }

            CLineBaseRectLeftToRight Comparitor = new CLineBaseRectLeftToRight();
            LineSet.Sort(Comparitor);

	        bool bResult = false;

	        int j = 0;
	        while (j < LineSet.Count)
	        {
		        int r = j + 1;

		        double dXLimit = LineSet[j].Rect.GetRight();

		        while (r < LineSet.Count && 
			           LineSet[r].Rect.GetLeft() < dXLimit)
		        {
			        double dDistTemp = 0;
			        C2DPoint ptOnThisTemp = new C2DPoint();
			        C2DPoint ptOnOtherTemp = new C2DPoint();
			        bool bOverlap = false;
			        if (  LineSet[j].bSetFlag )
			        {
				        if ( !LineSet[r].bSetFlag && 
					        LineSet[j].Line.OverlapsVertically( LineSet[r].Line , ref dDistTemp, 
													        ptOnThisTemp,  ptOnOtherTemp ))
				        {
					        bOverlap = true;
				        }
			        }
			        else 
			        {
				        if ( LineSet[r].bSetFlag && 
					        LineSet[r].Line.OverlapsVertically( LineSet[j].Line , ref dDistTemp, 
													        ptOnThisTemp,  ptOnOtherTemp ))
				        {
					        bOverlap = true;
				        }
			        }

			        if ( bOverlap && (dDistTemp < dVerticalDistance || !bResult) )
			        {
				        bResult = true;
				        dVerticalDistance = dDistTemp;
				        ptOnThis.Set(ptOnThisTemp);
				        ptOnOther.Set(ptOnOtherTemp);
				        if ( dDistTemp == 0)
				        {
					        j += Lines.Count; // escape;
                            r += Lines.Count; // escape;
				        }
			        }

			        r++;
		        }

		        j++;
	        }


	        return bResult;
        }


        /// <summary>
        /// Returns the minimum bounding box that is not necassarily horiztonal i.e. 
        /// the box can be at an angle and is defined by a line and the width to the right.
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="dWidthToRight"></param>
        public void GetMinBoundingBox( C2DLine Line, ref double dWidthToRight)
        {
	        int nCount = Lines.Count;
	        if (nCount == 0)
		        return;

	        if (!IsConvex())
	        {
		        C2DPolygon CH = new C2DPolygon();
		        CH.CreateConvexHull( this );
		        CH.GetMinBoundingBox( Line, ref dWidthToRight);
                return;
	        }


	        int nP1 = 0;//index of vertex with minimum y-coordinate;
	        int nP2 = 0;//index of vertex with maximum y-coordinate;

 	        int nP3 = 0;//index of vertex with minimum x-coordinate;
	        int nP4 = 0;//index of vertex with maximum x-coordinate;

	        double dMinY = Lines[0].GetPointFrom().y;
	        double dMaxY = dMinY;

	        double dMinX = Lines[0].GetPointFrom().x;
	        double dMaxX = dMinX;

	        for ( int i = 1 ; i < Lines.Count; i++)
	        {
		        C2DPoint pt = Lines[i].GetPointFrom();
		        if (pt.y < dMinY)
		        {
			        dMinY = pt.y;
			        nP1 = i;
		        }
		        else if (pt.y > dMaxY)
		        {
			        dMaxY = pt.y;
			        nP2 = i;
		        }

		        if (pt.x < dMinX)
		        {
			        dMinX = pt.x;
			        nP3 = i;
		        }
		        else if (pt.x > dMaxX)
		        {
			        dMaxX = pt.x;
			        nP4 = i;
		        }
	        }


	        double dRotatedAngle = 0;
	        double dMinArea = 1.7E+308;
         
        // 222222
        // 3	4
        // 3    4
        // 3    4
        // 111111

	        C2DLine Caliper1 = new C2DLine( Lines[nP1].GetPointFrom(), new C2DVector(-1,0) );    // Caliper 1 points along the negative x-axis
            C2DLine Caliper2 = new C2DLine(Lines[nP2].GetPointFrom(), new C2DVector(1, 0));   // Caliper 2 points along the positive x-axis

            C2DLine Caliper3 = new C2DLine(Lines[nP3].GetPointFrom(), new C2DVector(0, 1));    // Caliper 3 points along the positive y-axis
            C2DLine Caliper4 = new C2DLine(Lines[nP4].GetPointFrom(), new C2DVector(0, -1));   // Caliper 4 points along the negative y-axis

	        while( dRotatedAngle < Constants.conPI)
	        {
                int nMod = Lines.Count;
   		        // Determine the angle between each caliper and the next adjacent edge in the polygon
		        double dAngle1 = Caliper1.vector.AngleToRight( (Lines[ nP1%nMod] as C2DLine).vector);
                double dAngle2 = Caliper2.vector.AngleToRight((Lines[ nP2%nMod] as C2DLine ).vector);
                double dAngle3 = Caliper3.vector.AngleToRight((Lines[ nP3%nMod] as C2DLine ).vector);
                double dAngle4 = Caliper4.vector.AngleToRight((Lines[ nP4%nMod ]as C2DLine ).vector);

		        double dMinAngle;
		        if (dAngle1 < dAngle2 &&
			        dAngle1 < dAngle3 &&
			        dAngle1 < dAngle4)
		        {
			        dMinAngle = dAngle1;
			        nP1++;
                    Caliper1.point = Lines[nP1 % Lines.Count].GetPointFrom();
		        }
		        else if (dAngle2 < dAngle3 &&
			        dAngle2 < dAngle4)
		        {
			        dMinAngle = dAngle2;
			        nP2++;
                    Caliper2.point = Lines[nP2 % Lines.Count].GetPointFrom();
		        }
		        else if ( dAngle3 < dAngle4)
		        {
			        dMinAngle = dAngle3;
			        nP3++;
                    Caliper3.point = Lines[nP3 % Lines.Count].GetPointFrom();
		        }
		        else
		        {
			        dMinAngle = dAngle4;
			        nP4++;
                    Caliper4.point = Lines[nP4 % Lines.Count].GetPointFrom();
		        }
		        dRotatedAngle += dMinAngle;
		        dMinAngle -= 0.00000001;
		        Caliper1.vector.TurnRight( dMinAngle);
		        Caliper2.vector.TurnRight( dMinAngle);
		        Caliper3.vector.TurnRight( dMinAngle);
		        Caliper4.vector.TurnRight( dMinAngle);


		        double dWidth1 = Caliper1.DistanceAsRay( Caliper2.point );		
		        double dWidth2 = Caliper3.DistanceAsRay( Caliper4.point );	
        	
		        double dArea = dWidth1 * dWidth2;

		        if (dArea < dMinArea)
		        {
			        dMinArea = dArea;
			        Line.Set(Caliper1);
			        Line.point.ProjectOnRay( Caliper4 );
			        Line.vector.SetLength( dWidth2 );
        			
			        dWidthToRight = dWidth1;
		        }
	        }
        }


        /// <summary>
        /// Class to help with sorting. 
        /// </summary>
        public class CLineBaseRectLeftToRight : IComparer<CLineBaseRect>
        {
            #region IComparer Members
            /// <summary>
            /// Compare function. 
            /// </summary>
            public int Compare(CLineBaseRect L1, CLineBaseRect L2)
            {
                if (L1 == L2)
                    return 0;
                if (L1.Rect.TopLeft.x > L2.Rect.TopLeft.x)
                    return 1;
                else if (L1.Rect.TopLeft.x == L2.Rect.TopLeft.x)
                    return 0;
                else
                    return -1;
            }
            #endregion
        }

        /// <summary>
        /// Sub area 1.
        /// </summary>
        C2DPolygon subArea1 = null;
        /// <summary>
        /// Sub area 1 access.
        /// </summary>
        C2DPolygon SubArea1
        {
            get
            {
                return subArea1;
            }
        }
        /// <summary>
        /// Sub area 2.
        /// </summary>
	    C2DPolygon subArea2 = null;
        /// <summary>
        /// Sub area 2 access.
        /// </summary>
        C2DPolygon SubArea2
        {
            get
            {
                return subArea2;
            }
        }

    }
}
