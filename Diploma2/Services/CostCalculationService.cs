using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Diploma2.Model;
using GeoLib;
using Point = System.Windows.Point;

namespace Diploma2.Services
{
    public class CostCalculationService
    {
        //TODO: this might be problematic, that it is static
        public static Cost C { get; set; }
        public static _Model localModel { get; set; }
        public static double[] CalculateCost(_Model m)
        {
            //NOTE: summarycost calculation would be more flexible, if i could rapidly replace functions
            localModel = m;
            double summary = 100000000;
            double areacost = 0.0;
            double layoutcost = 0.0;
            double constaintcost = 0.0;

            areacost = CalculateParameterCost();
            layoutcost = CalculateLayoutCost();
            constaintcost = CalculateConstraintCost();
            summary = areacost + layoutcost + constaintcost;

            return new[] { summary, areacost, layoutcost, constaintcost };
        }
        public static Cost CalculateCostNew(_Model m)
        {
            localModel = m;
            localModel.FillAdjacencyMatrix();
            //localModel.FillDepthArray();
            localModel.CalculateRelatedRoomsForLines();
            C = new Cost(-1, 0, 0, 0, 0);

            C.AreaCost = CalculateParameterCost();
            C.LayoutCost = CalculateLayoutCost();
            C.ConstaintCost = CalculateConstraintCost();

            return C;
        }

        public static Cost CalculateDoorCost(_Model m)
        {
            localModel = m;
            localModel.FillAdjacencyMatrix(); //TODO: this might not be needed at all
            localModel.FillTransparencyMatrix();
            localModel.CalculateRelatedRoomsForLines(); //not sure about the ordering
            localModel.FillDepthArray();
            C = new Cost(-1, 0, 0, 0, 0);

            C.LayoutCost = CalculateDoorCost();
            return C;
        }
        private static double CalculateConstraintCost()
        {
            double asd = 0.0;
            List<_Point> boundaries2 = localModel.OutlinePolygonPoints;
            if (boundaries2 == null || (boundaries2 != null && !boundaries2.Any()))
            {
                return asd;
            }

            List<C2DPoint> convertedPointsForPolygon3 = boundaries2.Select(i => new C2DPoint(i.X, i.Y)).ToList();
            C2DPolygon BoundaryPolygon = new C2DPolygon();
            BoundaryPolygon.Create(convertedPointsForPolygon3, true);

            foreach (_Room room in localModel.rooms)
            {
                List<_Point> boundaries = room.GetPoints();
                if (!boundaries.Any()) continue;
                List<C2DPoint> convertedPointsForPolygon2 = boundaries.Select(i => new C2DPoint(i.X, i.Y)).ToList();

                C2DPolygon roomPolygon = new C2DPolygon();
                roomPolygon.Create(convertedPointsForPolygon2, true);
                room.Polygon = roomPolygon;
                CGrid grid = new CGrid();
                List<C2DHoledPolygon> asdasd = new List<C2DHoledPolygon>();
                roomPolygon.GetOverlaps(BoundaryPolygon, asdasd, grid);
                List<double> a = asdasd.Select(i => i.GetArea()).ToList();
                double sumoverlap = a.Sum() / 10000;
                double actualArea = room.Area;
                if (actualArea - sumoverlap > 0)
                {
                    asd += +10000000;
                }

                asd += Math.Pow(2, actualArea - sumoverlap);
            }

            for (var i = 0; i < localModel.rooms.Count; i++)
            {
                _Room modelRoom = localModel.rooms[i];
                for (int j = i + 1; j < localModel.rooms.Count; j++)
                {
                    _Room modelRoom2 = localModel.rooms[j];
                    CGrid grid = new CGrid();
                    List<C2DHoledPolygon> asdasd = new List<C2DHoledPolygon>();
                    modelRoom.Polygon.GetOverlaps(modelRoom2.Polygon, asdasd, grid);
                    List<double> a = asdasd.Select(k => k.GetArea()).ToList();
                    double sumoverlap = a.Sum() / 10000;
                    double actualArea = modelRoom.Area;
                    if (sumoverlap > 0)
                    {
                        asd += +10000000;
                    }

                }
            }

            //then avoid room overlaps

            return asd;
        }


