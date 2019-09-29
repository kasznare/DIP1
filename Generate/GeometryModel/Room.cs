using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using ONLAB2;

namespace WindowsFormsApp1 {
    public class Room :IGeometry{
        public Room(string name, string number) {
            Name = name;
            Number = number;
            boundaryPoints = new List<Point>();
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
        public List<Line> bundaryLines {
            //to return the lines only in ordered form
            get {
                CalculateBoundaryLineOrderIndex();
                if (_boundaryLines.Count > boundaryLineOrderIndex.Count) {
                    throw new Exception("Failed to order");
                }
                _boundaryLines = _boundaryLines.OrderBy(i => boundaryLineOrderIndex.ElementAt(_boundaryLines.IndexOf(i))).ToList();
                return _boundaryLines;
            }
            set {
                _boundaryLines = value;
                if (boundaryLineOrderIndex == null || !boundaryLineOrderIndex.Any()) {
                    InitializeBoundaryOrderIndex();
                }
            }
        }

        public List<int> boundaryLineOrderIndex;
        //store observable collections, implement Inotifypropertychanged to trigger recalculate
        //public ObservableCollection<Line> observableBoundaryLines = new ObservableCollection<Line>();

        private void ObservableBoundaryLinesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        }

        private void CalculateBoundaryLineOrderIndex() {
            if (!_boundaryLines.Any()) return;
            //if (boundaryLineOrderIndex == null || !boundaryLineOrderIndex.Any())
            {
                InitializeBoundaryOrderIndex();
            }

            Line actualLine = _boundaryLines.First();
            int orderIndex = 0;
            boundaryLineOrderIndex[0] = orderIndex;
            bool allFound = false;
            Stopwatch st = new Stopwatch();
            st.Start();
            while (!allFound) {
                bool nextFound = false;
                foreach (Line segment in _boundaryLines) {
                    if (segment.startPoint == actualLine.startPoint && segment.endPoint != actualLine.endPoint) {
                        actualLine = segment;
                        int indexInList = _boundaryLines.IndexOf(segment);
                        orderIndex++;
                        boundaryLineOrderIndex[indexInList] = orderIndex;
                        nextFound = true;
                        break;
                    }
                    if (segment.startPoint == actualLine.endPoint && segment.endPoint != actualLine.startPoint) {
                        actualLine = segment;
                        int indexInList = _boundaryLines.IndexOf(segment);
                        orderIndex++;
                        boundaryLineOrderIndex[indexInList] = orderIndex;
                        nextFound = true;
                        break;
                    }
                    if (segment.endPoint == actualLine.startPoint && segment.startPoint != actualLine.endPoint) {
                        actualLine = segment;
                        int indexInList = _boundaryLines.IndexOf(segment);
                        orderIndex++;
                        boundaryLineOrderIndex[indexInList] = orderIndex;
                        nextFound = true;
                        break;

                    }

                    if (segment.endPoint == actualLine.endPoint && segment.startPoint != actualLine.startPoint) {
                        actualLine = segment;
                        int indexInList = _boundaryLines.IndexOf(segment);
                        orderIndex++;
                        boundaryLineOrderIndex[indexInList] = orderIndex;
                        nextFound = true;
                        break;
                    }
                }

                if (!nextFound) {
                    //this is a problem
                }

                if (orderIndex == _boundaryLines.Count - 1 || st.ElapsedMilliseconds > 2000) {
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
            for (int i = boundaryLineOrderIndex.Count; i < _boundaryLines.Count; i++) {
                boundaryLineOrderIndex.Add(i);
            }

        }

        //todo, remove private field
        private List<Line> _boundaryLines;
        public List<Point> boundaryPoints {
            get {
                InitializeBoundaryOrderIndex();
                _boundaryLines = _boundaryLines.OrderBy(i => boundaryLineOrderIndex.ElementAt(_boundaryLines.IndexOf(i))).ToList();
                _boundaryPoints.Clear();
                //Line firstLine = _boundaryLines.First();
                //Line lastLine = _boundaryLines.Last();
                Line actualLastLine = _boundaryLines[boundaryLineOrderIndex.Last()];

                for (var index = 0; index < _boundaryLines.Count; index++) {
                    Line actualLine = _boundaryLines[boundaryLineOrderIndex[index]];
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

                    _boundaryPoints.Add(commonPoint);
                }

                return _boundaryPoints;
            }
            set { _boundaryPoints = value; }
        }
        //todo remove private field
        private List<Point> _boundaryPoints;

        public bool IsLineMissing() {
            return false;
        }

        public bool IsLineInRoom() {
            return false;
        }



    }
}