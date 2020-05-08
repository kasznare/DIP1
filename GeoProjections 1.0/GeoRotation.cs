



using System;


namespace GeoLib
{

    /// <summary>
    /// Class representing a circle.
    /// </summary>
    public class CGeoRotation : CTransformation
    {
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public CGeoRotation()
        {
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~CGeoRotation()
        {
        }

        /// <summary>
        /// Transform the given point.
        /// </summary>
        public override void Transform(double dx, double dy) 
        {
	        Rotate(dy, dx);
        }

        /// <summary>
        /// Inverse transform the given point.
        /// </summary>
        public override void InverseTransform(double dx, double dy) 
        {
	        InverseRotate(dy, dx);
        }



        /// <summary>
        /// Set origin.
        /// </summary>
        void SetOrigin(double dLatDegrees, double dLongDegrees)
        {
	        m_Origin.SetLatDegrees(dLatDegrees);
	        m_Origin.SetLongDegrees(dLongDegrees);
        }

        /// <summary>
        /// Inverse rotate.
        /// </summary>
        void Rotate(double dLatDegrees, double dLongDegrees) 
        {
	        CGeoLatLong rLatLong = new CGeoLatLong();
	        rLatLong.SetLatDegrees(dLatDegrees);
	        rLatLong.SetLongDegrees(dLongDegrees);
        	
	        Rotate(rLatLong);

	        dLatDegrees = rLatLong.GetLatDegrees();
	        dLongDegrees = rLatLong.GetLongDegrees();

        }
        
        /// <summary>
        /// Inverse rotate.
        /// </summary>
        void Rotate(CGeoLatLong rLatLong) 
        {
	        double dRange;
	        double dHeading;
	        m_Origin.RangeAndHeading(rLatLong, out dRange, out dHeading);

	        rLatLong.Set(dHeading, dRange, new CGeoLatLong());

        }
        
        /// <summary>
        /// Inverse rotate.
        /// </summary>
        void InverseRotate(double dLatDegrees, double dLongDegrees) 
        {
	        CGeoLatLong rLatLong = new CGeoLatLong();
	        rLatLong.SetLatDegrees(dLatDegrees);
	        rLatLong.SetLongDegrees(dLongDegrees);
        	
	        InverseRotate(rLatLong);

	        dLatDegrees = rLatLong.GetLatDegrees();
	        dLongDegrees = rLatLong.GetLongDegrees();
        }

        /// <summary>
        /// Inverse rotate.
        /// </summary>
        void InverseRotate(CGeoLatLong rLatLong) 
        {
	        double dRange;
	        double dHeading;
	        new CGeoLatLong().RangeAndHeading(rLatLong, out dRange, out dHeading);

	        rLatLong.Set(dHeading, dRange, m_Origin);

        }

        private CGeoLatLong m_Origin = new CGeoLatLong();
    }
}