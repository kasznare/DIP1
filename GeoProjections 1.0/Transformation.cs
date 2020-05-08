/*---------------------------------------------------------------------------
Copyright (C) GeoLib.
This code is used under license from GeoLib (www.geolib.co.uk). This or
any modified versions of this cannot be resold to any other party.
---------------------------------------------------------------------------*/



/*---------------------------------------------------------------------------
\file Transformation.h
Header file for a CTransformation class.

\class CTransformation
Abstract class representing a transformation of some sort.
The purpose of this class is to allow transformations to be defined outside the 
GeoLib main geometry library which can still be performed on GeoLib
shapes. For example a polygon will be capable of taking an object derived
from this and calling transform on all its points.
---------------------------------------------------------------------------*/


namespace GeoLib
{
    /// <summary>
    /// Transformation abstract class.
    /// </summary>
    public abstract class CTransformation 
    {
        /// <summary>
        /// Transform the given point.
        /// </summary>
        public abstract void Transform(double dx, double dy);
        /// <summary>
        /// Inverse transform the given point.
        /// </summary>
        public abstract void InverseTransform(double dx, double dy);

    };
}