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
            _Room.lines = ( new List<_Line>(){line1,line2,line3,line4});
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

            first.lines = (new List<_Line>() { line1, line2, line3, line4 });
            second.lines = (new List<_Line>() { l1, l2, l3, line1 });
            
            m.rooms.Add(first);
            m.rooms.Add(second);

            return m;
        }

    }
}
