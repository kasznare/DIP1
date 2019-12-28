using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.GeometryModel {
    public class FactoryRoomType :RoomType{
        public FactoryRoomType(int TypeId, string RoomName, double prop, bool Entrance, int Privacy, double AreaMin, double AreaMax, bool VisualConnection, Color fill) : base(TypeId, RoomName, prop, Entrance, Privacy, AreaMin, AreaMax, VisualConnection, fill)
        {
        }

        public static FactoryRoomType DobozosMagasraktar => new FactoryRoomType(1, "Dobozos magasraktár", 1, false, 1, 8000, 8100, false, Color.AliceBlue);
        public static FactoryRoomType GepeszetiHelyiseg => new FactoryRoomType(1, "Gépészeti helyiség", 1, false, 1, 160, 170, false, Color.Green);
        public static FactoryRoomType Trafo => new FactoryRoomType(1, "Trafó", 1, false, 1, 30, 35, false, Color.Yellow);
        public static FactoryRoomType KapcsoloHelyiseg20kv => new FactoryRoomType(1, "Kapcsoló helyiség 20KV", 1, false, 1, 30, 35, false, Color.Orange);
        public static FactoryRoomType KapcsoloHelyiseg0_4kv => new FactoryRoomType(1, "Kapcsoló helyiség 0.4KV", 1, false, 1, 60, 65, false, Color.DarkOrange);
        public static FactoryRoomType AtemeloEloter => new FactoryRoomType(1, "Átemelő előtér", 1, false, 1, 20, 30, false, Color.Gray);
        public static FactoryRoomType GeneratorHelyiseg => new FactoryRoomType(1, "Generátor helyiség", 1, false, 1, 60, 70, false, Color.YellowGreen);
        public static FactoryRoomType SprinklerHelyiseg => new FactoryRoomType(1, "Sprinkler helyiség", 1, false, 1, 20, 30, false, Color.Aqua);
    }
}
