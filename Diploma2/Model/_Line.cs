using System;

namespace UIWPF.Model {
    public class _Line : _GeometryBase {
        public _Point StartPoint { get; set; }
        public _Point EndPoint { get; set; }
        public _Line(_Point startPoint, _Point endPoint) {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public _Line DeepCopy() {
            return new _Line(StartPoint.DeepCopy(), EndPoint.DeepCopy());
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

        public override string ToString() {
            return $"From({StartPoint.X},{StartPoint.Y}) to ({EndPoint.X},{EndPoint.Y})";
        }
    }
}