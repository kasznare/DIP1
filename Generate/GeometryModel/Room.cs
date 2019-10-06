using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using ONLAB2;

namespace WindowsFormsApp1 {
    public class Room : IGeometry {
        public Room(string name, string number) {
            Name = name;
            Number = number;
            BoundaryPoints = new List<MyPoint>();
            BoundaryLines = new List<MyLine>();
            Guid = Guid.NewGuid();
        }

        internal Room GetCopy() {
            return new Room(Name, Number);
        }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        /// <summary>
        /// calculated actual degree from simulation
        /// </summary>
        public int Degree { get; set; }
        public bool isStartRoom { get; set; }
        public RoomType type { get; set; }
        private bool isBoundaryLinesPossiblyUnsorted = false;
        private bool isBoundaryPointsPossiblyUnsorted = false;
        private List<MyLine> boundaryLines;
        public List<MyLine> BoundaryLines {
            get => boundaryLines;
            set {
                boundaryLines = value;
                isBoundaryLinesPossiblyUnsorted = true;
            }
        }

        //to return the lines only in ordered form
        public List<MyLine> GetBoundaryLinesSorted() {
            if (isBoundaryLinesPossiblyUnsorted) {
                SortBoundaryLines();
            }
            return BoundaryLines;
        }
        private void SortBoundaryLines() {
            List<MyLine> orderedLines = new List<MyLine>();

            int actualIndex = 0;
            for (int i = 0; i < BoundaryLines.Count; i++) {
                MyLine actualMyLine = BoundaryLines.ElementAt(actualIndex);
                orderedLines.Add(actualMyLine);
                actualIndex = 0;
                foreach (MyLine line in BoundaryLines) {

                    if (orderedLines.Contains(line))
                    {
                        actualIndex++;
                        continue;
                    }
                    MyPoint p1 = line.StartMyPoint;
                    MyPoint p2 = line.EndMyPoint;
                    MyPoint p3 = actualMyLine.StartMyPoint;
                    MyPoint p4 = actualMyLine.EndMyPoint;

                    if (line.StartMyPoint == actualMyLine.StartMyPoint && line.EndMyPoint != actualMyLine.EndMyPoint) {
                        break;
                    }
                    if (line.StartMyPoint == actualMyLine.EndMyPoint && line.EndMyPoint != actualMyLine.StartMyPoint) {
                        break;
                    }
                    if (line.EndMyPoint == actualMyLine.EndMyPoint && line.StartMyPoint != actualMyLine.StartMyPoint) {
                        break;
                    }
                    if (line.EndMyPoint == actualMyLine.StartMyPoint && line.StartMyPoint != actualMyLine.EndMyPoint) {
                        break;
                    }

                    actualIndex++;
                }
                if (actualIndex > BoundaryLines.Count) {
                    throw new Exception("LineOrderingFailed");
                }
            }

            BoundaryLines = orderedLines;
            isBoundaryLinesPossiblyUnsorted = false;
        }
        public List<MyPoint> BoundaryPoints {
            get => boundaryPoints;
            set {
                boundaryPoints = value;
                isBoundaryPointsPossiblyUnsorted = true;
            }
        }

        public List<MyPoint> GetBoundaryPointsSorted() {
            if (isBoundaryPointsPossiblyUnsorted) {
                SortBoundaryPoints();
            }
            return BoundaryPoints;
        }

        private void SortBoundaryPoints() {
            if (isBoundaryLinesPossiblyUnsorted) {
                SortBoundaryLines();
            }
            boundaryPoints = new List<MyPoint>();

            for (var index = 0; index < BoundaryLines.Count; index++) {
                MyLine firstMyLine = BoundaryLines.ElementAt(index);
                MyLine nextMyLine = BoundaryLines.ElementAt((index + 1) % BoundaryLines.Count);
                MyPoint commonMyPoint = FindCommonPointOnTwoLines(firstMyLine, nextMyLine);
                boundaryPoints.Add(commonMyPoint);
            }
        }

        private static MyPoint FindCommonPointOnTwoLines(MyLine firstMyLine, MyLine nextMyLine) {
            MyPoint commonMyPoint = null;
            if (firstMyLine.StartMyPoint == nextMyLine.StartMyPoint) {
                commonMyPoint = nextMyLine.StartMyPoint;
            }
            if (firstMyLine.StartMyPoint == nextMyLine.EndMyPoint) {
                commonMyPoint = nextMyLine.EndMyPoint;
            }
            if (firstMyLine.EndMyPoint == nextMyLine.StartMyPoint) {
                commonMyPoint = nextMyLine.StartMyPoint;
            }
            if (firstMyLine.EndMyPoint == nextMyLine.EndMyPoint) {
                commonMyPoint = nextMyLine.EndMyPoint;
            }

            return commonMyPoint;
        }

        private List<MyPoint> boundaryPoints;
        public bool IsLineMissing() {
            return false;
        }
        public bool IsLineInRoom() {
            return false;
        }

        public double CalculateArea() {
            List<MyPoint> bp = GetBoundaryPointsSorted();

            double[] X = bp.Select(i => i.X).ToArray();
            double[] Y = bp.Select(i => i.Y).ToArray();
            double area = PolygonArea(X, Y, bp.Count);
            return area;
        }
        /// <summary>
        /// of a polygon using shoelace formula
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double PolygonArea(double[] X,
            double[] Y, int n) {
            // (X[i], Y[i]) are coordinates of i'th myPoint. 

            // Initialze area 
            double area = 0.0;

            // Calculate value of shoelace formula 
            int j = n - 1;

            for (int i = 0; i < n; i++) {
                area += (X[j] + X[i]) * (Y[j] - Y[i]);

                // j is previous vertex to i 
                j = i;
            }

            // Return absolute value 
            return Math.Abs(area / 2.0);
        }
    }
}