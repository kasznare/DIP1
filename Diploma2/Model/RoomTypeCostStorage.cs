using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma2.Model
{
    public static class RoomTypeCostStorage
    {
        public static List<_RoomTypeCost> typeCosts { get; set; } = new List<_RoomTypeCost>();
            public static List<_RoomType> allRoomTypes = new List<_RoomType>() { _RoomType.BedRoom, _RoomType.CorridorRoom, _RoomType.DiningRoom, _RoomType.Kitchen, _RoomType.LivingRoom, _RoomType.RestRoom, _RoomType.StairCase };

        public static void AddCost(_RoomTypeCost c)
        {
            if (typeCosts.Contains(c) || typeCosts.Contains(c.getInverse())) return;
            
            typeCosts.Add(c);
            typeCosts.Add(c.getInverse());

        }

        private static bool initialized = false;

        public static void Initialize()
        {
            typeCosts.Add(new _RoomTypeCost(_RoomType.BedRoom, _RoomType.BedRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.BedRoom, _RoomType.CorridorRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.BedRoom, _RoomType.DiningRoom, 200));
            typeCosts.Add(new _RoomTypeCost(_RoomType.BedRoom, _RoomType.Kitchen, 200));
            typeCosts.Add(new _RoomTypeCost(_RoomType.BedRoom, _RoomType.LivingRoom, 100));
            typeCosts.Add(new _RoomTypeCost(_RoomType.BedRoom, _RoomType.RestRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.BedRoom, _RoomType.StairCase, 0));

            //typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.BedRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.CorridorRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.DiningRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.Kitchen, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.LivingRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.RestRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.StairCase, 0));

            //typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.BedRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.CorridorRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.DiningRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.Kitchen, -500));
            typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.LivingRoom, -100));
            typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.RestRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.StairCase, 100));

            //typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.BedRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.CorridorRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.Kitchen, _RoomType.DiningRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.Kitchen, _RoomType.Kitchen, -500));
            typeCosts.Add(new _RoomTypeCost(_RoomType.Kitchen, _RoomType.LivingRoom, -100));
            typeCosts.Add(new _RoomTypeCost(_RoomType.Kitchen, _RoomType.RestRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.Kitchen, _RoomType.StairCase, 100));

            //typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.BedRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.CorridorRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.Kitchen, _RoomType.DiningRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.LivingRoom, _RoomType.Kitchen, -500));
            typeCosts.Add(new _RoomTypeCost(_RoomType.LivingRoom, _RoomType.LivingRoom, 100));
            typeCosts.Add(new _RoomTypeCost(_RoomType.LivingRoom, _RoomType.RestRoom, 100));
            typeCosts.Add(new _RoomTypeCost(_RoomType.LivingRoom, _RoomType.StairCase, 100));

            //typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.BedRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.CorridorRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.Kitchen, _RoomType.DiningRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.LivingRoom, _RoomType.Kitchen, -500));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.LivingRoom, _RoomType.LivingRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.RestRoom, _RoomType.RestRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.RestRoom, _RoomType.StairCase, 0));

            //typeCosts.Add(new _RoomTypeCost(_RoomType.CorridorRoom, _RoomType.BedRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.DiningRoom, _RoomType.CorridorRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.Kitchen, _RoomType.DiningRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.LivingRoom, _RoomType.Kitchen, -500));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.LivingRoom, _RoomType.LivingRoom, 0));
            //typeCosts.Add(new _RoomTypeCost(_RoomType.RestRoom, _RoomType.RestRoom, 0));
            typeCosts.Add(new _RoomTypeCost(_RoomType.StairCase, _RoomType.StairCase, 0));

            initialized = true;
        }

        public static int FindCost(_RoomType t1, _RoomType t2)
        {
            if (!initialized)
            {
                Initialize();
            }

            int cost = 0;

            _RoomTypeCost type = typeCosts.FirstOrDefault(i => i.type1.Equals(t1) && i.type2.Equals(t2));
            if (type!=null)
            {
                cost = type.Cost;
            }


            return cost;
        }
    }
}
