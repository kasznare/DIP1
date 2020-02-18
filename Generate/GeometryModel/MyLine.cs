using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApp1 {
    public class MyLine : IGeometry {
        #region Variables
        public Guid Guid { get; set; }
        public MyPoint StartMyPoint { get; set; }
        public MyPoint EndMyPoint { get; set; }
        public bool HasOpening { get; set; }
        public bool IsOpening { get; set; }
        public List<MyRoom> relatedRooms { get; set; }
        public List<MyPoint> relatedPoints { get; set; } 
        #endregion
        public MyLine(MyPoint startMyPoint, MyPoint endMyPoint) {
            Guid = Guid.NewGuid();
            this.StartMyPoint = startMyPoint;
            this.EndMyPoint = endMyPoint;

            this.relatedPoints = new List<MyPoint>();
            relatedPoints.Add(startMyPoint);
            relatedPoints.Add(endMyPoint);

            startMyPoint.RelatedLines.Add(this);
            endMyPoint.RelatedLines.Add(this);

            relatedRooms = new List<MyRoom>();
        }
        public MyPoint GetDirection() {
            return new MyPoint(StartMyPoint.X - EndMyPoint.X, StartMyPoint.Y - EndMyPoint.Y);
        }
        public double GetLength() {
            double x1 = StartMyPoint.X;
            double x2 = EndMyPoint.X;
            double y1 = StartMyPoint.Y;
            double y2 = EndMyPoint.Y;
            return Math.Sqrt(Math.Abs(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
        }
        public MyLine GetCopy() {
            return new MyLine(StartMyPoint, EndMyPoint);
        }
        public MyLine GetInverse() {
            return new MyLine(StartMyPoint, new MyPoint(-EndMyPoint.X, -EndMyPoint.Y));
        }
        public MyLine GetNormal(bool isNormalized = false) {
            //if we define dx = x2 - x1 and dy = y2 - y1, then the normals are(-dy, dx) and(dy, -dx).
            double dx = StartMyPoint.X - EndMyPoint.X;
            double dy = StartMyPoint.Y - EndMyPoint.Y;
            MyLine normaMyLine = new MyLine(new MyPoint(-dy, dx), new MyPoint(dy, -dx));
            if (!isNormalized) {
                return normaMyLine;
            }
            else {
                return Normalize(normaMyLine);
            }
        }
        public MyPoint GetNV(bool isNormalized = false) {
            //if we define dx = x2 - x1 and dy = y2 - y1, then the normals are(-dy, dx) and(dy, -dx).
            double dx = StartMyPoint.X - EndMyPoint.X;
            double dy = StartMyPoint.Y - EndMyPoint.Y;
            MyPoint p = new MyPoint(-dy, dx);
            if (!isNormalized) {
                return p;
            }
            else {
                return Normalize(p);
            }
        }
        public MyLine Normalize(MyLine myLine) {
            MyPoint sMyPoint = myLine.StartMyPoint;
            MyPoint eMyPoint = myLine.EndMyPoint;
            double length = myLine.GetLength();
            MyPoint neweMyPoint = new MyPoint(((-sMyPoint.X + eMyPoint.X) / length), ((-sMyPoint.Y + eMyPoint.Y) / length));
            MyLine line2 = new MyLine(new MyPoint(0, 0), neweMyPoint);
            return line2;
        }
        public MyPoint Normalize(MyPoint p) {
            double sum = Math.Sqrt(p.X * p.X + p.Y * p.Y);
            MyPoint p2 = new MyPoint(p.X / sum, p.Y / sum);
            return p2;
        }
        public MyPoint GetPointAt(double percentage) {
            if (percentage > 1) {
                percentage = percentage / 100;
            }
            if (percentage < 0) {
                percentage = -percentage;
            }
            double newx = StartMyPoint.X * (1 - percentage) + EndMyPoint.X * percentage;
            double newy = StartMyPoint.Y * (1 - percentage) + EndMyPoint.Y * percentage;
            newx = Math.Round(newx);
            newy = Math.Round(newy);
            return new MyPoint(newx, newy);
        }

        public override string ToString() {
            return $"MyLine from {StartMyPoint} to {EndMyPoint} \nWith rooms: {String.Join(Environment.NewLine, relatedRooms.Select(i=>i.Name))}";
        }
    }
}