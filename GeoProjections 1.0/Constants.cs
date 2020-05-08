/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file Constants.h
/// File containing useful constants and conversion factors.
---------------------------------------------------------------------------*/

namespace GeoLib
{
    /// <summary>
    /// Class to hold constants
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conRadiansPerDegree = 0.017453292519943295769236907684886;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conDegreesPerRadian = 57.295779513082320876798154814105;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conMetresPerNauticalMile = 1852.0;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conEARTH_RADIUS_METRES = 6370999.0; // Volumetric mean
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conSecondsPerDegree = 3600.0;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conMinutesPerDegree = 60.0;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conMetersPerFoot = 0.3048;
        /// <summary>
        /// This defined how close 2 doubles need to be to each other in order to be considered
        /// Equal. If the difference between the 2 divided by 1 of them is less than this they are
        /// equal.
        /// </summary>
        public const double conEqualityTolerance = 0.0000000001;
        /// <summary>
        /// Random number perturbation seed.
        /// </summary>
        public const double coniPerturbationFactor = 0.0568412;
        /// <summary>
        /// Random number perturbation seed.
        /// </summary>
        public const double conjPerturbationFactor = 0.0345687;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conPI = 3.14159265358979323846;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conTWOPI = 6.283185307179586476925286766559;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conSIXTHPI = Constants.conPI / 6.0;		// 30 degrees
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conQUARTPI = Constants.conPI / 4.0;		// 30 degrees
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conTHIRDPI = Constants.conPI / 3.0;		// 60 degrees
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conTWOTHIRDPI = conTWOPI / 3.0;	// 120 degrees
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conHALFPI = conPI / 2.0;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conMetresPerDataMile = 1828.80;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conDataMilesPerNauticalMile = 1.0111666666666666666666666666667;
        /// <summary>
        /// Earth semi-major axis of ellipsoid in meters
        /// </summary>
        public const double conGeocent_Major = 6378137.0;
        /// <summary>
        /// Earth semi-minor axis of ellipsoid in meters
        /// </summary>
        public const double conGeocent_Minor = 6356752.3142;
        /// <summary>
        /// Earth axis mean in meters
        /// </summary>
        public  double conGeocent_Mean = 6367444.6571;
        /// <summary>
        /// Earth axis eccentricity squared
        /// </summary>
        public const double conGeocent_e2 = 0.0066943799901413800;
        /// <summary>
        /// Earth axis 2nd eccentricity squared
        /// </summary>
        public const double conGeocent_ep2 = 0.00673949675658690300;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conLbsPerKilogram = 2.2;
        /// <summary>
        /// Constant.
        /// </summary>
        public const long conSecondsPerDay = 86400;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conSecondsPerHour = 3600.0;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conSecondsPerMinute = 60.0;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conE = 2.71828182845904523536;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conRoot2 = 1.4142135623731;
        /// <summary>
        /// Constant.
        /// </summary>
        public const double conRoot3 = 1.73205080756888;

    }
}