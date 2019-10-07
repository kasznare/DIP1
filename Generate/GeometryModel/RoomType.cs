using System.Drawing;

namespace WindowsFormsApp1
{
    public class RoomType
    {
        /// <summary>
        /// number of openings on room walls
        /// </summary>
        public int MaxDegree { get; set; }

        public int typeid;
        public string roomname;
        public bool entrance;
        public int privacy;
        public double areamin;
        public double areamax;
        public bool visualCommection;
        public Color fillColor;

        public static RoomType LivingRoom { get { return new RoomType(1, "LivingRoom", true, 1, 30,30,true, Color.DarkOliveGreen);} }
        public static RoomType Kitchen { get { return new RoomType(2, "Kitchen", true, 2, 15,15,true, Color.Chocolate);} }
        public static RoomType RestRoom { get { return new RoomType(3, "RestRoom", true, 2, 5,5,true, Color.LightBlue);} }
        public static RoomType BedRoom { get { return new RoomType(4, "BedRoom", true, 3, 15,15,true, Color.CornflowerBlue);} }
        public RoomType(int TypeId, string RoomName, bool Entrance, int Privacy, double AreaMin, double AreaMax, bool VisualConnection, Color fill) {
            typeid = TypeId;
            roomname = RoomName;
            entrance = Entrance;
            privacy = Privacy;
            areamin = AreaMin;
            areamax = AreaMax;
            visualCommection = VisualConnection;
            fillColor = fill;

        }

        public override string ToString()
        {
            return roomname;
        }
        //typeid room name entrance    privacy area min area max visual connection
        //1	living room yes	1	12	50	yes
        //2	kitchen no	2	10	40	yes
        //3	restroom no	2	2	10	no
        //4	bedroom no	4	10	20	yes


    }

}