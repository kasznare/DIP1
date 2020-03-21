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
            throw new NotImplementedException();
        }
    }
}