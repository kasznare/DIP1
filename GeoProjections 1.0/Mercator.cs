/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file CMercator.cpp
///Implementation file for a CMercator class.

Implementation file for a CMercator class.
---------------------------------------------------------------------------*/


using System;


namespace GeoLib
{

    /// <summary>
    /// Class representing a circle.
    /// </summary>
    public class CMercator : CProjection
    {


        /// <summary>
        ///Constructor.
        /// </summary>
        public CMercator()
        {
            m_dStandardLongitude = 0;
        }


        /// <summary>
        /// Destructor.
        /// </summary>
        ~CMercator()
        {
        }

        /// <summary>
        ///Constructor.
        /// </summary>
        public override void Project(double dLatY, double dLongX)
        {
            dLatY *= Constants.conRadiansPerDegree;

            dLongX *= Constants.conRadiansPerDegree;

            double x = dLongX - m_dStandardLongitude;

            double y = Math.Log( Math.Tan(dLatY) + 1 / Math.Cos(dLatY));

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
        /// Project the given x y to lat long using the input parameters to store 
        /// the result.	
        /// </summary>
        public override void InverseProject(double dLatY, double dLongX)
        {
            double dLat = Math.Atan(Math.Sinh(dLatY));

            double dLong = dLongX + m_dStandardLongitude;

            dLatY = dLat * Constants.conDegreesPerRadian;

            dLongX = dLong * Constants.conDegreesPerRadian;
        }


        /// <summary>
        /// Project the given x y to lat long using the input lat long class to get the result.
        /// </summary>
        public override void InverseProject(CGeoLatLong rLatLong, double dX, double dY)
        {
            double dLat = Math.Atan(Math.Sinh(dX));

            double dLong = dY + m_dStandardLongitude;

            rLatLong.SetLat(dLat);

            rLatLong.SetLong(dLong);
        }

        /// <summary>
        /// </summary>
        public void SetStandardLongitude(double dStandardLongitude)
        {
            m_dStandardLongitude = dStandardLongitude * Constants.conRadiansPerDegree;
        }

        private double m_dStandardLongitude;
    }
}