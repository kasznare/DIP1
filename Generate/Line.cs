using System;
using System.Collections.Generic;

namespace WindowsFormsApp1 {
    public class Line :IGeometry {
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }
        //public double length;
        public List<Room> relatedRooms { get; set; }
        public List<Point> relatedPoints { get; set; }
        public Point GetDirection() {
            return new Point(startPoint.X - endPoint.X, startPoint.Y - endPoint.Y);
        }
        public double GetLength() {
            double x1 = startPoint.X;
            double x2 = endPoint.X;
            double y1 = startPoint.Y;
            double y2 = endPoint.Y;
            return Math.Sqrt(Math.Abs(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
        }
        public Line GetCopy() {
            return new Line(startPoint, endPoint);
        }
        public Line GetInverse() {
            return new Line(startPoint, new Point(-endPoint.X, -endPoint.Y));
        }
        public Line GetNormal(bool isNormalized = false) {
            //if we define dx = x2 - x1 and dy = y2 - y1, then the normals are(-dy, dx) and(dy, -dx).
            double dx = startPoint.X - endPoint.X;
            double dy = startPoint.Y - endPoint.Y;
            Line normaLine = new Line(new Point(-dy, dx), new Point(dy, -dx));
            if (!isNormalized) {
                return normaLine;
            }
            else {
                return Normalize(normaLine);
            }

        }
        public Point GetNV(bool isNormalized = false) {
            //if we define dx = x2 - x1 and dy = y2 - y1, then the normals are(-dy, dx) and(dy, -dx).
            double dx = startPoint.X - endPoint.X;
            double dy = startPoint.Y - endPoint.Y;
            Point p = new Point(-dy, dx);
            if (!isNormalized) {
                return p;
            }
            else {
                return Normalize(p);
            }
        }
        public Line Normalize(Line line) {
            Point sPoint = line.startPoint;
            Point ePoint = line.endPoint;
            double length = line.GetLength();
            Point newePoint = new Point(((-sPoint.X + ePoint.X) / length), ((-sPoint.Y + ePoint.Y) / length));
            Line line2 = new Line(new Point(0, 0), newePoint);
            return line2;
        }
        public Point Normalize(Point p) {
            double sum = Math.Sqrt(p.X * p.X + p.Y * p.Y);
            Point p2 = new Point(p.X / sum, p.Y / sum);
            return p2;
        }
        public Point GetPointAt(double percentage) {
            if (percentage > 1) {
                percentage = percentage / 100;
            }

            if (percentage < 0) {
                percentage = -percentage;
            }

            double newx = startPoint.X * (1 - percentage) + endPoint.X * percentage;
            double newy = startPoint.Y * (1 - percentage) + endPoint.Y * percentage;
            newx = Math.Round(newx);
            newy = Math.Round(newy);
            return new Point(newx, newy);

        }
        public bool IsPointOnLine(Point p) {
            bool ison = false;



            return ison;
        }
        public Line(Point startPoint, Point endPoint) {
            this.startPoint = startPoint;
            this.endPoint = endPoint;

            this.relatedPoints = new List<Point>();
            relatedPoints.Add(startPoint);
            relatedPoints.Add(endPoint);

            startPoint.RelatedLines.Add(this);
            endPoint.RelatedLines.Add(this);

            relatedRooms = new List<Room>();
        }
        public override string ToString() {
            return $"Line from {startPoint} to {endPoint}";
        }
    }
}