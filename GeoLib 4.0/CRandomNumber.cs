using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{
    /// <summary>
    /// Class to random number.
    /// </summary>
    public class CRandomNumber
    {
        /// <summary>
	    /// Constructor
        /// </summary>
	    public CRandomNumber() {}

        /// <summary>
	    /// Constructor, initialises to 2 double forming the bounds
        /// </summary>
	    public CRandomNumber(double dMin, double dMax)
        {
            Min = dMin;
            Max = dMax;
        }

        /// <summary>
	    /// Destructor.
        /// </summary>
        ~CRandomNumber() { }

        /// <summary>
	    /// Sets the random number bound to 2 doubles.
        /// </summary>
	    public void Set(double dMin, double dMax)
        {
            Min = dMin;
            Max = dMax;
        }

        /// <summary>
	    /// Sets the max.
        /// </summary>
	    public void SetMax(double dMax) 
        {
            Max = dMax;
        }

        /// <summary>
	    /// Sets the min.
        /// </summary>
	    public void SetMin(double dMin) 
        {
            Min = dMin;
        }

        /// <summary>
	    /// Access to the min.
        /// </summary>
	    public double GetMin() 
        {
            return Min;
        }

        /// <summary>
	    /// Access to the max.
        /// </summary>
	    public double GetMax()  
        {
            return Max;
        }

        /// <summary>
	    /// Gets a random number based on the settings.
        /// </summary>
	    public double Get() 
        {
            double dResult = Min + (Max - Min) * _Random.NextDouble();

	        return dResult;
        }

        /// <summary>
        /// Gets an integer based on the settings. Sets up temporary new boundaries so that an interval
        /// of e.g. 0.8 to 3.7 will become 1.0 to 3.0 allowing integers 1 and 2 only.
        /// </summary>
	    public int GetInt() 
        {
            CRandomNumber Num = new CRandomNumber(Math.Ceiling(Min), Math.Floor(Max) + 1.0);
	        double dRes = Num.Get();
	        if (dRes == (int)Num.GetMax())
		        return (int) (dRes - 1);
	        else 
		        return (int) dRes;
        }

        /// <summary>
	    /// Returns true or false.
        /// </summary>
	    public bool GetBool()
        {
            return (_Random.NextDouble() > 0.5);
        }
        /// <summary>
	    /// The minimum possible value.
        /// </summary>
        public double Min;

        /// <summary>
	    /// The maximum possible value.
        /// </summary>
        public double Max;

        private static Random _Random = new Random();


    }
}
