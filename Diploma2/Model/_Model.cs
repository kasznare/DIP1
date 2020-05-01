using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Diploma2.Model {
    public class _Model {
        public List<Action> actionHistory { get; set; }
        public ObservableCollection<_Room> rooms { get; set; }
        public object loadedModelType { get; internal set; }
        public bool IsInInvalidState { get; internal set; }

        public _Model() {
            rooms = new ObservableCollection<_Room>();
        }
        public _Model(List<_Room> newRooms) {
            rooms = new ObservableCollection<_Room>(newRooms);
        }


        [JsonIgnore]
        [IgnoreDataMember]
        private Dictionary<_Room, _Room> oldNewRooms = new Dictionary<_Room, _Room>();
        [JsonIgnore]
        [IgnoreDataMember]
        private Dictionary<_Point, _Point> oldNewPoints = new Dictionary<_Point, _Point>();
        [JsonIgnore]
        [IgnoreDataMember]
        private Dictionary<_Line, _Line> oldNewLines = new Dictionary<_Line, _Line>();


        public _Model DeepCopy(bool isTagNeeded = false) {
            if (isTagNeeded) {
                oldNewLines.Clear();
                oldNewPoints.Clear();
                oldNewRooms.Clear();
            }
            List<_Room> newRooms = new List<_Room>();
            foreach (_Room room in rooms) {
                _Room deepCopy = room.DeepCopy();

                if (isTagNeeded) {
                    bool isroomthere = oldNewRooms.ContainsKey(deepCopy);
                    if (!isroomthere) {
                        oldNewRooms.Add(room, deepCopy);
                    }

                    for (var index = 0; index < room.Lines.Count; index++) {
                        _Line i = room.Lines[index];
                        _Line iCopy = deepCopy.Lines[index];
                        bool ifs = oldNewLines.ContainsKey(i);
                        if (!ifs) {
                            oldNewLines.Add(i, iCopy);
                        }
                    }
                }

                //this storage type is duplicate this way
                newRooms.Add(deepCopy);
            }

            _Model m = new _Model(newRooms);
            //RemoveRedundancy(m);

            return m;
        }
        public _Model DeepCopy(_Line oldMyLine, out _Line newMyLine) {

            _Model copy = this.DeepCopy(true);
            //then only need to find the needed line

            newMyLine = oldNewLines[oldMyLine];

            if (newMyLine.StartPoint == null || newMyLine.EndPoint == null) {
                throw new Exception("bad");
            }
            return copy;
        }
        //READY
        public _Model DeepCopy(_Room oldMyRoom1, _Room oldMyRoom2, out _Room newMyRoom1, out _Room newMyRoom2) {
            _Model copy = this.DeepCopy(true);
            newMyRoom1 = oldNewRooms[oldMyRoom1];
            newMyRoom2 = oldNewRooms[oldMyRoom2];
            return copy;
        }

        public _Model DeepCopy(_Room oldMyRoom1, out _Room newMyRoom1) {

            _Model copy = this.DeepCopy(true);

            newMyRoom1 = oldNewRooms[oldMyRoom1];
            return copy;

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
                        //if (oldNewLines.ContainsKey(line))
                        //{

                        //}
                        //else
                        {
                            room.Lines.Remove(line);
                            _Line @where = uniqueLines.FirstOrDefault(i => i.IsTheSame(line)) as _Line;
                            if (@where == null || @where.Number == -1) {
                                room.Lines.Add(line);
                            }
                            else {
                                room.Lines.Add(@where);

                            }
                        }

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
            foreach (_Room room in rooms) {
                lines.AddRange(room.Lines);
            }
            return lines;
        }

        public List<_Point> AllPointsFlat() {
            List<_Point> points = new List<_Point>();
            foreach (_Room room in rooms) {
                foreach (_Line line in room.Lines) {
                    points.Add(line.StartPoint);
                    points.Add(line.EndPoint);
                }
            }
            return points;
        }

        private int moveStepsCount = 0;

        public void MoveLine(int distance, _Line lineToMove) {
            if (lineToMove.length < 11) {
                return;
            }
            List<_Room> roomsTouchingStartPoint = new List<_Room>(); //the rooms need to have the line to care
            List<_Room> roomsTouchingEndPoint = new List<_Room>(); //if there are, we cant move the point, we need to copy
            List<_Room> roomsContainingTheLineToMove = new List<_Room>();                           //these rooms might need to change

            _Line movedLine = lineToMove.DeepCopy();                                 //first we copy the line we need to move
            movedLine.Name = $"Moved_in_step:{moveStepsCount}";                              //this is for debugging
            _Point moveVector = movedLine.GetNV(true) * distance;                //we scale it up
            movedLine.Move(moveVector);                                              //so we moved the copy (why not the actual?)

            _Line l1 = new _Line(lineToMove.StartPoint, lineToMove.StartPoint.Move(moveVector));
            _Line l2 = new _Line(lineToMove.EndPoint, lineToMove.EndPoint.Move(moveVector));
            l1.Name = $"New_Start_Line_{moveStepsCount}";
            l2.Name = $"New_End_Line_{moveStepsCount}";

            fillRelatedRoomListInfomration(lineToMove, ref l1, ref l2, ref roomsTouchingStartPoint, ref roomsTouchingEndPoint, ref roomsContainingTheLineToMove);

            //this is the big function
            foreach (_Room room in roomsContainingTheLineToMove) {
                room.Lines.Remove(lineToMove);
                int linesCount = room.Lines.Count;
                for (var index = 0; index < linesCount; index++) {
                    _Line lineInLoop = room.Lines[index];
                    _Point loopAndToMoveCommonPoint = lineInLoop.ConnectsPoint(lineToMove);
                    if (loopAndToMoveCommonPoint != null && loopAndToMoveCommonPoint.Equals(lineToMove.StartPoint)) //there is common point with startpoint, so this line touched the old startpoint
                    {
                        bool areweMovingOnLoopLine = IsOnLine(l1.EndPoint, lineInLoop);
                        //if there is a touching room, we need to keep the point. of course, might not in this room.
                        if (!roomsTouchingStartPoint.Any()) {
                            _Point loopNormal = lineInLoop.GetNV(true); //we need to either move it, if it is parallel, or keep it if it is merőleges
                            _Point moveNormal = lineToMove.GetNV(true); //THE LINE SHOULD MOVE - BOTH DIRECTIONS - MOVE P WITH MOVEVECTOR

                            bool NOTparallel = !(Equals(loopNormal, moveNormal) || Equals(loopNormal , moveNormal * -1) || Equals(loopNormal * -1, moveNormal));

                            if (lineInLoop.StartPoint.Equals(loopAndToMoveCommonPoint) && (NOTparallel)) {
                                lineInLoop.StartPoint = lineInLoop.StartPoint.Move(moveVector);
                            }

                            if (lineInLoop.EndPoint.Equals(loopAndToMoveCommonPoint) && (NOTparallel)) {
                                lineInLoop.EndPoint = lineInLoop.EndPoint.Move(moveVector);
                            }

                            if (!NOTparallel)
                            {
                                room.Lines.Add(l1);
                            }
                        }
                        if (roomsTouchingStartPoint.Any()) {
                            if (!areweMovingOnLoopLine && !room.Lines.Contains(l1)) room.Lines.Add(l1);  //this makes it easier to remove the small lines later
                            foreach (_Room room1 in roomsTouchingStartPoint) {
                                if (!room1.Lines.Contains(l1)) room1.Lines.Add(l1);
                            }
                        }
                    }

                    if (loopAndToMoveCommonPoint != null && loopAndToMoveCommonPoint.Equals(lineToMove.EndPoint)) {
                        bool areweMovingOnLoopLine = _Model.IsOnLine(l2.EndPoint, lineInLoop);
                        if (!roomsTouchingEndPoint.Any()) {
                            _Point objA = lineInLoop.GetNV(true);
                            _Point objB = lineToMove.GetNV(true);
                            bool NOTparallel = !(Equals(objA, objB) || Equals(objA, objB * -1) || Equals(objA * -1, objB));
                            if (lineInLoop.StartPoint.Equals(loopAndToMoveCommonPoint) && (NOTparallel)) {
                                lineInLoop.StartPoint = lineInLoop.StartPoint.Move(moveVector);
                            }

                            if (lineInLoop.EndPoint.Equals(loopAndToMoveCommonPoint) && (NOTparallel)) {
                                lineInLoop.EndPoint = lineInLoop.EndPoint.Move(moveVector);
                            }
                            if (!NOTparallel) {
                                room.Lines.Add(l2);
                            }
                        }
                        if (roomsTouchingEndPoint.Any()) {
                            if (!areweMovingOnLoopLine && !room.Lines.Contains(l2)) room.Lines.Add(l2);
                            foreach (_Room room2 in roomsTouchingEndPoint) {
                                if (!room2.Lines.Contains(l2)) room2.Lines.Add(l2);
                            }
                        }
                    }
                }

                room.Lines.Add(movedLine);
            }

           
            foreach (_Room room in roomsContainingTheLineToMove) {

                TryToDivideRoomLinesWithL1L2(room, l1, l2);
                bool shouldBeTrue = room.CanGetBoundarySorted();
            }

            foreach (_Room room in roomsTouchingEndPoint) {
                TryToDivideRoomLinesWithL1L2(room, l1, l2);
                TryToRemoveRemainderLines(room, l1, l2);
                bool shouldBeTrue = room.CanGetBoundarySorted();
            }

            foreach (_Room room in roomsTouchingStartPoint) {
                TryToDivideRoomLinesWithL1L2(room, l1, l2);
                TryToRemoveRemainderLines(room, l1, l2);
                bool shouldBeTrue = room.CanGetBoundarySorted();
            }

            
            //this handles null lines, can be removed at any time, probably should do it before sorting
            foreach (_Room room in rooms) {
                for (var index = 0; index < room.Lines.Count; index++) {
                    _Line roomLine = room.Lines[index];
                    if (roomLine.StartPoint.Equals(roomLine.EndPoint)) {
                        room.Lines.Remove(roomLine);
                    }
                }
            }
            GC.Collect();
         

            moveStepsCount++;
        }

        private void TryToDivideRoomLinesWithL1L2(_Room room, _Line l1, _Line l2) {
            bool isComplete = room.CanGetBoundarySorted(); //this might be bad
            if (!isComplete) {
                for (int i = 0; i < room.Lines.Count; i++) {
                    _Line chosenLine = room.Lines.ElementAt(i);

                    if (chosenLine.Equals(l1) || chosenLine.Equals(l2)) continue;

                    var connectsPoint = chosenLine.ConnectsPoint(l1);
                    if (connectsPoint != null) {
                        _Point otherPoint = l1.EndPoint.Equals(connectsPoint) ? l1.StartPoint : l1.EndPoint;
                        if (IsOnLine(otherPoint, chosenLine)) {
                            if (chosenLine.EndPoint.Equals(connectsPoint)) {
                                chosenLine.EndPoint = otherPoint;
                            }
                            else {
                                chosenLine.StartPoint = otherPoint;
                            }
                        }
                    }

                    connectsPoint = chosenLine.ConnectsPoint(l2);
                    if (connectsPoint != null) {
                        _Point otherPoint = l2.EndPoint.Equals(connectsPoint) ? l2.StartPoint : l2.EndPoint;
                        if (IsOnLine(otherPoint, chosenLine)) {
                            if (chosenLine.EndPoint.Equals(connectsPoint)) {
                                chosenLine.EndPoint = otherPoint;
                            }
                            else {
                                chosenLine.StartPoint = otherPoint;
                            }
                        }
                    }
                }
            }
            isComplete = room.CanGetBoundarySorted(); //we need to check it after the split

        }

        private void TryToRemoveRemainderLines(_Room room, _Line l1, _Line l2) {
            bool isComplete;
            try {
                if (room.Lines.Contains(l1)) {

                    room.Lines.Remove(l1);
                    isComplete = room.CanGetBoundarySorted();
                    if (!isComplete) room.Lines.Add(l1);
                }
            }
            catch (Exception e) {
            }
            try {
                if (room.Lines.Contains(l2)) {
                    room.Lines.Remove(l2);
                    isComplete = room.CanGetBoundarySorted();
                    if (!isComplete) room.Lines.Add(l2);

                }
            }
            catch (Exception e) {
            }

            try {
                if (room.Lines.Contains(l1) && room.Lines.Contains(l2)) {
                    room.Lines.Remove(l1);
                    room.Lines.Remove(l2);
                    isComplete = room.CanGetBoundarySorted();
                    if (!isComplete) room.Lines.Add(l1);
                    if (!isComplete) room.Lines.Add(l2);

                }
            }
            catch (Exception e) {
            }
        }

        private void fillRelatedRoomListInfomration(_Line lineToMove, ref _Line l1, ref _Line l2, ref List<_Room> roomsTouchingStartPoint,
            ref List<_Room> roomsTouchingEndPoint, ref List<_Room> roomsContainingTheLineToMove) {
            foreach (_Room room in rooms) {
                // the lines are movedline, l1, l2 already existed, then find them
                foreach (_Line line in room.Lines) {
                    if (line.IsTheSame(l1)) l1 = line;
                    if (line.IsTheSame(l2)) l2 = line;

                    if (line.Equals(lineToMove) && !roomsContainingTheLineToMove.Contains(room)) {
                        roomsContainingTheLineToMove.Add(room); //  roomsContainingTheLineToMove = rooms.Where(i => i.Lines.Contains(lineToMove)).ToList();
                    }
                    if ((line.StartPoint.Equals(lineToMove.StartPoint) ||
                         line.EndPoint.Equals(lineToMove.StartPoint)) &&
                        !line.Equals(lineToMove) &&
                        !roomsTouchingStartPoint.Contains(room) &&
                        !roomsContainingTheLineToMove.Contains(room)) {
                        roomsTouchingStartPoint.Add(room); //this might cause redundancy
                    }

                    if ((line.StartPoint.Equals(lineToMove.EndPoint) ||
                         line.EndPoint.Equals(lineToMove.EndPoint)) &&
                        !line.Equals(lineToMove) &&
                        !roomsTouchingEndPoint.Contains(room) &&
                        !roomsContainingTheLineToMove.Contains(room)) {
                        roomsTouchingEndPoint.Add(room); //this might cause redundancy
                    }

                }
            }

            foreach (_Room room in roomsContainingTheLineToMove) {
                if (roomsTouchingStartPoint.Contains(room)) {
                    roomsTouchingStartPoint.Remove(room);
                }

                if (roomsTouchingEndPoint.Contains(room)) {
                    roomsTouchingEndPoint.Remove(room);
                }
            }
            if (!roomsContainingTheLineToMove.Any())
                throw new Exception("LineIsMissing"); //if there are no rooms, inconsistent state
        }

        //TODO: implement
        public void SplitLine() {
            SplitLine(0.50, rooms.First().Lines.First());
        }
        //TODO: implement
        public void SplitLine(double percentage, _Line lineToSplit) {
        }

        internal void MoveLine() {

            MoveLine(10, rooms.First().Lines.First());
        }

        public void SwitchRooms(ref _Room room1, ref _Room room2) {
            _Room temp1 = room1.DeepCopy();
            _Room temp2 = room2.DeepCopy();
            _Room.ChangeAllParams(ref room1, temp2);
            _Room.ChangeAllParams(ref room2, temp1);
        }

        public static bool IsOnLine(_Point myPoint, _Line myLine) {
            return PointOnLine2D(myPoint, myLine.StartPoint, myLine.EndPoint);
        }
        public static bool PointOnLine2D(_Point p, _Point a, _Point b, float t = 1E-03f) {
            // ensure points are collinear
            var zero = (b.X - a.X) * (p.Y - a.Y) - (p.X - a.X) * (b.Y - a.Y);
            if (zero > t || zero < -t) return false;

            // check if X-coordinates are not equal
            if (a.X - b.X > t || b.X - a.X > t)
                // ensure X is between a.X & b.X (use tolerance)
                return a.X > b.X
                    ? p.X + t > b.X && p.X - t < a.X
                    : p.X + t > a.X && p.X - t < b.X;

            // ensure Y is between a.Y & b.Y (use tolerance)
            return a.Y > b.Y
                ? p.Y + t > b.Y && p.Y - t < a.Y
                : p.Y + t > a.Y && p.Y - t < b.Y;
        }
    }
}
