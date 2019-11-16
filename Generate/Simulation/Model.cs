﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Utilities;
using ONLAB2;

//still todo
//TODO: branchelés - model operáció sorrend invariáns? akkor lehet őket osszevonogatni
//TODO: a hurok esetben nem kapunk jó eredményt
//TODO: volt egy darab pont duplikálva - még mindig. - lehet, hogy több vonal van a rendszerben
//TODO: switch nem működik jól
//TODO: make tests and handle failures - find step that fails and corerct there

//done
//kézi léptetést engedjen - intuitívabb lenne a kolstegfüggvény tervezése
//implementláni kell az új bool propertyt
//a listában kijelölt vonalnak látszódnia kéne az UI-on
//a léptetés negatív irányban nem működik jól
//közvetlen következő teendő az, hogy a mozgatás és a split kiírja a költségeket is
//kérdés, hogy hol történjen a mozgatás, most ugye az UI részen van
//költség: belső fal (több mint 1 fajta relatedroom)
//helyiségarány - befoglaló téglalappal
//kell-e az observablecollection - nem kell az odavissza hatás
//load model
//save model
//körüljárás alapján lehet megmondani, hogy melyik szobába kerüljün
//körüljárás jó a kirajzoláshoz és a karbantartáshoz is
//le kell kezelni minden módosítás során a szobák állapotváltozásait, ha nem jó a lépés, dobjuk el
//a lépés előtt lehetne tárolni az előző állapotot
//implement deepcopy for model class
//hogyan változik a loss
namespace WindowsFormsApp1 {
    public class Model {
        public Model(List<MyLine> lines = null, List<Room> rooms = null) {
            if (lines != null) {
                this.modelLines = new ObservableCollection<MyLine>(lines);
            }
            if (rooms != null) {
                modelRooms = new ObservableCollection<Room>(rooms);
            }
        }
        //TODO: set this in the move/split/switch 
        public bool IsInInvalidState { get; set; }
        public string Save() {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return jsonString;
        }

        public void Load(string jsonString) {
            Model obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Model>(jsonString);
            modelLines = obj.modelLines;
            modelRooms = obj.modelRooms;
        }

        public ObservableCollection<MyLine> modelLines { get; set; } = new ObservableCollection<MyLine>();
        public ObservableCollection<Room> modelRooms = new ObservableCollection<Room>();
        Random rand = new Random(10);

        public List<MyPoint> ModelPoints {
            get {
                List<MyPoint> starts = modelLines.Select(i => i.StartMyPoint).ToList();
                List<MyPoint> ends = modelLines.Select(i => i.EndMyPoint).ToList();
                starts.AddRange(ends);
                starts = starts.OrderBy(x => x.Guid).ToList();
                return starts;
            }
        }

        public void InitSimplestModel() {
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<Room>();
            MyPoint p1 = new MyPoint(100, 100);
            MyPoint p2 = new MyPoint(400, 100);
            MyPoint p3 = new MyPoint(400, 400);
            MyPoint p4 = new MyPoint(100, 400);
            MyLine line1 = new MyLine(p1, p2);
            modelLines.Add(line1);
            MyLine line2 = new MyLine(p2, p3);
            modelLines.Add(line2);
            MyLine line3 = new MyLine(p3, p4);
            modelLines.Add(line3);
            MyLine line4 = new MyLine(p4, p1);
            modelLines.Add(line4);

            Room first = new Room("FirstRoom", "1", RoomType.Kitchen);

            foreach (MyLine modelLine in new List<MyLine>() { line1, line2, line3, line4 }) {
                modelLine.relatedRooms.Add(first);
            }


            CalculateRooms();
            Logger.WriteLog("InitSimplestModel() finished");
        }

