using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Diploma2.Annotations;
using Diploma2.Utilities;

namespace Diploma2.Model {
    public class _Room : _GeometryBase, INotifyPropertyChanged {
        public _Room(List<_Line> newLines) {
            lines = newLines;
        }

        public override string ToString()
        {
            return Name+"_"+Number.ToString();
        }

        public _Room() {

        }

        public List<_Line> Lines
        {
            get { return lines; }
            set
            {
                lines = value; 
                OnPropertyChanged("Lines");
            }
        }

        public _Room DeepCopy() {
            List<_Line> newLines = new List<_Line>();
            foreach (_Line line in lines) {
                newLines.Add(line.DeepCopy());
            }

            _Room deepCopy = new _Room(newLines);
            deepCopy.Name = Name;
            deepCopy.Number = Number;
            deepCopy.type = type;
            return deepCopy;
        }


        private List<_Point> Points = new List<_Point>();
        private List<_Line> lines = new List<_Line>();
        public _RoomType type { get; set; }
        public bool isStartRoom { get; internal set; }

        public List<_Point> GetBoundaryPointsSorted() {
            SortPoints();
            return Points;
        }
        public List<_Point> GetPoints() {
            return Points;
        }
        private void SortLines() {
            List<_Line> orderedLines = new List<_Line>();

            int actualIndex = 0;
            int boundCount = lines.Count;
            for (int i = 0; i < boundCount; i++) {
                if (actualIndex >= boundCount) {
                    continue;
                }
                _Line actualLine = lines.ElementAt(actualIndex);
                orderedLines.Add(actualLine);
                actualIndex = 0;
                foreach (_Line line in lines) {

                    if (orderedLines.Contains(line)) {
                        actualIndex++;
                        continue;
                    }
                    _Point p1 = line.StartPoint;
                    _Point p2 = line.EndPoint;
                    _Point p3 = actualLine.StartPoint;
                    _Point p4 = actualLine.EndPoint;

                    if (Equals(line.StartPoint, actualLine.StartPoint) && !Equals(line.EndPoint, actualLine.EndPoint)) {
                        break;
                    }
                    if (Equals(line.StartPoint, actualLine.EndPoint) && !Equals(line.EndPoint, actualLine.StartPoint)) {
                        break;
                    }
                    if (Equals(line.EndPoint, actualLine.EndPoint) && !Equals(line.StartPoint, actualLine.StartPoint)) {
                        break;
                    }
                    if (Equals(line.EndPoint, actualLine.StartPoint) && !Equals(line.StartPoint, actualLine.EndPoint)) {
                        break;
                    }

                    actualIndex++;
                }
                if (actualIndex > boundCount) {
                    throw new Exception("LineOrderingFailed");
                }
            }

            if (orderedLines.Count < lines.Count)
            {
                throw new Exception("ordering failed");
            }

            lines = orderedLines;
        }
        private void SortPoints() {
            SortLines();
            Points = new List<_Point>();

            for (var index = 0; index < lines.Count; index++) {
                _Line firstMyLine = lines.ElementAt(index);
                _Line nextMyLine = lines.ElementAt((index + 1) % lines.Count);
                _Point commonMyPoint = FindCommonPointOnTwoLines(firstMyLine, nextMyLine);
                Points.Add(commonMyPoint);
            }

        }

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

        public bool CanGetBoundarySorted() {
            try {
                GetBoundaryPointsSorted();

                List<_Line> linesAdded = new List<_Line>();
                for (var index = 0; index < Lines.Count; index++)
                {
                    _Line line = Lines[index];
                    for (var i = 0; i < Points.Count; i++)
                    {
                        _Point point = Points[i];
                        if (_Model.IsOnLine(point,line) && !Equals(point, line.StartPoint) && !Equals(point, line.EndPoint) )
                        {
                            //this means there is redundancy
                            //this point should be a splitter on the line, that contains it.
                            lines.Remove(line);
                            _Line l1 = new _Line(point, line.StartPoint) {Name = line.Name, Guid = line.Guid, Number = line.Number};
                            _Line l2 = new _Line(point, line.EndPoint) { Name = line.Name, Guid = line.Guid, Number = line.Number };
                            lines.Add(l1);
                            lines.Add(l2);

                            linesAdded.Add(l1);
                            linesAdded.Add(l2);
                            //then we hope, that redundancy is removed
                            break; //this might be bad

                        }
                    }
                }

                for (var index = 0; index < Lines.Count; index++)
                {
                    _Line line = Lines[index];
                    foreach (_Line line1 in linesAdded)
                    {
                        if (line.IsTheSame(line1))
                        {
                            Lines.Remove(line);
                            lines.Remove(line1);
                        }

                    }
                }
                //check that there is no overlap
                //if one of the points is on an other line, and not at an endpoint, there is overlap
                
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double CalculateArea()
        {
            List<_Point> bp = GetBoundaryPointsSorted();

            if (bp.Count == 0) {
                throw new Exception("Area is null");
            }

            double[] X = bp.Select(i => i.X).ToArray();
            double[] Y = bp.Select(i => i.Y).ToArray();
            double area = PolygonArea(X, Y, bp.Count);

            if (area < 0.01) {
                throw new Exception("Area is too small: " + area);
            }

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

                if (!X.Any() || !Y.Any())
                {
                    throw new Exception("bad");
                }
                _Point max = new _Point(X.Max(), Y.Max());
                _Point min = new _Point(X.Min(), Y.Min());


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

        public static void ChangeAllParams(ref _Room keep, _Room getDataFrom)
        {
            keep.Number = getDataFrom.Number;
            keep.Name = getDataFrom.Name;
            keep.type = getDataFrom.type;
        }
    }
}
