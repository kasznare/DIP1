using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.GeometryModel {
    public class Costs {
        public int Index { get; set; }
        public double SummaryCost { get; set; }

        public double AreaCost { get; set; }

        public double LayoutCost { get; set; }

        public double ConstaintCost { get; set; }

        public Simulation.Action lastAction { get; set; }

        public Costs(int index, double summary, double areacost, double layoutcost, double constaintcost, Simulation.Action a=null)
        {
            this.Index = index;
            this.SummaryCost = summary;
            AreaCost = areacost;
            LayoutCost = layoutcost;
            ConstaintCost = constaintcost;
            lastAction = a;
        }

    }
}
