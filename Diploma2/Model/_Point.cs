using System;

namespace Diploma2.Model {
    public class _Point:_GeometryBase {
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
            //im not sure that it needs to return a new point
            return new _Point(X-moveVector.X, Y-moveVector.Y);
        }

        public static _Point operator *(_Point a, double b) {
            return new _Point(a.X * b, a.Y * b);
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare RunSteps-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType())) {
                return false;
            }
            else {
                _Point p = (_Point)obj;
                return (Math.Abs(X - p.X) < 0.01) && (Math.Abs(Y - p.Y) < 0.01);
            }
        }

        public override string ToString() {
            return $"{X},{Y}";
        }
    }
}