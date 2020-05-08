/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file Gnomonic.cpp
///Implementation file for a CGnomonic class.

Implementation file for a CGnomonic class.
---------------------------------------------------------------------------*/


using System;


namespace GeoLib
{

    /// <summary>
    /// Class representing a circle.
    /// </summary>
    public class CGnomonic : CProjection
    {


        /// <summary>
        /// Constructor.
        /// </summary>
        public CGnomonic()
        {
	        m_dStandardLatitude = 0;

	        m_dStandardLongitude = 0;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CGnomonic()
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

	        double cos_lat = Math.Cos(dLatY);
	        double sin_lat = Math.Sin(dLatY);

	        double sin_Olat = Math.Sin(m_dStandardLatitude);
	        double cos_Olat = Math.Cos(m_dStandardLatitude);

	        double cos_Dlong = Math.Cos(dLongX - m_dStandardLongitude);

	        double cos_C = sin_Olat* sin_lat + cos_Olat* cos_lat* cos_Dlong;

	        dLongX = cos_lat * Math.Sin(dLongX - m_dStandardLongitude) / cos_C;
        					
	        dLatY = (cos_Olat * sin_lat - sin_Olat * cos_lat * cos_Dlong ) / cos_C;
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
        /// Project the given x y to lat long using the input parameters to store the result.	
        /// </summary>
        public override void InverseProject(double dLatY, double dLongX) 
        {
	        double P = Math.Sqrt(dLongX * dLongX + dLatY * dLatY);

	        double C = Math.Atan( P );

	        double sin_Olat = Math.Sin(m_dStandardLatitude);
	        double cos_Olat = Math.Cos(m_dStandardLatitude);

	        double sin_C = Math.Sin(C);
	        double cos_C = Math.Cos(C);

	        double dLat = Math.Asin(  cos_C* sin_Olat + (dLatY * sin_C * cos_Olat) / P);

        //	double dLong = m_dStandardLongitude + 
        //		a Math.Tan( dLongX * sin_C / (P * Math.Cos_Olat * Math.Cos_C - dLatY * sin_Olat * sin_C)  );
	        double dLong = m_dStandardLongitude + 
		         Math.Atan2( dLongX * sin_C , (P * cos_Olat * cos_C - dLatY * sin_Olat * sin_C)  );

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
        void SetOrigin(double dLat, double dLong)
        {
	        m_dStandardLatitude = dLat * Constants.conRadiansPerDegree;

	        m_dStandardLongitude = dLong * Constants.conRadiansPerDegree;

        }

        private double m_dStandardLatitude;

	    private double m_dStandardLongitude;
    }
}