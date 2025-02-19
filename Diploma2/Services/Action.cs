﻿using System.Collections.Generic;
using Diploma2.Model;
using Diploma2.Utilities;

namespace Diploma2.Services
{
    public abstract class Action {
        public abstract void Step(_Model m);
        public double cost;
        public double areacost;
        public double layoutcost;
        public double constaintCost;
        public Cost Cost;
        public override string ToString() {
            return this.GetType() + "-" + cost.ToString();
        }
    }

    public class Move : Action {
        private _Line myLine;
        private int moveDistance;
        private double summary;

        public Move(_Line myLine, int moveDistance) {
            this.myLine = myLine;
            this.moveDistance = moveDistance;
        }
        public Move(_Line myLine, double cost, int moveDistance) {
            this.myLine = myLine;
            this.cost = cost;
            this.moveDistance = moveDistance;
        }

        public Move(_Line myLine, double summary, double areacost, double layoutcost, int moveDistance) {
            this.myLine = myLine;
            this.cost = summary;
            this.areacost = areacost;
            this.layoutcost = layoutcost;
            this.moveDistance = moveDistance;
        }
        public Move(_Line myLine, Cost c, int moveDistance) {
            this.myLine = myLine;
            this.cost = c.SummaryCost;
            this.areacost = c.AreaCost;
            this.layoutcost = c.LayoutCost;
            this.constaintCost = c.ConstaintCost;
            this.moveDistance = moveDistance;
            Cost = c;
        }

        public override void Step(_Model m) {
            //Logger.WriteLog("Before transform" + myLine);
            m.MoveLine(moveDistance, myLine);
            //Logger.WriteLog("After transform" + myLine);
        }
    }
    public class Split : Action {
        private _Line myLine;
        private int split;
        public Split(int splitPercentage, _Line lineGridSelectedItem) {
            split = splitPercentage;
            myLine = lineGridSelectedItem;
        }

        public override void Step(_Model m) {
            m.SplitLine(split, myLine);
        }
    }

    public class MakeDoor : Action
    {
        private List<_Line> makeDoor;

        public MakeDoor(List<_Line> makeDoor, Cost cost)
        {
            this.makeDoor = makeDoor;
            Cost = cost;
        }
        public override void Step(_Model m)
        {
            m.MakeLineDoor(makeDoor);
        }

        public override string ToString()
        {
            return "Makedoor_"+Cost.SummaryCost;
        }
    }
    public class Switch : Action {
        private _Room r1;
        private _Room r2;

        //public Switch(ref _Room r1, ref _Room r2, double cost) {
        //    this.r1 = r1;
        //    this.r2 = r2;
        //    this.cost = cost;
        //}
        public Switch(ref _Room r1, ref _Room r2, Cost cost) {
            this.r1 = r1;
            this.r2 = r2;
            this.cost = cost.SummaryCost;
            Cost = cost;
        }

        public Switch(ref _Room r1, ref _Room r2) {
            this.r1 = r1;
            this.r2 = r2;
        }

        public override void Step(_Model m) {
            m.SwitchRooms(ref r1, ref r2);
        }
    }
}