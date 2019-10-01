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
            boundaryLineOrderIndex = new List<int>();
            Guid = Guid.NewGuid();

            //observableBoundaryLines.CollectionChanged += ObservableBoundaryLinesOnCollectionChanged;
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
            //if (boundaryLineOrderIndex == null || !boundaryLineOrderIndex.Any()) {
            //    InitializeBoundaryOrderIndex();
            //}
            if (isBoundaryLinesPossiblyUnsorted) {
                SortBoundaryLines();
                //CalculateBoundaryLineOrderIndex();
                isBoundaryLinesPossiblyUnsorted = false;
            }
            //if (BoundaryLines.Count > boundaryLineOrderIndex.Count) {
            //    throw new Exception("Failed to order");
            //}
            //if (boundaryLineOrderIndex != null) {
            //    try {
            //        BoundaryLines = BoundaryLines.OrderBy(i => boundaryLineOrderIndex.ElementAt(BoundaryLines.IndexOf(i))).ToList();
            //    }
            //    catch (Exception e) {
            //        Logger.WriteLog(e);
            //    }
            //}
            return BoundaryLines;
        }
        //public List<int> boundaryLineOrderIndex;

        private void SortBoundaryLines() {
            List<Line> orderedLines = new List<Line>();

            int actualIndex = 0;
            for (int i = 0; i < BoundaryLines.Count; i++) {
                Line actualLine = BoundaryLines.ElementAt(actualIndex);
                orderedLines.Add(actualLine);
                actualIndex = 0;
                foreach (Line line in BoundaryLines) {
                    if ((line.startPoint == actualLine.startPoint && line.endPoint != actualLine.endPoint) ||
                        (line.endPoint == actualLine.endPoint && line.startPoint != actualLine.startPoint)) {
                        break;
                    }
                    actualIndex++;
                }
                if (actualIndex > BoundaryLines.Count) {
                    throw new Exception("LineOrderingFailed");
                }
            }

            BoundaryLines = orderedLines;
        }
        private void CalculateBoundaryLineOrderIndex() {
            //if (!BoundaryLines.Any()) return;
            ////if (boundaryLineOrderIndex == null || !boundaryLineOrderIndex.Any())
            //InitializeBoundaryOrderIndex();

            //Line actualLine = BoundaryLines.First();
            //int orderIndex = 0;
            //boundaryLineOrderIndex[0] = orderIndex;
            //bool allFound = false;
            //Stopwatch st = new Stopwatch();
            //st.Start();
            //while (!allFound) {
            //    bool nextFound = false;
            //    foreach (Line segment in BoundaryLines) {
            //        if (segment.startPoint == actualLine.startPoint && segment.endPoint != actualLine.endPoint) {
            //            actualLine = segment;
            //            int indexInList = BoundaryLines.IndexOf(segment);
            //            orderIndex++;
            //            boundaryLineOrderIndex[indexInList] = orderIndex;
            //            nextFound = true;
            //            break;
            //        }
            //        if (segment.startPoint == actualLine.endPoint && segment.endPoint != actualLine.startPoint) {
            //            actualLine = segment;
            //            int indexInList = BoundaryLines.IndexOf(segment);
            //            orderIndex++;
            //            boundaryLineOrderIndex[indexInList] = orderIndex;
            //            nextFound = true;
            //            break;
            //        }
            //        if (segment.endPoint == actualLine.startPoint && segment.startPoint != actualLine.endPoint) {
            //            actualLine = segment;
            //            int indexInList = BoundaryLines.IndexOf(segment);
            //            orderIndex++;
            //            boundaryLineOrderIndex[indexInList] = orderIndex;
            //            nextFound = true;
            //            break;

            //        }

            //        if (segment.endPoint == actualLine.endPoint && segment.startPoint != actualLine.startPoint) {
            //            actualLine = segment;
            //            int indexInList = BoundaryLines.IndexOf(segment);
            //            orderIndex++;
            //            boundaryLineOrderIndex[indexInList] = orderIndex;
            //            nextFound = true;
            //            break;
            //        }
            //    }

            //    if (!nextFound) {
            //        //this is a problem
            //    }

            //    if (orderIndex == BoundaryLines.Count - 1 || st.ElapsedMilliseconds > 2000) {
            //        allFound = true;
            //        //InitializeBoundaryOrderIndex();
            //        return;
            //    }

            //}
        }
        private void InitializeBoundaryOrderIndex() {
            //if (boundaryLineOrderIndex == null || !boundaryLineOrderIndex.Any()) {
            //    boundaryLineOrderIndex = new List<int>();
            //}
            //for (int i = boundaryLineOrderIndex.Count; i < BoundaryLines.Count; i++) {
            //    boundaryLineOrderIndex.Add(i);
            //}

        }

        internal Room GetCopy() {
            return new Room(Name, Number);
        }


        public List<Point> BoundaryPoints {
            get {
                //InitializeBoundaryOrderIndex();
                //CalculateBoundaryLineOrderIndex();
                //BoundaryLines = BoundaryLines.OrderBy(i => boundaryLineOrderIndex.ElementAt(BoundaryLines.IndexOf(i))).ToList();
                SortBoundaryLines();
                //BoundaryPoints.Clear();
                boundaryPoints = new List<Point>();

                //Line firstLine = BoundaryLines.First();
                //Line lastLine = BoundaryLines.Last();
                //Line actualLastLine = BoundaryLines[boundaryLineOrderIndex.Last()];

                for (var index = 0; index < BoundaryLines.Count; index++) {
                    //Line actualLine = BoundaryLines[boundaryLineOrderIndex[index]];
                    Line firstLine = BoundaryLines.ElementAt(index);
                    Line nextLine = BoundaryLines.ElementAt((index + 1)%BoundaryLines.Count);
                    Point commonPoint = FindCommonPointOnTwoLines(firstLine, nextLine);

                    boundaryPoints.Add(commonPoint);
                }

                return boundaryPoints;
            }
            set { boundaryPoints = value; }
        }

        private static Point FindCommonPointOnTwoLines(Line firstLine, Line nextLine)
        {
            Point commonPoint = null;
            if (firstLine.startPoint == nextLine.startPoint)
            {
                commonPoint = nextLine.startPoint;
            }

            if (firstLine.startPoint == nextLine.endPoint)
            {
                commonPoint = nextLine.endPoint;
            }

            if (firstLine.endPoint == nextLine.startPoint)
            {
                commonPoint = nextLine.startPoint;
            }

            if (firstLine.endPoint == nextLine.endPoint)
            {
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
            List<Point> bp = BoundaryPoints;

            double[] X = bp.Select(i => i.X).ToArray();
            double[] Y = bp.Select(i => i.Y).ToArray();
            double area = polygonArea(X, Y, bp.Count);
            return area;
        }
        /// <summary>
        /// of a polygon using shoelace formula
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double polygonArea(double[] X,
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