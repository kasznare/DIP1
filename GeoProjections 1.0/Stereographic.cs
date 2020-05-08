/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file Stereographic.cpp
///Implementation file for a CStereographic class.

Implementation file for a CStereographic class.
---------------------------------------------------------------------------*/



using System;



namespace GeoLib
{

    /// <summary>
    /// Class representing an albers equal area projection.
    /// </summary>
    public class CStereographic : CProjection
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public CStereographic() 
        {
	        m_dStandardLatitude = 0;

	        m_dStandardLongitude = 0;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CStereographic()
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

	        double sin_olat = Math.Sin(m_dStandardLatitude);
	        double sin_lat = Math.Sin(dLatY);
	        double cos_olat = Math.Cos(m_dStandardLatitude);
	        double cos_lat = Math.Cos(dLatY);
	        double cos_dlong = Math.Cos(dLongX - m_dStandardLongitude);

	        double k = 2* Constants.conEARTH_RADIUS_METRES /
		        ( 1 + sin_olat * sin_lat + cos_olat * cos_lat * cos_dlong);

	        dLongX = k * cos_lat * Math.Sin(dLongX  - m_dStandardLongitude);

	        dLatY = k * (cos_olat * sin_lat - sin_olat * cos_lat * cos_dlong);

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
        /// Project the given x y to lat long using the input parameters to store /// the result.	
        /// </summary>
        public override void InverseProject(double dLatY, double dLongX) 
        {
	        double p = Math.Sqrt(dLongX * dLongX + dLatY * dLatY);

            double c = 2 * Math.Atan2(p, 2 * Constants.conEARTH_RADIUS_METRES);
        	
	        double cos_c = Math.Cos(c);
	        double sin_c = Math.Sin(c);
	        double sin_olat = Math.Sin(m_dStandardLatitude);

	        double cos_olat = Math.Cos(m_dStandardLatitude);

	        double dLat = Math.Asin( cos_c * sin_olat + dLatY * sin_c * cos_olat / p);

	        double dLong =  Math.Atan2(dLongX * sin_c, p * cos_olat * cos_c - dLatY * sin_olat * sin_c);

	        dLong += m_dStandardLongitude;


	        dLatY = dLat * Constants.conDegreesPerRadian;

	        dLongX = dLong * Constants.conDegreesPerRadian;
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
        public void SetOrigin(double dLat, double dLong)
        {
	        m_dStandardLatitude = dLat * Constants.conRadiansPerDegree;

	        m_dStandardLongitude = dLong * Constants.conRadiansPerDegree;
        }
        private double m_dStandardLatitude;
        private double m_dStandardLongitude;
    }
}