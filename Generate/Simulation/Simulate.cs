﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Simulation {
    public class Simulate {
        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);

        public Model model;
        public Action ActualAction;
        public event StatusUpdateHandler ModelChanged;
        readonly object locker = new object();
        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 20;
        public int actualSimulationIndex = 0;
        public int MaxSimulationIndex = 2000;
        public int moveDistance = 10;
        bool isFinished = false;
        bool isTimeout = false;
        bool isTreshold = false;
        double actualCost = 1000000;
        private double actualAreaCost;
        private double actualLayoutCost;

        public List<Model> modelCopyHistory = new List<Model>();
        //public Dictionary<Action, double> ActionsByCosts = new Dictionary<Action, double>();
        public List<Action> Actions = new List<Action>();
        public List<Dictionary<Action, double>> history = new List<Dictionary<Action, double>>();
        public bool IsStopped { get; set; }

        public void run() {
            Stopwatch st = new Stopwatch();
            st.Start();
            while (true && !isFinished && !isTimeout && !isTreshold && !IsStopped) {
                Actions.Clear();
                SaveState();
                actualCost = CostCalculationService.CalculateCost(model).ElementAt(0);
                CalculateCostsForState();
                MakeAStepByTheCalculatedCosts();
                HandleModelChangeUpdate();
                Thread.Sleep(5);
                actualSimulationIndex++;
                if (actualSimulationIndex > MaxSimulationIndex) {
                    isFinished = true;
                }
                if (st.ElapsedMilliseconds > 60000) {
                    isTimeout = true;
                }
                if (actualSimulationThreshold >= MaxSimulationThreshold) {
                    isTreshold = true;
                }
            }

            Logger.WriteLog($"Run Ended.\nFinished: {isFinished}\nTimeout: {isTimeout}\nTreshold: {isTreshold}\nStopped manually: {IsStopped}");
            //actualSimulationIndex = 0;
            actualSimulationThreshold = 0;
            isFinished = false;
            isTimeout = false;
            isTreshold = false;
            IsStopped = false;
            MaxSimulationIndex += MaxSimulationIndex;

        }

        private void HandleModelChangeUpdate() {
            if (ModelChanged == null) return;
            ProgressEventArgs args = new ProgressEventArgs(model, actualCost, actualSimulationIndex);
            args.areacost = actualAreaCost;
            args.layoutcost = actualLayoutCost;
            args.stepAction = ActualAction;
            ModelChanged(this, args);
        }

        private void CalculateCostsForState() {
            //ActionsByCosts = new Dictionary<Action, double>();

            CalculateMoveCosts();

            //CalculateSplitCosts();
            CalculateSwitchCosts();
        }
        private void CalculateSplitCosts() {
            throw new NotImplementedException();
        }
        private void CalculateSwitchCosts() {
            int rooms = model.modelRooms.Count;
            //Parallel.For(0, rooms, index => {
            for (int index = 0; index < rooms; index++) {
                for (int secondindex = index + 1; secondindex < rooms; secondindex++) {
                    //Parallel.For(index + 1, rooms, secondindex => {
                    MyRoom r1 = model.modelRooms.ElementAt(index);
                    MyRoom r2 = model.modelRooms.ElementAt(secondindex);
                    MyRoom r1target = null;
                    MyRoom r2target = null;
                    Model tempModel = model.DeepCopy(r1, r2, out r1target, out r2target);
                    tempModel.SwitchRooms(ref r1target, ref r2target);
                    if (!tempModel.IsInInvalidState) {
                        double cost = CostCalculationService.CalculateCost(tempModel).First();
                        lock (locker) {
                            Actions.Add(new Switch(ref r1, ref r2, cost));
                        }
                    }
                    //});
                }
            }
            //});
        }
        private void CalculateMoveCosts() {

            //Parallel.For(0, model.modelLines.Count,
            //    index => {
            for (int index = 0; index < model.modelLines.Count; index++) {

                MyLine myLine = model.modelLines.ElementAt(index);
                MyLine newMyLine = null;
                Model tempModel = model.DeepCopy(myLine, out newMyLine);
                tempModel.MoveLine(moveDistance, newMyLine);
                if (tempModel.IsInInvalidState) continue;

                double[] costs = CostCalculationService.CalculateCost(tempModel);
                double summary = costs[0];
                double areacost = costs[1];
                double layoutcost = costs[2];
                lock (locker) {
                    Actions.Add(new Move(myLine, summary, areacost, layoutcost, moveDistance));
                }
            }
            //});
            //Parallel.For(0, model.modelLines.Count,
            //    index => {
            for (int index = 0; index < model.modelLines.Count; index++) {

                MyLine myLine = model.modelLines.ElementAt(index);
                MyLine newMyLine = null;
                Model tempModel = model.DeepCopy(myLine, out newMyLine);
                tempModel.MoveLine(-moveDistance, newMyLine);
                if (tempModel.IsInInvalidState) continue;
                double[] costs = CostCalculationService.CalculateCost(tempModel);
                double summary = costs[0];
                double areacost = costs[1];
                double layoutcost = costs[2];
                lock (locker) {
                    Actions.Add(new Move(myLine, summary, areacost, layoutcost, -moveDistance));
                }
            }
            //});
        }

        public void SaveState() {
            modelCopyHistory.Add(model.DeepCopy());
        }
        public void LoadState(Model m) {
            model = m;
        }

        private void MakeAStepByTheCalculatedCosts() {
            Action a = FindStep();
            if (a != null) {
                a.Step(model);
            }
            else {
                actualSimulationThreshold++;
            }
        }

        Random r = new Random(30);
        private Action FindStep() {
            List<Action> sorted = Actions.OrderBy(i => i.cost).ToList();
            //Action a = sorted.FirstOrDefault();
            int j = r.Next(0, Math.Min(5, sorted.Count));
            ActualAction = sorted.ElementAt(j);
            if (actualCost >= ActualAction.cost) {
                actualCost = ActualAction.cost;
                actualAreaCost = ActualAction.areacost;
                actualLayoutCost = ActualAction.layoutcost;
                return ActualAction;
            }
            return null;
        }

        public void Split(int splitPercentage, MyLine lineGridSelectedItem) {
            Action a = new Split(splitPercentage, lineGridSelectedItem);
            a.Step(model);
            double[] costs = CostCalculationService.CalculateCost(model);

            actualCost = costs[0];
            actualAreaCost = costs[1];
            actualLayoutCost = costs[2];
            HandleModelChangeUpdate();
        }
        public void Move(MyLine lineGridSelectedItem, int movedistance) {
            Action a = new Move(lineGridSelectedItem, movedistance);
            a.Step(model);
            double[] costs = CostCalculationService.CalculateCost(model);

            actualCost = costs[0];
            actualAreaCost = costs[1];
            actualLayoutCost = costs[2];
            HandleModelChangeUpdate();
        }

        public void SwitchRoom(ref MyRoom r1, ref MyRoom r2) {
            Action a = new Switch(ref r1, ref r2);
            a.Step(model);
            double[] costs = CostCalculationService.CalculateCost(model);

            actualCost = costs[0];
            actualAreaCost = costs[1];
            actualLayoutCost = costs[2];
            HandleModelChangeUpdate();
        }
    }
    public class ProgressEventArgs : EventArgs {
        public Model model { get; private set; }
        public Action stepAction { get; set; }
        public double cost { get; private set; }
        public double areacost { get; set; }
        public double layoutcost { get; set; }

        public int simIndex { get; private set; }
        public ProgressEventArgs(Model status, double actualcost, int index) {
            cost = actualcost;
            model = status;
            simIndex = index;
        }
    }

    public abstract class Action {
        public abstract void Step(Model m);
        public double cost;
        public double areacost;
        public double layoutcost;
        public override string ToString() {
            return this.GetType() + "-" + cost.ToString();
        }
    }
    public class Move : Action {
        private MyLine myLine;
        private int moveDistance;
        private double summary;

        public Move(MyLine myLine, int moveDistance) {
            this.myLine = myLine;
            this.moveDistance = moveDistance;
        }
        public Move(MyLine myLine, double cost, int moveDistance) {
            this.myLine = myLine;
            this.cost = cost;
            this.moveDistance = moveDistance;
        }

        public Move(MyLine myLine, double summary, double areacost, double layoutcost, int moveDistance) {
            this.myLine = myLine;
            this.cost = summary;
            this.areacost = areacost;
            this.layoutcost = layoutcost;
            this.moveDistance = moveDistance;
        }

        public override void Step(Model m) {
            Logger.WriteLog("Before transform"+myLine);
            m.MoveLine(moveDistance, myLine);
            Logger.WriteLog("After transform"+myLine);
        }
    }
    public class Split : Action {
        private MyLine myLine;
        private int split;
        public Split(int splitPercentage, MyLine lineGridSelectedItem) {
            split = splitPercentage;
            myLine = lineGridSelectedItem;
        }

        public override void Step(Model m) {
            m.SplitEdge(split, myLine);
        }
    }
    public class Switch : Action {
        private MyRoom r1;
        private MyRoom r2;

        public Switch(ref MyRoom r1, ref MyRoom r2, double cost) {
            this.r1 = r1;
            this.r2 = r2;
            this.cost = cost;
        }

        public Switch(ref MyRoom r1, ref MyRoom r2) {
            this.r1 = r1;
            this.r2 = r2;
        }

        public override void Step(Model m) {
            m.SwitchRooms(ref r1, ref r2);
        }
    }
}
