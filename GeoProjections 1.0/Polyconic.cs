/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file Polyconic.cpp
///Implementation file for a CPolyconic class.

Implementation file for a CPolyconic class.
---------------------------------------------------------------------------*/


using System;



namespace GeoLib
{

    /// <summary>
    /// Class representing an albers equal area projection.
    /// </summary>
    public class CPolyconic : CProjection
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public CPolyconic()
        {
	        m_dStandardLatitude = 0;

	        m_dStandardLongitude = 0;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CPolyconic()
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

	        double E = (dLongX - m_dStandardLongitude) * Math.Sin(dLatY);

	        dLongX = Math.Sin(E) / Math.Tan (dLatY);

	        dLatY = (dLatY - m_dStandardLatitude ) + (1 - Math.Cos(E)) /  Math.Tan(dLatY);
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
	        double A = m_dStandardLatitude + dLatY;

	        double B = dLongX * dLongX + A*A;

	        double dTolerance = 0.00000001;

	        double theta1 = A;
	        if (dLongX != 0)
		        theta1 = A * Math.Abs(dLongX)* 0.1; // Iteration start

	        double theta0 = theta1 + dTolerance * 10.0;

	        int nMaxIt = 500;

	        int nIt = 0; 

	        while ( Math.Abs(theta1 - theta0) > dTolerance)
	        {
		        theta0 = theta1;

		        double tan_theta =  Math.Tan(theta0);

		        theta1 -= (   A*(theta0*tan_theta + 1) - theta0 - 0.5*(theta0*theta0 + B)* Math.Tan(theta0)  )   /
								        ( ( theta0 - A) /  Math.Tan(theta0) - 1 );

		        nIt++;
		        if(nIt == nMaxIt)
		        {
		        //	assert(0);
			        break;
		        }
	        }

	        dLatY = theta1;

	        dLongX = Math.Asin(dLongX *  Math.Tan(theta1) ) / Math.Sin(theta1) + m_dStandardLongitude;

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
        void SetOrigin(double dLat, double dLong)
        {
	        m_dStandardLatitude = dLat * Constants.conRadiansPerDegree;

	        m_dStandardLongitude = dLong * Constants.conRadiansPerDegree;

        }

    	private double m_dStandardLatitude;

	    private double m_dStandardLongitude;
    }
}