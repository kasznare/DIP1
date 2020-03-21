using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIWPF.Model;

namespace UIWPF.Services {
    public class ModelConfigurations {
        public _Model InitSimplestModel()
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
            _Room _Room = new _Room();
            _Room.Name = "FirstRoom";
            _Room.Number = 1;
            _Room.lines = ( new List<_Line>(){line1,line2,line3,line4});
            m.modelStorage.Add(_Room);
            return m;
        }

    }
}
