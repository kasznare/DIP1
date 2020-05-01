using Diploma2.Services;

namespace Diploma2.Model {
    public class Cost {
        public int Index { get; set; }
        public double SummaryCost {
            get { return AreaCost + LayoutCost + ConstaintCost; }
        }

        public double AreaCost { get; set; }

        public double LayoutCost { get; set; }

        public double ConstaintCost { get; set; }

        public Action lastAction { get; set; }

        public Cost(int index, double summary, double areacost, double layoutcost, double constaintcost, Action a=null)
        {
            this.Index = index;
            AreaCost = areacost;
            LayoutCost = layoutcost;
            ConstaintCost = constaintcost;
            lastAction = a;
        }

    }
}
