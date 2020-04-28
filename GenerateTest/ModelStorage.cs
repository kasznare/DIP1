using System.Collections.Generic;
using WindowsFormsApp1;
using Diploma2.Model;
using NUnit.Framework;

namespace GenerateTest {
    internal class ModelStorage {
        private List<_Model> history = new List<_Model>();
        public ModelStorage() {
        }

        public void AddModel(_Model m) {
            bool isEqual=false;
            foreach (_Model model in history) {
                isEqual = CheckEquivalence(m, model);
                if (isEqual) break;
            }

            if (!isEqual)
            {
                history.Add(m);
            }

        }

        public List<_Model> getHistory()
        {
            return history;
        }
        private bool CheckEquivalence(_Model model, _Model model1)
        {
            //TODO: implement equivalence checking, but test generation is complete even when redundant.
            return false;
        }
    }
}