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
        }
        //the test should be to try every single move and see if there is an error
        //if there is an error...
        //so hopefully there is no error, and this is a black box stress test
        //the test should log the process, and keep some history, because otherwise there is no way we can tell later what went wrong.
        //it needs to store the last model state
        //the test should save every model state
        //should the model store every model state in a text file? I guess so, why not? every line is a new model state... or every file?

        //or! I implement the state change logging but do not keep successful logs


        //the goal of the tests are to avoid exceptions, if there is an exception, it fails.
        //[Test]
        public void SimpleModel() {
            s.model.InitSimpleModel();
            s.run();
        }
        //[Test]
        public void FullSimulation() {
            s.model.InitSimpleModel();
            s.run();
        }

        //[Test]
        //public void SwitchTest() {
        //    Assert.AreEqual(10, 10);
        //}

        [TearDown]
        public void Teardown() {
            s = null;
        }
    }
}
