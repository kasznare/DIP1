using System;

namespace UIWPF.Model {
    public class _Point {
        public double[] XY => new double[]{X,Y};

        public double X { get; set; }
        public double Y { get; set; }

        public _Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public _Point DeepCopy()
        {
            return new _Point(X, Y);
        }

        internal _Point Move(_Point moveVector) {
            return new _Point(X-moveVector.X, Y-moveVector.Y);
        }

        public static _Point operator *(_Point a, double b) {
            return new _Point(a.X * b, a.Y * b);
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType())) {
                return false;
            }
            else {
                _Point p = (_Point)obj;
                return (X == p.X) && (Y == p.Y);
            }
        }
    }
}