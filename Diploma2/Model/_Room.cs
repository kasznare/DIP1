﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Diploma2.Annotations;
using Diploma2.Utilities;
using GeoLib;

namespace Diploma2.Model {
    public class _Room : _GeometryBase, INotifyPropertyChanged {

        #region Constructors
        public _Room(List<_Line> newLines) {
            lines = newLines;
        }
        public _Room() {

        }
        #endregion

        #region Properties

        private List<_Point> Points = new List<_Point>();
        public List<_Line> Lines {
            get { return lines; }
            set {
                lines = value;
                OnPropertyChanged("Lines");
            }
        }

        private List<_Line> lines = new List<_Line>();
        public _RoomType type { get; set; }
        public bool isStartRoom { get; set; }
        
        public C2DPolygon Polygon { get; set; }
        public int Level { get; set; }

        #endregion
        public _Room DeepCopy() {
            List<_Line> newLines = new List<_Line>();
            foreach (_Line line in lines) {
                newLines.Add(line.DeepCopy());
            }

            _Room deepCopy = new _Room(newLines);
            deepCopy.Name = Name;
            deepCopy.Number = Number;
            deepCopy.type = type;
            deepCopy.Points = Points;
            return deepCopy;
        }
        public static void ChangeAllParams(ref _Room keep, _Room getDataFrom) {
            keep.Number = getDataFrom.Number;
            keep.Name = getDataFrom.Name;
            keep.type = getDataFrom.type;
            keep.Guid = getDataFrom.Guid;
        }

        public List<_Point> GetSortedBoundaryPoints() {
            SortPoints();
            return Points;
        }
        public List<_Point> GetPoints() {
            return Points;
        }

        /// <summary>
        /// this function tries to sort lines, but it can throw exception when it fails
        /// </summary>
        public void SortLines() {
            List<_Line> orderedLines = new List<_Line>();
            int actualIndex = 0;//the basis of sorting is to loop and keep this actualindex 
            int boundCount = Lines.Count;
            int nullLinesCount = 0;
            for (int i = 0; i < boundCount; i++) {
                _Line loopLine = Lines[actualIndex];
                if (loopLine.StartPoint.Equals(loopLine.EndPoint))
                {
                    nullLinesCount++;
                    continue; //we remove null lines this way
                }

                orderedLines.Add(loopLine);
                actualIndex = 0;
                for (var j = 0; j < boundCount; j++) {
                    _Line innerLoopLine = Lines[j];
                    if (orderedLines.Contains(innerLoopLine)) {
                        actualIndex++; //so skip this line
                        continue;
                    }
                    //from these next statments, only one can be true
                    if (Equals(innerLoopLine.StartPoint, loopLine.StartPoint) && !Equals(innerLoopLine.EndPoint, loopLine.EndPoint)) break;

                    if (Equals(innerLoopLine.StartPoint, loopLine.EndPoint) && !Equals(innerLoopLine.EndPoint, loopLine.StartPoint)) break;

                    if (Equals(innerLoopLine.EndPoint, loopLine.EndPoint) && !Equals(innerLoopLine.StartPoint, loopLine.StartPoint)) break;

                    if (Equals(innerLoopLine.EndPoint, loopLine.StartPoint) && !Equals(innerLoopLine.StartPoint, loopLine.EndPoint)) break;

                    actualIndex++;
                }
              
            }
            _Point p1 = orderedLines.First().ConnectsPoint(orderedLines.Last()); //this is where the first and last line joins
            _Point p2 = orderedLines.ElementAt(1).ConnectsPoint(orderedLines.ElementAt(0)); //this is where the first and second line joins
            _Point p3 = orderedLines.Last().ConnectsPoint(orderedLines.ElementAt(orderedLines.Count-2)); //this is where the last and the line before last joins
            //so this is the point where the first and second line connect
            //so p0 is where it all started. the last line should have p0
            _Point p0 = orderedLines.First().StartPoint.Equals(p2)
                ? orderedLines.First().EndPoint
                : orderedLines.First().StartPoint;

            //to get full circle
            _Point p4 = orderedLines.Last().StartPoint.Equals(p3)
                ? orderedLines.Last().EndPoint
                : orderedLines.Last().StartPoint;

            bool isGoodOrdering = p1 != null && p1.Equals(p0) && p1.Equals(p4);

            bool isGoodCount = (orderedLines.Count + nullLinesCount) == Lines.Count;

            if (!isGoodOrdering) {
                throw new Exception("first and last line does not connect");
            }

            if (!isGoodCount)
            {
                throw new Exception("not enough lines");
            }

            Lines = orderedLines; 
        }

