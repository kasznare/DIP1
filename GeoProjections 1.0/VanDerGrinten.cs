/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file CVanDerGrinten.cpp
///Implementation file for a CVanDerGrinten class.

Implementation file for a CVanDerGrinten class.
---------------------------------------------------------------------------*/



using System;



namespace GeoLib
{

    /// <summary>
    /// Class representing an albers equal area projection.
    /// </summary>
    public class CVanDerGrinten : CProjection
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public CVanDerGrinten()
        {
	        m_dStandardLongitude = 0;
        }


        /// <summary>
        /// Destructor.
        /// </summary>
        ~CVanDerGrinten()
        {
        }

        /// <summary>
        ///Constructor.
        /// </summary>
        public override void Project(double dLatY, double dLongX) 
        {
	        dLatY *= Constants.conRadiansPerDegree;

	        dLongX *= Constants.conRadiansPerDegree;

	        double A1 = Constants.conPI / (dLongX  - m_dStandardLongitude);

	        double A = 0.5 * Math.Abs( A1 - 1.0 / A1);

	        double theta = Math.Asin( Math.Abs(2.0* dLatY / Constants.conPI) );

	        double cos_theta = Math.Cos(theta);

	        double sin_theta = Math.Sin(theta);

	        double G = cos_theta / ( sin_theta + cos_theta - 1.0);

	        double P = G*(2.0/ Math.Sin(theta) - 1.0);

	        double P2 = P*P;
	        double A2 = A*A;

	        double Q = A2 + G;


	        double P2A2 = P2 + A2;
	        double x0 = G - P2;
	        double x1 = A * x0;
	        double x2 = A2 * x0 * x0 - (P2A2)*(G*G - P2);
	        double x = Constants.conPI * ( x1 + Math.Sqrt( x2 ) ) /
					        P2A2;

	        if (dLongX  < m_dStandardLongitude )
		        x = -x;


	        double y = Constants.conPI * Math.Abs( P*Q - A* Math.Sqrt( (A2 + 1)*(P2A2) - Q*Q)   )  /
					        P2A2;

	        if (dLatY < 0)
		        y = -y;


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
        /// Project the given x y to lat long using the input parameters to store 
        /// the result.	
        /// </summary>
        public override void InverseProject(double dLatY, double dLongX) 
        {
	        double X = dLongX / Constants.conPI;
	        double X2 = X * X;
	        double Y = dLatY / Constants.conPI;
	        double Y2 = Y * Y;
	        double X2Y2 = X2 + Y2;
	        double c1 = - Math.Abs(Y) * (1 + X2Y2);
	        double c2 = c1 - 2.0 * Y2 + X2;
	        double c3 = -2.0 * c1 + 1 + 2 * Y2 + X2Y2* X2Y2;
	        double d = Y2 / c3 + (2* c2*c2*c2 / (c3*c3*c3) - 9 * c1*c2 / (c3*c3)) / 27.0;
	        double a1 = (c1 - c2*c2 / (3*c3)) / c3;
	        double m1 = 2 * Math.Sqrt( - a1 / 3.0);
	        double theta1 = Math.Acos(3.0 * d / ( a1 *m1)) / 3.0;

	        double dLat = Constants.conPI * ( -m1 * Math.Cos(theta1 + Constants.conTHIRDPI) - c2 / (3*c3));
	        if (dLatY < 0)
		        dLat = -dLat;

	        double dLong = Constants.conPI *( X2 + Y2 - 1 + Math.Sqrt( 1.0 + 2.0 * ( X2 - Y2) + X2Y2 *  X2Y2 )) / 
					        (2.0 * X);

	        dLong += m_dStandardLongitude;

	        if (dLong > Constants.conPI)
		        dLong = Constants.conPI;
	        if (dLong < -Constants.conPI)
		        dLong = - Constants.conPI;

	        if (dLat > Constants.conHALFPI)
		        dLat = Constants.conHALFPI;
	        if (dLat < -Constants.conHALFPI)
		        dLat = - Constants.conHALFPI;


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
        public void SetStandardLongitude(double dStandardLongitude)
        {
	        m_dStandardLongitude = dStandardLongitude * Constants.conRadiansPerDegree;
        }

        private double m_dStandardLongitude;
    }
}