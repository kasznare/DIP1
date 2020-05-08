/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file HorizontalRangeHeading.cpp
///Implementation file for a CHorizontalRangeHeading class.

Implementation file for a CHorizontalRangeHeading class.
---------------------------------------------------------------------------*/



using System;


namespace GeoLib
{

    /// <summary>
    /// Class representing a circle.
    /// </summary>
    public class CHorizontalRangeHeading : CProjection
    {


        /// <summary>
        /// Constructor.
        /// </summary>
        public CHorizontalRangeHeading()
        {
        	
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CHorizontalRangeHeading()
        {
        }

        /// <summary>
        /// Project the given lat long to x, y using the input parameters to store the 
        /// result.
        /// </summary>
        public override void Project(double dLatY, double dLongX) 
        {
            CGeoLatLong LatLong = new CGeoLatLong(dLatY * Constants.conRadiansPerDegree, dLongX * Constants.conRadiansPerDegree);

	        double dRange;
	        double dHeading;

	        m_Origin.RangeAndHeading(LatLong, out dRange, out dHeading);

            dRange = Constants.conEARTH_RADIUS_METRES * Math.Sin(dRange / Constants.conEARTH_RADIUS_METRES);

	        dLatY = dRange * Math.Cos(dHeading);

	        dLongX = dRange * Math.Sin(dHeading);
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
	        double dHdng =  Math.Atan2(dLongX, dLatY);

	        double dRange = Math.Sqrt(dLongX * dLongX + dLatY * dLatY);

            dRange = Math.Asin(dRange / Constants.conEARTH_RADIUS_METRES) * Constants.conEARTH_RADIUS_METRES;

	        CGeoLatLong Result = new CGeoLatLong();

	        Result.Set(dHdng, dRange, m_Origin);

	        dLatY = Result.GetLatDegrees();

	        dLongX = Result.GetLongDegrees();
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
	        m_Origin.SetLatDegrees(dLat);

	        m_Origin.SetLongDegrees(dLong);

        }

        /// <summary>
        ///
        /// </summary>
        CGeoLatLong GetOrigin() 
        {
	        return m_Origin;
        }

        private CGeoLatLong m_Origin = new CGeoLatLong();
    }
}