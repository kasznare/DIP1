using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UIWPF.Model {
    public class _Room : _GeometryBase {
        public _Room(List<_Line> newLines) {
            lines = newLines;
        }

        public _Room() {

        }

        public List<_Line> lines { get; set; }

        public _Room DeepCopy() {
            List<_Line> newLines = new List<_Line>();
            foreach (_Line line in lines) {
                newLines.Add(line.DeepCopy());
            }
            return new _Room(newLines);
        }


        private List<_Point> Points;
        public List<_Point> GetBoundaryPointsSorted() {
            SortPoints();
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
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
    }
}
