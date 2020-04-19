namespace Diploma2.Model {
    public class LineAndCost {
        public string line { get; set; }
        public double cost { get; set; }
        public int index { get; set; }

        public LineAndCost(_Line l, double d, int i) {
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
