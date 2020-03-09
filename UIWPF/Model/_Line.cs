using System;

namespace UIWPF.Model {
    public class _Line : _Geometry {
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
            if (lineToMove.StartPoint.XY == this.StartPoint.XY 
                && lineToMove.EndPoint.XY == this.EndPoint.XY) return true;
            if (lineToMove.StartPoint.XY == this.EndPoint.XY 
                && lineToMove.EndPoint.XY == this.StartPoint.XY) return true;
            return false;
        }

        public bool Connects(_Line lineToMove) {
            if (lineToMove.StartPoint.XY == this.StartPoint.XY) return true;
            if (lineToMove.StartPoint.XY == this.EndPoint.XY) return true;
            if (lineToMove.EndPoint.XY == this.StartPoint.XY) return true;
            if (lineToMove.EndPoint.XY == this.EndPoint.XY) return true;
            return false;
        }
        public _Point ConnectsPoint(_Line lineToMove) {
            if (lineToMove.StartPoint.XY == this.StartPoint.XY) return lineToMove.StartPoint;
            if (lineToMove.StartPoint.XY == this.EndPoint.XY) return lineToMove.StartPoint;
            if (lineToMove.EndPoint.XY == this.StartPoint.XY) return lineToMove.EndPoint;
            if (lineToMove.EndPoint.XY == this.EndPoint.XY) return lineToMove.EndPoint;
            return null;
        }

        internal void Move(_Point moveVector) {
            EndPoint.Move(moveVector);
            StartPoint.Move(moveVector);


        }
    }
}