        private static double CalculateParameterCost()
        {
            double summary = 0.0;

            //this part is responsible for measuring deviance from the given room standards
            foreach (var room in localModel.rooms)
            {
                double actualarea = room.CalculateArea();

                _RoomType type = room.type ?? _RoomType.BedRoom;

                if (Math.Abs(actualarea - type.areamax) > 0.01 || Math.Abs(actualarea - type.areamin) > 0.01)
                {

                    if (actualarea < type.areamin)
                    {
                        summary += Math.Pow(type.areamin - actualarea, 5);
                    }
                    else if (actualarea > type.areamax)
                    {
                        summary += Math.Pow(-type.areamax + actualarea, 5);
                    }
                }
            }

            //TODO: this fails when switched with simulation
            foreach (_Room room in localModel.rooms)
            {

                _RoomType type = room.type ?? _RoomType.BedRoom;

                double actualprop = room.CalculateProportion();
                if (actualprop > type.proportion)
                {
                    summary += Math.Pow(2, Math.Abs(-type.proportion + actualprop) + 5);
                }

                double minsidesize = room.CalculateMinSideSize();
                if (minsidesize < type.minsidesize)
                {
                    //summary += Math.Pow(2, type.minsidesize - minsidesize);
                }
            }

            //punish more edges
            foreach (_Room room in localModel.rooms)
            {
                double countCost = room.GetPoints().Count;
                if (countCost > 6)
                {
                    summary += Math.Pow(2, countCost - 6 + 10);
                }
            }

            return Math.Round(summary, 2);
        }

        public static Dictionary<_RoomType, Dictionary<_RoomType, int>> asd { get; set; } = new Dictionary<_RoomType, Dictionary<_RoomType, int>>();

