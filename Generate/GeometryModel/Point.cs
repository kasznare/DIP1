using System;
using System.Collections.Generic;
using ONLAB2;

namespace WindowsFormsApp1 {
    public class Point : IGeometry {
        public Point(double x, double y) {
            this.X = x;
            this.Y = y;
            RelatedLines = new List<Line>();
        }
        public List<Room> RelatedRooms {
            get {
                List<Room> rooms = new List<Room>();
                foreach (Line relatedLine in RelatedLines) {
                    rooms.AddRange(relatedLine.relatedRooms);
                }
                return rooms;
            }
            set { RelatedRooms = value; }
        }
        //A point can only have 4 lines starting from it
        public List<Line> RelatedLines;

        public int GetDegree() {
            return this.RelatedLines.Count;
        }
        /// <summary>
        /// sum override
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Point operator +(Point a, Point b) {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
        /// <summary>
        /// multiply override
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Point operator *(Point a, double b) {
            return new Point(a.X * b, a.Y * b);
        }

        public double X { get; set; }

        public double Y { get; set; }

        public Point GetCopy() {
            return new Point(X, Y);
        }

        /// <summary>
        /// is near to
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            Point a = obj as Point;
            if (a == null)
            {
                return false;
            }
            return Close(a.X, X) && Close(a.Y, Y);
            //return base.Equals(obj);
        }

        private bool Close(double a, double b) {
            return Math.Abs(a - b) < 0.01;
        }

        public override string ToString() {
            return $"Point {X},{Y}";
        }
    }
}