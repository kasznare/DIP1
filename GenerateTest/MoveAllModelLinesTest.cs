using System;
using WindowsFormsApp1;
using WindowsFormsApp1.Simulation;
using Newtonsoft.Json;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GenerateTest {
    public class MoveAllModelLinesTest {
        Model m;
        ModelStorage ms = new ModelStorage();

        [SetUp]
        public void Setup() {
            m = new Model();
        }

        [Test]
        public void MoveEveryLineInSimple() {
            m.InitSimpleModel();
            MyLine l = null;
            for (var i = 0; i < m.modelLines.Count; i++) {
                l = m.modelLines[i];
                m.MoveLine(10, l);
                m.MoveLine(-10, l);
                m.MoveLine(-10, l);
                m.MoveLine(10, l);
            }
        }

        [Test]
        public void MoveEveryLineInNormal() {
            m.InitNormalModel();
            MyLine l = null;
            for (var i = 0; i < m.modelLines.Count; i++) {
                l = m.modelLines[i];
                m.MoveLine(10, l);
                m.MoveLine(-10, l);
                m.MoveLine(-10, l);
                m.MoveLine(10, l);
            }
        }

        [Test]
        public void MoveEveryLineInAdvanced() {
            m.InitAdvancedModel();
            MyLine l = null;
            for (var i = 0; i < m.modelLines.Count; i++) {
                l = m.modelLines[i];
                m.MoveLine(10, l);
                m.MoveLine(-10, l);
            }
        }

        [Test]
        public void MoveEveryLineInFactory() {
            m.InitModelWithGivenRooms();
            MyLine l = null;
            for (var i = 0; i < m.modelLines.Count; i++) {
                l = m.modelLines[i];
                m.MoveLine(10, l);
                m.MoveLine(-10, l);
            }
        }

        [Test]
        public void AllCasesTestGeneration()
        {

        }


        public void Ommitsteps(Model m_mod)
        {
            ms.AddModel(m_mod);

            bool finished = ExitCondition(m_mod);
            if (finished) return;
            m_mod=Ommit(m_mod);
            Ommitsteps(m_mod);

        }

        private Model Ommit(Model mMod)
        {
            throw new NotImplementedException();
        }

        private bool ExitCondition(Model model)
        {
            if (m.modelRooms.Count == 1) return true;
            else return false;
        }

        private void SaveHistoryModel()
        {
            string s = JsonConvert.SerializeObject(ms);
        }


        [TearDown]
        public void Teardown() {
            m = null;
        }
    }
}
