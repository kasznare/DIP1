
#region plan

//kezdeti cost - így mennyire jó
//ezután menjünk végig az összes lépésen, és nézzük meg, hogy melyik lépésnél csökkent leginkább a költség
//elég azt eltárolni, ami akutálisan a legkisebb
//ezután azt hajtsuk végre
//ez az iteráció...

#endregion

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils = RevitTools.Util.Util;
using System.Windows.Forms;
using Autodesk.Revit.DB.Architecture;
using RevitTools.UsageControl;

namespace RevitTools.Util {
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]

    public class _testCommand : IExternalCommand {
        private Document _document;
        private List<RoomType> roomTypes = new List<RoomType>();
        private List<Element> rooms;
        List<Wall> walls = new List<Wall>();
        List<Wall> boundaryWalls = new List<Wall>();
        private Room outside;
        string path = @"c:\temp\test.csv";
        List<BoundarySegment> boundaryLines = new List<BoundarySegment>();
        Random random = new Random();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            #region init
            _document = commandData.Application.ActiveUIDocument.Document;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Utils.WriteLog("Document: " + _document.PathName + " Username: " + Environment.UserName + " Command: Test");
            #endregion

            rooms = new FilteredElementCollector(_document).OfCategory(BuiltInCategory.OST_Rooms).Where(e => e.Name != "Outside").ToList();
            outside = new FilteredElementCollector(_document).OfCategory(BuiltInCategory.OST_Rooms).Where(e => Utils.GetParameterValueByName(e, "Name") == "Outside").Cast<Room>().First();
            walls = new FilteredElementCollector(_document).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Cast<Wall>().ToList();

            walls = walls.Where(i => !i.Name.Contains("Finishing")).ToList();
            SpatialElementBoundaryOptions asd = new SpatialElementBoundaryOptions();
            CostForm costform = new CostForm();
            costform.Show();
            //Application.Run();

            ParseFileToRoomType(path);

            FillRoomTypeFromTypeList(roomTypes);

            bool run = true;
            int iteration = 0;
            int nocostdifferencecounter = 0;

            double currentMinCost = 100000000;
            double actualWallMoveCost;
            double actualRoomSwitchCost;
            double previousCost;
            double costdifference;

            Random rand = new Random();

            Room moveroom1 = (Room)rooms.First();
            Room moveroom2 = (Room)rooms.First();
            Wall movewall = walls.First();

            while (run) {
                iteration++;
                Utils.WriteLog("iteration: " + iteration);
                previousCost = currentMinCost;

                int num2 = rand.Next(0, 20);
                double flipper = Math.Pow(-1, num2);

                try {
                    boundaryLines = outside.GetBoundarySegments(asd)[1].ToList();
                }
                catch (Exception) {
                }
                boundaryLines.AddRange(outside.GetBoundarySegments(asd)[0].ToList());


                actualRoomSwitchCost = 100000;
                foreach (Room room1 in rooms) {
                    foreach (Room room2 in rooms) {
                        if (!room1.Equals(room2)) {
                            SwitchRoomData(room1, room2);
                            double localcost = CalculateCost();
                            Utils.WriteLog("Move room cost: " + localcost);
                            if (localcost < actualRoomSwitchCost) {
                                ActionType action = ActionType.switchrooms;
                                moveroom1 = room1;
                                moveroom2 = room2;
                                actualRoomSwitchCost = localcost;
                            }
                            else {
                                SwitchRoomData(room2, room1);
                            }
                            //redo the transaction
                        }
                    }


                }

                actualWallMoveCost = 100000;
                foreach (Wall wall in walls) {
                    MoveWall(wall, flipper);
                    double localcost = CalculateCost();
                    Utils.WriteLog("Move wall cost: " + localcost);
                    if (localcost < actualWallMoveCost) {
                        ActionType action = ActionType.switchrooms;
                        movewall = wall;
                        actualWallMoveCost = localcost;
                    }
                    else {
                        MoveWall(wall, -flipper);
                    }
                }

                if (actualRoomSwitchCost <= actualWallMoveCost) {
                    SwitchRoomData(moveroom1, moveroom2);
                    currentMinCost = actualRoomSwitchCost;
                }
                else {
                    MoveWall(movewall, flipper);
                    currentMinCost = actualWallMoveCost;
                }


                costdifference = previousCost - currentMinCost;
                if (Math.Abs(costdifference) < 0.01) {
                    nocostdifferencecounter++;
                }

                if (nocostdifferencecounter > 5) {
                    //MessageBox.Show("Nochange");
                    break;
                }
                #region old

                //DialogResult dialogResult = DialogResult.OK;

                //dialogResult =
                //    MessageBox.Show("Yes -> room No -> wall", "Details", MessageBoxButtons.YesNoCancel);

                //tegyük meg az adott lépést
                //switch (dialogResult)
                //{
                //    case DialogResult.Yes:
                //        {
                //            SwitchRooms();
                //            break;
                //        }
                //    case DialogResult.No:
                //        {
                //            MoveWall();
                //            break;
                //        }
                //    case DialogResult.Cancel:
                //        {
                //            run = false;
                //            break;
                //        }
                //    default:
                //        {
                //            break;
                //        }
                //} 
                #endregion

                Utils.WriteLog("Current cost: " + currentMinCost);

                if (stopwatch.ElapsedMilliseconds > 6000) {
                    Utils.WriteLog("Timeout");
                    break;
                }

                //_document.Regenerate();

            }

            stopwatch.Stop();
            Utils.WriteLog("Move command done in " + stopwatch.ElapsedMilliseconds + " ms.");
            Utils.UploadLogs();
            return Result.Succeeded;
        }

        private void FillRoomTypeFromTypeList(List<RoomType> roomTypes) {
            //List<Element> rooms = new FilteredElementCollector(_document)
            //    .OfCategory(BuiltInCategory.OST_Rooms).Where(e => e.Name != "Kültér")
            //    .ToList();
            foreach (Room element in rooms) {
                string name = Utils.GetParameterValueByName(element, "Name");
                foreach (RoomType type in roomTypes) {
                    if (type.roomname.ToLower().Equals(name.ToLower())) {
                        Utils.SetParameterValueByName(element, "Comments", type.typeid);
                    }
                }
            }

        }

        public void ParseFileToRoomType(string path) {
            //olvassuk be a fájlt, és menjünk végig soronként
            //hozzunk létre új roomtype objektumot
            //using (TextFieldParser parser = new TextFieldParser(path))
            //{
            //    parser.TextFieldType = FieldType.Delimited;
            //    parser.SetDelimiters(";");
            //    while (!parser.EndOfData)
            //    {
            //        //Process row
            //        string[] row = parser.ReadFields();
            //        //RoomType a = new RoomType();
            //        if (parser.LineNumber == 0)
            //        {
            //            continue; ;
            //        }

            //        string header = "typeid roomname entrance privacy areamin areamax visualconnection";

            //        //a.typeid = row[0];
            //        //a.roomname = row[1];
            //        //a.entrance = ConvertToBool(row[2]);
            //        //a.privacy = int.Parse(row[3]);
            //        //a.areamin = double.Parse(row[4]);
            //        //a.areamax = double.Parse(row[5]);
            //        //a.visualCommection = ConvertToBool(row[6]);


            //        foreach (string field in row)
            //        {

            //        }
            //    }
            //}

            roomTypes.Add(new RoomType("1", "Living room", true, 1, 300, 500, true));
            roomTypes.Add(new RoomType("2", "Kitchen", false, 2, 10, 40, true));
            roomTypes.Add(new RoomType("3", "Restroom", false, 2, 2, 10, true));
            roomTypes.Add(new RoomType("4", "Bedroom", false, 3, 10, 20, true));

        }

        private double CalculateCost() {
            double areacost = CalculateParameterCost();
            double layoutcost = CalculateLayoutCost();
            double constaintcost = CalculateConstraintCost();
            double summary = areacost + layoutcost;
            Utils.WriteLog("területköltség: " + areacost);
            Utils.WriteLog("kerületköltség: " + layoutcost);
            return summary;
        }

        private double CalculateConstraintCost() {
            //muszáj teljesülne
            return 0.0;
        }
        private double CalculateParameterCost() {
            double summary = 0.0;
            try {
                foreach (Room room in rooms) {
                    try {
                        string roomTypeId = Utils.GetParameterValueByName(room, "Comments");
                        double actualarea = room.Area;
                        RoomType type = roomTypes.Find(i => i.typeid.Equals(roomTypeId));
                        if (actualarea < type.areamin) {
                            summary += Math.Abs(type.areamin - actualarea);
                        }
                        else if (actualarea > type.areamax) {
                            summary += Math.Abs(type.areamax - actualarea);
                        }
                    }
                    catch (Exception e) {
                        //Utils.WriteLog(e);
                    }
                }
            }
            catch (Exception e) {
                summary = 10;
                Utils.WriteLog("outer exc" + e);
            }
            //elemszintű megfelelés
            //minden helyiségre
            //megkeresni a helyiség kategóriát a táblázatból
            //számítani a megfelelést
            //összegezni

            return summary;
        }

        private double CalculateLayoutCost() {
            double wallLength = 0.0;


            foreach (BoundarySegment seg in boundaryLines) {
                wallLength += Math.Sqrt(seg.GetCurve().Length * 3);
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

        private Wall ChooseRandomWall() {

            Random rand = new Random();
            int num1 = rand.Next(0, walls.Count);
            Wall wall = walls.ElementAt(num1);
            return wall;
        }
        private void MoveWall(Wall wall, double flip) {
            if (wall.Name.Contains("Finishing")) {
                return;
            }
            int split = random.Next(0, 10);
            int splitchance = random.Next(0, 10);

            LocationCurve a = ((LocationCurve)wall.Location);
            Curve b = a.Curve;

            XYZ curvestart = b.GetEndPoint(0);
            XYZ curveend = b.GetEndPoint(1);

            XYZ curvesplit = curvestart * split + curveend * (1 - split);


            // create line
            Line line1 = Line.CreateUnbound(curvestart, curvesplit);
            Line line2 = Line.CreateUnbound(curvesplit, curveend);


            XYZ direction = wall.Orientation * flip;

            using (Transaction t = new Transaction(_document, "wall")) {
                t.Start();
                if (true) {
                    //MessageBox.Show("Test");
                    ElementId copiedwallid = ElementTransformUtils.CopyElement(wall.Document, wall.Id, XYZ.Zero).First();
                    Wall copiedwall = (Wall)wall.Document.GetElement(copiedwallid);
                    ((LocationCurve)wall.Location).Curve = line1;
                    ((LocationCurve)copiedwall.Location).Curve = line2;

                }

                t.Commit();
            }
            using (Transaction t = new Transaction(_document, "wall")) {
                t.Start();

                wall.Location.Move(direction);
                t.Commit();
            }
        }

        private void SwitchRooms() {
            List<Room> roomsInPhase = new FilteredElementCollector(_document)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .Where(e => e.LookupParameter("Area").AsDouble() > 0).Where(e => e.Name != "Kültér")
                .Cast<Room>()
                .ToList();

            Random rand = new Random();
            int num1 = rand.Next(0, roomsInPhase.Count);
            int num2 = rand.Next(0, roomsInPhase.Count);

            Room room1 = roomsInPhase.ElementAt(num1);
            Room room2 = roomsInPhase.ElementAt(num2);


            SwitchRoomData(room1, room2);

            return;
        }

        private void SwitchRoomData(Room room1, Room room2) {
            string name1 = Utils.GetParameterValueByName(room1, "Name");// room1.Name + "";
            string name2 = Utils.GetParameterValueByName(room2, "Name");// room2.Name + "";
            if (name1.Equals("Outside") || name2.Equals("Outside")) {
                return;
            }
            string comment1 = Utils.GetParameterValueByName(room1, "Comments");// room1.Name + "";
            string comment2 = Utils.GetParameterValueByName(room2, "Comments");// room2.Name + "";
            using (Transaction t = new Transaction(_document, "room")) {
                t.Start();
                Utils.SetParameterValueByName(room1, "Name", name2);
                Utils.SetParameterValueByName(room2, "Name", name1);
                Utils.SetParameterValueByName(room1, "Comments", comment2);
                Utils.SetParameterValueByName(room2, "Comments", comment1);
                t.Commit();
            }
        }

        private void MigrateProblem() {
            //minden falra

            List<LocationCurve> falvonalak = new List<LocationCurve>();
            falvonalak = walls.Select(i => (LocationCurve)i.Location).ToList();
            //List<List<BoundarySegment>> szobakörvonalak = rooms.Select((Room)i => i.GetBoundary()).ToList();
            List<XYZ> kezdőpontok = falvonalak.Select(i => i.Curve.GetEndPoint(0)).ToList();
            List<XYZ> véppontok = falvonalak.Select(i => i.Curve.GetEndPoint(1)).ToList();


        }

        private void MoveWallSegment() {

        }
    }

    public enum ActionType {
        switchrooms,
        movewalls
    }

    public class RoomType {
        public string typeid;
        public string roomname;
        public bool entrance;
        public int privacy;
        public double areamin;
        public double areamax;
        public bool visualCommection;

        public RoomType(string TypeId, string RoomName, bool Entrance, int Privacy, double AreaMin, double AreaMax, bool VisualConnection) {
            typeid = TypeId;
            roomname = RoomName;
            entrance = Entrance;
            privacy = Privacy;
            areamin = AreaMin;
            areamax = AreaMax;
            visualCommection = VisualConnection;

        }

        //typeid room name entrance    privacy area min area max visual connection
        //1	living room yes	1	12	50	yes
        //2	kitchen no	2	10	40	yes
        //3	restroom no	2	2	10	no
        //4	bedroom no	4	10	20	yes


    }
}