        /// <summary>
        /// create new model
        /// </summary>
        /// <returns></returns>
        public void InitSimpleModel() {
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<Room>();
            MyPoint q1 = new MyPoint(100, 0);
            MyPoint q2 = new MyPoint(400, 0);
            MyPoint p1 = new MyPoint(100, 100);
            MyPoint p2 = new MyPoint(400, 100);
            MyPoint p3 = new MyPoint(400, 400);
            MyPoint p4 = new MyPoint(100, 400);
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
            Logger.WriteLog("InitSimpleModel() finished");
        }
        public void InitNormalModel() {
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<Room>();
            MyPoint a1 = new MyPoint(0, 0);
            MyPoint a2 = new MyPoint(200, 0);
            MyPoint a3 = new MyPoint(200, 200);
            MyPoint a4 = new MyPoint(0, 200);

            MyPoint a5 = new MyPoint(200, 400);
            MyPoint a6 = new MyPoint(0, 400);

            MyPoint a7 = new MyPoint(400, 0);
            MyPoint a8 = new MyPoint(400, 200);

            MyPoint a9 = new MyPoint(400, 400);

            MyLine l12 = new MyLine(a1, a2);
            MyLine l23 = new MyLine(a2, a3);
            MyLine l34 = new MyLine(a3, a4);
            MyLine l41 = new MyLine(a4, a1);

            MyLine l35 = new MyLine(a3, a5);
            MyLine l56 = new MyLine(a5, a6);
            MyLine l64 = new MyLine(a6, a4);

            MyLine l27 = new MyLine(a2, a7);
            MyLine l78 = new MyLine(a7, a8);
            MyLine l83 = new MyLine(a8, a3);

            MyLine l89 = new MyLine(a8, a9);
            MyLine l95 = new MyLine(a9, a5);

            modelLines.Add(l41);
            modelLines.Add(l34);
            modelLines.Add(l23);
            modelLines.Add(l12);
            modelLines.Add(l35);
            modelLines.Add(l56);
            modelLines.Add(l64);
            modelLines.Add(l27);
            modelLines.Add(l78);
            modelLines.Add(l83);
            modelLines.Add(l89);
            modelLines.Add(l95);


            Room first = new Room("FirstRoom", "1", RoomType.Kitchen);
            Room second = new Room("SecondRoom", "2", RoomType.LivingRoom);
            Room third = new Room("ThirdRoom", "3", RoomType.BedRoom);
            Room fourth = new Room("FourthRoom", "4", RoomType.RestRoom);

            foreach (MyLine modelLine in new List<MyLine>() { l12, l23, l34, l41 }) {
                modelLine.relatedRooms.Add(first);
            }

            foreach (MyLine modelLine in new List<MyLine>() { l35, l56, l64, l34 }) {
                modelLine.relatedRooms.Add(second);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l23, l27, l78, l83 }) {
                modelLine.relatedRooms.Add(third);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l83, l89, l95, l35 }) {
                modelLine.relatedRooms.Add(fourth);
            }


