/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file LambertConformalConic.cpp
///Implementation file for a CLambertConformalConic class.

Implementation file for a CLambertConformalConic class.
---------------------------------------------------------------------------*/



using System;


namespace GeoLib
{

    /// <summary>
    /// Class representing a circle.
    /// </summary>
    public class CLambertConformalConic : CProjection
    {


        /// <summary>
        /// Constructor.
        /// </summary>
        public CLambertConformalConic() 
        {
	        m_dStandardParallel1 = 50 * Constants.conRadiansPerDegree;

	        m_dStandardParallel2 = -10 * Constants.conRadiansPerDegree;

	        m_dOriginLat = 0;

	        m_dOriginLong = 0;

	        CalculateConstants();
        }
       
        /// <summary>
        /// Destructor.
        /// </summary>
        ~CLambertConformalConic()
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


            double p = m_dF * Math.Pow(1 / Math.Tan(Constants.conQUARTPI + dLatY / 2), m_dn);

            double p0 = m_dF * Math.Pow(1 / Math.Tan(Constants.conQUARTPI + m_dOriginLat / 2), m_dn);

	        double x = p* Math.Sin( m_dn * (dLongX - m_dOriginLong));

	        double y = p0 - p * Math.Cos( m_dn * (dLongX  - m_dOriginLong));

	        dLongX = x;

	        dLatY = y;
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
            double p0 = m_dF * Math.Pow(1 / Math.Tan(Constants.conQUARTPI + m_dOriginLat / 2), m_dn);


	        double p = Math.Sqrt(dLongX * dLongX + (p0 - dLatY)* (p0 - dLatY));
	        if (m_dn < 0)
		        p = -p;

	        double theta = Math.Atan(dLongX / (p0 - dLatY));

	        dLatY = 2 * Math.Atan(  Math.Pow( m_dF / p, 1/m_dn) ) - Constants.conHALFPI;

	        dLongX = m_dOriginLong + theta / m_dn;



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
        void SetStandardParallels(double dStandardParallel, double dStandardParalle2)
        {
	        m_dStandardParallel1 = dStandardParallel * Constants.conRadiansPerDegree;

	        m_dStandardParallel2 = dStandardParalle2 * Constants.conRadiansPerDegree;

	        CalculateConstants();
        }


        /// <summary>
        ///
        /// </summary>
        void SetOrigin(double dLat, double dLong)
        {
	        m_dOriginLat = dLat * Constants.conRadiansPerDegree;

	        m_dOriginLong = dLong * Constants.conRadiansPerDegree;

	        CalculateConstants();
        }

        /// <summary>
        ///
        /// </summary>
        private void CalculateConstants()
        {
	        double cos_phi1 = Math.Cos(m_dStandardParallel1);

	        double n1 = Math.Log( cos_phi1 * (1 / Math.Cos (m_dStandardParallel2)));
	        double n2 =  Math.Tan( Constants.conQUARTPI + m_dStandardParallel2 / 2);
            double n3 = 1 / (Math.Tan(Constants.conQUARTPI + m_dStandardParallel1 / 2));
	        m_dn = n1 / Math.Log(n2 * n3 );
            m_dF = cos_phi1 * Math.Pow(Math.Tan(Constants.conQUARTPI + m_dStandardParallel1 / 2), m_dn) / m_dn;
        }

	    private double m_dStandardParallel1;

        private double m_dStandardParallel2;

        private double m_dOriginLat;

        private double m_dOriginLong;

	    // derived constants

        private double m_dn;

        private double m_dF;
    }
}