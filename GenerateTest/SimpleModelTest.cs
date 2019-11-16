using System;
using WindowsFormsApp1;
using WindowsFormsApp1.Simulation;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GenerateTest {
    public class SimpleModelTest {

        Simulate s = new Simulate();
        [SetUp]
        public void Setup() {
            s.model = new Model();
            s.model.InitSimpleModel();

        }
        //the goal of the tests are to avoid exceptions, if there is an exception, it fails.
        [Test]
        public void FullSimulation() {
            s.run();
        }

        [Test]
        public void SwitchTest() {
            Assert.AreEqual(10, 10);
        }

        [TearDown]
        public void Teardown() {
            s = null;
        }
    }
}
