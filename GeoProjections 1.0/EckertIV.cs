/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file EckertIV.cpp
///Implementation file for a CEckertIV class.

Implementation file for a CEckertIV class.
---------------------------------------------------------------------------*/


using System;


namespace GeoLib
{

    /// <summary>
    /// Class representing a circle.
    /// </summary>
    public class CEckertIV : CProjection
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public CEckertIV()
        {
	        m_dStandardLongitude = 0;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CEckertIV()
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

	        double theta1 = dLatY / 2.0; // Iteration start at lat / 2

	        double theta0 = theta1 + dTolerance * 10.0;

	        int nMaxIt = 50;

	        int nIt = 0; 

	        double sin_lat = Math.Sin(dLatY);

	        while ( Math.Abs(theta1 - theta0) > dTolerance)
	        {
		        theta0 = theta1;

		        double dSinTheta = Math.Sin(theta0);

		        double dCosTheta = Math.Cos(theta0);

		        theta1 -= (  theta0 + dSinTheta * dCosTheta + 2.0*dSinTheta - (2.0 + Constants.conHALFPI) * sin_lat ) /
						        ( 2.0 * dCosTheta * ( 1.0 + dCosTheta)  );

		        nIt++;
		        if(nIt == nMaxIt)
		        {
		        //	assert(0);
			        return;
		        }
	        }


	        dLongX = 2.0 * (dLongX - m_dStandardLongitude) * (1.0 + Math.Cos( theta1 )) / 
					        Math.Sqrt( Constants.conPI * (4.0 + Constants.conPI));

	        dLatY = 2.0 * Math.Sqrt( Constants.conPI / (4.0 + Constants.conPI)) * Math.Sin( theta1 );
        }



        /// <summary>
        ///Project the given lat long to x, y using the input parameters to store the result and retaining 
        /// the lat long in the class passed.
        /// </summary>
        public override void Project( CGeoLatLong rLatLong, double dx, double dy) 
        {
	        dy = rLatLong.GetLat();

	        dx = rLatLong.GetLong();

	        Project(dy, dx);
        }

        /// <summary>
        ///Project the given x y to lat long using the input parameters to store the result.	
        /// </summary>
        public override void InverseProject(double dLatY, double dLongX) 
        {
	        double sin_theta = dLatY * Math.Sqrt( (4.0 + Constants.conPI) / Constants.conPI) / 2.0;

	        double theta = Math.Asin( sin_theta );

	        double cos_theta = Math.Cos(theta);

	        dLatY = Math.Asin(   (theta + sin_theta * cos_theta + 2.0 * sin_theta) /
						          (2.0 + Constants.conHALFPI)   );

        //	m_dLong = dStandardLongitude + Constants.conPI * Math.Sqrt(4 + Constants.conPI) * Point.x / 
	        //									(1 + dCosTheta);		// out by approx  * 2+ PI/2
        	
	        dLongX = m_dStandardLongitude + 1.00730304626 * (Constants.conPI * dLongX) / 
										        ((1.0 + cos_theta) * Math.Sqrt(1.0 + Constants.conHALFPI / 2.0)); // corrected.


	        dLatY *= Constants.conDegreesPerRadian;

	        dLongX *= Constants.conDegreesPerRadian;
        }


        /// <summary>
        ///Project the given x y to lat long using the input lat long class to get the result.
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
        /// Sets the standard longitude.
        /// </summary>
        public void SetStandardLongitude(double dStandardLongitude)
        {
	        m_dStandardLongitude = dStandardLongitude * Constants.conRadiansPerDegree;
        }

        private double m_dStandardLongitude;
    }
}