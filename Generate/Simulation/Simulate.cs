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
        public Action a;
        public event StatusUpdateHandler ModelChanged;
        readonly object locker = new object();
        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 5;
        public int actualSimulationIndex = 0;
        public int moveDistance = 10;
        bool isFinished = false;
        bool isTimeout = false;
        bool isTreshold = false;
        double actualCost=1000000;

        public List<Model> modelCopyHistory = new List<Model>();
        //public Dictionary<Action, double> ActionsByCosts = new Dictionary<Action, double>();
        public List<Action> Actions = new List<Action>();
        public List<Dictionary<Action, double>> history = new List<Dictionary<Action, double>>();
        public void run() {
            Stopwatch st = new Stopwatch();
            st.Start();
            while (true && !isFinished && !isTimeout && !isTreshold) {
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
            MessageBox.Show($"Run Ended. Finished: {isFinished} , Timeout: {isTimeout}, Treshold: {isTreshold}");
        }

        private void HandleModelChangeUpdate() {
            if (ModelChanged == null) return;
            ProgressEventArgs args = new ProgressEventArgs(model, actualCost, actualSimulationIndex);
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
                        double cost = tempModel.CalculateCost().First();
                        lock (locker) {
                            Actions.Add(new Switch(r1, r2, cost));
                        }
                    });
                });
        }
        private void CalculateMoveCosts() {

            Parallel.For(0, model.modelLines.Count,
                index => {
                    MyLine myLine = model.modelLines.ElementAt(index);
                    MyLine newMyLine = null;
                    Model tempModel = model.DeepCopy(myLine, out newMyLine);
                    tempModel.MoveLine(moveDistance, newMyLine);
                    double cost = tempModel.CalculateCost().First();
                    lock (locker) {
                        Actions.Add(new Move(myLine, cost, moveDistance));
                    }
                });
            Parallel.For(0, model.modelLines.Count,
                index => {
                    MyLine myLine = model.modelLines.ElementAt(index);
                    MyLine newMyLine = null;
                    Model tempModel = model.DeepCopy(myLine, out newMyLine);
                    tempModel.MoveLine(-moveDistance, newMyLine);
                    double cost = tempModel.CalculateCost().First();
                    lock (locker) {
                        Actions.Add(new Move(myLine, cost, -moveDistance));
                    }
                });
        }

        public void SaveState() {

        }
        public void LoadState() {

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

        private Action FindStep() {
            Action a = Actions.OrderBy(i => i.cost).FirstOrDefault();
            if (actualCost > a.cost) {
                actualCost = a.cost;
                return a;
            }
            return null;
        }
    }
    public class ProgressEventArgs : EventArgs {
        public Model model { get; private set; }
        public double cost { get; private set; }
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
    }
    public class Move : Action {
        private MyLine myLine;
        private int moveDistance;
        public Move(MyLine myLine, double cost, int moveDistance) {
            this.myLine = myLine;
            this.cost = cost;
            this.moveDistance = moveDistance;
        }
        public override void Step(Model m) {
            m.MoveLine(moveDistance, myLine);
        }
    }
    public class Split : Action {
        public override void Step(Model m) {
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
