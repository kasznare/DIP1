/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file LambertAzimuthalEqualArea.cpp
///Implementation file for a CLambertAzimuthalEqualArea class.

Implementation file for a CLambertAzimuthalEqualArea class.
---------------------------------------------------------------------------*/

using System;


namespace GeoLib
{

    /// <summary>
    /// Class representing a circle.
    /// </summary>
    public class CLambertAzimuthalEqualArea : CProjection
    {


        /// <summary>
        /// Constructor.
        /// </summary>
        public CLambertAzimuthalEqualArea()
        {
	        m_dStandardParallel = 0;

	        m_dCentralLongitude = 0;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CLambertAzimuthalEqualArea()
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

	        double sin_SP = Math.Sin(m_dStandardParallel);
	        double sin_lat = Math.Sin(dLatY);

	        double cos_SP = Math.Cos(m_dStandardParallel);
	        double cos_lat = Math.Cos(dLatY);
	        double cos_Dlong = Math.Cos(dLongX - m_dCentralLongitude);

	        double dK = Math.Sqrt(2.0 / ( 1 + sin_SP*sin_lat + cos_SP*cos_lat*cos_Dlong));

	        dLongX = dK * cos_lat * Math.Sin(dLongX - m_dCentralLongitude);

	        dLatY = dK*( cos_SP* sin_lat - sin_SP * cos_lat * cos_Dlong);
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
	        double P1 = dLongX * dLongX + dLatY * dLatY;

	        double P = Math.Sqrt( P1 );
        	
	        double C = 2 * Math.Asin( 0.5 * P);

	        double cos_C = Math.Cos(C);

	        double sin_C = Math.Sin(C);

	        double sin_SP = -Math.Sin(m_dStandardParallel);

	        double cos_SP = Math.Cos(m_dStandardParallel);

	        double dLat = Math.Asin( cos_C* sin_SP + (dLatY * sin_C * cos_SP) / P);

	        double dLong;

	        if (P1 < 2)
	        {
			        dLong = m_dCentralLongitude + Math.Atan(dLongX * sin_C /
								        (P *cos_SP * cos_C - dLatY * sin_SP * sin_C)  );
	        }
	        else
	        {
		        if (dLongX	> 0 )
			        dLong = m_dCentralLongitude + Constants.conPI + Math.Atan( dLongX * sin_C /
									        (P *cos_SP * cos_C - dLatY* sin_SP * sin_C)  );
		        else
			        dLong = m_dCentralLongitude - Constants.conPI + Math.Atan( dLongX * sin_C /
											        (P *cos_SP * cos_C - dLatY * sin_SP * sin_C)  );
	        }

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
	        m_dStandardParallel = dLat * Constants.conRadiansPerDegree;

	        m_dCentralLongitude = dLong * Constants.conRadiansPerDegree;

        }

    	double m_dStandardParallel;

	    double m_dCentralLongitude;
    }
}