/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file Geodetic.cpp
/// Implementation file for the CGeoLatLong class which contains data for latitude, 
longitude and height. 
---------------------------------------------------------------------------*/

using System;


namespace GeoLib
{
    /// <summary>
    /// Class for a lat long point.
    /// </summary>
    public class CGeoLatLong 
    {
        /// <summary>
        /// Constructor initialises members to 0.
        /// </summary>
        public CGeoLatLong()
        {
	        m_dLat = 0;
	        m_dLong = 0;
        }


        /// <summary>
        /// Constructor with assignment.
        /// </summary>
        public CGeoLatLong(double dLatitudeRadians, double dLongitudeRadians)
        {
	        m_dLat = dLatitudeRadians;
	        m_dLong = dLongitudeRadians;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public CGeoLatLong(CGeoLatLong Other)
        {
	        // simple assignment
	        m_dLat = Other.GetLat();
	        m_dLong = Other.GetLong();
        }

        /// <summary>
        /// Destrcutor.
        /// </summary>
        ~CGeoLatLong()
        {
        }

        /// <summary>
        /// Checks for valid values.
        /// </summary>
        public bool IsValid()
        {
	        return (m_dLat >= -Constants.conHALFPI && m_dLat <= Constants.conHALFPI &&
		        m_dLong >= -Constants.conPI && m_dLong < Constants.conPI);
        }


        /// <summary>
        /// Gets the latitude as a string. Target must be at least 10 chars long.
        /// </summary>
        public string GetLatString()
        {
	        return GetLatString(m_dLat * Constants.conDegreesPerRadian);
        }


        /// <summary>
        /// Longitude as a string. Target must be at least 11 chars long.
        /// </summary>
        public string GetLongString()
        {
	        return GetLongString(m_dLong * Constants.conDegreesPerRadian);
        }

        /// <summary>
        /// Latitude as a string. Target must be at least 10 chars long.
        /// </summary>
        public string GetLatString(double dLatitudeDegrees)
        {
	        string dest = "";

	        int nSec = (int) (Math.Abs( dLatitudeDegrees * 3600.0) + 0.5);

	        if (nSec >= 0 && nSec <= (90 * 3600))
	        {
                long remainder;
                long dDeg = Math.DivRem(nSec, 3600, out remainder);

                long dMin = Math.DivRem(remainder, 60, out remainder);

		        if(dLatitudeDegrees >= 0.0)
                    dest = string.Format("N{1:d2}:{2:d2}:{3:d2}", dDeg, dMin, remainder);
		        else
                    dest = string.Format("S{1:d2}:{2:d2}:{3:d2}", dDeg, dMin, remainder);
	        }

	        return dest;
        }


        ///// Longitude as a string. Target must be at least 11 chars long.
        string GetLongString(double dLongitudeDegrees)
        {
	        string dest = "";

	        int nSec = (int) (Math.Abs( dLongitudeDegrees * 3600.0) + 0.5);

	        if (nSec >= 0 && nSec <= (180 * 3600))
	        {
                long remainder;
                long dDeg = Math.DivRem(nSec, 3600, out remainder);

                long dMin = Math.DivRem(remainder, 60, out remainder);

		        if(dLongitudeDegrees >= 0.0)
                    dest = string.Format("E{1:d3}:{2:d2}:{3:d2}", dDeg, dMin, remainder);
		        else
                    dest = string.Format("W{1:d3}:{2:d2}:{3:d2}", dDeg, dMin, remainder);
	        }

	        return dest;
        }


        /// <summary>
        /// Assigment.
        /// </summary>
        public CGeoLatLong Set(CGeoLatLong Other)
        {
	        // Simple assignment
	        m_dLat = Other.GetLat();
	        m_dLong = Other.GetLong();
	        // Return reference to this
	        return this;
        }


        /// <summary>
        /// Equality test which is really a proximity.
        /// </summary>
        public bool Equals(CGeoLatLong Other)
        {
	        bool bLatClose;
	        bool bLongClose;

	        if ( m_dLat == 0 )
		        bLatClose = Other.GetLat() == 0;
	        else
		        bLatClose = Math.Abs(( Other.GetLat() - m_dLat ) / m_dLat ) 
												        < Constants.conEqualityTolerance;
	        if (!bLatClose)
		        return false;		// Get out early if we can.



	        if ( m_dLong == 0 )
		        bLongClose = Other.GetLong() == 0;
	        else
                bLongClose = Math.Abs((Other.GetLong() - m_dLong) / m_dLong)
                                                        < Constants.conEqualityTolerance;
	        return (bLongClose);

        }


        /// <summary>
        /// Finds the range from one position to another based on sperical earth and
        /// great circles. Range is in metres. Error between this and WSG84 slant range is
        /// less than 1% at ranges up to 2000km. (Spherical Law of Math.Cosines).
        /// </summary>
        public double Range(CGeoLatLong Other)
        {
	        double latO = Other.GetLat();
	        double longO = Other.GetLong();

	        if (m_dLat == latO && m_dLong == longO) return 0;

            double A = Math.Sin(m_dLat) * Math.Sin(latO) +
                Math.Cos(m_dLat) * Math.Cos(latO) * Math.Cos(m_dLong - longO);

            return Math.Acos(A) * Constants.conEARTH_RADIUS_METRES;

        }

        /// <summary>
        /// Calculates the heading to another location in degrees.
        /// </summary>
        double Heading(CGeoLatLong Other)
        {

	        double latO = Other.GetLat();
	        double longO = Other.GetLong();

	        //check to see if they are on the same line of longitude
	        if(m_dLong == longO)
	        {
		        // first check to see if the locations are the same
		        if (m_dLat == latO) 
			        return 0; //may as well return 0 as any other
        	
		        if(m_dLat > latO)
			        return 180;
		        else
			        return 0;
	        }

            if ((Math.Abs(m_dLat) == -Constants.conHALFPI) | (Math.Abs(latO) == Constants.conHALFPI))
	        {
		        //check to see if the first is at the south pole or the second is at the north pole
		        if((m_dLat == -Constants.conHALFPI) | (latO == Constants.conHALFPI))
			        return 0;

		        //check to see if the first is at the north pole or the second is at the south
		        if((m_dLat == Constants.conHALFPI) | (latO ==-Constants.conHALFPI)) 
			        return 180;
	        }

	        double sin_lat = Math.Sin(m_dLat);
	        double sin_latO = Math.Sin(latO);
	        double cos_lat = Math.Cos(m_dLat);
	        double cos_latO = Math.Cos(latO);
	        double cos_alpha = sin_latO * sin_lat + cos_latO * cos_lat * Math.Cos(longO - m_dLong);
	        double alpha = Math.Acos(cos_alpha);

	        double hdng;

	        if(longO > m_dLong)
	        {
		        hdng = Math.Acos(  ( sin_latO - sin_lat * cos_alpha )/( cos_lat * Math.Sin(alpha) )  );
		        if ( (longO - m_dLong) > Constants.conPI) 
		        {
			        // gone from e.g. e179 to w179
			        hdng = Constants.conTWOPI - hdng;
		        }
	        }
	        else
	        {
		        hdng = Constants.conTWOPI - Math.Acos(  ( sin_latO - sin_lat * cos_alpha )/( cos_lat * Math.Sin(alpha) )  );
		        if ( (m_dLong - longO) > Constants.conPI) 
		        {
			        // gone from e.g. w179 to e179
			        hdng = Constants.conTWOPI - hdng;
		        }
	        }

	        if (hdng == Constants.conTWOPI) 
		        return 0;

	        return hdng;
        }


        /// <summary>
        /// Calculates the Range and heading to another location in degrees.
        /// </summary>
        public void RangeAndHeading(CGeoLatLong Other, 
					        out double dRangeMetres, out double hdng)
        {
	        double latO = Other.GetLat();
	        double longO = Other.GetLong();

	        if (m_dLat == latO && m_dLong == longO) 
	        {
		        dRangeMetres = 0;
		        hdng = 0;
		        return;
	        }

	        double sin_lat = Math.Sin(m_dLat);
	        double sin_latO = Math.Sin(latO);
	        double cos_lat = Math.Cos(m_dLat);
	        double cos_latO = Math.Cos(latO);
	        double cos_alpha = sin_latO * sin_lat + cos_latO * cos_lat * Math.Cos(longO - m_dLong);
	        double alpha = Math.Acos(cos_alpha);

	        dRangeMetres = alpha * Constants.conEARTH_RADIUS_METRES;


	        //check to see if they are on the same line of longitude
	        if(m_dLong == longO)
	        {
		        // No need to check if the locations are the same as done already.
		        if(m_dLat > latO)
		        {
			        hdng = Constants.conPI;
			        return;
		        }
		        else
		        {
			        hdng = 0;//may as well return 0 as any other
			        return ; 
		        }
	        }

	        if((Math.Abs(m_dLat) == -Constants.conHALFPI) | (Math.Abs(latO) == Constants.conHALFPI))
	        {
		        //check to see if the first is at the south pole or the second is at the north pole
		        if((m_dLat == -Constants.conHALFPI) | (latO == Constants.conHALFPI))
		        {
			        hdng = 0;
			        return;
		        }

		        //check to see if the first is at the north pole or the second is at the south
		        if((m_dLat == Constants.conHALFPI) | (latO ==-Constants.conHALFPI)) 
		        {
			        hdng = Constants.conPI;
			        return;
		        }
	        }

	        if(longO > m_dLong)
	        {
		        hdng = Math.Acos(  ( sin_latO - sin_lat * cos_alpha )/( cos_lat * Math.Sin(alpha) )  );
		        if ( (longO - m_dLong) > Constants.conPI) 
		        {
			        // gone from e.g. e179 to w179
			        hdng = Constants.conTWOPI - hdng;
		        }
	        }
	        else
	        {
		        hdng = Constants.conTWOPI - Math.Acos(  ( sin_latO - sin_lat * cos_alpha )/( cos_lat * Math.Sin(alpha) )  );
		        if ( (m_dLong - longO) > Constants.conPI) 
		        {
			        // gone from e.g. w179 to e179
			        hdng = Constants.conTWOPI - hdng;
		        }
	        }

	        if (hdng == Constants.conTWOPI)
					        hdng = 0;
        }



        /// <summary>
        /// Set using a heading and a range from an origin. Range in metres. 
        /// </summary>
        public void Set(double dHeading, double dRange, CGeoLatLong Origin)
        {
	        // alpha is the angular Distance travelled round the earths surface
	        double alpha = dRange /Constants.conEARTH_RADIUS_METRES;
	        double sin_alpha = Math.Sin(alpha);
	        double cos_alpha = Math.Cos(alpha);

	        double latO = Origin.GetLat();
	        double longO = Origin.GetLong();

	        double sin_Olat = Math.Sin(latO);
	        double cos_Olat = Math.Cos(latO);

	        m_dLat = Math.Asin(  sin_Olat * cos_alpha + cos_Olat * sin_alpha * Math.Cos(dHeading)  );

            m_dLong = longO + Math.Atan2(Math.Sin(dHeading) * sin_alpha * cos_Olat, cos_alpha - sin_Olat * Math.Sin(m_dLat));

	        while (m_dLong > Constants.conPI) 
		        m_dLong -= Constants.conTWOPI;
	        while (m_dLong < -Constants.conPI) 
		        m_dLong += Constants.conTWOPI;

        }





        /// <summary>
        /// Rotation.
        /// </summary>
        public void Rotate(CGeoLatLong Other)
        {
	        m_dLat += Other.GetLat();
	        m_dLong += Other.GetLong();
        	
	        if (m_dLat < -Constants.conHALFPI)
		        m_dLat = Constants.conPI + m_dLat;
        		
	        if (m_dLat > Constants.conHALFPI)
		        m_dLat = -Constants.conPI + m_dLat;

	        if (m_dLong < -Constants.conPI)
		        m_dLong = Constants.conTWOPI + m_dLong;

	        if (m_dLong > Constants.conPI)
		        m_dLong = -Constants.conTWOPI + m_dLong;

        }


        /// <summary>
        /// Conversion to geocentric.
        /// </summary>
        public void GeocentricWSG84(C3DPoint xyzPoint)
        {
	        if(!IsValid())
		        return;

	        double Sin_Lat = Math.Sin(m_dLat);  //  Math.Sin(Latitude)
	        double Cos_Lat = Math.Cos(m_dLat);  //  Math.Cos(Latitude)
	        double Sin2_Lat = Sin_Lat * Sin_Lat; // Square of Math.Sin(Latitude)
            double Rn = Constants.conGeocent_Major / (Math.Sqrt(1.0e0 - Constants.conGeocent_e2 * Sin2_Lat)); //  Earth radius at location

	        xyzPoint.x = Rn * Cos_Lat * Math.Cos(m_dLong);

	        xyzPoint.y = Rn * Cos_Lat * Math.Sin(m_dLong);

            xyzPoint.z = (Rn * (1 - Constants.conGeocent_e2)) * Sin_Lat;

	        return;
        }


        /// <summary>
        /// Conversion to geocentric.
        /// </summary>
        public void Geocentric(C3DPoint xyzPoint)
        {
	        if(!IsValid())
		        return;

	        double Sin_Lat = Math.Sin(m_dLat);  //  Math.Sin(Latitude)
	        double Cos_Lat = Math.Cos(m_dLat);  //  Math.Cos(Latitude)

            xyzPoint.x = (Constants.conEARTH_RADIUS_METRES) * Cos_Lat * Math.Cos(m_dLong);

            xyzPoint.y = (Constants.conEARTH_RADIUS_METRES) * Cos_Lat * Math.Sin(m_dLong);

            xyzPoint.z = (Constants.conEARTH_RADIUS_METRES) * Sin_Lat;

	        return;
        }

        /// <summary>
        /// Slant Range.
        /// </summary>
        public double SlantRangeWSG84(CGeoLatLong Other)
        {
	        C3DPoint ptThis = new C3DPoint();

	        GeocentricWSG84(ptThis);

	        C3DPoint ptOther = new C3DPoint();

	        Other.GeocentricWSG84(ptOther);

	        return ptThis.Distance(ptOther);

        }


        /// <summary>
        /// Slant Range.
        /// </summary>
        public double SlantRange(CGeoLatLong Other)
        {
	        C3DPoint ptThis = new C3DPoint();

	        Geocentric(ptThis);

	        C3DPoint ptOther = new C3DPoint();

	        Other.Geocentric(ptOther);

	        return ptThis.Distance(ptOther);
        }

        /// <summary>
        /// Converts Range to Slant Range.
        /// </summary>
        public double RangeToSlantRange(double dRange)
        {
            return 2 * Constants.conEARTH_RADIUS_METRES * Math.Sin(dRange / (2 * Constants.conEARTH_RADIUS_METRES));

        }
        /// <summary>
        /// Converts Slant Range to Range.
        /// </summary>
        public double SlantRangeToRange(double dSlantRange)
        {
            return 2 * Constants.conEARTH_RADIUS_METRES * Math.Asin(dSlantRange / (2 * Constants.conEARTH_RADIUS_METRES));
        }

        /// <summary>
        /// Converts latitude to degrees.
        /// </summary>
        public double LatitudeToDegrees(string pText)
        {
	        double dDegrees = 0.0;

	        int nDegrees = 0;
            int nMinutes = 0;
            int nSeconds = 0;

            char[] separators = new char[] {':'};

            string text = pText;

            text.Insert(1, ":");

            string[] parts = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 4 && int.TryParse(parts[1], out nDegrees)
                && int.TryParse(parts[2], out nMinutes) && int.TryParse(parts[3], out nSeconds))
            {

	            if( nDegrees <= 90 && nMinutes < 60 && nSeconds < 60 )
                {
		            dDegrees = nDegrees * 3600 + nMinutes * 60 + nSeconds;
                }

	            dDegrees /= 3600;

	            if(parts[0].Equals("S") || parts[0].Equals("s"))
		            dDegrees = -dDegrees;


            }
            return dDegrees;
        }


