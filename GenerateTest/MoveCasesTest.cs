using System;
using WindowsFormsApp1;
using WindowsFormsApp1.Simulation;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GenerateTest {
    public class MoveTest
    {
        private Model m;
        [SetUp]
        public void Setup() {
            m = new Model();
            m.InitTestModel();
        }
        
        [Test]
        public void MoveTestLine() {
            m.InitSimpleModel();
            MyLine l = null;
            foreach (MyLine line in m.modelLines)
            {
                l = line;
                break;
            }
            m.MoveLine(10,l);
        }

        [TearDown]
        public void Teardown() {
            m = null;
        }
    }
}
