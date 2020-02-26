using System;
using System.Collections.Generic;
using System.Linq;
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
        //[Attribute a=1]
        public void MoveLineInAllModel()
        {
            m=LoadModelFromJsonString();
            Line l = Find200200400200Line();
            m.MoveLine(10,l);
            Assert m.IsInInvalidState.Equals(false);
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
            m.InitTestModel();
            List<Model> newModels = new List<Model>();
            newModels.Add(m);
            List<Model> allModels = new List<Model>();
            while (newModels.Any())
            {
                List<Model> loop = new List<Model>();
                foreach (Model model in newModels)
                {
                    List<Model> currentModels = AllRoomPairs(model);

                    foreach (Model currentModel in currentModels)
                    {
                        if (currentModel.modelRooms.Count > 1)
                        {
                            loop.Add(currentModel);
                        }
                    }

                    allModels.AddRange(currentModels);
                }

                newModels = loop;
            }

            foreach (Model model in allModels)
            {
                Ommitsteps(model);
            }

            SaveHistoryModel();
        }

        public List<Model> AllRoomPairs(Model m_mod)
        {
            List<Model> storage = new List<Model>();
            for (var i = 0; i < m_mod.modelRooms.Count; i++)
            {
                MyRoom room = m_mod.modelRooms[i];
                for (var j = i+1; j < m_mod.modelRooms.Count; j++)
                {
                    MyRoom modelRoom = m_mod.modelRooms[j];
                    //if (modelRoom.Guid == room.Guid) continue;

                    bool a = DoTheyHaveCommmonWall(room, modelRoom);
                    if (!a) continue;

                    else
                    {
                        Model newModel = MergeRooms(m_mod, room, modelRoom);
                        storage.Add(newModel);
                    }
                }
            }

            return storage;
        }

        private Model MergeRooms(Model mMod, MyRoom room, MyRoom modelRoom)
        {
            Model m=null;
            RemoveCommonWalls();
            MergeBoundaryLineListToSmallerIdRooms();
            RemoveOneRoom();
            return m;
        }

        private bool DoTheyHaveCommmonWall(MyRoom room, MyRoom modelRoom)
        {
            if (room.BoundaryLines.Intersect(modelRoom.BoundaryLines).Any()) return true;
            return false;
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
