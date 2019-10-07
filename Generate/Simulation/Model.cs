using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Utilities;
using ONLAB2;

//TODO: körüljárás alapján lehet megmondani, hogy melyik szobába kerüljün
//TODO: körüljárás jó a kirajzoláshoz és a karbantartáshoz is
//le kell kezelni minden módosítás során a szobák állapotváltozásait, ha nem jó a lépés, dobjuk el
//a lépés előtt lehetne tárolni az előző állapotot
//TODO: implement deepcopy for model class
//TODO: hogyan változik a loss
namespace WindowsFormsApp1 {
    public class Model {
        public Model(List<MyLine> lines = null, List<Room> rooms = null) {
            if (lines != null) {
                this.modelLines = lines;
            }
            if (rooms != null) {
                modelRooms = rooms;
            }
        }
        public List<MyLine> modelLines = new List<MyLine>();
        public List<Room> modelRooms = new List<Room>();
        Random rand = new Random(10);

        //TODO: load model
        //TODO: save model

        public List<MyPoint> ModelPoints => modelLines.Select(i => i.StartMyPoint).ToList();

        /// <summary>
        /// create new model
        /// </summary>
        /// <returns></returns>
        public void InitModel() {

            MyPoint q1 = new MyPoint(100, 0);
            MyPoint q2 = new MyPoint(500, 0);
            MyPoint p1 = new MyPoint(100, 100);
            MyPoint p2 = new MyPoint(500, 100);
            MyPoint p3 = new MyPoint(500, 500);
            MyPoint p4 = new MyPoint(100, 500);
            MyLine line1 = new MyLine(p1, p2);
            modelLines.Add(line1);
            MyLine line2 = new MyLine(p2, p3);
            modelLines.Add(line2);
            MyLine line3 = new MyLine(p3, p4);
            modelLines.Add(line3);
            MyLine line4 = new MyLine(p4, p1);
            modelLines.Add(line4);
            MyLine l1 = new MyLine(q1, p1);
            MyLine l2 = new MyLine(q2, p2);
            MyLine l3 = new MyLine(q1, q2);
            modelLines.Add(l1);
            modelLines.Add(l2);
            modelLines.Add(l3);

            Room first = new Room("FirstRoom", "1", RoomType.Kitchen);
            Room second = new Room("SecondRoom", "2", RoomType.LivingRoom);

            foreach (MyLine modelLine in new List<MyLine>() { line1, line2, line3, line4 }) {
                modelLine.relatedRooms.Add(first);
            }

            foreach (MyLine modelLine in new List<MyLine>() { l1, l2, l3, line1 }) {
                modelLine.relatedRooms.Add(second);
            }

            CalculateRooms();
            Logger.WriteLog("InitModel() finished");
        }

        public void InitRoomTypes() {
            //typeid room name entrance    privacy area min area max visual connection
            //1	living room yes	1	12	50	yes
            //2	kitchen no	2	10	40	yes
            //3	restroom no	2	2	10	no
            //4	bedroom no	4	10	20	yes
            roomTypes.Add(RoomType.LivingRoom);
            roomTypes.Add(RoomType.Kitchen);
            roomTypes.Add(RoomType.BedRoom);
            roomTypes.Add(RoomType.RestRoom);
        }


