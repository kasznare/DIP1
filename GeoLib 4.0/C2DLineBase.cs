using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{


    /// <summary>
    /// Abstract base class for a line going from one point to another
    /// </summary>
    public abstract class C2DLineBase : C2DBase
    {
        /// <summary>
	    /// Intersection with another
        /// </summary>
        public abstract bool Crosses(C2DLineBase Other,  List<C2DPoint> IntersectionPts);
        /// <summary>
        /// Minimum distance to a point.
        /// </summary>
        public abstract double Distance(C2DPoint TestPoint,  C2DPoint ptOnThis);
        /// <summary>
	    /// Minimum distance to another.
        /// </summary>
        public abstract double Distance(C2DLineBase Other,  C2DPoint ptOnThis,  C2DPoint ptOnOther);
        /// <summary>
        /// The point from.
        /// </summary>
        public abstract C2DPoint GetPointFrom();
        /// <summary>
	    /// The point to.
        /// </summary>
        public abstract C2DPoint GetPointTo();
        /// <summary>
	    /// The length.
        /// </summary>
        public abstract double GetLength();
        /// <summary>
	    /// Reverse direction of the line.
        /// </summary>
        public abstract void ReverseDirection();
        /// <summary>
	    /// Given a set of points on the line, this function creates sub lines defined by those points.
	    /// Required by intersection, union and difference functions in the C2DPolyBase class.
        /// </summary>
        public abstract void GetSubLines(List<C2DPoint> PtsOnLine,  List<C2DLineBase> LineSet);
        /// <summary>
        /// Creats a copy of the line.
         /// </summary> 
        public abstract C2DLineBase CreateCopy();

        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
        public abstract  void Transform(CTransformation pProject);

        /// <summary>
        ///  Transform by a user defined transformation. e.g. a projection.
        /// </summary>
	    public abstract  void InverseTransform(CTransformation pProject);

    }

}
