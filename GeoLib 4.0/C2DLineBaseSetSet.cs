using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{
    /// <summary>
    /// Class representing a set of a set of lines. Each line set can be thought
    /// of as a route or a part of a polygon. Generally used for breaking up
    /// polygns and putting them back together again.
    /// </summary>
    public class C2DLineBaseSetSet : List< C2DLineBaseSet>
    {
        /// <summary>
	    /// Constructor
        /// </summary>
        public C2DLineBaseSetSet() { }
        /// <summary>
	    /// Destructor
        /// </summary>
        ~C2DLineBaseSetSet() { }

        /// <summary>
        /// Extracts all of the line sets frm the other.
        /// </summary>
        /// <param name="S2">The other set.</param>
        public void ExtractAllOf(List<C2DLineBaseSet> S2)
        {
            for (int i = 0; i < S2.Count; i++)
            {
                Add(S2[i]);
            }
            S2.Clear();
        }

        /// <summary>
        /// Extracts at the index supplied.
        /// </summary>
        /// <param name="nIndex">The index to extract at.</param>
        public C2DLineBaseSet ExtractAt(int nIndex)
        {
            C2DLineBaseSet Result = this[nIndex];
            this.RemoveAt(nIndex);
            return Result;
        }
        /// <summary>
        /// Merges the joining routes together if there are any.
        /// </summary>
	    public void MergeJoining()
        {
	        C2DLineBaseSetSet Temp = new C2DLineBaseSetSet();

	        while (Count > 0)
	        {
		        // pop the last one.
		        C2DLineBaseSet pLast = this[Count - 1];
                this.RemoveAt(Count - 1);

		        if (!pLast.IsClosed(true))
		        {
			        int i = 0 ;
			        while ( i < Count )
			        {
				        if ( ! this[i].IsClosed(true))
				        {
					        if (this[i].AddIfCommonEnd( pLast))
					        {
						        pLast = null;
                                i += Count;	// escape
					        }
				        }

				        i++;
			        }
		        }

		        if (pLast != null)
		        {
			        Temp.Add( pLast);
		        }
	        }

	        this.ExtractAllOf(Temp);
        }

    //public void DebugOut()
    //{
    //   // String strOut = new string("r");

    //    String strOut = String.Format("Count {0} \n", Count);
    //    System.Diagnostics.Trace.TraceInformation(strOut);

    //    for (int i = 0; i < Count; i++)
    //    {
    //        String strOut0 = String.Format("{0}: Count {1} ", i, this[i].Count);
    //      //  System.Diagnostics.Trace.TraceInformation(strOut);

    //        //for (int j = 0 ; j < this->GetAt(i)->size(); j++)
    //        //{
    //        //	C2DPoint p1 = this->GetAt(i)->GetAt(j)->GetPointFrom();
    //        //	sprintf(buff, "x:%f  y:%f", p1.x, p1.y);
    //        //}

    //        C2DPoint p1 = this[i][0].GetPointFrom();
    //        String strOut1 = String.Format("From x:{0}  y:{1}", p1.x, p1.y);
    //      //  System.Diagnostics.Trace.TraceInformation(strOut);

    //        C2DPoint p2 = this[i][this[i].Count - 1].GetPointTo();
    //        String strOut2 = String.Format("To x:{0}  y:{1} \n", p2.x, p2.y);
    //        System.Diagnostics.Trace.TraceInformation(strOut0 + strOut1 + strOut2);
    //    }
    //}



        /// <summary>
        /// Adds all the routes from the other to this if the join a routes in this.
        /// </summary>
        /// <param name="Other">The other set.</param>
	    public void AddJoining(  C2DLineBaseSetSet Other )
        {
	        C2DLineBaseSetSet Temp = new C2DLineBaseSetSet();
        	
	        while (Other.Count > 0 )
	        {
		        C2DLineBaseSet pLast = Other.ExtractAt(Other.Count - 1);

		        int i = 0;
		        while ( i < Count)
		        {
			        if ( !this[i].IsClosed(true) && this[i].AddIfCommonEnd( pLast))
			        {
				        pLast = null;
                        i += Count;	// escape
			        }

			        i++;
		        }

		        if (pLast != null)
		        {
			        Temp.Add(pLast);
		        }
	        }

	        while (Temp.Count > 0)
	        {
                Other.Add(Temp.ExtractAt(Temp.Count - 1));
	        }
        }

        /// <summary>
        /// Adds the routes in the other set that are closed.        
        /// </summary>
        /// <param name="Other">The other set.</param>
        /// <param name="bEndsOnly">True if only the ends require checking.</param>
	    public void AddClosed( C2DLineBaseSetSet Other , bool bEndsOnly)
        {
            C2DLineBaseSetSet Temp = new C2DLineBaseSetSet();

            while (Other.Count > 0)
            {
                C2DLineBaseSet pLast = Other.ExtractAt(Other.Count - 1);
                if (pLast.IsClosed(bEndsOnly))
                {
                    this.Add(pLast);
                }
                else
                {
                    Temp.Add(pLast);
                }
            }

            while (Temp.Count > 0)
            {
                Other.Add(Temp.ExtractAt(Temp.Count - 1));
            }
        }
    }
}
