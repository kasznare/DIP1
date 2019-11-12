using System;
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
        private int MaxSimulationThreshold = 50;
        public int actualSimulationIndex = 0;
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
                actualCost = model.CalculateCost().ElementAt(0);
                CalculateCostsForState();
                MakeAStepByTheCalculatedCosts();
                HandleModelChangeUpdate();
                Thread.Sleep(50);
                actualSimulationIndex++;
                if (actualSimulationIndex > 1000) {
                    isFinished = true;
                }
                if (st.ElapsedMilliseconds > 10000) {
                    isTimeout = true;
                }
                if (actualSimulationThreshold >= MaxSimulationThreshold) {
                    isTreshold = true;
                }
            }

            MessageBox.Show($"Run Ended.\nFinished: {isFinished}\nTimeout: {isTimeout}\nTreshold: {isTreshold}\nStopped manually: {IsStopped}");
            //actualSimulationIndex = 0;
            actualSimulationThreshold = 0;
            isFinished = false;
            isTimeout = false;
            isTreshold = false;
            IsStopped = false;
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
            Parallel.For(0, rooms,
                index => {
                    Parallel.For(index + 1, rooms, secondindex => {
                        Room r1 = model.modelRooms.ElementAt(index);
                        Room r2 = model.modelRooms.ElementAt(secondindex);
                        Room r1target = null;
                        Room r2target = null;
                        Model tempModel = model.DeepCopy(r1, r2, out r1target, out r2target);
                        tempModel.SwitchRooms(ref r1target, ref r2target);
                        if (!tempModel.IsInInvalidState) {
                            double cost = tempModel.CalculateCost().First();
                            lock (locker) {
                                Actions.Add(new Switch(r1, r2, cost));
                            }
                        }
                    });
                });
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

                double[] costs = tempModel.CalculateCost();
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
                double[] costs = tempModel.CalculateCost();
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
            int j = r.Next(0, Math.Min(0, sorted.Count));
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
            double[] costs = model.CalculateCost();

            actualCost = costs[0];
            actualAreaCost = costs[1];
            actualLayoutCost = costs[2];
            HandleModelChangeUpdate();
        }
        public void Move(MyLine lineGridSelectedItem, int movedistance) {
            Action a = new Move(lineGridSelectedItem, movedistance);
            a.Step(model);
            double[] costs = model.CalculateCost();

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
            m.MoveLine(moveDistance, myLine);
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
        private Room r1;
        private Room r2;

        public Switch(Room r1, Room r2, double cost) {
            this.r1 = r1;
            this.r2 = r2;
            this.cost = cost;
        }
        public override void Step(Model m) {
            m.SwitchRooms(ref r1, ref r2);
        }
    }
}
