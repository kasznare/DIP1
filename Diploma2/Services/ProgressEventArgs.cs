using System;
using Diploma2.Model;

namespace Diploma2.Services
{
    public class ProgressEventArgs : EventArgs {
        public _Model model { get; private set; }
        public Action stepAction { get; set; }
        public Cost Cost { get; set; }
        public double summarycost { get; private set; }
        public double areacost { get; set; }
        public double layoutcost { get; set; }

        public int simIndex { get; private set; }
        public ProgressEventArgs(_Model status, Cost c, int index) {
            //summarycost = actualcost;
            Cost = c;
            model = status;
            simIndex = index;
        }
    }
}