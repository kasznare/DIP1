/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file CCassini.cpp
\brief Implementation file for a CCassini class.

Implementation file for a CCassini class.
---------------------------------------------------------------------------*/


using System;



namespace GeoLib
{

    /// <summary>
    /// Class representing an albers equal area projection.
    /// </summary>
    public class CCassini : CProjection
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public CCassini()
        {
	        m_dStandardLongitude = 0;
        }


        /// <summary>
        /// Destructor.
        /// </summary>
        ~CCassini()
        {
        }

        /// <summary>
        /// Project the given lat long to x, y using the input parameters to store the 
        /// result.
        /// </summary>
        public override void Project(double dLatY, double dLongX)
        {
            dLatY *= Constants.conRadiansPerDegree;

	        dLongX *= Constants.conRadiansPerDegree;

            double dX = Math.Asin(Math.Cos(dLatY) * Math.Sin(dLongX - m_dStandardLongitude));

            double dY = Math.Atan2(Math.Tan(dLatY), Math.Cos(dLongX - m_dStandardLongitude));

	        dLatY = dY;

	        dLongX = dX;
        }


        /// <summary>
        /// Project the given lat long to x, y using the input parameters to store the result and retaining 
        /// the lat long in the class passed.
        /// </summary>
        public override void Project(CGeoLatLong rLatLong, double dx, double dy)
        {
	        dy = rLatLong.GetLat();
	        dx = rLatLong.GetLong();

	        Project(dy, dx);
        }

        /// <summary>
        /// Project the given x y to lat long using the input parameters to store the result.	
        /// </summary>
        public override void InverseProject(double dLatY, double dLongX)
        {
	        double dD = dLatY + m_dStandardLongitude;

	        double dLat = Math.Asin( Math.Sin(dD) * Math.Cos( dLongX) );

	        double dLong = m_dStandardLongitude + Math.Atan2( Math.Tan(dLongX) , Math.Cos(dD));

	        dLatY = dLat * Constants.conDegreesPerRadian;

	        dLongX = dLong * Constants.conDegreesPerRadian;
        }


        /// <summary>
        /// Project the given x y to lat long using the input lat long class to get the result.
        /// </summary>
        public override void InverseProject(CGeoLatLong rLatLong, double dX, double dY)
        {
	        double dD = dY + m_dStandardLongitude;

	        double dLat = Math.Asin( Math.Sin(dD) * Math.Cos( dX) );

	        double dLong = m_dStandardLongitude + Math.Atan2( Math.Tan(dX) , Math.Cos(dD));

	        rLatLong.SetLat(dLat);

	        rLatLong.SetLong(dLong);
        }

        /// <summary>
        /// Set Standard Longitude.
        /// </summary>
        public void SetStandardLongitude(double dStandardLongitude)
        {
	        m_dStandardLongitude = dStandardLongitude * Constants.conRadiansPerDegree;
        }

        private	double m_dStandardLongitude;
    }
}