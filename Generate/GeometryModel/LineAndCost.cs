using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.GeometryModel {
    public class LineAndCost {
        public MyLine line { get; set; }
        public double cost { get; set; }
        public int index { get; set; }

        public LineAndCost(MyLine l, double d, int i) {
            line = l;
            cost = d;
            index = i;
        }
    }
}