        public static void InitializeASD()
        {
            asd.Add(_RoomType.LivingRoom, new Dictionary<_RoomType, int>() { { _RoomType.Kitchen, -20 }, { _RoomType.BedRoom, -20 }, { _RoomType.CorridorRoom, -100 }, { _RoomType.LivingRoom, -20 }, { _RoomType.RestRoom, -100 } });
            asd.Add(_RoomType.Kitchen, new Dictionary<_RoomType, int>() { { _RoomType.Kitchen, -20 }, { _RoomType.LivingRoom, -20 }, { _RoomType.BedRoom, 1000 }, { _RoomType.CorridorRoom, -100 }, { _RoomType.RestRoom, 100 } });
            asd.Add(_RoomType.BedRoom, new Dictionary<_RoomType, int>() { { _RoomType.LivingRoom, -20 }, { _RoomType.Kitchen, 1000 }, { _RoomType.CorridorRoom, -100 }, { _RoomType.BedRoom, -100 }, { _RoomType.RestRoom, -100 } });
            asd.Add(_RoomType.RestRoom, new Dictionary<_RoomType, int>() { { _RoomType.LivingRoom, -100 }, { _RoomType.Kitchen, 1000 }, { _RoomType.CorridorRoom, -100 }, { _RoomType.BedRoom, -100 }, { _RoomType.RestRoom, -100 } });
            asd.Add(_RoomType.CorridorRoom, new Dictionary<_RoomType, int>() { { _RoomType.CorridorRoom, -100 }, { _RoomType.LivingRoom, -100 }, { _RoomType.Kitchen, -100 }, { _RoomType.BedRoom, -100 }, { _RoomType.RestRoom, -100 } });
            //List<_RoomType> rooms = new List<_RoomType>(){_RoomType.BedRoom, _RoomType.CorridorRoom, _RoomType.DiningRoom, _RoomType.Kitchen, _RoomType.LivingRoom , _RoomType.RestRoom , _RoomType.StairCase};
            //int[,] costs = new int[rooms.Count,rooms.Count];


        }
        private static double CalculateLayoutCost()
        {
            double wallLength = 0.0;
            foreach (_Line seg in localModel.AllLinesFlat())
            {
                {
                    double d = seg.GetLength();
                    if (d > 0)
                    {
                        if (seg.relatedrooms.Count < 1)            //i need to calculate exterior walls with different cost
                        {
                            wallLength += Math.Sqrt((d / 100)) * 100;
                        }
                        else
                        {
                            wallLength += Math.Sqrt((d / 100)) * 3;
                        }
                    }

                    if (d < 30) //WE dont like small walls
                    {
                        wallLength += Math.Sqrt((d / 100)) * 10;
                    }
                }
            }



            //TODO: what rooms are adjacent?
            // |         |        |        |
            // |         |        |        |
            // |     2   |    1,3 |   2    |
            // |         |        |        |
            // |_________|________|________|
            //bellmann ford, dijktra
            //szomszédossági mátrix
            double layoutcost = 0.0;

            for (var i = 0; i < localModel.rooms.Count; i++)
            {
                _Room room = localModel.rooms[i];
                for (var j = i + 1; j < localModel.rooms.Count; j++)
                {
                    _Room localModelRoom = localModel.rooms[j];
                    if (localModel.AdjacencyMatrix[i, j] == 1)
                    {
                        layoutcost += RoomTypeCostStorage.FindCost(room.type, localModelRoom.type);
                    }
                }
            }

            //elrendezésszintű

            //ajtókat, nyílásokat letenni...(kérdés)
            //bejárhatóság generálás
            double privacygradientcost = 0.0;
            //kerületszámítás
            //minimális optimum kerület = sqrt(minden szoba area összege)*4
            double summary = privacygradientcost + wallLength + layoutcost;
            summary = Math.Round(summary, 2);
            return summary;
        }
        private static double CalculateDoorCost()
        {
            //bejárhatóság
            double cost = 0.0;
            //Dictionary<int, List<_Room>> processedRooms = new Dictionary<int, List<_Room>>();

            //int accessRoomDepth = 0;
            //List<_Room> actualRooms = localModel.rooms.Where(i => i.isStartRoom).ToList();
            //processedRooms.Add(accessRoomDepth, actualRooms);

            int doorCount = 0;
            foreach (_Room room in localModel.rooms)
            {
                foreach (_Line roomLine in room.Lines)
                {
                    if (roomLine.HasDoor) doorCount++;
                }
            }

            cost += doorCount * 100;

            if (doorCount > localModel.rooms.Count)
            {
                cost += Math.Pow(2, doorCount - localModel.rooms.Count);
            }
            //TODO: dikhstra

            for (var i = 0; i < localModel.rooms.Count; i++)
            {
                _Room room = localModel.rooms[i];
                int depth = localModel.DepthMatrix[i];
                if (room.type.privacy > depth && depth < 100)
                {
                    cost += Math.Pow(2, room.type.privacy - depth);
                }

                if (depth > 100)
                {
                    cost += 1000000;
                }
            }
            //todo: what if unreachable? check it! that is bad.

            //bejárhatóságra ez elég
            //már feldolgozott elemek
            //éppen feldolgozás alatt állló elemek
            //szaggatott lsita az éppen szomszédokról

            //szomszédossági mátrix n-n - át lehet-e menni, és milyen költséggel
            //bellmann ford, floyd, dijkstra


            //while (accessRoomDepth < localModel.modelRooms.Count) {
            //    List<MyRoom> actualRoomsForThisDepth = new List<MyRoom>(); 
            //    bool isOnGoing = processedRooms.TryGetValue(accessRoomDepth, out actualRoomsForThisDepth);
            //    if (!isOnGoing) break;

            //    accessRoomDepth++;
            //    foreach (MyRoom room in actualRoomsForThisDepth) {
            //        foreach (MyLine line in room.BoundaryLines) {
            //            if (line.HasOpening) {
            //                List<MyRoom> reachableRoomsFromThisLine = line.relatedRooms;
            //                reachableRoomsFromThisLine.Remove(room);

            //                List<MyRoom> alreadyInRoomsAtThisDepth = new List<MyRoom>();
            //                bool isAlready = processedRooms.TryGetValue(accessRoomDepth, out alreadyInRoomsAtThisDepth);

            //                if (isAlready) {
            //                    reachableRoomsFromThisLine.AddRange(alreadyInRoomsAtThisDepth);
            //                    processedRooms.Remove(accessRoomDepth);
            //                }
            //                processedRooms.Add(accessRoomDepth, reachableRoomsFromThisLine);
            //            }
            //        }
            //    }
            //}
            //van-e helyiség ami kimarad, ekkor hozzáadjuk az area-t a költséghez
            //helyiség privacy gradientje megfelel-e
            //megfelelő szobák egymásból nyílnak-e

            //külső vonal változtatás nem változtat bejárhatóságot

            return cost;

        }
    }
}
