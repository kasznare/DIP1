using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIWPF.Model;

namespace UIWPF.Services {
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
            _Room _Room = new _Room {Name = "FirstRoom", Number = 1};
            _Room.Lines = ( new List<_Line>(){line1,line2,line3,line4});
            m.rooms.Add(_Room);
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
            _Room first = new _Room { Name = "FirstRoom", Number = 1 };
            _Room second = new _Room { Name = "SecondRoom", Number = 2 };

            first.Lines = (new List<_Line>() { line1, line2, line3, line4 });
            second.Lines = (new List<_Line>() { l1, l2, l3, line1 });
            
            m.rooms.Add(first);
            m.rooms.Add(second);

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


            _Room first = new _Room { Name = "FirstRoom", Number = 1 };
            _Room second = new _Room { Name = "SecondRoom", Number = 2 };
            _Room third = new _Room { Name = "ThirdRoom", Number = 3 };
            _Room fourth = new _Room { Name = "FourthRoom", Number = 4 };
            first.Lines = (new List<_Line>() { l12, l23, l34, l41 });
            second.Lines = (new List<_Line>() { l35, l56, l64, l34 });
            third.Lines = (new List<_Line>() { l23, l27, l78, l83 });
            fourth.Lines = (new List<_Line>() { l83, l89, l95, l35 });

            m.rooms.Add(first);
            m.rooms.Add(second);
            m.rooms.Add(third);
            m.rooms.Add(fourth);

            return m;
        }

    }
}
