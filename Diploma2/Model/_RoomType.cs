using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using Diploma2.Annotations;

namespace Diploma2.Model
{
    public class _RoomType: _GeometryBase, INotifyPropertyChanged
    {
        public int typeid;
        public string roomname { get; set; }
        public bool entrance;
        public int privacy;
        public double areamin { get; set; }
        public double areamax { get; set; }
        public double proportion { get; set; }
        public double minsidesize { get; set; }
        public bool visualCommection;
        public Color fillColor { get; set; }

        public static _RoomType BedRoom =
            new _RoomType(1, "BedRoom",
                2, true, 3, 15, 15, true, Color.CornflowerBlue, 300);
        public static _RoomType CorridorRoom =
            new _RoomType(2, "CorridorRoom",
                50, true, 1, 2, 200, true, Color.Wheat, 80);
        public static _RoomType DiningRoom =
            new _RoomType(3, "DiningRoom",
                1.5, true, 2, 10, 10, true, Color.Brown,200);

        public static _RoomType Kitchen =
            new _RoomType(4, "Kitchen",
                1.5, true, 2, 10, 10, true, Color.Chocolate, 180);
        public static _RoomType LivingRoom =
            new _RoomType(5, "LivingRoom",
                1.5, true, 1, 20, 20, true, Color.DarkOliveGreen, 300);

        public static _RoomType RestRoom =
            new _RoomType(6, "RestRoom",
                2, true, 2, 5, 5, true, Color.LightBlue, 90);
        public static _RoomType StairCase =
            new _RoomType(7, "StairCase",
                50, true, 1, 8,8, true, Color.White, 200);



        public override bool Equals(object obj) {
            _RoomType a = obj as _RoomType;
            if (a == null) {
                return false;
            }

            return this.typeid == a.typeid;
            //return Close(a.X, X) && Close(a.Y, Y);
            //return base.Equals(obj);
        }

        public static List<_RoomType> getRoomTypes()
        {
            List<_RoomType> roomtypes = new List<_RoomType>();
            roomtypes.Add(_RoomType.BedRoom);
            roomtypes.Add(_RoomType.LivingRoom);
            roomtypes.Add(_RoomType.RestRoom);
            roomtypes.Add(_RoomType.Kitchen);

            return roomtypes;
        }

        //public static RoomType LivingRoom {
        //    get {
        //        return new RoomType(1, "LivingRoom",
        //            1.5, true, 1, 200, 200, true, Color.DarkOliveGreen);
        //    }
        //}
        //public static RoomType Kitchen {
        //    get {
        //        return new RoomType(2, "Kitchen",
        //            1.5, true, 2, 100, 100, true, Color.Chocolate);
        //    }
        //}
        //public static RoomType RestRoom {
        //    get {
        //        return new RoomType(3, "RestRoom",
        //            2, true, 2, 50, 50, true, Color.LightBlue);
        //    }
        //}
        //public static RoomType BedRoom {
        //    get {
        //        return new RoomType(4, "BedRoom",
        //            2, true, 3, 150, 150, true, Color.CornflowerBlue);
        //    }
        //}
        public _RoomType(int TypeId, string RoomName, double prop, bool Entrance, int Privacy, double AreaMin, double AreaMax, bool VisualConnection, Color fill, double MinsideSize) {
            typeid = TypeId;
            proportion = prop;
            roomname = RoomName;
            entrance = Entrance;
            privacy = Privacy;
            areamin = AreaMin;
            areamax = AreaMax;
            visualCommection = VisualConnection;
            fillColor = fill;
            minsidesize = MinsideSize;
        }
        public override string ToString() {
            return roomname;
        }
        //typeid room name entrance    privacy area min area max visual connection
        //1	living room yes	1	12	50	yes
        //2	kitchen no	2	10	40	yes
        //3	restroom no	2	2	10	no
        //4	bedroom no	4	10	20	yes
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}