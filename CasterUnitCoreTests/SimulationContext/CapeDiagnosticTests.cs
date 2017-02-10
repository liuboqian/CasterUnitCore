using Microsoft.VisualStudio.TestTools.UnitTesting;
using CasterUnitCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CasterUnitCoreTests.SimulationContext;

namespace CasterUnitCore.Tests
{
    [TestClass()]
    public class CapeDiagnosticTests
    {
        [TestInitialize()]
        public void SetSimulationContextTest()
        {
            CapeDiagnostic.SetSimulationContext(new SimpleSimulationContext());
        }

        [TestMethod()]
        public void Test()
        {
            string message = "message to be loged";
            CapeDiagnostic.LogMessage(message);

            message = "message to be poped up";
            CapeDiagnostic.PopUpMessage(message);
        }
    }
}