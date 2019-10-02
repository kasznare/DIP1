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
            BoundaryPoints = new List<Point>();
            BoundaryLines = new List<Line>();
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
        private List<Line> boundaryLines;
        public List<Line> BoundaryLines {
            get => boundaryLines;
            set {
                boundaryLines = value;
                isBoundaryLinesPossiblyUnsorted = true;
            }
        }

        //to return the lines only in ordered form
        public List<Line> GetBoundaryLinesSorted() {
            if (isBoundaryLinesPossiblyUnsorted) {
                SortBoundaryLines();
            }
            return BoundaryLines;
        }
        private void SortBoundaryLines() {
            List<Line> orderedLines = new List<Line>();

            int actualIndex = 0;
            for (int i = 0; i < BoundaryLines.Count; i++) {
                Line actualLine = BoundaryLines.ElementAt(actualIndex);
                orderedLines.Add(actualLine);
                actualIndex = 0;
                foreach (Line line in BoundaryLines) {

                    if (orderedLines.Contains(line))
                    {
                        actualIndex++;
                        continue;
                    }
                    Point p1 = line.startPoint;
                    Point p2 = line.endPoint;
                    Point p3 = actualLine.startPoint;
                    Point p4 = actualLine.endPoint;

                    if (line.startPoint == actualLine.startPoint && line.endPoint != actualLine.endPoint) {
                        break;
                    }
                    if (line.startPoint == actualLine.endPoint && line.endPoint != actualLine.startPoint) {
                        break;
                    }
                    if (line.endPoint == actualLine.endPoint && line.startPoint != actualLine.startPoint) {
                        break;
                    }
                    if (line.endPoint == actualLine.startPoint && line.startPoint != actualLine.endPoint) {
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
        public List<Point> BoundaryPoints {
            get => boundaryPoints;
            set {
                boundaryPoints = value;
                isBoundaryPointsPossiblyUnsorted = true;
            }
        }

        public List<Point> GetBoundaryPointsSorted() {
            if (isBoundaryPointsPossiblyUnsorted) {
                SortBoundaryPoints();
            }
            return BoundaryPoints;
        }

        private void SortBoundaryPoints() {
            if (isBoundaryLinesPossiblyUnsorted) {
                SortBoundaryLines();
            }
            boundaryPoints = new List<Point>();

            for (var index = 0; index < BoundaryLines.Count; index++) {
                Line firstLine = BoundaryLines.ElementAt(index);
                Line nextLine = BoundaryLines.ElementAt((index + 1) % BoundaryLines.Count);
                Point commonPoint = FindCommonPointOnTwoLines(firstLine, nextLine);
                boundaryPoints.Add(commonPoint);
            }
        }

        private static Point FindCommonPointOnTwoLines(Line firstLine, Line nextLine) {
            Point commonPoint = null;
            if (firstLine.startPoint == nextLine.startPoint) {
                commonPoint = nextLine.startPoint;
            }
            if (firstLine.startPoint == nextLine.endPoint) {
                commonPoint = nextLine.endPoint;
            }
            if (firstLine.endPoint == nextLine.startPoint) {
                commonPoint = nextLine.startPoint;
            }
            if (firstLine.endPoint == nextLine.endPoint) {
                commonPoint = nextLine.endPoint;
            }

            return commonPoint;
        }

        private List<Point> boundaryPoints;
        public bool IsLineMissing() {
            return false;
        }
        public bool IsLineInRoom() {
            return false;
        }

        public double CalculateArea() {
            List<Point> bp = GetBoundaryPointsSorted();

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
            // (X[i], Y[i]) are coordinates of i'th point. 

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