        /// <summary>
        /// This calls sortlines, too
        /// and overrides Points
        /// </summary>
        public void SortPoints() {
            SortLines();
            Points = new List<_Point>();

            for (var index = 0; index < lines.Count; index++) {
                _Line firstMyLine = lines.ElementAt(index);
                _Line nextMyLine = lines.ElementAt((index + 1) % lines.Count);
                _Point commonMyPoint = FindCommonPointOnTwoLines(firstMyLine, nextMyLine);
                Points.Add(commonMyPoint);
            }
        }

        /// <summary>
        /// Tries to sort the lines, CAN NOT HANDLE LINES WITH SAME START+END
        /// </summary>
        /// <returns>If lines are sortable</returns>
        public bool CanGetBoundarySorted() {
            //TODO: sort only when nessesary
            try
            {
                SortPoints();
                return true;
            }
            //we have plenty of these, but they are excepcted
            catch (ArgumentOutOfRangeException a)
            {
                return false;
            }
            catch (Exception e) {
                
                Logger.WriteLog(e);
                  
                return false;
            }
        }

        //TODO: this seems redundant, line has the same function (connectspoint)
        private static _Point FindCommonPointOnTwoLines(_Line firstMyLine, _Line nextMyLine) {
            _Point commonMyPoint = null;
            if (Equals(firstMyLine.StartPoint, nextMyLine.StartPoint)) {
                commonMyPoint = nextMyLine.StartPoint;
            }
            if (Equals(firstMyLine.StartPoint, nextMyLine.EndPoint)) {
                commonMyPoint = nextMyLine.EndPoint;
            }
            if (Equals(firstMyLine.EndPoint, nextMyLine.StartPoint)) {
                commonMyPoint = nextMyLine.StartPoint;
            }
            if (Equals(firstMyLine.EndPoint, nextMyLine.EndPoint)) {
                commonMyPoint = nextMyLine.EndPoint;
            }

            if (commonMyPoint == null) {
                throw new Exception("no common point");
            }
            return commonMyPoint;
        }
        /// <summary>
        /// this value is in m2 so divide the coordinates with 10000
        /// </summary>
        public double Area { get; set; }

        public double CalculateArea() {

            List<_Point> bp = GetPoints();

            if (bp.Count == 0) {
                throw new Exception("Area is null, points should be sorted at this point");
            }

            double[] X = bp.Select(i => i.X).ToArray();
            double[] Y = bp.Select(i => i.Y).ToArray();
            double area = PolygonArea(X, Y, bp.Count);

            if (area < 0.01) {
                throw new Exception("Area is too small: " + area);
            }

            Area = area / 10000;
            OnPropertyChanged(nameof(Area));
            return area / 10000; //this is for unit conversion
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

        public double CalculateProportion() {
            double proportion = 0.0;
            try {
                List<_Point> bp = GetPoints();

                double[] X = bp.Select(i => i.X).ToArray();
                double[] Y = bp.Select(i => i.Y).ToArray();

                if (!X.Any() || !Y.Any()) {
                    throw new Exception("bad proportion");
                }
                _Point max = new _Point(X.Max(), Y.Max());
                _Point min = new _Point(X.Min(), Y.Min());


                //TODO: avoid <80cm room sides

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
        public double CalculateMinSideSize()
        {
            double proportion = 0.0;
            try
            {
                List<_Point> bp = GetPoints();

                double[] X = bp.Select(i => i.X).ToArray();
                double[] Y = bp.Select(i => i.Y).ToArray();

                if (!X.Any() || !Y.Any())
                {
                    throw new Exception("bad proportion");
                }

                double xdiff = X.Max() - X.Min();
                double ydiff = Y.Max() - Y.Min();

                return xdiff>ydiff ? ydiff : xdiff;
            }
            catch (Exception exception)
            {
                Logger.WriteLog(exception);
            }

            return proportion;

        }


        #region Basic operations and overrides
        public override string ToString() {
            return Name + "_" + Number + "_" + type;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion

       
    }
}
