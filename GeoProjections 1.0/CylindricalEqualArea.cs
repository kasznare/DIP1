/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file CylindricalEqualArea.cpp
///Implementation file for a CCylindricalEqualArea class.

Implementation file for a CCylindricalEqualArea class.
---------------------------------------------------------------------------*/


using System;


namespace GeoLib
{

    /// <summary>
    /// Class representing a circle.
    /// </summary>
    public class CCylindricalEqualArea : CProjection
    {

        /// <summary>
        /// constructor.
        /// </summary>
        public CCylindricalEqualArea()
        {
            m_dStandardLatitude = 0;

            m_dStandardLongitude = 0;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CCylindricalEqualArea()
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

            double cos_SL = Math.Cos(m_dStandardLatitude);

            dLongX = (dLongX - m_dStandardLongitude) * cos_SL;

            dLatY = Math.Sin(dLatY) / cos_SL;
        }


        /// <summary>
        ///Project the given lat long to x, y using the input parameters to store the result and retaining 
        /// the lat long in the class passed.
        /// </summary>
        public override void Project(CGeoLatLong rLatLong, double dx, double dy)
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
            double cos_SL = Math.Cos(m_dStandardLatitude);

            dLatY = Math.Asin(dLatY * cos_SL);

            dLongX = dLongX / cos_SL + m_dStandardLongitude;

            dLatY *= Constants.conDegreesPerRadian;

            dLongX *= Constants.conDegreesPerRadian;
        }


        /// <summary>
        /// Project the given x y to lat long using the input lat long class to get the result.
        /// </summary>
        public override void InverseProject(CGeoLatLong rLatLong, double dX, double dY)
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
