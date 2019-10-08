using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.GeometryModel {
    public class Costs {
        public int index { get; set; }
        public double value { get; set; }

        public Costs(int index, double value)
        {
            this.index = index;
            this.value = value;
        }
    }
}
