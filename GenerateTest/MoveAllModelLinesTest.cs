using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsFormsApp1;
using WindowsFormsApp1.Simulation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        public void MoveLineInAllModel() {
            //m = LoadModelFromJsonString();
            //Line l = Find200200400200Line();
            //m.MoveLine(10, l);
            //Assert m.IsInInvalidState.Equals(false);
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
        public void AllCasesTestGeneration() {
            m.InitTestModel();
            List<Model> newModels = new List<Model>();
            newModels.Add(m);
            List<Model> allModels = new List<Model>();
            while (newModels.Any()) {
                List<Model> loop = new List<Model>();
                foreach (Model model in newModels) {
                    List<Model> currentModels = AllRoomPairs(model);

                    foreach (Model currentModel in currentModels) {
                        if (currentModel.modelRooms.Count > 1) {
                            loop.Add(currentModel);
                        }
                    }

                    allModels.AddRange(currentModels);
                }

                newModels = loop;
            }

            foreach (Model model in allModels) {
                Ommitsteps(model);
            }

            foreach (Model m1 in ms.getHistory())
            {
                SaveHistoryModel(m1, GenerateModelNameFromState(m1));
            }
        }

        private string GenerateModelNameFromState(Model currentModel) {
            return
                $"{currentModel.loadedModelType}_{currentModel.modelRooms.Count}_{currentModel.modelLines.Count}_{string.Join("-", currentModel.ModelPoints.Take(10))}";
        }

        public List<Model> AllRoomPairs(Model m_mod) {
            List<Model> returnList = new List<Model>();
            for (var i = 0; i < m_mod.modelRooms.Count; i++) {
                MyRoom room = m_mod.modelRooms[i];
                for (var j = i + 1; j < m_mod.modelRooms.Count; j++) {
                    MyRoom modelRoom = m_mod.modelRooms[j];
                    //if (room2.Guid == room1.Guid) continue;

                    bool a = DoTheyHaveCommmonWall(room, modelRoom);
                    if (!a) continue;

                    else
                    {
                        MyRoom room2;
                        MyRoom modelRoom2;
                        Model m_mod2 = m_mod.DeepCopy(room, modelRoom, out room2, out modelRoom2);

                        Model newModel = MergeRooms(m_mod2, room2, modelRoom2);
                        returnList.Add(newModel);
                    }
                }
            }

            return returnList;
        }

        private Model MergeRooms(Model mMod, MyRoom room, MyRoom modelRoom) {
            //Model m = mMod.DeepCopy();
            mMod = RemoveCommonWalls(mMod, room, modelRoom);
            //MergeBoundaryLineListToSmallerIdRooms();
            return m;
        }

        private Model RemoveCommonWalls(Model m, MyRoom room1, MyRoom room2) {
            List<MyLine> common = room1.BoundaryLines.Intersect(room2.BoundaryLines).ToList();
            foreach (MyLine line in common) {
                room1.BoundaryLines.Remove(line);
                room2.BoundaryLines.Remove(line);
            }
            room1.BoundaryLines.AddRange(room2.BoundaryLines);
            room2.BoundaryLines.AddRange(room1.BoundaryLines);

            int result1;
            int result2;
            bool parsed1 = int.TryParse(room1.Number, out result1);
            bool parsed2 = int.TryParse(room2.Number, out result2);
            if (parsed1 && parsed2 && result1 < result2) {
                m.modelRooms.Remove(room2);
            }
            else {
                m.modelRooms.Remove(room1);
            }

            return m;
        }


        private bool DoTheyHaveCommmonWall(MyRoom room, MyRoom modelRoom) {
            if (room.BoundaryLines.Intersect(modelRoom.BoundaryLines).Any()) return true;
            return false;
        }


        public void Ommitsteps(Model m_mod) {
            ms.AddModel(m_mod.DeepCopy());

            if (ExitCondition(m_mod)) return;

            Ommit(m_mod.DeepCopy());

        }

        private void Ommit(Model mMod) {
            foreach (MyRoom room in mMod.modelRooms)
            {
                MyRoom room2;
                MyRoom modelRoom2;
                Model m_mod2 = mMod.DeepCopy(room, null, out room2, out modelRoom2);

                m_mod2.modelRooms.Remove(room2);
                Ommitsteps(m_mod2);
            }

        }

        private bool ExitCondition(Model model) {
            if (m.modelRooms.Count == 1) return true;
            else return false;
        }

        private string savepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private void SaveHistoryModel(Model ms, string name) {
            //string data = JsonConvert.SerializeObject(jsondata);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter($"{savepath}\\{name}.json"))
            using (JsonWriter writer = new JsonTextWriter(sw)) {
                serializer.Serialize(writer, ms);
            }
        }


        [TearDown]
        public void Teardown() {
            m = null;
        }
    }
}
