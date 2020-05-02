using System.Collections.Generic;
using Diploma2.Model;

namespace Diploma2
{
    internal class ModelStorage {
        private List<_Model> history = new List<_Model>();
        public ModelStorage() {
        }

        public void AddModel(_Model m) {
            bool isEqual = false;
            foreach (_Model model in history) {
                isEqual = CheckEquivalence(m, model);
                if (isEqual) break;
                if (history.Count > 100000)
                {
                    isEqual = true;
                    break;
                }
            }

            if (!isEqual) {
                history.Add(m);
            }

        }

        public List<_Model> getHistory() {
            return history;
        }
        private bool CheckEquivalence(_Model model, _Model model1) {
            //if (model.rooms.Count == model1.rooms.Count)
            //{
            //    if (model.AllLinesFlat().Count == model1.AllLinesFlat().Count)
            //    {
            //        return true;
            //    }
            //}
            //TODO: implement equivalence checking, but test generation is complete even when redundant.
            return false;
        }
    }
}