using System;
using System.Collections.Generic;
using System.Text;


namespace GeoLib
{
    /// <summary>
    /// Abstract class to represent a geometric entity of some type with basic generic functions.
    /// </summary>
    public abstract class C2DBase
    {
        /// <summary>
        /// Moves the entity by the vector provided.
        /// </summary>
	    public abstract  void Move(C2DVector Vector);
        /// <summary>
        /// Rotates this to the right about the origin provided.
        /// </summary>
        /// <param name="dAng">The angle through which to rotate.</param>
        /// <param name="Origin">The origin about which to rotate.</param>
        public abstract  void RotateToRight(double dAng, C2DPoint Origin);
        /// <summary>
	    /// Grows the entity
        /// </summary>
        public abstract  void Grow(double dFactor, C2DPoint Origin);
        /// <summary>
        /// Reflection in a point
        /// </summary>
        public abstract  void Reflect(C2DPoint Point);
        /// <summary>
        /// Reflection in a line
        /// </summary>
        public abstract  void Reflect( C2DLine Line);
        /// <summary>
        /// Distance to a point
        /// </summary>
        public abstract  double Distance( C2DPoint Point) ;
        /// <summary>
        /// Return the bounding rect
        /// </summary>
        public abstract  void GetBoundingRect( C2DRect Rect) ;
        /// <summary>
        /// Projects this onto the line provided with the interval on the line returned.
        /// </summary>
        public abstract  void Project( C2DLine Line,  CInterval Interval);
        /// <summary>
        /// Projects this onto the vector provided with the interval on the line returned.
        /// </summary>
        public abstract  void Project(C2DVector Vector,  CInterval Interval);
        /// <summary>
        /// Snaps this to the conceptual grid.
        /// </summary>
        public abstract void SnapToGrid(CGrid grid);
    }
}
