using System;
using WindowsFormsApp1;
using WindowsFormsApp1.Simulation;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GenerateTest {
    public class MoveAllModelLinesTest {
        Model m;
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


        [TearDown]
        public void Teardown() {
            m = null;
        }
    }
}
