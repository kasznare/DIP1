/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file Orthographic.cpp
///Implementation file for a COrthographic class.

Implementation file for a COrthographic class.
---------------------------------------------------------------------------*/




using System;



namespace GeoLib
{

    /// <summary>
    /// Class representing an albers equal area projection.
    /// </summary>
    public class COrthographic : CProjection
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public COrthographic()
        {
            m_dStandardLatitude = 0;

            m_dStandardLongitude = 0;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~COrthographic()
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

            double cos_Lat = Math.Cos(dLatY);

            double x = cos_Lat * Math.Sin(dLongX - m_dStandardLongitude);

            double y = Math.Cos(m_dStandardLatitude) * Math.Sin(dLatY) -
                        Math.Sin(m_dStandardLatitude) * cos_Lat * Math.Cos(dLongX - m_dStandardLongitude);

            dLatY = y;

            dLongX = x;
        }


        /// <summary>
        /// Project the given lat long to x, y using the input parameters to store the result and retaining 
        /// the lat long in the class passed.
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
            double p = Math.Sqrt(dLongX * dLongX + dLatY * dLatY);

            double c = Math.Asin(p);

            double cos_c = Math.Cos(c);

            double sin_c = Math.Sin(c);

            double sin_olat = Math.Sin(m_dStandardLatitude);

            double cos_olat = Math.Cos(m_dStandardLatitude);

            double dLat = Math.Asin(cos_c * sin_olat + dLatY * sin_c * cos_olat / p);

            double dLong = m_dStandardLongitude +
                 Math.Atan2(dLongX * sin_c, p * cos_olat * cos_c - dLatY * sin_olat * sin_c);



            dLatY = dLat * Constants.conDegreesPerRadian;

            dLongX = dLong * Constants.conDegreesPerRadian;
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
        void SetOrigin(double dLat, double dLong)
        {
            m_dStandardLatitude = dLat * Constants.conRadiansPerDegree;

            m_dStandardLongitude = dLong * Constants.conRadiansPerDegree;

        }

        private double m_dStandardLatitude;

        private double m_dStandardLongitude;
    }
}