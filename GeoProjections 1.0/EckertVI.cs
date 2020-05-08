/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file EckertVI.cpp
///Implementation file for a CEckertVI class.

Implementation file for a CEckertVI class.
---------------------------------------------------------------------------*/


using System;


namespace GeoLib
{

    /// <summary>
    /// Class representing a circle.
    /// </summary>
    public class CEckertVI : CProjection
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public CEckertVI()
        {
	        m_dStandardLongitude = 0;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CEckertVI()
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


	        double dTolerance = 0.00000001;

	        double theta1 = dLatY; // Iteration start at lat

	        double theta0 = theta1 + dTolerance * 10;

	        int nMaxIt = 50;

	        int nIt = 0; 

	        double sin_lat = Math.Sin(dLatY);

	        while ( Math.Abs(theta1 - theta0) > dTolerance)
	        {
		        theta0 = theta1;

		        theta1 -= (  theta0 + Math.Sin(theta0) - (1 + Constants.conHALFPI) * sin_lat ) /
						        ( 1 + Math.Cos (theta0));

		        nIt++;
		        if(nIt == nMaxIt)
		        {
		        //	assert(0);

			        return;
		        }
	        }

	        double dDen = Math.Sqrt(2 + Constants.conPI);

	        dLongX = (dLongX - m_dStandardLongitude) * (1 + Math.Cos (theta1)) /
					        dDen;

	        dLatY = 2 * theta1 / dDen;
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
	        double dA = Math.Sqrt( 2 + Constants.conPI);

	        double theta = 0.5 * dA * dLatY;

	        dLatY = Math.Asin(  (theta + Math.Sin(theta)) / ( 1 + Constants.conHALFPI));

	        dLongX = m_dStandardLongitude + (dA * dLongX ) / (1 + Math.Cos(theta));


	        dLatY *= Constants.conDegreesPerRadian;

	        dLongX *= Constants.conDegreesPerRadian;
        }


        /// <summary>
        /// Project the given x y to lat long using the input lat long class to get the result.
        /// </summary>
        public override void InverseProject(CGeoLatLong rLatLong,  double dX,  double dY) 
        {
	        double dLatY = dY;

	        double dLongX = dX;

	        InverseProject(dLatY, dLongX);

	        rLatLong.SetLat(dLatY);

	        rLatLong.SetLong(dLongX);
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