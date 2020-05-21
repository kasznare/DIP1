using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Diploma2.Model;
using Diploma2.Utilities;

namespace Diploma2.Services
{
    public class Simulation
    {
        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
        public _Model Model { get; set; }
        public Action ActualAction { get; set; }
        public event StatusUpdateHandler ModelChanged;
        readonly object locker = new object();

        int ActualTreshold = 0;
        int MaxTreshold = 30;
        int CurrentIndex = 0;
        int MaxIndex = 30;
        int baseMoveDistance = 10;
        private int maxSeconds = 10;

        ExitCondition exitCondition = ExitCondition.Running;

        Cost actualCost = new Cost(0, 10000000, 10000000, 0, 0);

        public ObservableCollection<_Model> modelCopyHistory = new ObservableCollection<_Model>();
        public Dictionary<Action, Cost> ActionsByCosts = new Dictionary<Action, Cost>();
        public List<Action> Actions = new List<Action>();
        public List<Action> DoorActions = new List<Action>();
        public List<Dictionary<Action, Cost>> history = new List<Dictionary<Action, Cost>>();

        Stopwatch st = new Stopwatch();
        public void RunSteps()
        {
            st.Start();

            while (exitCondition == ExitCondition.Running)
            {
                Actions.Clear();
                DoorActions.Clear();
                SaveState();
                actualCost = CostCalculationService.CalculateCostNew(Model);
                actualCost.Index = CurrentIndex;
                CalculateCostsForState();
                
                bool m = Model.IsInInvalidState;
               
                MakeAStepByTheCalculatedCosts();
               
                //CalculateDoorCosts();
                //MakeStepByDoorChanges();

                HandleModelChangeUpdate();

                if (CurrentIndex >= MaxIndex)
                {
                    exitCondition = ExitCondition.isFinished;
                }
                if (st.ElapsedMilliseconds > maxSeconds*1000)
                {
                    exitCondition = ExitCondition.isTimeout;
                }
                if (ActualTreshold >= MaxTreshold)
                {
                    exitCondition = ExitCondition.isTreshold;
                }
                CurrentIndex++;
                Thread.Sleep(5);
            }

            Logger.WriteLog($"Run Ended.\nExitCondition : {exitCondition}");
            exitCondition = ExitCondition.Running;
            ActualTreshold = 0;
            MaxIndex += MaxIndex;

            st.Reset();
        }




        public void UndoStep()
        {
            Model = modelCopyHistory.ElementAt(CurrentIndex - 1);
            HandleModelChangeUpdate();
        }
        private void HandleModelChangeUpdate()
        {
            if (ModelChanged == null) return;
            ProgressEventArgs args = new ProgressEventArgs(Model, actualCost, CurrentIndex, ActualAction);
            ModelChanged(this, args);
        }

        private void CalculateCostsForState()
        {
            history.Add(ActionsByCosts);
            ActionsByCosts.Clear();
            CalculateMoveCosts();
            //CalculateSplitCosts(); TODO: do I need this anyway??
            CalculateSwitchCosts();
        }

