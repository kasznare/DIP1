using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1;
using WindowsFormsApp1.Simulation;
using Diploma2.Model;
using Diploma2.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GenerateTest {
    public class MoveAllModelLinesTest {
        _Model m;
        ModelStorage ms = new ModelStorage();

        private int numberofmodels = 0;
        [SetUp]
        public void Setup() {
            m = new _Model();
        }

        [Test]
        //[Attribute a=1]
        public void MoveLineInAllModel() {
            List<_Model> contents = new List<_Model>();
            foreach (string filename in Directory.GetFiles(@"C:\Users\Master\Desktop\Models"))
            {
                MessageBox.Show(filename);
                if (!filename.EndsWith("json"))
                {
                    continue;
                }
                try
                {
                    string json = File.ReadAllText(filename);
                    MessageBox.Show(json);
                    _Model account = JsonConvert.DeserializeObject<_Model>(json);
                    contents.Add(account);

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "\n" + e.StackTrace);
                }
            }

            MessageBox.Show(contents.Count.ToString());
            foreach (_Model m in contents)
            {
                _Model mcopy =m.DeepCopy();
                List<_Line> lines = mcopy.AllLinesFlat();
                for (var i = 0; i < lines.Count; i++) {
                    _Line l = lines[i];
                    mcopy.MoveLine(10, l);
                    mcopy.MoveLine(-10, l);
                    mcopy.MoveLine(-10, l);
                    mcopy.MoveLine(10, l);
                    
                }

            }
            //m = LoadModelFromJsonString();
            //Line l = Find200200400200Line();
            //m.MoveLine(10, l);
            //Assert m.IsInInvalidState.Equals(false);
        }


        [Test]
        public void MoveEveryLineInSimple() {
            m = ModelConfigurations.InitSimpleModel();
            _Line l = null;
            List<_Line> lines = m.AllLinesFlat();
            for (var i = 0; i < lines.Count; i++) {
                l = lines[i];
                m.MoveLine(10, l);
                m.MoveLine(-10, l);
                m.MoveLine(-10, l);
                m.MoveLine(10, l);
            }
        }

        [Test]
        public void MoveEveryLineInNormal() {
            m = ModelConfigurations.InitNormalModel();
            _Line l = null;
            List<_Line> lines = m.AllLinesFlat();
            for (var i = 0; i < lines.Count; i++) {
                l = lines[i];
                m.MoveLine(10, l);
                m.MoveLine(-10, l);
                m.MoveLine(-10, l);
                m.MoveLine(10, l);
            }
        }


        [Test]
        public void AllCasesTestGeneration() {
            m = ModelConfigurations.InitTestModel();
            List<_Model> new_Models = new List<_Model>();
            new_Models.Add(m);
            List<_Model> all_Models = new List<_Model>();
            while (new_Models.Any()) {
                List<_Model> loop = new List<_Model>();
                foreach (_Model _Model in new_Models) {
                    List<_Model> current_Models = AllRoomPairs(_Model);

                    foreach (_Model current_Model in current_Models) {
                        if (current_Model.rooms.Count > 1) {
                            loop.Add(current_Model);
                        }
                    }

                    all_Models.AddRange(current_Models);
                }

                new_Models = loop;
                GC.Collect();
            }

            foreach (_Model _Model in all_Models) {
                Ommitsteps(_Model);
            }

            foreach (_Model m1 in ms.getHistory())
            {
                SaveHistoryModel(m1, GenerateModelNameFromState(m1));
            }
        }

        private string GenerateModelNameFromState(_Model currentModel) {
            return
                $"{currentModel.loadedModelType}_{currentModel.rooms.Count}_" +
                $"{currentModel.AllLinesFlat().Count}";
        }

        public List<_Model> AllRoomPairs(_Model m_mod) {
            List<_Model> returnList = new List<_Model>();
            for (var i = 0; i < m_mod.rooms.Count; i++) {
                _Room room = m_mod.rooms[i];
                for (var j = i + 1; j < m_mod.rooms.Count; j++) {
                    _Room modelRoom = m_mod.rooms[j];
                    //if (room2.Guid == room1.Guid) continue;

                    bool a = DoTheyHaveCommmonWall(room, modelRoom);
                    if (!a) continue;

                    else
                    {
                        _Room room2;
                        _Room modelRoom2;
                        _Model m_mod2 = m_mod.DeepCopy(room, modelRoom, out room2, out modelRoom2);

                        _Model newModel = MergeRooms(m_mod2, room2, modelRoom2);
                        returnList.Add(newModel);
                    }
                }
            }

            return returnList;
        }

        private _Model MergeRooms(_Model mMod, _Room room, _Room modelRoom) {
            //Model m = mMod.DeepCopy();
            mMod = RemoveCommonWalls(mMod, room, modelRoom);
            //MergeBoundaryLineListToSmallerIdRooms();
            return m;
        }

        private _Model RemoveCommonWalls(_Model m, _Room room1, _Room room2) {
            List<_Line> common = room1.Lines.Intersect(room2.Lines).ToList();
            foreach (_Line line in common) {
                room1.Lines.Remove(line);
                room2.Lines.Remove(line);
            }
            room1.Lines.AddRange(room2.Lines);
            room2.Lines.AddRange(room1.Lines);

            int result1 = room1.Number;
            int result2 = room2.Number;
            bool parsed1 = room1.Number is int;
            bool parsed2 = room2.Number is int;
            if (parsed1 && parsed2 && result1 < result2) {
                m.rooms.Remove(room2);
            }
            else {
                m.rooms.Remove(room1);
            }

            return m;
        }


        private bool DoTheyHaveCommmonWall(_Room room, _Room modelRoom) {
            if (room.Lines.Intersect(modelRoom.Lines).Any()) return true;
            return false;
        }


        public void Ommitsteps(_Model m_mod) {
            ms.AddModel(m_mod.DeepCopy());

            if (ExitCondition(m_mod)) return;

            Ommit(m_mod.DeepCopy());

        }

        private void Ommit(_Model mMod) {
            foreach (_Room room in mMod.rooms)
            {
                _Room room2;
                _Room modelRoom2;
                _Model m_mod2 = mMod.DeepCopy(room, null, out room2, out modelRoom2);

                m_mod2.rooms.Remove(room2);
                Ommitsteps(m_mod2);
            }

        }

        private bool ExitCondition(_Model model) {
            if (m.rooms.Count == 1) return true;
            else return false;
        }

        private string savepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private void SaveHistoryModel(_Model ms, string name) {
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
