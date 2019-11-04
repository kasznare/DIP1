using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Simulation {
    /// <summary>
    /// this class is resposible for managing the simulation steps
    /// </summary>
    public class Simulation {
        public void DetermineSimulationStep() {

        }

        public void ExecuteOperation(IOperation op, OperationParameters parameters) {

        }
    }

    public class SwitchOperation : IOperation
    {
        public void SimulationStep()
        {
            throw new NotImplementedException();
        }
    }
    public class MoveOperation : IOperation {
        public void SimulationStep()
        {
            throw new NotImplementedException();
        }
    }
    public interface IOperation
    {
        void SimulationStep();
    }

    public class OperationParameters {
    }
}
