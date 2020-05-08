using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{

    /// <summary>
    /// Class to hold an interval as 2 doubles.
    /// </summary>
    public class CInterval
    {
        /// <summary>
	    /// Constructor
        /// </summary>
        public CInterval()
        {
	        dMin = 0;
    	    dMax = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CInterval(double dMinimum, double dMaximum)
        {
            dMin = dMinimum;
            dMax = dMaximum;
        }

        /// <summary>
	    /// Destructor
        /// </summary>
	    ~CInterval()
        {

        }

        /// <summary>
	    /// Expands the interval to include the other
        /// </summary>
        public void ExpandToInclude( CInterval Other)
        {
    	    if (Other.dMax > dMax) dMax = Other.dMax;
    	    if (Other.dMin < dMin) dMin = Other.dMin;
        }

        /// <summary>
	    /// Expands the interval to include the value
        /// </summary>
        public void ExpandToInclude(double dValue)
        {
    	    if (dValue > dMax) dMax = dValue;
    	    else if (dValue < dMin) dMin = dValue;
        }

        /// <summary>
	    /// Returns the distance between the min and the max
        /// </summary>
        public double GetLength() 
        {
            return dMax - dMin;
        }

        /// <summary>
	    /// Assignement
        /// </summary>
        public void Set(CInterval Other)
        {
    	    dMax = Other.dMax;
    	    dMin = Other.dMin;	
        }

        /// <summary>
	    /// True if this overlaps the other
        /// </summary>
        public bool Overlaps(CInterval Other)
        {
            return (!IsBelow(Other) && !IsAbove(Other));
        }


        /// <summary>
        /// True is this overlaps the other.
        /// </summary>
        /// <param name="Other"></param>
        /// <param name="Overlap"></param>
        /// <returns></returns>
        public bool Overlaps(CInterval Other, CInterval Overlap)
        {
	        if (Other.dMin < dMax &&
		        Other.dMax > dMin)
	        {
		        Overlap.dMin = Math.Max(Other.dMin, dMin);
		        Overlap.dMax = Math.Min(Other.dMax, dMax);
		        return true;
	        }
	        else
	        {
		        return false;
	        }
        }

        /// <summary>
	    /// True if this contains the value
        /// </summary>
        public bool Contains(double dValue)
        {
    	    return ( dMin <= dValue && dValue <= dMax);
        }

        /// <summary>
	    /// True if this contains the other
        /// </summary>
        public bool Contains(CInterval Other)
        {
	        return (   Contains(Other.dMin) && Contains (Other.dMax)   );
        }

        /// <summary>
	    /// True if this is entirely above the other
        /// </summary>
        public bool IsAbove(CInterval Other)
        {
            return  dMin > Other.dMax;
        }

        /// <summary>
	    /// True if this is entirely below the other
        /// </summary>
        public bool IsBelow(CInterval Other)
        {
    	    return dMax < Other.dMin;
        }

        /// <summary>
	    /// The min 
        /// </summary>
        public double dMin;

        /// <summary>
	    /// The max
        /// </summary>
        public double dMax;

    }
}
