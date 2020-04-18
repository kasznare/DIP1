using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Diploma2.Model {
    public class _Model {
        public ObservableCollection<_Room> rooms { get; set; }

        public _Model() {
            rooms = new ObservableCollection<_Room>();
        }

        public _Model(List<_Room> newRooms) {
            rooms = new ObservableCollection<_Room>(newRooms);
        }

        public _Model DeepCopy() {
            List<_Room> newRooms = new List<_Room>();
            foreach (_Room room in rooms) {
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
            ObservableCollection<_Room> rooms = m.rooms;
            //List<List<_Line>> lines = m.AllLines();
            //List<List<List<_Point>>> points = m.AllPoints();
            List<_Point> uniquePoints = new List<_Point>();
            List<_Line> uniqueLines = new List<_Line>();
            //clean points first
            //TODO: might need to override equals for the contains filter
            foreach (_Room room in rooms) {
                foreach (_Line line in room.Lines) {
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
                foreach (_Line line in room.Lines) {
                    if (uniqueLines.Contains(line)) {
                        room.Lines.Remove(line);
                        _Line @where = uniqueLines.FirstOrDefault(i => i.IsTheSame(line)) as _Line;
                        room.Lines.Add(@where);
                    }
                    else {
                        uniqueLines.Add(line);
                    }
                }
            }

        }

        public List<List<List<_Point>>> AllPoints() {
            List<List<List<_Point>>> asd = rooms
                .Select(i => i.Lines.Select(j => new List<_Point>() { j.StartPoint, j.EndPoint }).ToList()).ToList();
            return asd;
        }

        public List<List<_Line>> AllLines() {
            List<List<_Line>> asd = rooms.Select(i => i.Lines).ToList();
            return asd;
        }
        public List<_Line> AllLinesFlat() {
            List<_Line> lines = new List<_Line>();
            foreach (_Room room in rooms)
            {
                lines.AddRange(room.Lines);
            }
            return lines;
        }

        public List<_Point> AllPointsFlat() {
            List<_Point> points = new List<_Point>();
            foreach (_Room room in rooms) {
                foreach (_Line line in room.Lines)
                {
                    points.Add(line.StartPoint);
                    points.Add(line.EndPoint);
                }
            }
            return points;
        }

        private int moveStepsCount = 0;

        public void MoveLine(int distance, _Line lineToMove) {
            List<_Room> roomsThatCare = new List<_Room>();                           //these rooms might need to change
            roomsThatCare = rooms.Where(i => i.Lines.Contains(lineToMove)).ToList(); //the rooms need to have the line to care
            if (!roomsThatCare.Any()) throw new Exception("LineIsMissing");   //if there are no rooms, inconsistent state

            _Line movedLine = lineToMove.DeepCopy();                                 //first we copy the line we need to move
            movedLine.Name = $"Moved_{moveStepsCount}";                              //this is for debugging
            _Point moveVector = new _Point(0, 10);                               //this was before we knew the normal
            moveVector = movedLine.GetNV(true) * distance;                //we scale it up
            movedLine.Move(moveVector);                                              //so we moved the copy (why not the actual?)

            _Line l1 = new _Line(lineToMove.StartPoint, lineToMove.StartPoint.Move(moveVector)); 
            l1.Name = $"NewSmallStart_{moveStepsCount}";
            _Line l2 = new _Line(lineToMove.EndPoint, lineToMove.EndPoint.Move(moveVector));
            l2.Name = $"NewSmallEnd_{moveStepsCount}";

           
            foreach (_Room room in rooms) {                                          // the lines are movedline, l1, l2 already existed, then find them
                foreach (_Line line in room.Lines) {
                    if (line.IsTheSame(l1)) l1 = line;
                    if (line.IsTheSame(l2)) l2 = line;
                }
            }

            foreach (_Room room in roomsThatCare) {
                room.Lines.Remove(lineToMove);
                foreach (_Line getLine in room.Lines.ToList()) {                      //is tolist legal? is this same object references?
                    _Point p = getLine.ConnectsPoint(lineToMove);

                    _Point objA = getLine.GetNV(true);
                    _Point objB = lineToMove.GetNV(true);
                    if (p != null && p.Equals(lineToMove.StartPoint))                 //there is common point with startpoint, so this line touched the old startpoint
                    {                                                                 //we need to either move it, if it is parallel, or keep it if it is merőleges
                        //THE LINE SHOULD MOVE - BOTH DIRECTIONS - MOVE P WITH MOVEVECTOR
                        
                        bool parallel = Equals(objA, objB);
                        bool inverseParallel = Equals(objA * -1, objB);
                        if (getLine.StartPoint.Equals(p)&&!(parallel || inverseParallel))//párhuzamos)
                        {
                            getLine.StartPoint=getLine.StartPoint.Move(moveVector);
                        }
                        if (getLine.EndPoint.Equals(p) && !(parallel || inverseParallel))
                        {
                            getLine.EndPoint= getLine.EndPoint.Move(moveVector);
                        }
                        
                        
                    }

                    if (p != null && p.Equals(lineToMove.EndPoint))
                    {
                        bool parallel = Equals(objA, objB);
                        bool inverseParallel = Equals(objA * -1, objB);
                        if (getLine.StartPoint.Equals(p) && !(parallel || inverseParallel)) {
                            getLine.StartPoint = getLine.StartPoint.Move(moveVector);
                        }

                        if (getLine.EndPoint.Equals(p) && !(parallel || inverseParallel)) {
                            getLine.EndPoint = getLine.EndPoint.Move(moveVector);
                        }
                    }

                }
                //List<_Line> l = room.lines.Where(i => i.Connects(lineToMove)).ToList();

                room.Lines.Add(movedLine); //this detached the model
            }

            //MIGHT BE UNNESSESARY
            _Line l5 = null;                                                             //lets start with if the line already existed fully
            foreach (_Line line in AllLinesFlat())
            {
                if (line.Guid!= movedLine.Guid && line.IsTheSame(movedLine))
                {
                    l5 = line;
                    break;                                                               //TODO: here we could allow multiple
                }
            }

            foreach (_Room room in rooms)
            {
                if (room.Lines.Contains(l5))
                {
                    room.Lines.Remove(l5);                                                  //Should we handle points?
                    room.Lines.Add(movedLine);
                }
            }

            //ROOMS FILLED WITH L1 OR L2 IF BOUNDARYLINES ORDERING THROW EXCEPTION
            //ROOMS MIGHT NEED TO REMOVE PART OF L1 -- BOTTOM LEFT
            //szobák jatítsák ki magukat - l1 vagy l2 melyikkel lenne teljes

            foreach (_Room room in rooms)
            {
                bool isComplete = room.CanGetBoundarySorted();
                if (!isComplete)
                {
                    room.Lines.Add(l1);
                    isComplete = room.CanGetBoundarySorted(); //the rooms might be with overlapping lines at this point, we need to handle that
                    if (!isComplete)
                    {
                        room.Lines.Remove(l1);
                        room.Lines.Add(l2);
                        isComplete = room.CanGetBoundarySorted();
                        if (!isComplete)
                        {
                            room.Lines.Add(l1);
                            isComplete = room.CanGetBoundarySorted();
                            if (!isComplete)
                            {
                                room.Lines.Remove(l2);
                                room.Lines.Remove(l1);
                            }
                            //throw new Exception("room cannot be fixed in move step");
                        }
                    }
                }
            }


            GC.Collect();
            

            //at this point we only need to solve
            //1. same line already existed before - fully or partially
            //2. new point was not needed


            //we need to handle created lines with no rooms



            //we still need to find if there are any rooms just touching the line.

            moveStepsCount++;
        }

        //TODO: implement
        public void SplitLine()
        {
            SplitLine(0.50, rooms.First().Lines.First());
        }
        public void SplitLine(double percentage, _Line lineToSplit)
        {

        }
        //TODO: implement
        public void SwitchRoom()
        {

        }


        internal void MoveLine() {
            MoveLine(10,rooms.First().Lines.First());
        }
    }
}
