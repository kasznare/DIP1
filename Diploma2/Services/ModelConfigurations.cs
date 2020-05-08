using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;
using Diploma2.Model;

namespace Diploma2.Services {
    public static class ModelConfigurations {
        public static _Model InitSimplestModel()
        {
            _Model m = new _Model();
            _Point p1 = new _Point(100, 100);
            _Point p2 = new _Point(400, 100);
            _Point p3 = new _Point(400, 400);
            _Point p4 = new _Point(100, 400);
            _Line line1 = new _Line(p1, p2);
            _Line line2 = new _Line(p2, p3);
            _Line line3 = new _Line(p3, p4);
            _Line line4 = new _Line(p4, p1);
            _Room _Room = new _Room {Name = "FirstRoom", Number = 1, type = _RoomType.LivingRoom, isStartRoom = true};
            _Room.Lines = ( new List<_Line>(){line1,line2,line3,line4});
            m.rooms.Add(_Room);
            foreach (var room in m.rooms) {
                room.CanGetBoundarySorted();
            }

            List<_Point> boundaries =new List<_Point>(){new _Point(0,0),new _Point(0,450), new _Point(450,450), new _Point(450,0)};
            m.OutlinePolygonPoints = boundaries;
            List<System.Windows.Point> convertedPointsForPolygon = boundaries.Select(i => new System.Windows.Point(i.X, i.Y)).ToList();
            m.AvailableOutlinePolygon = new Polygon(){Points = new PointCollection(convertedPointsForPolygon)};
            return m;
        }
        public static _Model InitSimpleModel() {
            _Model m = new _Model();
      
            _Point q1 = new _Point(100, 0);
            _Point q2 = new _Point(400, 0);
            _Point p1 = new _Point(100, 100);
            _Point p2 = new _Point(400, 100);
            _Point p3 = new _Point(400, 400);
            _Point p4 = new _Point(100, 400);
            _Line line1 = new _Line(p1, p2);
            _Line line2 = new _Line(p2, p3);
            _Line line3 = new _Line(p3, p4);
            _Line line4 = new _Line(p4, p1);
            _Line l1 = new _Line(q1, p1);
            _Line l2 = new _Line(q2, p2);
            _Line l3 = new _Line(q1, q2);
            _Room first = new _Room { Name = "FirstRoom", Number = 1 , type = _RoomType.Kitchen};
            _Room second = new _Room { Name = "SecondRoom", Number = 2 , type = _RoomType.LivingRoom, isStartRoom = true};

            first.Lines = (new List<_Line>() { line1, line2, line3, line4 });
            second.Lines = (new List<_Line>() { l1, l2, l3, line1 });
            
            m.rooms.Add(first);
            m.rooms.Add(second);

            foreach (var room in m.rooms) {
                room.CanGetBoundarySorted();
            }

            List<_Point> boundaries = new List<_Point>() { new _Point(0, 0), new _Point(0, 900), new _Point(900, 900), new _Point(900, 0) };
            m.OutlinePolygonPoints = boundaries;
            List<System.Windows.Point> convertedPointsForPolygon = boundaries.Select(i => new System.Windows.Point(i.X, i.Y)).ToList();
            m.AvailableOutlinePolygon = new Polygon() { Points = new PointCollection(convertedPointsForPolygon) };

            return m;
        }

        public static _Model InitNormalModel() {
            _Model m = new _Model();
            _Point a1 = new _Point(0, 0);
            _Point a2 = new _Point(200, 0);
            _Point a3 = new _Point(200, 200);
            _Point a4 = new _Point(0, 200);

            _Point a5 = new _Point(200, 400);
            _Point a6 = new _Point(0, 400);
            _Point a7 = new _Point(400, 0);
            _Point a8 = new _Point(400, 200);
            _Point a9 = new _Point(400, 400);

            _Line l12 = new _Line(a1, a2);
            _Line l23 = new _Line(a2, a3);
            _Line l34 = new _Line(a3, a4);
            _Line l41 = new _Line(a4, a1);

            _Line l35 = new _Line(a3, a5);
            _Line l56 = new _Line(a5, a6);
            _Line l64 = new _Line(a6, a4);

            _Line l27 = new _Line(a2, a7);
            _Line l78 = new _Line(a7, a8);
            _Line l83 = new _Line(a8, a3);

            _Line l89 = new _Line(a8, a9);
            _Line l95 = new _Line(a9, a5);


            _Room first = new _Room { Name = "FirstRoom", Number = 1 , type = _RoomType.Kitchen };
            _Room second = new _Room { Name = "SecondRoom", Number = 2 , type = _RoomType.LivingRoom};
            _Room third = new _Room { Name = "ThirdRoom", Number = 3 , type = _RoomType.BedRoom };
            _Room fourth = new _Room { Name = "FourthRoom", Number = 4, type = _RoomType.RestRoom };
            first.Lines = (new List<_Line>() { l12, l23, l34, l41 });
            second.Lines = (new List<_Line>() { l35, l56, l64, l34 });
            third.Lines = (new List<_Line>() { l23, l27, l78, l83 });
            fourth.Lines = (new List<_Line>() { l83, l89, l95, l35 });

            m.rooms.Add(first);
            m.rooms.Add(second);
            m.rooms.Add(third);
            m.rooms.Add(fourth);
            foreach (var room in m.rooms)
            {
                room.CanGetBoundarySorted();
            }
            return m;
        }

