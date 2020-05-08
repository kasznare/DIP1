using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GeoLib
{

    /// <summary>
    /// Set of abstract lines.
    /// </summary>
    public class C2DLineBaseSet : List<C2DLineBase>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public C2DLineBaseSet() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~C2DLineBaseSet() { }
        /// <summary>
        /// Makes a value copy of the other set.
        /// </summary>
        /// <param name="Other">The other set.</param>
        public void MakeValueCopy(List<C2DLineBase> Other)
        {
            this.Clear();
            for (int i = 0; i < Other.Count; i++)
            {
                this.Add(Other[i].CreateCopy());
            }
        }

        /// <summary>
        /// Makes a refenence copy of the other set.
        /// </summary>
        /// <param name="Other">The other set.</param>
        public void MakeRefCopy(List<C2DLineBase> Other)
        {
            this.Clear();
            for (int i = 0; i < Other.Count; i++)
            {
                this.Add(Other[i]);
            }
        }

        /// <summary>
        /// Adds a copy of the item.
        /// </summary>
        /// <param name="NewItem">The line as a line base.</param>
        public void AddCopy(C2DLineBase NewItem)
        {
            if (NewItem is C2DLine)
            {
                this.Add(new C2DLine(NewItem as C2DLine));
            }
            else if (NewItem is C2DArc)
            {
                this.Add(new C2DArc(NewItem as C2DArc));
            }
        }

        /// <summary>
        /// Extracts all of the other set.
        /// </summary>
        /// <param name="S2">The other set.</param>
        public void ExtractAllOf(List<C2DLineBase> S2)
        {
            for (int i = 0; i < S2.Count; i++)
            {
                Add(S2[i]);
            }
            S2.Clear();
        }

        /// <summary>
        /// Extracts at the index given.
        /// </summary>
        /// <param name="nIndex">The index.</param>
        public C2DLineBase ExtractAt(int nIndex)
        {
            C2DLineBase Result = this[nIndex];
            this.RemoveAt(nIndex);
            return Result;
        }

        /// <summary>
        /// Class to hold a line reference and bounding rect for line intersection algorithm.
        /// </summary>
        public class CLineBaseRect
        {
            /// <summary>
            /// Line reference
            /// </summary>
	        public C2DLineBase Line = null;
            /// <summary>
            /// Line bounding rect
            /// </summary>
            public C2DRect Rect = new C2DRect();
            /// <summary>
            /// Line index
            /// </summary>
	        public int usIndex = 0;
            /// <summary>
            /// Set flag
            /// </summary>
            public bool bSetFlag = true;
        };

        /// <summary>
        /// Returns the intersections within the set. Each intersection has and
        /// associated point an 2 indexes corresponding to the lines that
        /// created the intersection.
        /// </summary>
        /// <param name="pPoints">Output. The point set.</param>
        /// <param name="pIndexes1">Output. The indexes.</param>
        /// <param name="pIndexes2">Output. The indexes.</param>
	    public void GetIntersections( List<C2DPoint> pPoints,  List<int> pIndexes1,
                             List<int> pIndexes2)
        {
            List<CLineBaseRect> Lines = new List<CLineBaseRect>();

	        for (int i = 0 ; i < Count ; i++)
	        {
		        CLineBaseRect LineRect = new CLineBaseRect();
                LineRect.Line = this[i];
                LineRect.Line.GetBoundingRect( LineRect.Rect);
                LineRect.usIndex = i;
		        Lines.Add(LineRect);
	        }

            CLineBaseRectLeftToRight Comparitor = new CLineBaseRectLeftToRight();
            Lines.Sort(Comparitor);

            int j = 0;
            List<C2DPoint> IntPt = new List<C2DPoint>();
	        // For each line...
	        while (j < Lines.Count)
	        {
		        int r = j + 1;

		        double dXLimit = Lines[j].Rect.GetRight();
		        // ...search forward untill the end or a line whose rect starts after this ends
		        while (r < Lines.Count && Lines[r].Rect.GetLeft() < dXLimit)
		        {
        			
			        if ( Lines[j].Rect.Overlaps(  Lines[r].Rect) &&
				        Lines[j].Line.Crosses(  Lines[r].Line,  IntPt ))
			        {
				        while (IntPt.Count > 0)
				        {
					        pPoints.Add( IntPt[IntPt.Count - 1] );
                            IntPt.RemoveAt(IntPt.Count - 1);

					        pIndexes1.Add( Lines[j].usIndex );
					        pIndexes2.Add( Lines[r].usIndex );
				        }
			        }
			        r++;
		        }	
		        j++;
	        }
        }


        /// <summary>
        /// Returns the intersections with this set and the other. 
        /// Each intersection has an associated point and 2 indexes 
        /// corresponding to the lines that created the intersection.
        /// </summary>
        /// <param name="Other">Input. The other line set.</param>
        /// <param name="pPoints">Output. The intersection points.</param>
        /// <param name="pIndexesThis">Output. The indexes for this.</param>
        /// <param name="pIndexesOther">Output. The indexes for the other set.</param>
        /// <param name="pBoundingRectThis">Input. The bounding rect for this.</param>
        /// <param name="pBoundingRectOther">Input. The bounding rect for the other.</param>
        public void GetIntersections(List<C2DLineBase> Other,  List<C2DPoint> pPoints,
             List<int> pIndexesThis,  List<int> pIndexesOther,
            C2DRect pBoundingRectThis, C2DRect pBoundingRectOther)
        {

            List<CLineBaseRect> Lines = new List<CLineBaseRect>();

	        for (int i = 0 ; i <  Count ; i++)
	        {
		        CLineBaseRect LineRect = new CLineBaseRect();
                LineRect.Line = this[i];
                LineRect.Line.GetBoundingRect( LineRect.Rect);
                LineRect.usIndex = i;
                LineRect.bSetFlag = true;

		        if ( pBoundingRectOther.Overlaps( LineRect.Rect))
		        {
			        Lines.Add(LineRect);
		        }
	        }

	        for (int d = 0 ; d <  Other.Count ; d++)
	        {
		        CLineBaseRect LineRect = new CLineBaseRect();
		        LineRect.Line = Other[d];
		        LineRect.Line.GetBoundingRect( LineRect.Rect);
		        LineRect.usIndex = d;
		        LineRect.bSetFlag = false;

		        if ( pBoundingRectThis.Overlaps( LineRect.Rect))
		        {
			        Lines.Add(LineRect);
		        }
	        }

            CLineBaseRectLeftToRight Comparitor = new CLineBaseRectLeftToRight();
            Lines.Sort(Comparitor);

            int j = 0;
	        List<C2DPoint> IntPt = new List<C2DPoint>();
	        while (j < Lines.Count)
	        {
		        int r = j + 1;

		        double dXLimit = Lines[j].Rect.GetRight();

		        while (r < Lines.Count && 
			           Lines[r].Rect.GetLeft() < dXLimit)
		        {
        			
			        if (  ( Lines[j].bSetFlag ^ Lines[r].bSetFlag  ) &&				
					        Lines[j].Rect.Overlaps(  Lines[r].Rect) &&
					        Lines[j].Line.Crosses(  Lines[r].Line,  IntPt) )
			        {
				        while (IntPt.Count > 0)
				        {
					        pPoints.Add( IntPt[ IntPt.Count - 1]);
                            IntPt.RemoveAt( IntPt.Count - 1);


					        if (Lines[j].bSetFlag)
						        pIndexesThis.Add( Lines[j].usIndex );
					        else
						        pIndexesThis.Add( Lines[r].usIndex );

					        if (Lines[j].bSetFlag)
						        pIndexesOther.Add( Lines[r].usIndex );
					        else
						        pIndexesOther.Add( Lines[j].usIndex );

				        }
			        }
			        r++;
		        }	
		        j++;
	        }

        }

        /// <summary>
        /// True if there are crossing lines within the set.
        /// </summary>
	    public bool HasCrossingLines()
        {

	        // Set up an array of these structures and the left most points of the line rects
            List<CLineBaseRect> Lines = new List<CLineBaseRect>();


	        for (int i = 0 ; i <  Count ; i++)
	        {
		        CLineBaseRect LineRect = new CLineBaseRect();
                LineRect.Line = this[i];
                LineRect.Line.GetBoundingRect( LineRect.Rect);
		        Lines.Add(LineRect);
	        }

            CLineBaseRectLeftToRight Comparitor = new CLineBaseRectLeftToRight();
            Lines.Sort(Comparitor);

	        int j = 0;
	        List<C2DPoint> IntPt = new List<C2DPoint>();
	        bool bIntersect = false;
	        // For each line...
	        while (j < Lines.Count && !bIntersect)
	        {
		        int r = j + 1;

		        double dXLimit = Lines[j].Rect.GetRight();
		        // ...search forward untill the end or a line whose rect starts after this ends
		        while ( !bIntersect && r < Lines.Count && Lines[r].Rect.GetLeft() < dXLimit )
		        {
			        if ( Lines[j].Rect.Overlaps(  Lines[r].Rect) &&
				        Lines[j].Line.Crosses(  Lines[r].Line,  IntPt) )
			        {
				        bIntersect = true;
			        }
			        r++;
		        }	
		        j++;
	        }

	        return bIntersect;

        }

        /// <summary>
        /// Checks for closure i.e. it forms a closed shape.
        /// </summary>
        /// <param name="bEndsOnly">Input. True to only check the ends of the array.</param>
        public bool IsClosed(bool bEndsOnly)
        {
	        int usSize = Count;
        	
	        if (bEndsOnly)
	        {
		        if (this[0].GetPointFrom().PointEqualTo(  this[usSize - 1].GetPointTo())  )
			        return true;
		        else
			        return false;
	        }
	        else
	        {
		        for (int i = 0; i < usSize; i++)
		        {
			        int usNext = (i + 1) % usSize;

			        if (!this[i].GetPointTo().PointEqualTo(  this[usNext].GetPointFrom() ))
				        return false;
		        }

		        return true;
	        }
        }

        /// <summary>
        /// Adds the other to this if there is a common end i.e. they can be joined up.
        /// </summary>
        /// <param name="Other">Input. The other set.</param>
        public bool AddIfCommonEnd( C2DLineBaseSet Other)
        {
            Debug.Assert(!IsClosed(true));
            Debug.Assert(!Other.IsClosed(true));

            int nThisCount = Count;
            if (nThisCount < 1) 
                return false;

            int nOtherCount = Other.Count;
            if (nOtherCount < 1) 
                return false;

            if (this[0].GetPointFrom().PointEqualTo(  Other[0].GetPointFrom())  )
            {
                ReverseDirection();

                this.ExtractAllOf(Other);

                return true;
            }
            else if (this[0].GetPointFrom().PointEqualTo(Other[nOtherCount - 1].GetPointTo()) )
            {
                ReverseDirection();

                Other.ReverseDirection();

                this.ExtractAllOf(Other);

                return true;
            }
            else if (this[nThisCount - 1].GetPointTo().PointEqualTo(  Other[0].GetPointFrom()))
            {
                this.ExtractAllOf(Other);

                return true;
            }
            else if (this[nThisCount - 1].GetPointTo().PointEqualTo( Other[nOtherCount - 1].GetPointTo()) )
            {
                Other.ReverseDirection();

                this.ExtractAllOf(Other);

                return true;
            }

            return false;

        }

        /// <summary>
        /// Removes lines that are small, based on the tolerance. 
        /// </summary>
        /// <param name="dTolerance">Input. The length defined to be null.</param>
        public void Remove0Lines(double dTolerance)
        {
	        for (int i = 0 ; i < Count; i++)
	        {
		        double dLength = this[i].GetLength();

		        if (dLength < dTolerance)
		        {
			        RemoveAt(i);
		        }
	        }
        }
        /// <summary>
        /// Reverses the direction. 
        /// </summary>
        public void ReverseDirection()
        {
	        this.Reverse();

	        for (int i = 0; i < Count ; i++)
	        {
		        this[i].ReverseDirection();
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
    }
}