        private void CalculateDoorCosts()
        {
            //calculate every possible scenario, every yesno, the outer lines are not nessesary 2,3,7,8
            _Model tempModel = Model;//.DeepCopy();
            List<_Line> allLinesFlat = tempModel.AllLinesFlat();

            int[] arr = Enumerable.Range(0, allLinesFlat.Count).ToArray();

            //TODO: we could ommit outside lines
            List<int[]> indicesToSetAsDoors = CFG.printCombination(arr, allLinesFlat.Count, Model.rooms.Count);
            for (int index = 0; index < indicesToSetAsDoors.Count; index++)
            {
                List<_Line> currentLines = new List<_Line>();
                int[] elementAt = indicesToSetAsDoors.ElementAt(index);
               

                if (elementAt.Contains(1) && elementAt.Contains(2) && elementAt.Contains(7) && elementAt.Contains(8))
                {
                    int breakpointint = 0;
                }

                foreach (int i in elementAt)
                {
                    currentLines.Add(allLinesFlat.ElementAt(i));
                }
                tempModel.MakeLineDoor(currentLines);
                Cost c = CostCalculationService.CalculateDoorCost(tempModel);

                DoorActions.Add(new MakeDoor(currentLines, c));
            }

            int breakpoint = DoorActions.Count;
        }
        private void CalculateSplitCosts()
        {
            throw new NotImplementedException();
        }
        private void CalculateSwitchCosts()
        {
            int rooms = Model.rooms.Count;
            for (int index = 0; index < rooms; index++)
            {
                for (int secondindex = index + 1; secondindex < rooms; secondindex++)
                {
                    _Room r1 = Model.rooms.ElementAt(index);
                    _Room r2 = Model.rooms.ElementAt(secondindex);
                    _Model tempModel = Model.DeepCopy(r1, r2, out _Room r1target, out _Room r2target);
                    tempModel.SwitchRooms(ref r1target, ref r2target);
                    if (tempModel.IsInInvalidState) continue;

                    Cost cost = CostCalculationService.CalculateCostNew(tempModel);
                    lock (locker)
                    {
                        Actions.Add(new Switch(ref r1, ref r2, cost));
                    }
                }
            }
        }
        private void CalculateMoveCosts()
        {
            List<_Line> allLinesFlat = Model.AllLinesFlat();
            for (int index = 0; index < allLinesFlat.Count; index++)
            {

                _Line myLine = allLinesFlat.ElementAt(index);
                _Model tempModel = Model.DeepCopy(myLine, out _Line newMyLine);
                tempModel.MoveLine(baseMoveDistance, newMyLine);
                if (tempModel.IsInInvalidState) continue;

                Cost costsnew = CostCalculationService.CalculateCostNew(tempModel);

                lock (locker)
                {
                    Actions.Add(new Move(myLine, costsnew, baseMoveDistance));
                }
            }

            for (int index = 0; index < allLinesFlat.Count; index++)
            {

                _Line myLine = allLinesFlat.ElementAt(index);
                _Model tempModel = Model.DeepCopy(myLine, out _Line newMyLine);
                tempModel.MoveLine(-baseMoveDistance, newMyLine);
                if (tempModel.IsInInvalidState) continue;

                Cost costsnew = CostCalculationService.CalculateCostNew(tempModel);

                lock (locker)
                {
                    Actions.Add(new Move(myLine, costsnew, -baseMoveDistance));
                }
            }

        }

        public void SaveState()
        {
            modelCopyHistory.Add(Model.DeepCopy());
        }
        private void MakeStepByDoorChanges()
        {
            List<Action> sorted = DoorActions.OrderBy(i => i.Cost.SummaryCost).ToList();
            Action a = sorted.ElementAt(0);
            if (a != null)
            {
                a.Step(Model);
            }
            else
            {
                ActualTreshold++;
            }
        }
        private void MakeAStepByTheCalculatedCosts()
        {
            Action a = FindStep();
            if (a != null)
            {
                a.Step(Model);
            }
            else
            {
                ActualTreshold++;
            }
        }

        Random r = new Random(30);
        private Action FindStep()
        {
            List<Action> sorted = Actions.OrderBy(i => i.Cost.SummaryCost).ToList(); //TODO: is it descending or ascending??? the simulation converges...
            //Action a = sorted.FirstOrDefault();

            if (!sorted.Any())
            {
                return null;
            }
            int j = r.Next(0, Math.Min(10, sorted.Count));
            ActualAction = sorted.ElementAt(j);
            //TODO: here maybe we should return
            if (true || actualCost.SummaryCost >= ActualAction.Cost.SummaryCost)
            {
                actualCost = ActualAction.Cost;
                return ActualAction;
            }
            return null;
        }

        //public void Split(int splitPercentage, _Line lineGridSelectedItem) {
        //    Action a = new Split(splitPercentage, lineGridSelectedItem);
        //    a.Step(Model);
        //    double[] costs = CostCalculationService.CalculateCost(Model);

        //    actualCost = costs[0];
        //    actualAreaCost = costs[1];
        //    actualLayoutCost = costs[2];
        //    HandleModelChangeUpdate();
        //}
        //public void Move(_Line lineGridSelectedItem, int movedistance) {
        //    Action a = new Move(lineGridSelectedItem, movedistance);
        //    a.Step(Model);
        //    double[] costs = CostCalculationService.CalculateCost(Model);

        //    actualCost = costs[0];
        //    actualAreaCost = costs[1];
        //    actualLayoutCost = costs[2];
        //    HandleModelChangeUpdate();
        //}

        //public void SwitchRoom(ref _Room r1, ref _Room r2) {
        //    Action a = new Switch(ref r1, ref r2);
        //    a.Step(Model);
        //    double[] costs = CostCalculationService.CalculateCost(Model);

        //    actualCost = costs[0];
        //    actualAreaCost = costs[1];
        //    actualLayoutCost = costs[2];
        //    HandleModelChangeUpdate();
        //}
    }
}

