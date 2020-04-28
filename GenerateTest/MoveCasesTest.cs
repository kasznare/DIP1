using System;
using WindowsFormsApp1.Simulation;
using Diploma2.Model;
using Diploma2.Services;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GenerateTest {
    public class MoveTest
    {
        private _Model m;
        [SetUp]
        public void Setup() {
            m = ModelConfigurations.InitTestModel();
        }
        
        [Test]
        public void MoveTestLine() {

            foreach (_Room mRoom in m.rooms)
            {
                foreach (_Line line in mRoom.Lines)
                {
                    m.MoveLine(10,line);
                }
            }
            
        }

        [Test]
        public void CopiedModelMoveTestLine()
        {

            m = m.DeepCopy();
            foreach (_Room mRoom in m.rooms) {
                foreach (_Line line in mRoom.Lines) {
                    m.MoveLine(10, line);
                }
            }
        }

        [TearDown]
        public void Teardown() {
            m = null;
        }
    }
}
