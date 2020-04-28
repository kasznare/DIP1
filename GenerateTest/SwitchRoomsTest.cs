using System;
using WindowsFormsApp1;
using WindowsFormsApp1.Simulation;
using Diploma2.Model;
using Diploma2.Services;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GenerateTest {
    public class SwitchRoomsTest
    {
        private _Model m;
        [SetUp]
        public void Setup()
        {
            m = new _Model();
        }

        [Test]
        public void SwitchEveryRoomInSimple() {
            m = ModelConfigurations.InitSimpleModel();
            for (int i = 0; i < m.rooms.Count; i++) {
                var mModelRoom = m.rooms[i];
                for (int j = i + 1; j < m.rooms.Count; j++) {
                    var modelRoom = m.rooms[j];
                    m.SwitchRooms(ref mModelRoom, ref modelRoom);
                    m.SwitchRooms(ref modelRoom, ref mModelRoom);
                }
            }
        }
       
        [TearDown]
        public void Teardown() {
            m = null;
        }
    }
}
