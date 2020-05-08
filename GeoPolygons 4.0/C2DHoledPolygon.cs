using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace GeoLib
{

    /// <summary>
    /// Class to represent a 2D polygon with holes.
    /// </summary>
    public class C2DHoledPolygon : C2DHoledPolyBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DHoledPolygon() 
        {
            _Rim = new C2DPolygon();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DHoledPolygon(C2DHoledPolyBase Other)
        {
            _Rim = new C2DPolygon(Other.Rim);
            for (int i = 0; i < Other.HoleCount; i++)
            {
                _Holes.Add(new C2DPolygon(Other.GetHole(i) ));
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DHoledPolygon(C2DHoledPolygon Other)
        {
            _Rim = new C2DPolygon(Other.Rim);
            for (int i = 0; i < Other.HoleCount; i++)
            {
                _Holes.Add(new C2DPolygon(Other.GetHole(i)));
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public C2DHoledPolygon(C2DPolyBase Other)
        {
            _Rim = new C2DPolygon(Other);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public C2DHoledPolygon(C2DPolygon Other)
        {
            _Rim = new C2DPolygon(Other);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~C2DHoledPolygon() { }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public new void Set(C2DHoledPolyBase Other)
        {
            _Holes.Clear();
            _Rim = new C2DPolygon(Other.Rim);
            for (int i = 0; i < Other.HoleCount; i++)
            {
                _Holes.Add(new C2DPolygon(Other.GetHole(i)));
            }
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public void Set(C2DHoledPolygon Other)
        {
            _Holes.Clear();
            _Rim = new C2DPolygon(Other.Rim);
            for (int i = 0; i < Other.HoleCount; i++)
            {
                _Holes.Add(new C2DPolygon(Other.GetHole(i)));
            }
        }

        /// <summary>
        /// Rotates to the right by the angle around the centroid
        /// </summary>
        /// <param name="dAng">Angle in radians to rotate by.</param> 
        public void RotateToRight(double dAng)
        {
            RotateToRight(dAng, GetCentroid());
        }

        /// <summary>
        /// Grows around the centroid.
        /// </summary>
        /// <param name="dFactor">Factor to grow by.</param> 
        public void Grow(double dFactor)
        {
            Grow(dFactor, GetCentroid());
        }


        /// <summary>
        /// Calculates the centroid of the polygon by moving it according to each holes
        /// weighted centroid.
        /// </summary>
        public C2DPoint GetCentroid()
        {
                 
	        C2DPoint HoleCen = new C2DPoint(0, 0);

            if (_Holes.Count == 0)
                return Rim.GetCentroid();


            C2DPoint PolyCen = Rim.GetCentroid();

            double dPolyArea = Rim.GetArea();
	        double dHoleArea = 0;

	        for ( int i = 0 ; i < _Holes.Count; i++)
	        {
		        dHoleArea += GetHole(i).GetArea();
	        }


	        if (dHoleArea == 0 || dHoleArea == dPolyArea)
		        return Rim.GetCentroid();
	        else
	        {
		        for (int i = 0 ; i < _Holes.Count; i++)
		        {
                    C2DPoint pt = GetHole(i).GetCentroid();
                    pt.Multiply(GetHole(i).GetArea() / dHoleArea);
			        HoleCen += pt;
		        }
	        }

	        C2DVector Vec = new C2DVector(HoleCen, PolyCen);

	        Vec.Multiply( dHoleArea / (dPolyArea - dHoleArea));

	        PolyCen.Move(Vec);

	        return PolyCen;
        }


        /// <summary>
        /// Calculates the area.
        /// </summary>
        public double GetArea() 
        {
	        double dResult = 0;

            dResult += Rim.GetArea();

	        for (int i = 0 ; i < _Holes.Count; i++)
	        {
                dResult -= GetHole(i).GetArea();
	        }
	        return dResult;
        }

        /// <summary>
        /// Buffers the polygon by buffering all shapes to expand the shape.
        /// No attempt to handle resulting crossing lines as designed for 
        /// very small buffers.
        /// </summary>
        /// <param name="dBuffer">The buffer amount</param>
        public void SimpleBuffer(double dBuffer)
        {
            Rim.SimpleBuffer(dBuffer);

            for (int i = 0; i < _Holes.Count; i++)
            {
                GetHole(i).SimpleBuffer(-dBuffer);
            }
        }

        /// <summary>
        /// Removes null areas within the shape according to the tolerance.
        /// </summary>
        /// <param name="dTolerance"></param>
        /// <returns>True if the shape is no longer valid.</returns>
        public bool RemoveNullAreas(double dTolerance)
        {
            if (Rim is C2DPolygon)
            {
                if ((Rim as C2DPolygon).RemoveNullAreas(dTolerance))
                {
                    return true;
                }
            }

            int i = 0;
            while ( i < _Holes.Count)
            {
                if (GetHole(i).RemoveNullAreas(dTolerance))
                {
                    _Holes.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            return false;
        }



        /// <summary>
        /// Gets the non overlaps i.e. the parts of this that aren't in the other.
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetNonOverlaps(C2DHoledPolygon Other, List<C2DHoledPolygon> Polygons,
                                            CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetNonOverlaps(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolygon(NewPolys[i]));
        }

        /// <summary>
        /// Gets the union of the 2 shapes.
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetUnion(C2DHoledPolygon Other, List<C2DHoledPolygon> Polygons,
                                            CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetUnion(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolygon(NewPolys[i]));
        }


        /// <summary>
        /// Gets the overlaps of the 2 shapes.	
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetOverlaps(C2DHoledPolygon Other, List<C2DHoledPolygon> Polygons,
                                            CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetOverlaps(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolygon(NewPolys[i]));
        }







        /// <summary>
        /// The rim.
        /// </summary>
        public new C2DPolygon Rim
        {
            get
            {
                return _Rim as C2DPolygon;
            }
            set
            {
                _Rim = value;
            }
        }

        /// <summary>
        /// Gets the Hole as a C2DPolygon.
        /// </summary>
        /// <param name="i">Hole index.</param> 
        public new C2DPolygon GetHole(int i)
        {
            return _Holes[i] as C2DPolygon;
        }

        /// <summary>
        /// Hole access.
        /// </summary>
        public void SetHole(int i, C2DPolygon Poly)
        {
            _Holes[i] = Poly;
        }

        /// <summary>
        /// Hole addition.
        /// </summary>
        public void AddHole(C2DPolygon Poly)
        {
            _Holes.Add(Poly);
        }

        /// <summary>
        /// Hole access.
        /// </summary>
        public new void SetHole(int i, C2DPolyBase Poly)
        {
            if (Poly is C2DPolygon)
            {
                _Holes[i] = Poly;
            }
            else
            {
                Debug.Assert(false, "Invalid Hole type");
            }
        }

        /// <summary>
        /// Hole addition.
        /// </summary>
        public new void AddHole(C2DPolyBase Poly)
        {
            if (Poly is C2DPolygon)
            {
                _Holes.Add(Poly);
            }
            else
            {
                Debug.Assert(false, "Invalid Hole type");
            }
        }

        /// <summary>
        /// Hole removal.
        /// </summary>
        public new void RemoveHole(int i)
        {
            _Holes.RemoveAt(i);
        }
    }
}
