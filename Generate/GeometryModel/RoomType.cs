namespace WindowsFormsApp1
{
    public class RoomType
    {
        /// <summary>
        /// number of openings on room walls
        /// </summary>
        public int MaxDegree { get; set; }

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