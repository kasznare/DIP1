using System;

namespace Diploma2.Model {
    public class _Line : _GeometryBase {
        public _Point StartPoint { get; set; }
        public _Point EndPoint { get; set; }
        public _Line(_Point startPoint, _Point endPoint) {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public _Line DeepCopy()
        {
            _Line deepCopy = new _Line(StartPoint.DeepCopy(), EndPoint.DeepCopy());
            deepCopy.Name = Name;
            deepCopy.Number = Number;
            return deepCopy;
        }

        public bool IsTheSame(_Line lineToMove) {
            if (lineToMove.StartPoint.Equals( this.StartPoint)
                && lineToMove.EndPoint.Equals( this.EndPoint) )return true;
            if (lineToMove.StartPoint.Equals( this.EndPoint)
                && lineToMove.EndPoint.Equals( this.StartPoint)) return true;
            return false;
        }

        public bool Connects(_Line lineToMove) {
            if (lineToMove.StartPoint.Equals( this.StartPoint)) return true;
            if (lineToMove.StartPoint.Equals( this.EndPoint)) return true;
            if (lineToMove.EndPoint.Equals( this.StartPoint)) return true;
            if (lineToMove.EndPoint.Equals( this.EndPoint)) return true;
            return false;
        }
        public _Point ConnectsPoint(_Line lineToMove) {
            if (lineToMove.StartPoint.Equals(this.StartPoint)) return lineToMove.StartPoint;
            if (lineToMove.StartPoint.Equals(this.EndPoint)) return lineToMove.StartPoint;
            if (lineToMove.EndPoint.Equals(this.StartPoint)) return lineToMove.EndPoint;
            if (lineToMove.EndPoint.Equals(this.EndPoint)) return lineToMove.EndPoint;
            return null;
        }

        internal void Move(_Point moveVector) {
            EndPoint = EndPoint.Move(moveVector);
            StartPoint = StartPoint.Move(moveVector);
        }

        public _Point GetNV(bool isNormalized = false) { //if we define dx = x2 - x1 and dy = y2 - y1, then the normals are(-dy, dx) and(dy, -dx).
            double dx = StartPoint.X - EndPoint.X;
            double dy = StartPoint.Y - EndPoint.Y;
           _Point p = new _Point(-dy, dx);
            if (!isNormalized) {
                return p;
            }
            else {
                return Normalize(p);
            }
        }

        public bool isParallel(_Line line)
        {
            return false;
        }

        public _Line Normalize(_Line _Line) {
           _Point sMyPoint = _Line.StartPoint;
           _Point eMyPoint = _Line.EndPoint;
            double length = _Line.GetLength();
           _Point neweMyPoint = new _Point(((-sMyPoint.X + eMyPoint.X) / length), ((-sMyPoint.Y + eMyPoint.Y) / length));
            _Line line2 = new _Line(new _Point(0, 0), neweMyPoint);
            return line2;
        }
        public _Point Normalize(_Point p) {
            double sum = Math.Sqrt(p.X * p.X + p.Y * p.Y);
           _Point p2 = new _Point(p.X / sum, p.Y / sum);
            return p2;
        }

        public double GetLength() {
            double x1 = StartPoint.X;
            double x2 = EndPoint.X;
            double y1 = StartPoint.Y;
            double y2 = EndPoint.Y;
            return Math.Sqrt(Math.Abs(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
        }


        public override string ToString() {
            return $"From({StartPoint.X},{StartPoint.Y}) to ({EndPoint.X},{EndPoint.Y})";
        }
    }
}