            CalculateRooms();
            Logger.WriteLog("InitSimpleModel() finished");
        }

        public void InitSkewedModel() {
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<Room>();
            MyPoint a1 = new MyPoint(0, 0);
            MyPoint a2 = new MyPoint(300, 0);
            MyPoint a3 = new MyPoint(300, 300);
            MyPoint a4 = new MyPoint(0, 300);

            MyPoint a5 = new MyPoint(300, 400);
            MyPoint a6 = new MyPoint(0, 400);

            MyPoint a7 = new MyPoint(400, 0);
            MyPoint a8 = new MyPoint(400, 300);

            MyPoint a9 = new MyPoint(400, 400);

            MyLine l12 = new MyLine(a1, a2);
            MyLine l23 = new MyLine(a2, a3);
            MyLine l34 = new MyLine(a3, a4);
            MyLine l41 = new MyLine(a4, a1);

            MyLine l35 = new MyLine(a3, a5);
            MyLine l56 = new MyLine(a5, a6);
            MyLine l64 = new MyLine(a6, a4);

            MyLine l27 = new MyLine(a2, a7);
            MyLine l78 = new MyLine(a7, a8);
            MyLine l83 = new MyLine(a8, a3);

            MyLine l89 = new MyLine(a8, a9);
            MyLine l95 = new MyLine(a9, a5);

            modelLines.Add(l41);
            modelLines.Add(l34);
            modelLines.Add(l23);
            modelLines.Add(l12);
            modelLines.Add(l35);
            modelLines.Add(l56);
            modelLines.Add(l64);
            modelLines.Add(l27);
            modelLines.Add(l78);
            modelLines.Add(l83);
            modelLines.Add(l89);
            modelLines.Add(l95);


            Room first = new Room("FirstRoom", "1", RoomType.Kitchen);
            Room second = new Room("SecondRoom", "2", RoomType.LivingRoom);
            Room third = new Room("ThirdRoom", "3", RoomType.BedRoom);
            Room fourth = new Room("FourthRoom", "4", RoomType.RestRoom);

            foreach (MyLine modelLine in new List<MyLine>() { l12, l23, l34, l41 }) {
                modelLine.relatedRooms.Add(first);
            }

            foreach (MyLine modelLine in new List<MyLine>() { l35, l56, l64, l34 }) {
                modelLine.relatedRooms.Add(second);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l23, l27, l78, l83 }) {
                modelLine.relatedRooms.Add(third);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l83, l89, l95, l35 }) {
                modelLine.relatedRooms.Add(fourth);
            }


            CalculateRooms();
            Logger.WriteLog("InitSimpleModel() finished");
        }
        public Model DeepCopy() {
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

            return new Model(oldNewLines.Values.ToList(), oldNewRooms.Values.ToList());
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
        public Model DeepCopy(Room oldMyRoom1, Room oldMyRoom2, out Room newMyRoom1, out Room newMyRoom2) {
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

            foreach (KeyValuePair<Room, Room> oldNew in oldNewRooms)
            {
                Room oldRoom = oldNew.Key;
                Room newRoom = oldNew.Value;

                foreach (MyLine oldLine in oldRoom.BoundaryLines)
                {
                    //TODO: some line is already registered to the room
                    newRoom.BoundaryLines.Add(oldNewLines[oldLine]);
                    
                }
            }

            newMyRoom1 = oldNewRooms[oldMyRoom1];
            newMyRoom2 = oldNewRooms[oldMyRoom2];

            return new Model(oldNewLines.Values.ToList(), oldNewRooms.Values.ToList());
        }
        public MyLine GetRandomLine() {
            int randint = rand.Next(0, modelLines.Count);
            return modelLines.ElementAt(randint);
        }
        public void SplitEdge(int splitPercentage, MyLine selectedEdge) {
            if (modelLines.Count == 0) return;

            double length = selectedEdge.GetLength();
            if (length < 1) return;

            Logger.WriteLog("Selected: " + selectedEdge + " rooms: " + String.Join(",", selectedEdge.relatedRooms.Select(i => i.Name).ToArray()));
            List<Room> selectedEdgeRelatedRooms = selectedEdge.relatedRooms;
            modelLines.Remove(selectedEdge);
            selectedEdge.StartMyPoint.RelatedLines.Remove(selectedEdge);
            selectedEdge.EndMyPoint.RelatedLines.Remove(selectedEdge);

            MyPoint splitMyPoint = selectedEdge.GetPointAt(splitPercentage);
            MyLine a = new MyLine(selectedEdge.StartMyPoint, splitMyPoint);
            a.relatedRooms.AddRange(selectedEdgeRelatedRooms);
            modelLines.Add(a);
            Logger.WriteLog("Added instead: " + a + " rooms: " + String.Join(",", selectedEdge.relatedRooms.Select(i => i.Name).ToArray()));
            MyLine b = new MyLine(splitMyPoint, selectedEdge.EndMyPoint);
            b.relatedRooms.AddRange(selectedEdgeRelatedRooms);
            modelLines.Add(b);
            Logger.WriteLog("Added instead: " + b + " rooms: " + String.Join(",", selectedEdge.relatedRooms.Select(i => i.Name).ToArray()));

            foreach (Room edgeRelatedRoom in selectedEdgeRelatedRooms) {
                try {
                    edgeRelatedRoom.BoundaryLines.Remove(selectedEdge);
                    edgeRelatedRoom.BoundaryLines.Add(a);
                    edgeRelatedRoom.BoundaryLines.Add(b);
                }
                catch (Exception) {

                    Logger.WriteLog("Error: there is something wrong with the split. Check it out.");
                }
            }

            Logger.WriteLog($"Lines to that room {selectedEdge.relatedRooms.First()}: {String.Join(",", selectedEdge.relatedRooms.First().BoundaryLines.Select(i => i))}");
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
        public void SwitchRooms(ref Room room1, ref Room room2) {
            Room temp1 = room1.GetCopy();
            Room temp2 = room2.GetCopy();
            Room.ChangeAllParams(ref room1, temp2);
            Room.ChangeAllParams(ref room2, temp1);
            //make a constructor by another room, or preferably two rooms, and only copy the info of the second, and kepp the reference
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
                IsInInvalidState = true;
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
                modelRooms = new ObservableCollection<Room>();

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
            modelRooms = new ObservableCollection<Room>(allRooms);//modelRooms.Distinct().ToList();
            Logger.WriteLog(modelRooms.ToString());
            //TraceValues();
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

        public double[] CalculateCost() {
            double summary = 0.0;
            double areacost = 0.0;
            double layoutcost = 0.0;
            double constaintcost = 0.0;

            try {
                areacost = CalculateParameterCost();
                layoutcost = CalculateLayoutCost();
                constaintcost = CalculateConstraintCost();
                summary = areacost + layoutcost + constaintcost;
                Logger.WriteLog("területköltség: " + areacost);
                Logger.WriteLog("kerületköltség: " + layoutcost);
            }
            catch (Exception ex) {
                Logger.WriteLog("Error during cost calculation" + ex);
            }
            return new double[] { summary, areacost, layoutcost, constaintcost };
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
                        //TODO: this fails when switched with simulation
                        double actualarea = room.CalculateArea();

                        RoomType type = room.type;
                        if (actualarea < type.areamin) {
                            summary += Math.Pow(2, type.areamin - actualarea);
                        }
                        else if (actualarea > type.areamax) {
                            summary += Math.Pow(2, type.areamax - actualarea);
                        }

                        double actualprop = room.CalculateProportion();
                        if (actualprop > type.proportion) {
                            summary += Math.Pow(2, actualprop - type.proportion);
                        }

                        //punish more edges
                        double countCost = room.BoundaryPoints.Count;
                        if (countCost > 6)
                        {
                            summary += Math.Pow(2, countCost - 6);
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
            summary = Math.Round(summary, 2);
            return summary;
        }
        private double CalculateLayoutCost() {
            double wallLength = 0.0;
            foreach (MyLine seg in this.modelLines) {
                if (seg.relatedRooms.Count > 1) {
                    wallLength += Math.Sqrt((seg.GetLength() / 100)) / 10;
                }
                else {
                    wallLength += Math.Sqrt((seg.GetLength() / 100)) * 3;

                }
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
            summary = Math.Round(summary, 2);
            return summary;
        }

    }

}