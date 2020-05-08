/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file VerticalPerspective.cpp
///Implementation file for a CVerticalPerspective class.

Implementation file for a CVerticalPerspective class.
---------------------------------------------------------------------------*/



using System;



namespace GeoLib
{

    /// <summary>
    /// Class representing an albers equal area projection.
    /// </summary>
    public class CVerticalPerspective : CProjection
    {

        /// <summary>
        ///Destructor.
        /// </summary>
        public CVerticalPerspective() 
        {
	        m_dStandardLatitude = 0;

	        m_dStandardLongitude = 0;

	        m_P = 10.0F;
        }

        /// <summary>
        ///Destructor.
        /// </summary>
        ~CVerticalPerspective()
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


	        double sin_lat = Math.Sin(dLatY);

	        double cos_lat = Math.Cos(dLatY);

	        double sin_olat = Math.Sin(m_dStandardLatitude);

	        double cos_olat = Math.Cos(m_dStandardLatitude);

	        double cos_dlong = Math.Cos(dLongX  - m_dStandardLongitude);

	        double cos_c = sin_olat * sin_lat + cos_olat * cos_lat * cos_dlong;

	        double k = (m_P - 1) / (m_P - cos_c);

	        dLongX = k * cos_lat * Math.Sin(dLongX  - m_dStandardLongitude);

	        dLatY = k * ( cos_olat * sin_lat - sin_olat * cos_lat * cos_dlong);

        //	if (pBack != NULL)
	        //	*pBack = Math.Cos_c < (1 / P);


        }


        /// <summary>
        /// Project the given lat long to x, y using the input parameters to store the result and retaining 
        /// /// the lat long in the class passed.
        /// </summary>
        public override void Project(CGeoLatLong rLatLong, double dx, double dy) 
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
	        // TO DO - Need proper inverse projection. Current inverse is valid for infinite P only
	        double dHeading =  Math.Atan2(dLongX,dLatY);

	        double dHRange = Math.Sqrt(dLongX * dLongX + dLatY * dLatY);

	        // alpha is the angular Distance travelled round the earths surface
	        double alpha = Math.Asin( dHRange );

        //	double k = (P - 1) / (P - alpha);

	        double sin_alpha = dHRange;
	        double cos_alpha = Math.Cos(alpha);

	        double latO = m_dStandardLatitude;
	        double longO = m_dStandardLongitude;

	        double sin_Olat = Math.Sin(latO);
	        double cos_Olat = Math.Cos(latO);

	        double dLat = Math.Asin(  sin_Olat * cos_alpha + cos_Olat * sin_alpha * Math.Cos(dHeading)  );

	        double dLong = longO +  Math.Atan2(Math.Sin(dHeading) * sin_alpha * cos_Olat , cos_alpha - sin_Olat * Math.Sin(dLat));

	        while (dLong > Constants.conPI)
                dLong -= Constants.conTWOPI;
	        while (dLong < -Constants.conPI)
                dLong += Constants.conTWOPI;


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


        /// <summary>
        ///
        /// </summary>
        public void SetPointOfPerspective(float p)
        {
            m_P = p;
        }

        private double m_dStandardLatitude;

	    private double m_dStandardLongitude;

	    private float m_P;
    }
}