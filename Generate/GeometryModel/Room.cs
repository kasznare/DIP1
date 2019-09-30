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
            //observableBoundaryLines.CollectionChanged += ObservableBoundaryLinesOnCollectionChanged;
        }
        public string Name { get; set; }
        public string Number { get; set; }

        /// <summary>
        /// calculated actual degree from simulation
        /// </summary>
        public int Degree { get; set; }
        public bool isStartRoom { get; set; }
        public RoomType type { get; set; }

        public List<Line> BoundaryLines {
            //to return the lines only in ordered form
            get {
                if (boundaryLineOrderIndex != null) {
                    try {
                        boundaryLines = boundaryLines.OrderBy(i => boundaryLineOrderIndex.ElementAt(boundaryLines.IndexOf(i))).ToList();
                    }
                    catch (Exception e) {
                        Logger.WriteLog(e);
                    }
                }
                return boundaryLines;
            }
            set {
                boundaryLines = value;
                if (boundaryLineOrderIndex == null || !boundaryLineOrderIndex.Any()) {
                    InitializeBoundaryOrderIndex();
                }
                CalculateBoundaryLineOrderIndex();
                if (value.Count > boundaryLineOrderIndex.Count) {
                    throw new Exception("Failed to order");
                }
            }
        }
        private List<Line> boundaryLines;

        public List<int> boundaryLineOrderIndex;
        //store observable collections, implement Inotifypropertychanged to trigger recalculate
        //public ObservableCollection<Line> observableBoundaryLines = new ObservableCollection<Line>();

        private void ObservableBoundaryLinesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        }

        private void CalculateBoundaryLineOrderIndex() {
            if (!BoundaryLines.Any()) return;
            //if (boundaryLineOrderIndex == null || !boundaryLineOrderIndex.Any())
            {
                InitializeBoundaryOrderIndex();
            }

            Line actualLine = BoundaryLines.First();
            int orderIndex = 0;
            boundaryLineOrderIndex[0] = orderIndex;
            bool allFound = false;
            Stopwatch st = new Stopwatch();
            st.Start();
            while (!allFound) {
                bool nextFound = false;
                foreach (Line segment in BoundaryLines) {
                    if (segment.startPoint == actualLine.startPoint && segment.endPoint != actualLine.endPoint) {
                        actualLine = segment;
                        int indexInList = BoundaryLines.IndexOf(segment);
                        orderIndex++;
                        boundaryLineOrderIndex[indexInList] = orderIndex;
                        nextFound = true;
                        break;
                    }
                    if (segment.startPoint == actualLine.endPoint && segment.endPoint != actualLine.startPoint) {
                        actualLine = segment;
                        int indexInList = BoundaryLines.IndexOf(segment);
                        orderIndex++;
                        boundaryLineOrderIndex[indexInList] = orderIndex;
                        nextFound = true;
                        break;
                    }
                    if (segment.endPoint == actualLine.startPoint && segment.startPoint != actualLine.endPoint) {
                        actualLine = segment;
                        int indexInList = BoundaryLines.IndexOf(segment);
                        orderIndex++;
                        boundaryLineOrderIndex[indexInList] = orderIndex;
                        nextFound = true;
                        break;

                    }

                    if (segment.endPoint == actualLine.endPoint && segment.startPoint != actualLine.startPoint) {
                        actualLine = segment;
                        int indexInList = BoundaryLines.IndexOf(segment);
                        orderIndex++;
                        boundaryLineOrderIndex[indexInList] = orderIndex;
                        nextFound = true;
                        break;
                    }
                }

                if (!nextFound) {
                    //this is a problem
                }

                if (orderIndex == BoundaryLines.Count - 1 || st.ElapsedMilliseconds > 2000) {
                    allFound = true;
                    //InitializeBoundaryOrderIndex();
                    return;
                }

            }
        }

        internal Room GetCopy() {
            return new Room(Name, Number);
        }

        private void InitializeBoundaryOrderIndex() {
            if (boundaryLineOrderIndex == null || !boundaryLineOrderIndex.Any()) {
                boundaryLineOrderIndex = new List<int>();
            }
            for (int i = boundaryLineOrderIndex.Count; i < BoundaryLines.Count; i++) {
                boundaryLineOrderIndex.Add(i);
            }

        }

        public List<Point> BoundaryPoints {
            get {
                InitializeBoundaryOrderIndex();
                BoundaryLines = BoundaryLines.OrderBy(i => boundaryLineOrderIndex.ElementAt(BoundaryLines.IndexOf(i))).ToList();
                //BoundaryPoints.Clear();
                boundaryPoints = new List<Point>();

                //Line firstLine = BoundaryLines.First();
                //Line lastLine = BoundaryLines.Last();
                Line actualLastLine = BoundaryLines[boundaryLineOrderIndex.Last()];

                for (var index = 0; index < BoundaryLines.Count; index++) {
                    Line actualLine = BoundaryLines[boundaryLineOrderIndex[index]];
                    Point commonPoint = null;

                    if (actualLastLine.startPoint == actualLine.startPoint) {
                        commonPoint = actualLine.startPoint;
                    }
                    if (actualLastLine.startPoint == actualLine.endPoint) {
                        commonPoint = actualLine.endPoint;
                    }
                    if (actualLastLine.endPoint == actualLine.startPoint) {
                        commonPoint = actualLine.startPoint;
                    }
                    if (actualLastLine.endPoint == actualLine.endPoint) {
                        commonPoint = actualLine.endPoint;
                    }

                    actualLastLine = actualLine;

                    boundaryPoints.Add(commonPoint);
                }

                return boundaryPoints;
            }
            set { boundaryPoints = value; }
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
            double area = polygonArea(bp.Select(i => i.X).ToArray(), bp.Select(i => i.Y).ToArray(), bp.Count);
            return area;
        }

        // (X[i], Y[i]) are coordinates of i'th point. 
        public static double polygonArea(double[] X,
            double[] Y, int n) {

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