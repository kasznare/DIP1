using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{
    /// <summary>
    /// Class to represent a grid with settings for polygon degenerate handling.
    /// </summary>
    public class CGrid
    {
        /// <summary>
	    /// Enumeration for degenerate handling methods.
        /// </summary>
	    public enum eDegenerateHandling
	    {
            /// <summary> No degenerate handling. </summary>
		    None,
            /// <summary> Randomly perturb shapes to avoid coincident points. </summary>
		    RandomPerturbation,
            /// <summary> Grid method. Calucate grid size automatically. </summary>
		    DynamicGrid,
            /// <summary> Grid method. Grid size already set. </summary>
		    PreDefinedGrid,
            /// <summary> Grid method. Shapes presnapped to the grid. </summary>
		    PreDefinedGridPreSnapped,
	    };
        /// <summary>
	    /// Constructor
        /// </summary>
	    public CGrid() {;}

        /// <summary>
	    /// Destructor
        /// </summary>
	    ~CGrid() {;}

        /// <summary>
	    /// Sets the size of the grid.
        /// </summary>
        public void SetGridSize(double dGridSize)
        {
            if (dGridSize != 0)
            {
                gridSize = Math.Abs(dGridSize);
            }
        }

        /// <summary>
	    /// Finds a recommended minimum grid size to avoid point equality problems.
        /// </summary>
        public static double GetMinGridSize(C2DRect cRect, bool bRoundToNearestDecimalFactor)
        {
            // Find the furthest possible linear distance from the origin.
            C2DPoint pt = cRect.GetPointFurthestFromOrigin();

            double dRes = Math.Abs(Math.Max(pt.x, pt.y));
            // Now multiply this by the eq tol. Now, 2 points which are this far apart from each other
            // (in x and y) and at the edge of the rect would be considered only just not equal.
            dRes *= Constants.conEqualityTolerance;
            // Now multiple this by an avoidance factor.
            dRes *= const_dEqualityAvoidanceFactor;

            if (dRes == 0)
                dRes = const_dEqualityAvoidanceFactor;

            if (bRoundToNearestDecimalFactor)
            {
                double dRound = 0.0001;

                while (dRound >= dRes)
                    dRound /= 10.0;

                while (dRound < dRes)
                    dRound *= 10.0;

                dRes = dRound;
            }
            return dRes;
        }

        /// <summary>   
        /// Sets to the minimum recommended size for degenerate handling.
        /// </summary>
        public void SetToMinGridSize(C2DRect cRect, bool bRoundToNearestDecimalFactor)
        {
	        SetGridSize( GetMinGridSize(cRect, bRoundToNearestDecimalFactor));

        }

        /// <summary>
	    /// Resets the degenerate count.
        /// </summary>
        public void ResetDegenerateErrors()
        {
            degenerateErrors = 0;
        }

        /// <summary>
	    /// Used to log a degenerate error.
        /// </summary>
        public void LogDegenerateError()
        {
            degenerateErrors++;
        }

        /// <summary>
        /// Grid size.
        /// </summary>
        private double gridSize = 0.0001;
        /// <summary>
        /// Grid size.
        /// </summary>
        public double GridSize
        {
            get
            {
                return gridSize;
            }
        }

        /// <summary>
        /// Degenerate errors.
        /// </summary>
        private int degenerateErrors = 0;
        /// <summary>
        /// Degenerate errors.
        /// </summary>
        public int DegenerateErrors
        {
            get
            {
                return degenerateErrors;
            }
        }
        /// <summary>
        /// Degenerate Handling.
        /// </summary>
        public eDegenerateHandling DegenerateHandling = eDegenerateHandling.None;

        /// <summary>
        /// Equality Avoidance Factor.
        /// </summary>
        private const double const_dEqualityAvoidanceFactor = 1000.0;
    }
}