        public Model DeepCopy(MyLine oldMyLine, out MyLine newMyLine) {
            Dictionary<Room, Room> oldNewRooms = new Dictionary<Room, Room>();
            Dictionary<MyPoint, MyPoint> oldNewPoints = new Dictionary<MyPoint, MyPoint>();
            Dictionary<MyLine, MyLine> oldNewLines = new Dictionary<MyLine, MyLine>();


            foreach (MyLine line in modelLines) {
                MyPoint p1 = null;
                MyPoint p2 = null;

                if (!oldNewPoints.TryGetValue(line.StartMyPoint, out p1)) {
                    p1 = line.StartMyPoint.GetCopy();
                    oldNewPoints.Add(line.StartMyPoint, p1);
                }

                if (!oldNewPoints.TryGetValue(line.EndMyPoint, out p2)) {
                    p2 = line.EndMyPoint.GetCopy();
                    oldNewPoints.Add(line.EndMyPoint, p2);
                }
                MyLine l = new MyLine(p1, p2);
                oldNewLines.Add(line, l);

                foreach (Room room in line.relatedRooms) {
                    Room r = null;
                    if (!oldNewRooms.TryGetValue(room, out r)) {
                        r = room.GetCopy();
                        oldNewRooms.Add(room, r);
                    }
                    l.relatedRooms.Add(r);
                }
            }

            newMyLine = oldNewLines[oldMyLine];

            return new Model(oldNewLines.Values.ToList(), oldNewRooms.Values.ToList());
        }
        public MyLine GetRandomLine() {
            int randint = rand.Next(0, modelLines.Count);
            return modelLines.ElementAt(randint);
        }
        //public List<MyLine> ModelLines
        //{
        //    get { return modelLines; }
        //    set { modelLines = value; }
        //}
        //public List<Room> ModelRooms
        //{
        //    get { return modelRooms; }
        //    set { modelRooms = value; }
        //}
        public void SplitEdge(int splitPercentage, MyLine selectedEdge) {
            if (modelLines.Count == 0) return;

            double length = selectedEdge.GetLength();
            if (length < 1) return;

            Logger.WriteLog("Selected: "+selectedEdge+ " rooms: " + String.Join(",",selectedEdge.relatedRooms.Select(i=> i.Name).ToArray()));
            List<Room> selectedEdgeRelatedRooms = selectedEdge.relatedRooms;
            modelLines.Remove(selectedEdge);
            selectedEdge.StartMyPoint.RelatedLines.Remove(selectedEdge);
            selectedEdge.EndMyPoint.RelatedLines.Remove(selectedEdge);

            MyPoint splitMyPoint = selectedEdge.GetPointAt(splitPercentage);
            MyLine a = new MyLine(selectedEdge.StartMyPoint, splitMyPoint);
            a.relatedRooms.AddRange(selectedEdgeRelatedRooms);
            modelLines.Add(a);
            Logger.WriteLog("Added instead: "+a+ " rooms: " + String.Join(",",selectedEdge.relatedRooms.Select(i=> i.Name).ToArray()));
            MyLine b = new MyLine(splitMyPoint, selectedEdge.EndMyPoint);
            b.relatedRooms.AddRange(selectedEdgeRelatedRooms);
            modelLines.Add(b);
            Logger.WriteLog("Added instead: "+b+ " rooms: " + String.Join(",",selectedEdge.relatedRooms.Select(i=> i.Name).ToArray()));

            foreach (Room edgeRelatedRoom in selectedEdgeRelatedRooms)
            {
                try {
                    edgeRelatedRoom.BoundaryLines.Remove(selectedEdge);
                    edgeRelatedRoom.BoundaryLines.Add(a);
                    edgeRelatedRoom.BoundaryLines.Add(b);
                }
                catch (Exception) {

                    Logger.WriteLog("Error: there is something wrong with the split. Check it out.");
                }
            }
            
            Logger.WriteLog($"Lines to that room {selectedEdge.relatedRooms.First()}: {String.Join(",",selectedEdge.relatedRooms.First().BoundaryLines.Select(i=>i))}");
        }
        public void HandleOpening(MyLine l) {
            if (l.relatedRooms.Count == 1) return; //külső fal

            ReachableRooms();

        }
        //hierarchikus koltsegfuggveny
        /// <summary>
        /// calculate degree for all rooms starting from the starter room
        /// </summary>
        /// <returns></returns>
        public int ReachableRooms() {
            return 0;
        }
        public void MoveLine(int offsetDistance, MyLine myLineToMove) {
            try {
                MyPoint p1 = myLineToMove.StartMyPoint;
                MyPoint lineToMoveNormal = myLineToMove.GetNV(true);
                #region MyRegion

                MyPoint p3 = p1 + lineToMoveNormal * offsetDistance;
                MyPoint p2 = myLineToMove.EndMyPoint;
                MyPoint p4 = p2 + lineToMoveNormal * offsetDistance;

                #region exception checking
                foreach (MyLine relatedLine in p1.RelatedLines) {
                    if (!relatedLine.Equals(myLineToMove) && (!relatedLine.GetNV(true).Equals(lineToMoveNormal)
                         || !relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                        if (relatedLine.GetLength() < offsetDistance) {
                            throw new Exception("Vonalhossz hiba: " + relatedLine.GetLength());
                        }
                    }
                }
                foreach (MyLine relatedLine in p2.RelatedLines) {
                    if (!relatedLine.Equals(myLineToMove) && (!relatedLine.GetNV(true).Equals(lineToMoveNormal)
                                                      || !relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                        if (relatedLine.GetLength() < offsetDistance) {
                            throw new Exception("Hiba");
                        }
                    }
                }
                #endregion
                bool copyp1 = false;
                MyLine parallelMyLine = null;

                foreach (MyLine relatedLine in p1.RelatedLines) {
                    //ha van olyan vonal, ami miatt másolni kell:
                    if (!relatedLine.Equals(myLineToMove) && (relatedLine.GetNV(true).Equals(lineToMoveNormal)
                                                      || relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                        copyp1 = true;
                        parallelMyLine = relatedLine;
                        break;
                    }
                }
                if (!copyp1) //then relocate, this is easy
                {
                    p1.X = p3.X;
                    p1.Y = p3.Y;
                    //itt szoba nem változik
                }
                else {
                    MyLine myLineInMoveDirection = null;
                    foreach (MyLine relatedLine in p1.RelatedLines) {
                        //ha a mozgatás irányába van vonal
                        bool on = IsOnLine(p3, relatedLine);
                        if (on) {
                            //nem kéne itt copy-t készíteni?
                            myLineInMoveDirection = relatedLine;
                            break;
                        }
                    }
                    if (myLineInMoveDirection != null) {
                        if (myLineInMoveDirection.StartMyPoint.Equals(p1)) {
                            myLineInMoveDirection.StartMyPoint = p3;

                        }
                        else {
                            myLineInMoveDirection.EndMyPoint = p3;
                        }
                        p3.RelatedLines.Add(myLineInMoveDirection);
                        p1.RelatedLines.Remove(myLineInMoveDirection);
                    }

                    myLineToMove.StartMyPoint = p3;
                    p3.RelatedLines.Add(myLineToMove);
                    p1.RelatedLines.Remove(myLineToMove);

                    List<Room> p1Rooms = p1.RelatedRooms;
                    List<Room> p3Rooms = p3.RelatedRooms;

                    List<Room> commonRooms = p1Rooms.Intersect(p3Rooms).ToList();

                    MyLine newConnectionEdge1 = new MyLine(p1, p3);
                    //TODO: add related modelRooms to this new myLine.
                    newConnectionEdge1.relatedRooms = commonRooms;

                    modelLines.Add(newConnectionEdge1);
                }
                #endregion

                #region MyRegion
                bool copyp2 = false;
                MyLine parallelLine2 = null;
                foreach (MyLine relatedLine in p2.RelatedLines) {
                    if (!relatedLine.Equals(myLineToMove) &&
                        (relatedLine.GetNV(true).Equals(lineToMoveNormal) || relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                        copyp2 = true;
                        parallelLine2 = relatedLine;
                        break;
                    }
                }
                if (!copyp2) {
                    p2.X = p4.X;
                    p2.Y = p4.Y;
                }
                else {
                    MyLine myLineInMoveDirection = null;
                    foreach (MyLine relatedLine in p2.RelatedLines) {
                        bool on = IsOnLine(p4, relatedLine);
                        if (on) {
                            myLineInMoveDirection = relatedLine;
                            break;
                        }
                    }
                    if (myLineInMoveDirection != null) {
                        if (myLineInMoveDirection.StartMyPoint.Equals(p2)) {
                            myLineInMoveDirection.StartMyPoint = p4;
                        }
                        else {
                            myLineInMoveDirection.EndMyPoint = p4;
                        }
                        p4.RelatedLines.Add(myLineInMoveDirection);
                        p2.RelatedLines.Remove(myLineInMoveDirection);
                    }

                    myLineToMove.EndMyPoint = p4;

                    p4.RelatedLines.Add(myLineToMove);
                    p2.RelatedLines.Remove(myLineToMove);

                    List<Room> p2Rooms = p2.RelatedRooms;
                    List<Room> p4Rooms = p4.RelatedRooms;

                    List<Room> commonRooms = p2Rooms.Intersect(p4Rooms).ToList();

                    MyLine newConnectionEdge2 = new MyLine(p2, p4);
                    newConnectionEdge2.relatedRooms = commonRooms;
                    modelLines.Add(newConnectionEdge2);
                }

                #endregion
            }
            catch (Exception e) {
                Logger.WriteLog("Not legal move " + e.Message);
                Logger.WriteLog(e);
                //MessageBox.Show();
            }


            List<MyLine> toremove = new List<MyLine>();
            foreach (MyLine line1 in modelLines) {
                if (line1.StartMyPoint.Equals(line1.EndMyPoint) || Math.Abs(line1.GetLength()) < 0.01) {
                    toremove.Add(line1);

                    foreach (MyLine endLine in line1.EndMyPoint.RelatedLines) {
                        if (endLine != line1) {
                            line1.StartMyPoint.RelatedLines.Add(endLine);
                            if (endLine.StartMyPoint == line1.EndMyPoint) {
                                endLine.StartMyPoint = line1.StartMyPoint;

                            }
                            else if (endLine.EndMyPoint == line1.EndMyPoint) {
                                endLine.EndMyPoint = line1.StartMyPoint;
                            }
                        }
                    }

                    line1.StartMyPoint.RelatedLines.Remove(line1);
                    line1.EndMyPoint.RelatedLines.Clear();
                }
            }

            foreach (var line2 in toremove) {
                modelLines.Remove(line2);
            }
            CalculateRooms();
        }
        //TODO: this could be calculated for only the lines, that actually changed, this is huge resource waste
        public void CalculateRooms() {
            if (modelRooms == null)
                modelRooms = new List<Room>();

            modelRooms.Clear();
            List<Room> allRooms = new List<Room>();
            foreach (MyLine line in modelLines) {
                //minden modellinera megnézzük, hogy
                foreach (Room room in line.relatedRooms) {
                    if (!allRooms.Contains(room)) {
                        allRooms.Add(room);
                    }
                    //annak a hozzá tartozó relatedroomjaiban
                    //a room boundarylinejai kozott szerepel-e a myLine
                    if (!room.BoundaryLines.Contains(line)) {
                        room.BoundaryLines.Add(line);
                        //Logger.WriteLog($"CalculateRooms for myLine {myLine} {room.Name} ");
                    }
                }
                // modelRooms.AddRange(myLine.relatedRooms);
            }
            modelRooms = allRooms;//modelRooms.Distinct().ToList();
            Logger.WriteLog(modelRooms.ToString());
            //TraceValues();
        }
        void TraceValues() {
            foreach (Room room in modelRooms)
                Logger.WriteLog(room.Name);
        }
        private void RunRedundancyCheck(MyPoint p1, MyPoint p2) {
            #region test
            //List<MyLine> conflictList = new List<MyLine>();
            //foreach (MyLine edge in ModelLines)
            //{
            //    if (edge.startMyPoint.Equals(p1) ||
            //        edge.startMyPoint.Equals(p2) ||
            //        edge.endMyPoint.Equals(p2) ||
            //        edge.endMyPoint.Equals(p1))
            //    {
            //        conflictList.Add(edge);
            //    }
            //} 
            #endregion
            List<MyLine> toRemoveLines = new List<MyLine>();
            List<MyLine> toAddLines = new List<MyLine>();
            List<MyPoint> toRemovePoints = new List<MyPoint>();

            #region test
            //foreach (MyLine line1 in conflictList)
            //{
            //    foreach (MyLine line2 in conflictList)
            //    {
            //        bool lineEQ = line1.Equals(line2);
            //        bool sameDir = line1.GetNV(true).Equals(line2.GetNV(true));
            //        bool oppDir = (line1.GetNV(true) * (-1)).Equals(line2.GetNV(true));
            //        if (!lineEQ && sameDir || oppDir)
            //        {

            //            if (line1.startMyPoint.Equals(line2.startMyPoint) && !line1.endMyPoint.Equals(line2.endMyPoint))
            //            {
            //                MyLine goodline = new MyLine(line1.endMyPoint, line2.endMyPoint);
            //                toAddLines.Add(goodline);
            //                if (!toRemoveLines.Contains(line1))
            //                {
            //                    toRemoveLines.Add(line1);

            //                }
            //                if (!toRemoveLines.Contains(line2))
            //                {
            //                    toRemoveLines.Add(line2);

            //                }

            //                if (!toRemovePoints.Contains(line1.startMyPoint))
            //                {
            //                    toRemovePoints.Add(line1.startMyPoint);

            //                }
            //            }
            //            if (line1.startMyPoint.Equals(line2.endMyPoint) && !line1.endMyPoint.Equals(line2.startMyPoint))
            //            {
            //                MyLine goodline = new MyLine(line1.endMyPoint, line2.startMyPoint);
            //                toAddLines.Add(goodline);
            //                toRemoveLines.Add(line1);
            //                toRemoveLines.Add(line2);
            //                toRemovePoints.Add(line1.startMyPoint);
            //            }
            //            if (line1.endMyPoint.Equals(line2.startMyPoint) && !line1.startMyPoint.Equals(line2.endMyPoint))
            //            {
            //                MyLine goodline = new MyLine(line1.startMyPoint, line2.endMyPoint);
            //                toAddLines.Add(goodline);
            //                toRemoveLines.Add(line1);
            //                toRemoveLines.Add(line2);
            //                toRemovePoints.Add(line1.endMyPoint);
            //            }
            //            if (line1.endMyPoint.Equals(line2.startMyPoint) && !line1.startMyPoint.Equals(line2.endMyPoint))
            //            {
            //                MyLine goodline = new MyLine(line1.startMyPoint, line2.endMyPoint);
            //                toAddLines.Add(goodline);
            //                toRemoveLines.Add(line1);
            //                toRemoveLines.Add(line2);
            //                toRemovePoints.Add(line1.endMyPoint);
            //            }
            //        }
            //    }
            //} 
            #endregion


            List<List<MyLine>> results = new List<List<MyLine>>();
            toAddLines.Clear();
            toRemoveLines.Clear();
            for (var index = 0; index < modelLines.Count; index++) {
                MyLine line1 = modelLines[index];
                for (var i = index + 1; i < modelLines.Count; i++) {
                    MyLine line2 = modelLines[i];
                    bool isLineEQ = (line1 == line2);
                    if (!isLineEQ) {
                        results = CalculateContaining(line1, line2);
                        toAddLines.AddRange(results.First());
                        toRemoveLines.AddRange(results.Last());
                    }
                }
            }

            MessageBox.Show("ToAddLines: " + WriteOutList(toAddLines));
            MessageBox.Show("ToRemoveLines: " + WriteOutList(toRemoveLines));

            foreach (var goodline in toAddLines) {
                try {
                    modelLines.Add(goodline);
                }
                catch (Exception) {
                }
            }
            foreach (var badline in toRemoveLines) {
                try {
                    modelLines.Remove(badline);
                }
                catch (Exception) {
                }
            }
            //delete duplicate points
            //join points if possible
        }
        public static string WriteOutList<T>(List<T> list) {
            string output = String.Empty;

            foreach (T listItem in list) {
                MyLine item = listItem as MyLine;

                output += String.Concat(item.StartMyPoint.X + "," + item.StartMyPoint.Y + "  " +
                                        item.EndMyPoint.X + "," + item.EndMyPoint.Y, Environment.NewLine);
            }

            return output;
        }
        private List<List<MyLine>> CalculateContaining(MyLine line1, MyLine line2) {

            List<List<MyLine>> results = new List<List<MyLine>>();
            List<MyLine> addLines = new List<MyLine>();
            List<MyLine> remLines = new List<MyLine>();
            bool isSameDir = line1.GetNV(true).Equals(line2.GetNV(true));
            bool isOppDir = (line1.GetNV(true) * (-1)).Equals(line2.GetNV(true));
            if (isOppDir || isSameDir) {
                bool line1IncludesLine2 = LineIncludes(line1, line2);
                if (line1IncludesLine2) {
                    remLines.Add(line2);
                }

                bool line2IncludesLine1 = LineIncludes(line2, line1);
                if (line2IncludesLine1) {
                    remLines.Add(line1);
                }

                if (!line2IncludesLine1 && !line1IncludesLine2) {
                    MyLine newMyLine = LineHaveCommonPoint(line1, line2);

                    if (newMyLine != null) {
                        remLines.Add(line1);
                        remLines.Add(line2);
                        addLines.Add(newMyLine);
                    }
                }
            }
            results.Add(addLines);
            results.Add(remLines);

            return results;
        }
        //todo: include partial overlapping
        private bool LineIncludes(MyLine line1, MyLine line2) {
            if (IsOnLine(line2.StartMyPoint, line1) && IsOnLine(line2.EndMyPoint, line1)) {
                return true;
            }
            return false;
        }
        private bool IsOnLine(MyPoint myPoint, MyLine myLine) {
            return PointOnLine2D(myPoint, myLine.StartMyPoint, myLine.EndMyPoint);
        }
        public static bool PointOnLine2D(MyPoint p, MyPoint a, MyPoint b, float t = 1E-03f) {
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
        private MyLine LineHaveCommonPoint(MyLine line1, MyLine line2) {
            MyPoint s1 = line1.StartMyPoint;
            MyPoint s2 = line2.StartMyPoint;
            MyPoint e1 = line1.EndMyPoint;
            MyPoint e2 = line2.EndMyPoint;
            MyLine newMyLine = null;
            if (s1.Equals(s2)) {
                newMyLine = new MyLine(e1, e2);
            }
            else if (e1.Equals(e2)) {
                newMyLine = new MyLine(s1, s2);
            }
            else if (e1.Equals(s2)) {
                newMyLine = new MyLine(e2, s1);
            }
            else if (s1.Equals(e2)) {
                newMyLine = new MyLine(e1, s2);
            }

            return newMyLine;
        }
        public void SwitchRoom() {

        }
        public double CalculateCost() {
            double summary = 0.0;
            try {
                double areacost = CalculateParameterCost();
                double layoutcost = CalculateLayoutCost();
                double constaintcost = CalculateConstraintCost();
                summary = areacost + layoutcost + constaintcost;
                Logger.WriteLog("területköltség: " + areacost);
                Logger.WriteLog("kerületköltség: " + layoutcost);
            }
            catch (Exception ex) {
                Logger.WriteLog("Error during cost calculation" + ex);
            }
            return summary;
        }

        private double CalculateConstraintCost() {
            //muszáj teljesülne
            return 0.0;
        }
        private double CalculateParameterCost() {
            double summary = 0.0;
            try {
                foreach (Room room in this.modelRooms) {
                    try {
                        //string roomTypeId = room.Number;

                        double actualarea = room.CalculateArea();

                        RoomType type = room.type;
                        if (actualarea < type.areamin) {
                            summary += Math.Abs(type.areamin - actualarea);
                        }
                        else if (actualarea > type.areamax) {
                            summary += Math.Abs(type.areamax - actualarea);
                        }
                    }
                    catch (Exception e) {
                        Logger.WriteLog(e);
                    }
                }
            }
            catch (Exception e) {
                summary = 10;
                Logger.WriteLog("outer exc" + e);
            }
            //elemszintű megfelelés
            //minden helyiségre
            //megkeresni a helyiség kategóriát a táblázatból
            //számítani a megfelelést
            //összegezni

            return summary;
        }

        public List<RoomType> roomTypes { get; set; }

        private double CalculateLayoutCost() {
            double wallLength = 0.0;


            foreach (MyLine seg in this.modelLines) {
                wallLength += Math.Sqrt(seg.GetLength() * 3);
            }
            //Utils.WriteLog("Walllength: " + wallLength);
            //elrendezésszintű
            double passagewaycost = 0.0;

            //ajtókat, nyílásokat letenni...(kérdés)
            //bejárhatóság generálás
            double privacygradientcost = 0.0;


            //kerületszámítás
            //minimális optimum kerület = sqrt(minden szoba area összege)*4
            double summary = passagewaycost + privacygradientcost + wallLength;
            return summary;
        }

    }

}