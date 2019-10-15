using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.GeometryModel {
    public class LineAndCost {
        public string line { get; set; }
        public double cost { get; set; }
        public int index { get; set; }

        public LineAndCost(MyLine l, double d, int i) {
            line = l.ToString();
            cost = d;
            index = i;
        }
        public LineAndCost(string l, double d, int i)
        {
            line = l;
            cost = d;
            index = i;
        }
    }
}
