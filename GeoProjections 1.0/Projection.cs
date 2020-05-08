/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/


/*---------------------------------------------------------------------------
\file CProjection.cpp
/// Implementation file for a CProjection class.

Implementation file for a CProjection class.
---------------------------------------------------------------------------*/









namespace GeoLib
{
    /// <summary>
    /// Abstract class for a projection.
    /// </summary>
    public abstract class CProjection : CTransformation
    {


        /// <summary>
        /// Constructor.
        /// </summary>
        public CProjection()
        {
        }

        	/// Project the given lat long to x, y using the input parameters to store the result.
	    public abstract void Project(double dLatY, double dLongX);

	    /// Project the given lat long to x, y using the input parameters to store the result and retaining 
	    /// the lat long in the class passed.
	    public abstract void Project(CGeoLatLong rLatLong, double dx, double dy);

	    /// Project the given x y to lat long using the input parameters to store the result.	
	    public abstract void InverseProject(double dLatY, double dLongX);
	    /// Project the given x y to lat long using the input lat long class to get the result.
	    public abstract void InverseProject(CGeoLatLong rLatLong, double dX, double dY);

        /// <summary>
        /// Transform override function.
        /// </summary>
        public override void Transform(double dx, double dy)
        {
	        Project(dy, dx);
        }
        /// <summary>
        /// Inverse transform the given point.
        /// </summary>
        public override void InverseTransform(double dx, double dy)
        {
	        InverseProject(dy, dx);
        }



        /// <summary>
        /// GetProjectionName.
        /// </summary>
        string GetProjectionName(int nIndex)
        {
            if (nIndex < const_strMercator.Length)
	        {
		        return const_strMercator[nIndex];
	        }
	        return "";
        }


        /// <summary>
        /// GetProjectionCount.
        /// </summary>
        int GetProjectionCount()
        {
            return const_strMercator.Length;
        }


        /// <summary>
        /// CreateProjection.
        /// </summary>
        CProjection CreateProjection(string strName)
        {
	        if (strName.Equals("Albers Equal Area Conic"))
	        {
		        return new CAlbersEqualAreaConic();
	        }
            
	        if (strName.Equals("Bonne"))
	        {
		        return new CBonneProjection();
	        }
        	
	        if (strName.Equals("Cassini"))
	        {
		        return new CCassini();
	        }

	        if (strName.Equals("Conic Equidistant"))
	        {
		        return new CConicEquidistant();
	        }
	        if (strName.Equals("Cylindrical Equal Area"))
	        {
		        return new CCylindricalEqualArea();
	        }
	        if (strName.Equals("Cylindrical Equidistant"))
	        {
		        return new CCylindricalEquidistant();
	        }
	        if (strName.Equals("Eckert VI"))
	        {
		        return new CEckertVI();
	        }
	        if (strName.Equals("Eckert IV"))
	        {
		        return new CEckertIV();
	        }
	        if (strName.Equals("Gnomonic"))
	        {
		        return new CGnomonic();
	        }
	        if (strName.Equals("HorizontalRangeHeading"))
	        {
		        return new CHorizontalRangeHeading();
	        }

	        if (strName.Equals("Lambert Azimuthal Equal Area"))
	        {
		        return new CLambertAzimuthalEqualArea();
	        }
	        if (strName.Equals("Lambert Conformal Conic"))
	        {
		        return new CLambertConformalConic();
	        }
	        if (strName.Equals("Mercator"))
	        {
		        return new CMercator();
	        }
	        if (strName.Equals("Miller Cylindrical"))
	        {
		        return new CMillerCylindrical();
	        }
	        if (strName.Equals("Mollweide"))
	        {
		        return new CMollweide();
	        }
	        if (strName.Equals("Orthographic"))
	        {
		        return new COrthographic();
	        }
	        if (strName.Equals("Polyconic"))
	        {
		        return new CPolyconic();
	        }
	        if (strName.Equals("RangeHeading"))
	        {
		        return new CRangeHeading();
	        }
	        if (strName.Equals("Sinusoidal"))
	        {
		        return new CSinusoidal();
	        }
	        if (strName.Equals("SlantRangeHeading"))
	        {
		        return new CSlantRangeHeading();
	        }
	        if (strName.Equals("Stereographic"))
	        {
		        return new CStereographic();
	        }
	        if (strName.Equals("Van Der Grinten"))
	        {
		        return new CVanDerGrinten();
	        }
	        if (strName.Equals("Vertical Perspective"))
	        {
		        return new CVerticalPerspective();
	        }
	        if (strName.Equals("Vertical"))
	        {
		        return new CVertical();
	        }
        	
        	
	        return null;
        }

        static string[] const_strMercator =
        {
	       "Albers Equal Area Conic",
	        "Bonne",
	        "Cassini",
	        "Conic Equidistant",
	        "Cylindrical Equal Area",
	        "Cylindrical Equidistant",
	        "Eckert VI",
	        "Eckert IV",
	        "Gnomonic",
	        "HorizontalRangeHeading",
	        "Lambert Azimuthal Equal Area",
	        "Lambert Conformal Conic",
	        "Mercator",
	        "Miller Cylindrical",
	        "Mollweide",
	        "Orthographic",
	        "Polyconic",
	        "RangeHeading",
	        "Sinusoidal",
	        "SlantRangeHeading",
	        "Stereographic",
	        "Van Der Grinten",
	        "Vertical Perspective",
	        "Vertical",
        };
    }
}