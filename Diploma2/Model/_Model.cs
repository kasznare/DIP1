using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace UIWPF.Model {
    public class _Model {
        public ObservableCollection<_Room> modelStorage { get; set; }

        public _Model() {
            modelStorage = new ObservableCollection<_Room>();
        }

        public _Model(List<_Room> newRooms) {
            modelStorage = new ObservableCollection<_Room>(newRooms);
        }

        public _Model DeepCopy() {
            List<_Room> newRooms = new List<_Room>();
            foreach (_Room room in modelStorage) {
                newRooms.Add(room.DeepCopy());
            }

            _Model m = new _Model(newRooms);

            RemoveRedundancy(m);
            return m;
        }

        /// <summary>
        /// this is extremely inefficient, avoid this if possible
        /// supposedly complete, should time it
        /// </summary>
        /// <param name="m"></param>
        private void RemoveRedundancy(_Model m) {
            ObservableCollection<_Room> rooms = m.modelStorage;
            //List<List<_Line>> lines = m.AllLines();
            //List<List<List<_Point>>> points = m.AllPoints();
            List<_Point> uniquePoints = new List<_Point>();
            List<_Line> uniqueLines = new List<_Line>();
            //clean points first
            //TODO: might need to override equals for the contains filter
            foreach (_Room room in rooms) {
                foreach (_Line line in room.lines) {
                    if (uniquePoints.Contains(line.StartPoint)) {
                        line.StartPoint = uniquePoints.Find(i => i.XY == line.StartPoint.XY);
                    }
                    else {
                        uniquePoints.Add(line.StartPoint);
                    }
                    if (uniquePoints.Contains(line.EndPoint)) {
                        line.EndPoint = uniquePoints.Find(i => i.XY == line.EndPoint.XY);
                    }
                    else {
                        uniquePoints.Add(line.EndPoint);
                    }
                }
            }
            //TODO: might need to override equals for the contains filter
            //clean lines
            foreach (_Room room in rooms) {
                foreach (_Line line in room.lines) {
                    if (uniqueLines.Contains(line)) {
                        room.lines.Remove(line);
                        _Line @where = uniqueLines.Where(i => i.IsTheSame(line)) as _Line;
                        room.lines.Add(@where);
                    }
                    else {
                        uniqueLines.Add(line);
                    }
                }
            }

        }

        public List<List<List<_Point>>> AllPoints() {
            List<List<List<_Point>>> asd = modelStorage
                .Select(i => i.lines.Select(j => new List<_Point>() { j.StartPoint, j.EndPoint }).ToList()).ToList();
            return asd;
        }

        public List<List<_Line>> AllLines() {
            List<List<_Line>> asd = modelStorage.Select(i => i.lines).ToList();
            return asd;
        }
        public List<_Line> AllLinesFlat() {
            List<_Line> lines = new List<_Line>();
            foreach (_Room room in modelStorage)
            {
                lines.AddRange(room.lines);
            }
            return lines;
        }

        public List<_Point> AllPointsFlat() {
            List<_Point> points = new List<_Point>();
            foreach (_Room room in modelStorage) {
                foreach (_Line line in room.lines)
                {
                    points.Add(line.StartPoint);
                    points.Add(line.EndPoint);
                }
            }
            return points;
        }

        public void MoveLine(int distance, _Line lineToMove) {
            List<_Room> roomsThatCare = new List<_Room>(); //these rooms might need to change
            roomsThatCare = modelStorage.Where(i => i.lines.Contains(lineToMove)).ToList();
            if (!roomsThatCare.Any()) throw new Exception("LineIsMissing");

            _Line movedLine = lineToMove.DeepCopy();
            _Point moveVector = new _Point(10, 10);
            movedLine.Move(moveVector);

            _Line l1 = new _Line(lineToMove.StartPoint, lineToMove.StartPoint.Move(moveVector));
            _Line l2 = new _Line(lineToMove.EndPoint, lineToMove.EndPoint.Move(moveVector));

            // the lines are movedline, l1, l2
            foreach (_Room room in modelStorage) {
                foreach (_Line line in room.lines) {
                    if (line.IsTheSame(l1)) l1 = line;
                    if (line.IsTheSame(l2)) l2 = line;
                }
            }

            foreach (_Room room in roomsThatCare) {
                room.lines.Remove(lineToMove);
                foreach (_Line getLine in room.lines) {
                    _Point p = getLine.ConnectsPoint(lineToMove);
                    if (p != null && p == lineToMove.StartPoint) room.lines.Add(l1);
                    if (p != null && p == lineToMove.EndPoint) room.lines.Add(l2);

                }
                List<_Line> l = room.lines.Where(i => i.Connects(lineToMove)).ToList();

                room.lines.Add(movedLine); //this detached the model
            }


            //lets start with if the line already existed fully
            
            

            //at this point we only need to solve
            //1. same line already existed before - fully or partially
            //2. new point was not needed

            //we still need to find if there are any rooms just touching the line.


        }
    }
}
