using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace GeoLib
{
    /// <summary>
    /// Class to represent a 2D polyarc with holes.
    /// </summary>
    public class C2DHoledPolyArc : C2DHoledPolyBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public C2DHoledPolyArc()
        {
            _Rim = new C2DPolyArc();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public C2DHoledPolyArc(C2DHoledPolyBase Other)
        {
            _Rim = new C2DPolyArc(Other.Rim);
            for (int i = 0; i < Other.HoleCount; i++)
            {
                _Holes.Add(new C2DPolyArc(Other.GetHole(i)));
            }
        }

                /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public C2DHoledPolyArc(C2DPolyBase Other)
        {
            _Rim = new C2DPolyArc(Other);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public C2DHoledPolyArc(C2DPolyArc Other)
        {
            _Rim = new C2DPolyArc(Other);
        }


        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public new void Set(C2DHoledPolyBase Other)
        {
            _Rim.Set(Other.Rim);
            _Holes.Clear();
            for (int i = 0; i < Other.HoleCount; i++)
            {
                _Holes.Add(new C2DPolyArc(Other.GetHole(i)));
            }
        }

        /// <summary>
        /// Assignment.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public void Set(C2DHoledPolyArc Other)
        {
            _Rim.Set(Other.Rim);
            _Holes.Clear();
            for (int i = 0; i < Other.HoleCount; i++)
            {
                _Holes.Add(new C2DPolyArc(Other.GetHole(i)));
            }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Other">Other polygon to set this to.</param> 
        public C2DHoledPolyArc(C2DHoledPolyArc Other)
        {
            _Rim = new C2DPolyArc(Other.Rim);
            for (int i = 0; i < Other.HoleCount; i++)
            {
                _Holes.Add(new C2DPolyArc(Other.GetHole(i)));
            }
        }


        /// <summary>
        /// Gets the area.
        /// </summary>
         public double GetArea() 
        {
	        double dResult = 0;

		    dResult += Rim.GetArea();

	        for ( int i = 0 ; i < _Holes.Count; i++)
	        {
		        dResult -= GetHole(i).GetArea();
	        }
	        return dResult;


        }

        /// <summary>
        /// Gets the centroid.
        /// </summary>
        C2DPoint GetCentroid()
        {
            C2DPoint Centroid = Rim.GetCentroid();
            double dArea = Rim.GetArea();

	        for (int i = 0; i < _Holes.Count; i++)
	        {
			        C2DVector vec = new C2DVector( Centroid, GetHole(i).GetCentroid());

			        double dHoleArea = GetHole(i).GetArea();

			        double dFactor =  dHoleArea / (dHoleArea + dArea);	

			        vec.Multiply( dFactor);
			        Centroid.x += vec.i;
                    Centroid.y += vec.j;
			        dArea += dHoleArea;
	        }


	        return Centroid;

        }


        /// <summary>
        /// Gets the non overlaps i.e. the parts of this that aren't in the other.
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetNonOverlaps(C2DHoledPolyArc Other, List<C2DHoledPolyArc> Polygons,
                                            CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetNonOverlaps(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolyArc(NewPolys[i]));
        }

        /// <summary>
        /// Gets the union of the 2 shapes.
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetUnion(C2DHoledPolyArc Other, List<C2DHoledPolyArc> Polygons,
                                            CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetUnion(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolyArc(NewPolys[i]));
        }


        /// <summary>
        /// Gets the overlaps of the 2 shapes.	
        /// </summary>
        /// <param name="Other">The other shape.</param> 
        /// <param name="Polygons">The set to recieve the result.</param> 
        /// <param name="grid">The degenerate settings.</param> 
        public void GetOverlaps(C2DHoledPolyArc Other, List<C2DHoledPolyArc> Polygons,
                                            CGrid grid)
        {
            List<C2DHoledPolyBase> NewPolys = new List<C2DHoledPolyBase>();

            base.GetOverlaps(Other, NewPolys, grid);

            for (int i = 0; i < NewPolys.Count; i++)
                Polygons.Add(new C2DHoledPolyArc(NewPolys[i]));
        }











        /// <summary>
        /// Rim access.
        /// </summary>
        public new C2DPolyArc Rim
        {
            get
            {
                return _Rim as C2DPolyArc;
            }
            set
            {
                _Rim = value;
            }
        }


        /// <summary>
        /// Gets the Hole as a C2DPolyArc.
        /// </summary>
        /// <param name="i">Hole index.</param> 
        public new C2DPolyArc GetHole(int i)
        {
            return _Holes[i] as C2DPolyArc;
        }

        /// <summary>
        /// Hole assignment.
        /// </summary>
        public void SetHole(int i, C2DPolyArc Poly)
        {
            _Holes[i] = Poly;
        }

        /// <summary>
        /// Hole addition.
        /// </summary>
        public void AddHole(C2DPolyArc Poly)
        {
            _Holes.Add(Poly);
        }

        /// <summary>
        /// Hole assignment.
        /// </summary>
        public new void SetHole(int i, C2DPolyBase Poly)
        {
            if (Poly is C2DPolyArc)
            {
                _Holes[i] = Poly;
            }
            else
            {
                Debug.Assert(false, "Invalid Hole type" );
            }
        }

        /// <summary>
        /// Hole addition.
        /// </summary>
        public new void AddHole(C2DPolyBase Poly)
        {
            if (Poly is C2DPolyArc)
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