        /// <summary>
        /// Converts latitude to degrees.
        /// </summary>
        public double LongitudeToDegrees(string pText)
        {
	        double dDegrees = 0.0;

	        int nDegrees = 0;
            int nMinutes = 0;
            int nSeconds = 0;

            char[] separators = new char[] {':'};

            string text = pText;

            text.Insert(1, ":");

            string[] parts = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 4 && int.TryParse(parts[1], out nDegrees)
                && int.TryParse(parts[2], out nMinutes) && int.TryParse(parts[3], out nSeconds))
            {
                if (nDegrees <= 180 && nMinutes < 60 && nSeconds < 60)
                {
                    dDegrees = nDegrees * 3600 + nMinutes * 60 + nSeconds;
                }

	            dDegrees /= 3600;

	            if(parts[0].Equals("W") || parts[0].Equals("w"))
		            dDegrees = -dDegrees;
            }

	        return dDegrees;
        }






        /// <summary>
        /// Validate Latitude.
        /// </summary>
        public bool ValidateLatitude(string strText)
        {
            if (strText.Length == 0)
		        return false;

            if (strText.Length < 9 || strText.Length > 11)
                return false;

	        if(strText[0] != 'N' && strText[0] != 'S')
		        return false;

	        // First numeric must be less than or equal to 9
	        if(strText[1] < '0' || strText[1] > '9')
		        return false;

	        // Second numeric must be less than or equal to 9
	        if(strText[2] < '0' || strText[2] > '9')
		        return false;

	        // If first numeric is 9 then don't allow second digit to be > 0
	        if(((int)strText[1] - 48 == 9) && (strText[2] > '0'))
		        return false;


	        // Third char must be a colon
	        if(strText[3] != ':')
		        return false;

	        // Fourth numeric must be less than or equal to 5
	        if(strText[4] < '0' || strText[4] > '5')
		        return false;

	        else
	        {
		        // If first numeric is 9 then don't allow fourth digit to be > 0
		        if(((int)strText[1] - 48 == 9) && (strText[4] > '0'))
			        return false;
	        }

	        // Fifth numeric must be less than or equal to 9
	        if(strText[5] < '0' || strText[5] > '9')
		        return false;

	        else
	        {
		        // If first numeric is 9 then don't allow fifth digit to be > 0
		        if(((int)strText[1] - 48 == 9) && (strText[5] > '0'))
			        return false;
	        }

	        // Third char must be a colon or decimal point
	        if(strText[6] != ':')
		        return false;



	        // If we're using seconds seventh numeric must be less than or equal to 5

	        if(strText[7] < '0' || strText[7] > '5')
		        return false;

	        // If first numeric is 9 then don't allow seventh digit to be > 0
	        if(((int)strText[1] - 48 == 9) && (strText[7] > '0'))
		        return false;



	        // Eighth numeric must be less than or equal to 9
	        if(strText[8] < '0' || strText[8] > '9')
		        return false;

	        // If first numeric is 9 then don't allow eighth digit to be > 0
	        if(((int)strText[1] - 48 == 9) && (strText[8] > '0'))
		        return false;

            if (strText.Length > 9)
            {
		        if(strText[9] != '.')
			        return false;

		        if(strText[10] < '0' || strText[10] > '9')
			        return false;
            }

	        return true;
        }




