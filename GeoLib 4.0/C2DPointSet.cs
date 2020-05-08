using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GeoLib
{
    /// <summary>
    /// Class representing a point set.
    /// </summary>
    public class C2DPointSet : List<C2DPoint>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DPointSet() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~C2DPointSet() {}
        /// <summary>
        /// Makes a copy of the other set.
        /// </summary>
        /// <param name="Other">The other set.</param>
        public void MakeCopy(List<C2DPoint> Other)
        {
            this.Clear();
            for (int i = 0; i < Other.Count; i++)
            {
                this.Add(new C2DPoint(Other[i]));
            }
        }
        /// <summary>
        /// Extracts all of the other set.
        /// </summary>
        /// <param name="S2">The other set.</param>
        public void ExtractAllOf(C2DPointSet S2)
        {
            for (int i = 0; i < S2.Count; i++)
            {
                Add(S2[i]);
            }
            S2.Clear();
        }
        /// <summary>
        /// Adds a copy of the point.
        /// </summary>
        /// <param name="P1">The point.</param>
        public void AddCopy(C2DPoint P1)
        {
            Add(new C2DPoint(P1));
        }
        /// <summary>
        /// Adds a copy of the point set.
        /// </summary>
        /// <param name="Other">The point set.</param>
        public void AddCopy(List<C2DPoint> Other)
        {
            for (int i = 0 ; i < Other.Count ; i++)
                Add(new C2DPoint(Other[i]));
        }
        /// <summary>
        /// Extracts at the index given.
        /// </summary>
        /// <param name="nIndex">The index.</param>
        public C2DPoint ExtractAt(int nIndex)
        {
            C2DPoint Result = this[nIndex];
            this.RemoveAt(nIndex);
            return Result;
        }
        /// <summary>
        /// Removes the convex hull from the point set given.
        /// Will affect the input set.
        /// </summary>
        /// <param name="Other">The other set.</param>
	    public void ExtractConvexHull(  C2DPointSet Other)
        {
	        Clear();

	        if (Other.Count < 4)
	        {
		        this.ExtractAllOf(Other);
		        return;
	        }

	        C2DPoint ptLeftMost = Other[0];
	        int nLeftMost = 0;
        	
	        // Find left most
	        for (int i = 1 ; i < Other.Count; i++)
	        {
		        C2DPoint pt = Other[i];
		        if (pt.x < ptLeftMost.x)
		        {
			        ptLeftMost = pt;
			        nLeftMost = i;
		        }
	        }

	        Add(Other.ExtractAt(nLeftMost));

	        Other.SortByAngleFromNorth( this[0]);

	        // Always add the left most and the first of the rest.
	        Add(Other.ExtractAt(0));

	        // Add others if needed.
	        int nIndx = 0;

	        C2DPointSet Unused = new C2DPointSet();

	        while (nIndx < Other.Count)
	        {
			        int nLast = Count - 1;
			        C2DLine LastLine = new C2DLine( this[nLast-1], this[nLast]);

			        C2DVector Test = new C2DVector( this[nLast], Other[nIndx]);

			        double dAng = Test.AngleFromNorth();

			        if (dAng < LastLine.vector.AngleFromNorth())
			        {
				        Unused.Add( ExtractAt(nLast) );
			        }
			        else
			        {
				        Add(Other.ExtractAt(nIndx));
			        }
	        }

	        Other.ExtractAllOf(Unused);	

        }

        /// <summary>
        /// Sorts by the angle from north relative to the origin given.
        /// </summary>
        /// <param name="Origin">The origin.</param>
        public void SortByAngleFromNorth(C2DPoint Origin)
        {
            AngleFromNorth Comparer = new AngleFromNorth();
            Comparer.Origin = Origin;
            Sort(Comparer);
        }
        /// <summary>
        /// Sorts by the angle to the right of the line.
        /// </summary>
        /// <param name="Line">The Line.</param>
        public void SortByAngleToRight(C2DLine Line)
        {
            AngleToRight Comparer = new AngleToRight();
            Comparer.Line = Line;
            Sort(Comparer);
        }

        /// <summary>
        /// Sorts by the angle to the left of the line.
        /// </summary>
        /// <param name="Line">The Line.</param>
        public void SortByAngleToLeft(C2DLine Line)
        {
            AngleToLeft Comparer = new AngleToLeft();
            Comparer.Line = Line;
            Sort(Comparer);

        }
        /// <summary>
        /// Gets the bounding rectangle.
        /// </summary>
        /// <param name="Rect">Ouput. The Rect.</param>
        public void GetBoundingRect(C2DRect Rect)
        {
	        if (Count == 0)
	        {
		        Rect.Clear();
		        return;
	        }
	        else
	        {
                Rect.Set(this[0]);

		        for (int i = 1 ; i < Count; i++)
		        {
			        Rect.ExpandToInclude( this[i]);
		        }
	        }
        }
        /// <summary>
        /// Gets the minimum bounding circle.
        /// </summary>
        /// <param name="Circle">Ouput. The Circle.</param>
        public void GetBoundingCircle(C2DCircle Circle)
        {
	        if (this.Count < 3)
	        {
		        if (this.Count == 2)
		        {
			        Circle.SetMinimum(this[0], this[1] );
		        }
		        else if (this.Count == 1)
		        {
			        Circle.Set( this[0], 0);
		        }
		        else
		        {
                    Debug.Assert(false, "Point set with no points. Cannot calculate bounding circle.");
		        }
		        return;
	        }

	        int nIndx1 = 0;
            int nIndx2 = 0; ;
            int nIndx3 = 0; ;
            double dDist = 0; ;

	        // First get the points that are furthest away from each other.
	        GetExtremePoints(ref nIndx1, ref nIndx2, ref dDist);
	        // Set the circle to bound these.
	        Circle.SetMinimum( this[nIndx1], this[nIndx2]);
	        // Set up a flag to show if we are circumscibed. (Once we are, we always will be).
	        bool bCircum = false;
	        // Cycle through and if any points aren't in the circle, then set the circle to be circumscribed.
	        for (int i = 0 ; i < Count; i++)
	        {
		        if ( i != nIndx1 && i != nIndx2)
		        {
			        if (!Circle.Contains(  this[i]))
			        {
				        nIndx3 = i;
				        Circle.SetCircumscribed(  this[nIndx1], this[nIndx2], this[nIndx3]  );
				        bCircum = true;
				        // Break out and try again.
				        break;
			        }
		        }
	        }

	        // If we didn't succeed first time then go through again setting it to be circumscribed every time.
	        if (bCircum)
	        {
		        for ( int i = 0 ; i < Count; i++)
		        {
			        if ( i != nIndx1 && i != nIndx2 && i != nIndx3)
			        {
				        if (!Circle.Contains(  this[i] ))
				        {
					        double Dist1 = this[i].Distance(  this[nIndx1] );
					        double Dist2 = this[i].Distance(  this[nIndx2] );
                            double Dist3 = this[i].Distance( this[nIndx3]);
					        if (Dist1 < Dist2 && Dist1 < Dist3)
					        {
						        // Closest to point 1 so elimitate this
						        nIndx1 = i;
					        }
					        else if (Dist2 < Dist3)
					        {
						        // Closest to point 2 so elimitate this
						        nIndx2 = i;
					        }
					        else
					        {
						        // Closest to point 3 so elimitate this
						        nIndx3 = i;
					        }
					        Circle.SetCircumscribed(  this[nIndx1], this[nIndx2], this[nIndx3]  );
				        }
			        }
		        }
	        }
        }
        /// <summary>
        /// Gets the points that are furthest apart as an estimate.
        /// </summary>
        /// <param name="nIndx1">Ouput. The first index.</param>
        /// <param name="nIndx2">Ouput. The second index.</param>
        /// <param name="dDist">Ouput. The distance between.</param>
        /// <param name="nStartEst">Input. The guess at one of the points.</param>
	    public void GetExtremePointsEst(ref int nIndx1, ref int nIndx2, 
		ref double dDist, int nStartEst) 
        {
	        if (Count < 3)
	        {
		        if (Count == 2)
		        {
			        nIndx1 = 0;
			        nIndx2 = 1;
		        }
		        else if (Count == 1)
		        {
			        nIndx1 = 0;
			        nIndx2 = 0;
		        }
		        else
		        {
			        nIndx1 = 0;
			        nIndx2 = 0;
                    Debug.Assert(false, "Point set with no points. Cannot calculate extreme points.");
		        }

		        return;
	        }

	        // Index 1 is the provided starting guess (default to 0).
	        nIndx1 = nStartEst;
	        // Index 2 is the furthest point from this.
	        nIndx2 = GetFurthestPoint(nIndx1, ref dDist);

	        int nIndx3 = ~(int)0;

	        while (true)
	        {
		        nIndx3 = GetFurthestPoint(nIndx2, ref dDist);
		        if (nIndx3 == nIndx1)
		        {
			        return;
		        }
		        else
		        {
			        nIndx1 = nIndx2;
			        nIndx2 = nIndx3;
		        }
	        }
        }
        /// <summary>
        /// Gets the points that are furthest apart.
        /// </summary>
        /// <param name="nIndx1">Ouput. The first index.</param>
        /// <param name="nIndx2">Ouput. The second index.</param>
        /// <param name="dDist">Ouput. The distance.</param>
        public void GetExtremePoints(ref int nIndx1, ref int nIndx2, 
		        ref double dDist)
        {
	        // First take a guess at them.
	        GetExtremePointsEst(ref nIndx1, ref nIndx2, ref dDist, 0);

	        // Set up a circle to bound the 2 guesses.
	        C2DVector Vec = new C2DVector( this[nIndx1], this[nIndx2]);
	        Vec.Multiply( 0.5);
	        C2DCircle Circle = new C2DCircle( this[nIndx1] + new C2DPoint(Vec), dDist / 2);

	        // Now, if the guess was wrong, there must be a point outside the circle which is part of
	        // the right solution. Go through all these, check and reset the result each time.
	        for (int i = 0 ; i < Count; i++)
	        {
		        if ( i != nIndx1 && i != nIndx2)
		        {
			        if ( !Circle.Contains( this[i] ))
			        {
				        double dDistCheck = 0;
				        int nCheck1 = GetFurthestPoint(i,  ref dDistCheck);
				        if (dDistCheck > dDist)
				        {
					        nIndx1 = i;
					        nIndx2 = nCheck1;
					        dDist = dDistCheck;
				        }				
			        }
		        }
	        }
        }
        /// <summary>
        /// Removes all repeated points.
        /// </summary>
        public void RemoveRepeatedPoints()
        {
	        if (Count < 2)
		        return;

	        int i = 0;
	        while (i < Count)
	        {
		        int r = i + 1;
                while (r < Count)
		        {
			        if ( this[i].PointEqualTo(this[r]))
			        {
				        this.RemoveAt(r);
			        }
			        else
			        {
				        r++;
			        }
		        }
		        i++;
	        }
        }

        /// <summary>
        /// Returns the index of the furthest point from the point specified by the 
        /// index given.
        /// </summary>
        /// <param name="nIndex">Input. The index.</param>
        /// <param name="dDist">Ouput. The distance.</param>
        int GetFurthestPoint(int nIndex, ref double dDist)
        {
	        if (Count < 2 || nIndex >= Count)
	        {
		        Debug.Assert(false);
		        return 0;
	        }
        	
	        int usResult;

	        if (nIndex == 0)
	        {
		        dDist = this[1].Distance(this[nIndex]);
		        usResult = 1;
	        }
	        else
	        {
		        dDist = this[0].Distance(this[nIndex]);
		        usResult = 0;
	        }

	        for (int i = 1 ; i < Count; i++)
	        {
		        if (i != nIndex)
		        {
			        double dD = this[i].Distance(this[nIndex]);
			        if (dD > dDist)
			        {
				        dDist = dD;
				        usResult = i;
			        }
		        }
	        }

	        return usResult;
        }
        /// <summary>
        /// Sorts by distance to the point.
        /// </summary>
        /// <param name="pt">Input. The point.</param>
        public void SortByDistance(C2DPoint pt)
        {
            SortByDistance Comparer = new SortByDistance();
            Comparer.Point = pt;
            Sort(Comparer);
        }
        /// <summary>
        /// Sorts left to right.
        /// </summary>
        public void SortLeftToRight()
        {
            PointLeftToRight Comparer = new PointLeftToRight();
            Sort(Comparer);
        }
        /// <summary>
        /// Sorts by index. The index set must have the same number of entries.
        /// The index set will also be sorted.
        /// </summary>
        /// <param name="Indexes">Input. The indexes.</param>
        public void SortByIndex(List<int> Indexes)
        {
            if (Indexes.Count == Count)
            {
                SortByIndex(Indexes, 0, Count - 1);
            }
        }
        /// <summary>
        /// Quicksort index sorting.
        /// </summary>
        /// <param name="Indexes">Input. The indexes.</param>
        /// <param name="lo0">Input. The sort start.</param>
        /// <param name="hi0">Input. The sort end.</param>
        private void SortByIndex(List<int> Indexes, int lo0, int hi0)
        {
            int lo = lo0;
            int hi = hi0;
            if (lo >= hi) return;

            else if (lo == hi - 1)
            {
                // sort a two element list by swapping if necessary 
                if (Indexes[lo] > Indexes[hi])
                {
                    int T = Indexes[lo];
                    Indexes[lo] = Indexes[hi];
                    Indexes[hi] = T;

                    C2DPoint PAR = this[lo];
                    this[lo] = this[hi];
                    this[hi] = PAR;
                }
                return;
            }

            //  Pick a pivot and move it out of the way

            int pivot = Indexes[(lo + hi) / 2];
            Indexes[(lo + hi) / 2] = Indexes[hi];
            Indexes[hi] = pivot;

            C2DPoint ParPivot = this[(lo + hi) / 2];
            this[(lo + hi) / 2] = this[hi];
            this[hi] = ParPivot;


            while (lo < hi)
            {
                //  Search forward from a[lo] until an element is found that
                //  is greater than the pivot or lo >= hi 

                while (Indexes[lo] <= pivot && lo < hi)
                {
                    lo++;
                }
                // Search backward from a[hi] until element is found that
                //  is less than the pivot, or lo >= hi

                while (pivot <= Indexes[hi] && lo < hi)
                {
                    hi--;
                }
                //  Swap elements a[lo] and a[hi]
                if (lo < hi)
                {
                    int T = Indexes[lo];
                    Indexes[lo] = Indexes[hi];
                    Indexes[hi] = T;

                    C2DPoint PAR = this[lo];
                    this[lo] = this[hi];
                    this[hi] = PAR;

                }
            }

            //  Put the median in the "center" of the list

            Indexes[hi0] = Indexes[hi];
            Indexes[hi] = pivot;

            this[hi0] = this[hi];
            this[hi] = ParPivot;

            //Recursive calls, elements a[lo0] to a[lo-1] are less than or
            //equal to pivot, elements a[hi+1] to a[hi0] are greater than
            //pivot.

            SortByIndex(Indexes, lo0, lo - 1);
            SortByIndex(Indexes,  hi + 1, hi0);
        }

    }
    /// <summary>
    /// Sort helper.
    /// </summary>
    public class PointLeftToRight : IComparer< C2DPoint>
    {
        #region IComparer Members
        /// <summary>
        /// Compare function.
        /// </summary>
        public int Compare(C2DPoint A, C2DPoint B)
        {
            if (A == B)
                return 0;
            if (A.x > B.x)
                return 1;
            else if (A.x < B.x)
                return -1;
            else
                return 0;
        }
        #endregion
    }
    /// <summary>
    /// Sort helper.
    /// </summary>
    public class AngleFromNorth : IComparer<C2DPoint>
    {
        /// <summary>
        /// Origin.
        /// </summary>
        public C2DPoint Origin;
        #region IComparer Members
        /// <summary>
        /// Compare function.
        /// </summary>
        public int Compare(C2DPoint P1, C2DPoint P2)
        {
            if (P1 == P2)
                return 0;

            C2DVector Vec1 = new C2DVector(Origin, P1);
            C2DVector Vec2 = new C2DVector(Origin, P2);

            double dAng1 = Vec1.AngleFromNorth();
            double dAng2 = Vec2.AngleFromNorth();

            if (dAng1 > dAng2)
                return 1;
            else if (dAng1 < dAng2)
                return -1;
            else
                return 0;
        }
        #endregion
    }
    /// <summary>
    /// Sort helper.
    /// </summary>
    public class AngleToRight : IComparer<C2DPoint>
    {
        /// <summary>
        /// Line.
        /// </summary>
        public C2DLine Line;
        #region IComparer Members
        /// <summary>
        /// Compare function.
        /// </summary>
        public int Compare(C2DPoint P1, C2DPoint P2)
        {
            if (P1 == P2)
                return 0;

	        C2DVector Vec1 = new C2DVector(Line.point, P1);
            C2DVector Vec2 = new C2DVector(Line.point, P2);

            double dAng1 = Line.vector.AngleToRight(Vec1);
            double dAng2 = Line.vector.AngleToRight(Vec2);

            if (dAng1 > dAng2)
                return 1;
            else if (dAng1 < dAng2)
                return -1;
            else
                return 0;
        }
        #endregion
    }
    /// <summary>
    /// Sort helper.
    /// </summary>
    public class AngleToLeft : IComparer<C2DPoint>
    {
        /// <summary>
        /// Line.
        /// </summary>
        public C2DLine Line;
        #region IComparer Members
        /// <summary>
        /// Compare function.
        /// </summary>
        public int Compare(C2DPoint P1, C2DPoint P2)
        {
            if (P1 == P2)
                return 0;

            C2DVector Vec1 = new C2DVector(Line.point, P1);
            C2DVector Vec2 = new C2DVector(Line.point, P2);

            double dAng1 = Line.vector.AngleToLeft(Vec1);
            double dAng2 = Line.vector.AngleToLeft(Vec2);

            if (dAng1 > dAng2)
                return 1;
            else if (dAng1 < dAng2)
                return -1;
            else
                return 0;
        }
        #endregion
    }
    /// <summary>
    /// Sort helper.
    /// </summary>
    public class SortByDistance : IComparer<C2DPoint>
    {
        /// <summary>
        /// Point to calculate distance from.
        /// </summary>
        public C2DPoint Point;
        #region IComparer Members
        /// <summary>
        /// Compare function.
        /// </summary>
        public int Compare(C2DPoint P1, C2DPoint P2)
        {
            if (P1 == P2)
                return 0;

            double d1 = P1.Distance(Point);
            double d2 = P2.Distance(Point);

            if (d1 > d2)
                return 1;
            else if (d1 < d2)
                return -1;
            else
                return 0;
        }
        #endregion
    }

}
