using System.Collections.Generic;
using WindowsFormsApp1;

namespace GenerateTest {
    internal class ModelStorage {
        private List<Model> history = new List<Model>();
        public ModelStorage() {
        }

        public void AddModel(Model m) {
            bool isEqual=false;
            foreach (Model model in history) {
                isEqual = CheckEquivalence(m, model);
                if (isEqual) break;
            }

            if (!isEqual)
            {
                history.Add(m);
            }

        }

        public List<Model> getHistory()
        {
            return history;
        }
        private bool CheckEquivalence(Model model, Model model1)
        {
            //TODO: implement equivalence checking, but test generation is complete even when redundant.
            return false;
        }
    }
}