using System;
using System.Collections.Generic;
using ONLAB2;

namespace WindowsFormsApp1 {
    public class MyPoint : IGeometry {
        public MyPoint(double x, double y) {
            this.X = x;
            this.Y = y;
            RelatedLines = new List<MyLine>();
            Guid = Guid.NewGuid();
        }

        static void CopyProperies(MyPoint replaceFrom, MyPoint replaceTo)
        {
            replaceTo.RelatedLines.AddRange(replaceFrom.RelatedLines);
        }
        public double X { get; set; }
        public double Y { get; set; }
        public Guid Guid { get; set; }
        public List<MyRoom> RelatedRooms {
            get {
                List<MyRoom> rooms = new List<MyRoom>();
                foreach (MyLine relatedLine in RelatedLines) {
                    foreach (MyRoom room in relatedLine.relatedRooms) {
                        if (!rooms.Contains(room)) {
                            rooms.Add(room);
                        }
                    }
                }
                return rooms;
            }
            set { RelatedRooms = value; }
        }
        //A myPoint can only have 4 lines starting from it
        public List<MyLine> RelatedLines;

        public MyPoint GetCopy() {
            return new MyPoint(X, Y);
        }
        public int GetDegree() {
            return this.RelatedLines.Count;
        }
        /// <summary>
        /// sum override
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static MyPoint operator +(MyPoint a, MyPoint b) {
            return new MyPoint(a.X + b.X, a.Y + b.Y);
        }
        /// <summary>
        /// multiply override
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static MyPoint operator *(MyPoint a, double b) {
            return new MyPoint(a.X * b, a.Y * b);
        }

        /// <summary>
        /// Is near to
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            MyPoint a = obj as MyPoint;
            if (a == null) {
                return false;
            }
            return Close(a.X, X) && Close(a.Y, Y);
            //return base.Equals(obj);
        }
        private bool Close(double a, double b) {
            return Math.Abs(a - b) < 0.01;
        }
        public override string ToString() {
            return $"MyPoint {X},{Y}";
        }
    }
}