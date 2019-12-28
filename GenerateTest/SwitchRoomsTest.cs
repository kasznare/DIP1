using System;
using WindowsFormsApp1;
using WindowsFormsApp1.Simulation;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GenerateTest {
    public class SwitchRoomsTest
    {
        private Model m;
        [SetUp]
        public void Setup() {
            m = new Model();
        }

        [Test]
        public void SwitchEveryRoomInSimple() {
            m.InitSimpleModel();
            for (int i = 0; i < m.modelRooms.Count; i++) {
                var mModelRoom = m.modelRooms[i];
                for (int j = i + 1; j < m.modelRooms.Count; j++) {
                    var modelRoom = m.modelRooms[j];
                    m.SwitchRooms(ref mModelRoom, ref modelRoom);
                    m.SwitchRooms(ref modelRoom, ref mModelRoom);
                }
            }
        }
       
        [Test]
        public void MoveEveryLineInNormal() {
            m.InitNormalModel();
            for (int i = 0; i < m.modelRooms.Count; i++) {
                var mModelRoom = m.modelRooms[i];
                for (int j = i + 1; j < m.modelRooms.Count; j++) {
                    var modelRoom = m.modelRooms[j];
                    m.SwitchRooms(ref mModelRoom, ref modelRoom);
                    m.SwitchRooms(ref modelRoom, ref mModelRoom);
                }
            }
        }
        [Test]
        public void MoveEveryLineInAdvanced() {
            m.InitAdvancedModel();
            for (int i = 0; i < m.modelRooms.Count; i++) {
                var mModelRoom = m.modelRooms[i];
                for (int j = i + 1; j < m.modelRooms.Count; j++) {
                    var modelRoom = m.modelRooms[j];
                    m.SwitchRooms(ref mModelRoom, ref modelRoom);
                    m.SwitchRooms(ref modelRoom, ref mModelRoom);
                }
            }
        }

        [Test]
        public void MoveEveryLineInFactory() {
            m.InitModelWithGivenRooms();
            for (int i = 0; i < m.modelRooms.Count; i++) {
                var mModelRoom = m.modelRooms[i];
                for (int j = i + 1; j < m.modelRooms.Count; j++) {
                    var modelRoom = m.modelRooms[j];
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