        /// <summary>
        /// Validation.
        /// </summary>
        public bool ValidateLongitude(string strText)
        {
            if (strText.Length < 10 || strText.Length > 12)
                return false;

	        if(strText[0] != 'E' && strText[0] != 'W')
		        return false;

	        // First numeric must be less than 2
	        if(strText[1] < '0' || strText[1] > '1')
		        return false;

	        // Second numeric must be less than or equal to 9
	        if(strText[2] < '0' || strText[2] > '9')
		        return false;


	        // If previous value is 1 then don't alow value > 7
	        if(((int)strText[1] - 48 == 1) && (strText[2] > '7'))
		        return false;


	        // Third numeric must be less than or equal to 9
	        if(strText[3] < '0' || strText[3] > '9')
		        return false;

	        // Fourth char must be a colon
	        if(strText[4] != ':')
		        return false;

	        // Fifth numeric must be less than or equal to 5
	        if(strText[5] < '0' || strText[5] > '5')
		        return false;

	        // Sixth numeric must be less than or equal to 9
	        if(strText[6] < '0' || strText[6] > '9')
		        return false;

	        // Seventh char must be a colon or decimal point
	        if(strText[7] != ':')
		        return false;


	        // If we're using seconds eighth numeric must be less than or equal to 5
	        if(strText[8] < '0' || strText[8] > '5')
		        return false;


	        // Eighth numeric must be less than or equal to 9
	        if(strText[9] < '0' || strText[9] > '9')
		        return false;

	        return true;
        }


