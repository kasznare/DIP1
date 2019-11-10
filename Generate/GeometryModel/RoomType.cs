using System.Drawing;

namespace WindowsFormsApp1 {
    public class RoomType {
        /// <summary>
        /// number of openings on room walls
        /// </summary>
        //public int MaxDegree { get; set; }

        public int typeid;
        public string roomname { get; set; }
        public bool entrance;
        public int privacy;
        public double areamin { get; set; }
        public double areamax { get; set; }
        public double proportion { get; set; }
        public bool visualCommection;
        public Color fillColor { get; set; }

        //public static RoomType LivingRoom { get { return new RoomType(1, "LivingRoom",
        //    1.5, true, 1, 20, 20, true, Color.DarkOliveGreen); } }
        //public static RoomType Kitchen { get { return new RoomType(2, "Kitchen",
        //    1.5,true, 2, 10, 10, true, Color.Chocolate); } }
        //public static RoomType RestRoom { get { return new RoomType(3, "RestRoom",
        //    2,true, 2, 5, 5, true, Color.LightBlue); } }
        //public static RoomType BedRoom { get { return new RoomType(4, "BedRoom",
        //    2, true, 3, 15, 15, true, Color.CornflowerBlue); } }

        public static RoomType LivingRoom {
            get {
                return new RoomType(1, "LivingRoom",
                    1.5, true, 1, 200, 200, true, Color.DarkOliveGreen);
            }
        }
        public static RoomType Kitchen {
            get {
                return new RoomType(2, "Kitchen",
                    1.5, true, 2, 100, 100, true, Color.Chocolate);
            }
        }
        public static RoomType RestRoom {
            get {
                return new RoomType(3, "RestRoom",
                    2, true, 2, 50, 50, true, Color.LightBlue);
            }
        }
        public static RoomType BedRoom {
            get {
                return new RoomType(4, "BedRoom",
                    2, true, 3, 150, 150, true, Color.CornflowerBlue);
            }
        }
        public RoomType(int TypeId, string RoomName, double prop, bool Entrance, int Privacy, double AreaMin, double AreaMax, bool VisualConnection, Color fill) {
            typeid = TypeId;
            proportion = prop;
            roomname = RoomName;
            entrance = Entrance;
            privacy = Privacy;
            areamin = AreaMin;
            areamax = AreaMax;
            visualCommection = VisualConnection;
            fillColor = fill;

        }
        public override string ToString() {
            return roomname;
        }
        //typeid room name entrance    privacy area min area max visual connection
        //1	living room yes	1	12	50	yes
        //2	kitchen no	2	10	40	yes
        //3	restroom no	2	2	10	no
        //4	bedroom no	4	10	20	yes
    }
}