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
            bundaryLines = new List<Line>();
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
                    try
                    {
                        BoundaryLines = BoundaryLines.OrderBy(i => boundaryLineOrderIndex.ElementAt(BoundaryLines.IndexOf(i))).ToList();
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLog(e);
                    }
                }
                return BoundaryLines;
            }
            set {
                BoundaryLines = value;
                if (boundaryLineOrderIndex == null || !boundaryLineOrderIndex.Any()) {
                    InitializeBoundaryOrderIndex();
                }
                CalculateBoundaryLineOrderIndex();
                if (BoundaryLines.Count > boundaryLineOrderIndex.Count) {
                    throw new Exception("Failed to order");
                }
            }
        }

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

        //todo, remove private field
        public List<Point> BoundaryPoints {
            get {
                InitializeBoundaryOrderIndex();
                BoundaryLines = BoundaryLines.OrderBy(i => boundaryLineOrderIndex.ElementAt(BoundaryLines.IndexOf(i))).ToList();
                BoundaryPoints.Clear();
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

                    BoundaryPoints.Add(commonPoint);
                }

                return BoundaryPoints;
            }
            set { BoundaryPoints = value; }
        }

        public bool IsLineMissing() {
            return false;
        }

        public bool IsLineInRoom() {
            return false;
        }


        public double CalculateArea()
        {
            return 0.0;
        }
    }
}