        /// <summary>
        /// Sets the latitude in degrees.
        /// </summary>
        public bool SetLatString(string strText)
        {
	        if (ValidateLatitude(strText))
	        {
		        double dLat = LatitudeToDegrees(strText);
		        SetLatDegrees(dLat);
		        return true;
	        }

	        return false;
        }


        /// <summary>
        /// Sets the longitude in degrees.
        /// </summary>
        public bool SetLongString(string strText)
        {
	        if (ValidateLongitude(strText))
	        {
		        double dLong = LongitudeToDegrees(strText);
		        SetLongDegrees(dLong);
		        return true;
	        }

	        return false;

        }

        /// <summary>
        /// Gets the latitude in degrees.
        /// </summary>
	    public double GetLatDegrees() 
		{
            return m_dLat * Constants.conDegreesPerRadian;
        }

        /// <summary>
	    /// Gets the longitude in degrees.
        /// </summary>
	    public double GetLongDegrees() 
        {
            return m_dLong * Constants.conDegreesPerRadian;
        }

        /// <summary>
	    /// Sets the latitude in radians.
        /// </summary>
        public void SetLat(double dLatRadians)
        {
            m_dLat = dLatRadians;
        }

        /// <summary>
	    /// Sets the longitude in radians.
        /// </summary>
	    public void SetLong(double dLongRadians)
		{
            m_dLong = dLongRadians;
        }

        /// <summary>
    	/// Sets the latitude in degrees.
        /// </summary>
	    public void SetLatDegrees(double dLatDegrees) 
		{
            m_dLat = dLatDegrees * Constants.conRadiansPerDegree;
        }

        /// <summary>
	    /// Sets the longitude in degrees.
        /// </summary>
	    public void SetLongDegrees(double dLongDegrees)
		{
            m_dLong = dLongDegrees * Constants.conRadiansPerDegree;
        }

        /// <summary>
        /// Gets the latitude.
        /// </summary>
        public double GetLat() 
        {
            return m_dLat;
        }
        /// <summary>
	    /// Gets the longitude in radians.
        /// </summary>
	    public double GetLong() 
        {
            return m_dLong;
        }


        /// Latitude
        double m_dLat;
        /// Longitude
        double m_dLong;


    }
}






































