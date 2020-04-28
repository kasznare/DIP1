using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Diploma2.Model;
using Diploma2.Utilities;

namespace Diploma2.Services {
    public class Simulation {

        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);

        public _Model Model { get; set; }
        public Action ActualAction { get; set; }
        public event StatusUpdateHandler ModelChanged;
        readonly object locker = new object();

        int ActualTreshold = 0;
        int MaxTreshold = 1;
        int CurrentIndex = 0;
        int MaxIndex = 1;
        int baseMoveDistance = 10;

        bool isFinished = false;
        bool isTimeout = false;
        bool isTreshold = false;
        bool IsStopped  = false;

        double actualCost = 1000000;
        double actualAreaCost;
        double actualLayoutCost;

        public ObservableCollection<_Model> modelCopyHistory = new ObservableCollection<_Model>();
        public Dictionary<Action, double> ActionsByCosts = new Dictionary<Action, double>();
        public List<Action> Actions = new List<Action>();
        public List<Dictionary<Action, double>> history = new List<Dictionary<Action, double>>();
        
        Stopwatch st = new Stopwatch();
        public void RunSteps() {
            st.Start();

            while (!isFinished && !isTimeout && !isTreshold && !IsStopped) {
                Actions.Clear();
                SaveState();
                actualCost = CostCalculationService.CalculateCost(Model).ElementAt(0);
                CalculateCostsForState();
                MakeAStepByTheCalculatedCosts();
                HandleModelChangeUpdate();
                Thread.Sleep(5);
                CurrentIndex++;
                if (CurrentIndex > MaxIndex) {
                    isFinished = true;
                }
                if (st.ElapsedMilliseconds > 60000) {
                    isTimeout = true;
                }
                if (ActualTreshold >= MaxTreshold) {
                    isTreshold = true;
                }
            }

            Logger.WriteLog($"Run Ended.\nFinished: {isFinished}\nTimeout: {isTimeout}\nTreshold: {isTreshold}\nStopped manually: {IsStopped}");
            
            ActualTreshold = 0;
            isFinished = false;
            isTimeout = false;
            isTreshold = false;
            IsStopped = false;
            MaxIndex += MaxIndex;

            st.Reset();
        }

        public void UndoStep()
        {
            LoadState(modelCopyHistory.ElementAt(CurrentIndex-1));
            HandleModelChangeUpdate();
        }
        private void HandleModelChangeUpdate() {
            if (ModelChanged == null) return;
            ProgressEventArgs args = new ProgressEventArgs(Model, actualCost, CurrentIndex);
            args.areacost = actualAreaCost;
            args.layoutcost = actualLayoutCost;
            args.stepAction = ActualAction;
            ModelChanged(this, args);
        }

        private void CalculateCostsForState() {
            ActionsByCosts = new Dictionary<Action, double>();

            CalculateMoveCosts();

            //CalculateSplitCosts();
            CalculateSwitchCosts();
        }
        private void CalculateSplitCosts() {
            throw new NotImplementedException();
        }
        private void CalculateSwitchCosts() {
            int rooms = Model.rooms.Count;
            //Parallel.For(0, rooms, index => {
            for (int index = 0; index < rooms; index++) {
                for (int secondindex = index + 1; secondindex < rooms; secondindex++) {
                    //Parallel.For(index + 1, rooms, secondindex => {
                    _Room r1 = Model.rooms.ElementAt(index);
                    _Room r2 = Model.rooms.ElementAt(secondindex);
                    _Room r1target = null;
                    _Room r2target = null;
                    _Model tempModel = Model.DeepCopy(r1, r2, out r1target, out r2target);
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

            //Parallel.For(0, Model.modelLines.Count,
            //    index => {
            var allLinesFlat = Model.AllLinesFlat();
            for (int index = 0; index < allLinesFlat.Count; index++) {

                _Line myLine = allLinesFlat.ElementAt(index);
                _Line newMyLine = null;
                _Model tempModel = Model.DeepCopy(myLine, out newMyLine);
                tempModel.MoveLine(baseMoveDistance, newMyLine);
                if (tempModel.IsInInvalidState) continue;

                double[] costs = CostCalculationService.CalculateCost(tempModel);
                double summary = costs[0];
                double areacost = costs[1];
                double layoutcost = costs[2];
                lock (locker) {
                    Actions.Add(new Move(myLine, summary, areacost, layoutcost, baseMoveDistance));
                }
            }
            //});
            //Parallel.For(0, Model.modelLines.Count,
            //    index => {
            for (int index = 0; index < allLinesFlat.Count; index++) {

                _Line myLine = allLinesFlat.ElementAt(index);
                _Line newMyLine = null;
                _Model tempModel = Model.DeepCopy(myLine, out newMyLine);
                tempModel.MoveLine(-baseMoveDistance, newMyLine);
                if (tempModel.IsInInvalidState) continue;
                double[] costs = CostCalculationService.CalculateCost(tempModel);
                double summary = costs[0];
                double areacost = costs[1];
                double layoutcost = costs[2];
                lock (locker) {
                    Actions.Add(new Move(myLine, summary, areacost, layoutcost, -baseMoveDistance));
                }
            }
            //});
        }

        public void SaveState() {
            modelCopyHistory.Add(Model.DeepCopy());
        }
        public void LoadState(_Model m) {
            Model = m;
        }

        private void MakeAStepByTheCalculatedCosts() {
            Action a = FindStep();
            if (a != null) {
                a.Step(Model);
            }
            else {
                ActualTreshold++;
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

        public void Split(int splitPercentage, _Line lineGridSelectedItem) {
            Action a = new Split(splitPercentage, lineGridSelectedItem);
            a.Step(Model);
            double[] costs = CostCalculationService.CalculateCost(Model);

            actualCost = costs[0];
            actualAreaCost = costs[1];
            actualLayoutCost = costs[2];
            HandleModelChangeUpdate();
        }
        public void Move(_Line lineGridSelectedItem, int movedistance) {
            Action a = new Move(lineGridSelectedItem, movedistance);
            a.Step(Model);
            double[] costs = CostCalculationService.CalculateCost(Model);

            actualCost = costs[0];
            actualAreaCost = costs[1];
            actualLayoutCost = costs[2];
            HandleModelChangeUpdate();
        }

        public void SwitchRoom(ref _Room r1, ref _Room r2) {
            Action a = new Switch(ref r1, ref r2);
            a.Step(Model);
            double[] costs = CostCalculationService.CalculateCost(Model);

            actualCost = costs[0];
            actualAreaCost = costs[1];
            actualLayoutCost = costs[2];
            HandleModelChangeUpdate();
        }
    }
}
