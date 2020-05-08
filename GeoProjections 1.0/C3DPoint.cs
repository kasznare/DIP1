/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file 3DPoint.cpp
/// Implementation file for a 3D point class.

Implementation file for a simple 3D point class.
---------------------------------------------------------------------------*/

using System;


namespace GeoLib
{
    /// <summary>
    /// Class for a 3D point.
    /// </summary>
    public class C3DPoint 
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public C3DPoint()
        {
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~C3DPoint()
        {
        }

        /// <summary>
        /// Distance to another.
        /// </summary>
        public double Distance(C3DPoint Other)
        {
	        double dx = x - Other.x;
	        double dy = y - Other.y;
	        double dz = z - Other.z;
	        return Math.Sqrt(  dx*dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// x co-ordinate.
        /// </summary>
        public double x;
        /// <summary>
        /// y co-ordinate.
        /// </summary>
        public double y;
        /// <summary>
        /// z co-ordinate.
        /// </summary>
        public double z;
    }
}