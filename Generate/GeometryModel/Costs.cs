using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.GeometryModel {
    public class Costs {
        public int Index { get; set; }
        public double Value { get; set; }

        public Costs(int index, double value)
        {
            this.Index = index;
            this.Value = value;
        }
    }
}