        public static _Model InitTestModel() {
            _Model m = new _Model();

            _Point a1 = new _Point(0, 0);
            _Point a2 = new _Point(0, 100);
            _Point a3 = new _Point(0, 200);
            _Point a4 = new _Point(0, 300);

            _Point a5 = new _Point(100, 0);
            _Point a6 = new _Point(100, 100);
            _Point a7 = new _Point(100, 200);
            _Point a8 = new _Point(100, 300);

            _Point a9 = new _Point(200, 0);
            _Point a10 = new _Point(200, 100);
            _Point a11 = new _Point(200, 200);
            _Point a12 = new _Point(200, 300);

            _Point a13 = new _Point(300, 0);
            _Point a14 = new _Point(300, 100);
            _Point a15 = new _Point(300, 200);
            _Point a16 = new _Point(300, 300);



            _Line l12 = new _Line(a1, a2);
            _Line l23 = new _Line(a2, a3);
            _Line l34 = new _Line(a3, a4);

            _Line l56 = new _Line(a5, a6);
            _Line l67 = new _Line(a6, a7);
            _Line l78 = new _Line(a7, a8);

            _Line l910 = new _Line(a9, a10);
            _Line l1011 = new _Line(a10, a11);
            _Line l1112 = new _Line(a11, a12);

            _Line l1314 = new _Line(a13, a14);
            _Line l1415 = new _Line(a14, a15);
            _Line l1516 = new _Line(a15, a16);


            _Line l15 = new _Line(a1, a5);
            _Line l59 = new _Line(a5, a9);
            _Line l913 = new _Line(a9, a13);

            _Line l26 = new _Line(a2, a6);
            _Line l610 = new _Line(a6, a10);
            l610.Name = "key";
            _Line l1014 = new _Line(a10, a14);

            _Line l37 = new _Line(a3, a7);
            _Line l711 = new _Line(a7, a11);
            _Line l1115 = new _Line(a11, a15);

            _Line l48 = new _Line(a4, a8);
            _Line l812 = new _Line(a8, a12);
            _Line l1216 = new _Line(a12, a16);

            _Room first = new _Room { Name = "FirstRoom",   Number = 1, type = _RoomType.Kitchen };
            _Room second = new _Room { Name = "SecondRoom", Number = 1, type = _RoomType.LivingRoom };
            _Room third = new _Room { Name = "ThirdRoom",   Number = 1, type = _RoomType.CorridorRoom };
            _Room fourth = new _Room { Name = "FourthRoom", Number = 1, type = _RoomType.RestRoom };
            _Room fifth = new _Room { Name = "fifthRoom",   Number = 1, type = _RoomType.BedRoom };
            _Room sixth = new _Room { Name = "sixthRoom",     Number = 1, type = _RoomType.BedRoom };
            _Room seventh = new _Room { Name = "seventhRoom", Number = 1, type = _RoomType.BedRoom };
            _Room eight = new _Room { Name = "eightRoom",     Number = 1, type = _RoomType.BedRoom };
            _Room nineth = new _Room { Name = "ninethRoom",  Number = 1, type = _RoomType.BedRoom };;
            second.isStartRoom = true;


            first.Lines = (new List<_Line>() { l12, l26, l56, l15 });
            second.Lines = (new List<_Line>() { l23, l37, l67, l26 });
            third.Lines = (new List<_Line>() { l34, l48, l78, l37 });
            fourth.Lines = (new List<_Line>() { l56, l610, l910, l59 });
            fifth.Lines = (new List<_Line>() { l67, l711, l1011, l610 });
            sixth.Lines = (new List<_Line>() { l78, l812, l1112, l711 });
            seventh.Lines = (new List<_Line>() { l910, l1014, l1314, l913 });
            eight.Lines = (new List<_Line>() { l1011, l1115, l1415, l1014 });
            nineth.Lines = (new List<_Line>() { l1112, l1216, l1115, l1516 });


            m.rooms.Add(first);
            m.rooms.Add(second);
            m.rooms.Add(third);
            m.rooms.Add(fourth);
            m.rooms.Add(fifth);
            m.rooms.Add(sixth);
            m.rooms.Add(seventh);
            m.rooms.Add(eight);
            m.rooms.Add(nineth);

            foreach (var room in m.rooms) {
                room.CanGetBoundarySorted();
            }

            return m;

        }

    }
}
