/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file CSinusoidal.cpp
///Implementation file for a CSinusoidal class.

Implementation file for a CSinusoidal class.
---------------------------------------------------------------------------*/

using System;



namespace GeoLib
{

    /// <summary>
    /// Class representing an albers equal area projection.
    /// </summary>
    public class CSinusoidal : CProjection
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public CSinusoidal()
        {
	        m_dStandardLongitude = 0;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CSinusoidal()
        {
        }

        /// <summary>
        ///Constructor.
        /// </summary>
        public override void Project(double dLatY, double dLongX) 
        {
	        dLatY *= Constants.conRadiansPerDegree;

	        dLongX *= Constants.conRadiansPerDegree;

	        dLongX = (dLongX - m_dStandardLongitude) * Math.Cos(dLatY);
        }

        /// <summary>
        /// Project the given lat long to x, y using the input parameters to store the result and retaining 
        /// the lat long in the class passed.
        /// </summary>
        public override void Project( CGeoLatLong rLatLong, double dx, double dy) 
        {
	        dy = rLatLong.GetLat();

	        dx = rLatLong.GetLong();

	        Project(dy, dx);
        }

        /// <summary>
        /// Project the given x y to lat long using the input parameters to store 
        /// the result.	
        /// </summary>
        public override void InverseProject(double dLatY, double dLongX) 
        {
	        dLongX = m_dStandardLongitude + dLongX / Math.Cos(dLatY);

	        dLatY *= Constants.conDegreesPerRadian;

	        dLongX *= Constants.conDegreesPerRadian;
        }


        /// <summary>
        /// Project the given x y to lat long using the input lat long class to get the result.
        /// </summary>
        public override void InverseProject(CGeoLatLong rLatLong,  double dX,  double dY) 
        {
	        double dLat = dY;

	        double dLong = dX;

	        InverseProject(dLat, dLong);

	        rLatLong.SetLatDegrees(dLat);

	        rLatLong.SetLongDegrees(dLong);
        }

        /// <summary>
        ///
        /// </summary>
        public void SetStandardLongitude(double dStandardLongitude)
        {
	        m_dStandardLongitude = dStandardLongitude * Constants.conRadiansPerDegree;
        }

        private double m_dStandardLongitude;
    }
}