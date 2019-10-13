using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Simulation {
    public class Simulate {
        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);

        public Model model;
        public ActionParameters ap;
        public Action a;
        public event StatusUpdateHandler ModelChanged;
        readonly object locker = new object();
        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 5;
        public int actualSimulationIndex = 0;
        public int moveDistance = 10;


        public Dictionary<Action, double> ActionsByCosts = new Dictionary<Action, double>();

        public void run() {
            CalculateCostsForState();
            MakeAStepByTheCalculatedCosts(a, ap);
            HandleModelChangeUpdate();
        }

        private void HandleModelChangeUpdate() {
            if (ModelChanged == null) return;
            ProgressEventArgs args = new ProgressEventArgs(model);
            ModelChanged(this, args);
        }

        private void CalculateCostsForState() {
            CalculateMoveCosts();
            //CalculateSplitCosts();
            CalculateSwitchCosts();
        }
        private void CalculateSplitCosts() {
            throw new NotImplementedException();
        }
        private void CalculateSwitchCosts() {
            Dictionary<Room, double> RoomCosts = new Dictionary<Room, double>();

            double actualCost = model.CalculateCost().First();
            double mincost = actualCost;
            int rooms = model.modelRooms.Count;
            Room switchThisRoomFrom = null;
            Room switchThisRoomTo = null;
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
                            RoomCosts.Add(r1, cost);
                            if (mincost >= cost) {
                                mincost = cost;
                                //this might need to be switched later
                                switchThisRoomFrom = r1;
                                switchThisRoomTo = r2;
                            }
                        }
                    });
                });

            if (mincost >= actualCost) {
                actualSimulationThreshold++;
            }

            double[] costArray = model.CalculateCost();

            actualSimulationIndex++;
        }
        private void CalculateMoveCosts() {

            Dictionary<MyLine, double> Costs = new Dictionary<MyLine, double>();
            MyLine minline = null;
            double actualCost = model.CalculateCost().First();
            double mincost = actualCost;
            Parallel.For(0, model.modelLines.Count,
                index => {
                    MyLine myLine = model.modelLines.ElementAt(index);
                    MyLine newMyLine = null;
                    Model tempModel = model.DeepCopy(myLine, out newMyLine);
                    tempModel.MoveLine(moveDistance, newMyLine);
                    double cost = tempModel.CalculateCost().First();
                    lock (locker) {
                        Costs.Add(myLine, cost);
                        if (mincost > cost) {
                            mincost = cost;
                        }
                    }
                });
            if (mincost >= actualCost) {
                actualSimulationThreshold++;
            }

            
            
            actualSimulationIndex++;

        }

        public void SaveState() {

        }
        public void LoadState() {

        }

        private void MakeAStepByTheCalculatedCosts(Action a, ActionParameters ap) {
            throw new NotImplementedException();
        }
    }
    public class ProgressEventArgs : EventArgs {
        public Model Status { get; private set; }

        public ProgressEventArgs(Model status) {
            Status = status;
        }
    }
    public class ActionParameters {
        public string Name { get; set; }
    }
    public abstract class Action {
        public abstract void Step();
        public ActionParameters parameters;
        public Model model;
    }
    public class Move : Action {
        public override void Step() {
        }
    }
    public class Split : Action {
        public override void Step() {
        }
    }
    public class Switch : Action {
        public override void Step() {
        }
    }
}
