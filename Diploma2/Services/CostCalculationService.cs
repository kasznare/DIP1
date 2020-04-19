using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diploma2.Model;

namespace Diploma2.Services {
    public class CostCalculationService {

        public static _Model localModel { get; set; }
        public static double[] CalculateCost(_Model m) {
            //NOTE: cost calculation would be more flexible, if i could rapidly replace functions
            localModel = m;
            double summary = 0.0;
            double areacost = 0.0;
            double layoutcost = 0.0;
            double constaintcost = 0.0;

            areacost = CalculateParameterCost();
            layoutcost = CalculateLayoutCost();
            constaintcost = CalculateConstraintCost();
            summary = areacost + layoutcost + constaintcost;
            
            return new double[] { summary, areacost, layoutcost, constaintcost };
        }
        private static double CalculateConstraintCost() {
            return 0.0;
        }
        private static double CalculateParameterCost() {
            double summary = 0.0;

            //this part is responsible for measuring deviance from the given room standards
            foreach (var room in localModel.rooms) {
                double actualarea = room.CalculateArea();

                _RoomType type = room.type ?? _RoomType.BedRoom;

                if (Math.Abs(actualarea - type.areamax) > 0.01 || Math.Abs(actualarea - type.areamin) > 0.01) {

                    if (actualarea < type.areamin) {
                        summary += Math.Pow(type.areamin - actualarea, 5);
                    }
                    else if (actualarea > type.areamax) {
                        summary += Math.Pow(-type.areamax + actualarea, 5);
                    }
                }
            }

            //TODO: this fails when switched with simulation
            foreach (_Room room in localModel.rooms) {
                _RoomType type = room.type;
                if (type == null) {
                    type = _RoomType.BedRoom;
                }
                double actualprop = room.CalculateProportion();
                if (actualprop > type.proportion) {
                    summary += Math.Pow(2, Math.Abs(-type.proportion + actualprop));
                }
            }

            //punish more edges
            foreach (_Room room in localModel.rooms) {
                double countCost = room.GetPoints().Count;
                if (countCost > 6) {
                    summary += Math.Pow(2, countCost - 6);
                }
            }

            return Math.Round(summary, 2);
        }
        private static double CalculateLayoutCost() {
            double wallLength = 0.0;
            foreach (_Line seg in localModel.AllLinesFlat()) {
                {
                    if (seg.GetLength() > 0) {

                        wallLength += Math.Sqrt((seg.GetLength() / 100)) * 3;
                    }
                }
            }

            Dictionary<_RoomType, Dictionary<_RoomType, int>> asd = new Dictionary<_RoomType, Dictionary<_RoomType, int>>();
            asd.Add(_RoomType.LivingRoom, new Dictionary<_RoomType, int>() { { _RoomType.Kitchen, -20 }, { _RoomType.BedRoom, -20 }, { _RoomType.CorridorRoom, -100 }, { _RoomType.RestRoom, -100 } });
            asd.Add(_RoomType.Kitchen, new Dictionary<_RoomType, int>() { { _RoomType.LivingRoom, -20 }, { _RoomType.BedRoom, 1000 }, { _RoomType.CorridorRoom, -100 }, { _RoomType.RestRoom, 100 } });
            asd.Add(_RoomType.BedRoom, new Dictionary<_RoomType, int>() { { _RoomType.LivingRoom, -20 }, { _RoomType.Kitchen, 1000 }, { _RoomType.CorridorRoom, -100 }, { _RoomType.RestRoom, -100 } });
            asd.Add(_RoomType.RestRoom, new Dictionary<_RoomType, int>() { { _RoomType.LivingRoom, -100 }, { _RoomType.Kitchen, 1000 }, { _RoomType.CorridorRoom, -100 }, { _RoomType.RestRoom, -100 } });
            asd.Add(_RoomType.CorridorRoom, new Dictionary<_RoomType, int>() { { _RoomType.LivingRoom, -100 }, { _RoomType.Kitchen, -100 }, { _RoomType.BedRoom, -100 }, { _RoomType.RestRoom, -100 } });


            double layoutcost = 0.0;
            //TODO:here is problematic, that i dont know degree, fix this
            //foreach (_Line modelLine in localModel.AllLinesFlat()) {
            //    var count = modelLine.relatedRooms.Count;
            //    if (count > 1) {
            //        for (var index = 0; index < count; index++) {
            //            MyRoom r1 = modelLine.relatedRooms[index];
            //            for (int i = index + 1; i < count; i++) {
            //                //TODO: make a 2D grid and choose based on the combination. I dont know the solution
            //                MyRoom r2 = modelLine.relatedRooms[i];

            //                Dictionary<RoomType, int> asd2 = new Dictionary<RoomType, int>();
            //                bool isIn = asd.TryGetValue(r1.type, out asd2);

            //                if (isIn) {
            //                    int value = 0;
            //                    bool isInOther = asd2.TryGetValue(r2.type, out value);
            //                    layoutcost += value;
            //                }

            //            }
            //        }
            //        //have linear 2fold combination of list, and calculate value based on the 2d array/datasheet
            //    }
            //}

            //elrendezésszintű
            double passagewaycost = 0.0;
            passagewaycost = CalculatePassageWayCost();
            //ajtókat, nyílásokat letenni...(kérdés)
            //bejárhatóság generálás
            double privacygradientcost = 0.0;
            //kerületszámítás
            //minimális optimum kerület = sqrt(minden szoba area összege)*4
            double summary = passagewaycost + privacygradientcost + wallLength + layoutcost;
            summary = Math.Round(summary, 2);
            return summary;
        }
        private static double CalculatePassageWayCost() {
            //bejárhatóság
            double cost = 0.0;
            Dictionary<int, List<_Room>> processedRooms = new Dictionary<int, List<_Room>>();

            int accessRoomDepth = 0;
            List<_Room> actualRooms = localModel.rooms.Where(i => i.isStartRoom).ToList();
            processedRooms.Add(accessRoomDepth, actualRooms);


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
