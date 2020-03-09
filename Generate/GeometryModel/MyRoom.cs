using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using WindowsFormsApp1.Utilities;
using Newtonsoft.Json;
using ONLAB2;

namespace WindowsFormsApp1 {
    public class MyRoom : IGeometry {
        public MyRoom(string name, string number, RoomType rt) {
            Name = name;
            Number = number;
            BoundaryPoints = new List<MyPoint>();
            BoundaryLines = new List<MyLine>();
            Guid = Guid.NewGuid();
            type = rt;
        }

        public static void ChangeAllParams(ref MyRoom keep, MyRoom getDataFrom) {
            keep.Number = getDataFrom.Number;
            keep.Name = getDataFrom.Name;
            keep.type = getDataFrom.type;

        }

        public int GetNumberAsInt()
        {
            int i = 0;
            bool asd = int.TryParse(Number, out i);
            return i;
        }
        internal MyRoom GetCopy() {
            return new MyRoom(Name, Number, type);
        }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        /// <summary>
        /// calculated actual degree from simulation
        /// </summary>
        /// 
        public int Degree { get; set; }
        public bool isStartRoom { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public List<MyRoom> NeighboorRooms = new List<MyRoom>();
        [JsonIgnore]
        [IgnoreDataMember]
        public string boundaryLineNames {
            get { return String.Join("\n", GetBoundaryLinesSorted().Select(i => i.ToString()).ToArray()); }
        }
        public RoomType type { get; set; }

        private bool isBoundaryLinesPossiblyUnsorted = false;
        private bool isBoundaryPointsPossiblyUnsorted = false;
        [JsonIgnore]
        [IgnoreDataMember]
        private List<MyLine> boundaryLines;
        [JsonIgnore]
        [IgnoreDataMember]
        public List<MyLine> BoundaryLines {
            get => boundaryLines;
            set {
                if (value == null || value.Count == 0) {
                    //Logger.WriteLog("someone set to null");
                }
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
            if (BoundaryLines.Count == 0) {
                Logger.WriteLog("boundary is null, this is bad");
            }
            int boundCount = BoundaryLines.Count;
            for (int i = 0; i < boundCount; i++) {
                if (actualIndex >= boundCount) {
                    continue;
                }
                MyLine actualMyLine = BoundaryLines.ElementAt(actualIndex);
                orderedLines.Add(actualMyLine);
                actualIndex = 0;
                foreach (MyLine line in BoundaryLines) {

                    if (orderedLines.Contains(line)) {
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
                if (actualIndex > boundCount) {
                    throw new Exception("LineOrderingFailed");
                }
            }

            if (orderedLines.Count == 0) {
                Logger.WriteLog("this is bad");
            }
            BoundaryLines = orderedLines;
            isBoundaryLinesPossiblyUnsorted = false;
        }
        [JsonIgnore]
        [IgnoreDataMember]
        public List<MyPoint> BoundaryPoints {
            get => boundaryPoints;
            set {
                boundaryPoints = value;
                isBoundaryPointsPossiblyUnsorted = true;
            }
        }

        public List<MyPoint> GetBoundaryPointsSorted() {
            if (isBoundaryPointsPossiblyUnsorted || true) {
                SortBoundaryPoints();
            }

            boundaryPoints.RemoveAll(item => item == null);

            if (boundaryPoints.Count == 0) {
                throw new Exception("Ordering not possible on no points");
            }
            return BoundaryPoints;
        }

        private void SortBoundaryPoints() {
            if (isBoundaryLinesPossiblyUnsorted || true) {
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

            //if (commonMyPoint==null)
            //{
            //    throw new Exception($"NoCommonPoint! between {firstMyLine} and {nextMyLine}");
            //}

            return commonMyPoint;
        }
        [JsonIgnore]
        [IgnoreDataMember]
        private List<MyPoint> boundaryPoints;
        public bool IsLineMissing() {
            return false;
        }
        public bool IsLineInRoom() {
            return false;
        }

        public double CalculateProportion() {
            double proportion = 0.0;
            try {
                List<MyPoint> bp = GetBoundaryPointsSorted();

                double[] X = bp.Select(i => i.X).ToArray();
                double[] Y = bp.Select(i => i.Y).ToArray();

                MyPoint max = new MyPoint(X.Max(), Y.Max());
                MyPoint min = new MyPoint(X.Min(), Y.Min());


                proportion = (max.X - min.X) / (max.Y - min.Y);
                if (proportion < 1 && Math.Abs(proportion) > 0.01) {
                    proportion = 1 / proportion;
                }
            }
            catch (Exception exception) {
                Logger.WriteLog(exception);
            }

            return proportion;
        }


        public double CalculateArea() {
            List<MyPoint> bp = GetBoundaryPointsSorted();

            if (bp.Count == 0) {
                throw new Exception("Area is null");
            }

            double[] X = bp.Select(i => i.X).ToArray();
            double[] Y = bp.Select(i => i.Y).ToArray();
            double area = PolygonArea(X, Y, bp.Count);

            if (area < 0.01) {
                //throw new Exception("Area is too small: " + area);
            }

            return area / 10000;

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

            // Calculate Value of shoelace formula 
            int j = n - 1;

            for (int i = 0; i < n; i++) {
                area += (X[j] + X[i]) * (Y[j] - Y[i]);

                // j is previous vertex to i 
                j = i;
            }

            // Return absolute Value 
            return Math.Abs(area / 2.0);
        }

        public override string ToString()
        {
            return Name+"_"+Number+"_"+ String.Join(Environment.NewLine,GetBoundaryLinesSorted().Select(i=>i.ToString()));
        }
    }
}