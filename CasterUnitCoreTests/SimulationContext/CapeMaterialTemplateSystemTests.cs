using Microsoft.VisualStudio.TestTools.UnitTesting;
using CasterUnitCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CasterUnitCoreTests.SimulationContext;
using CAPEOPEN;

namespace CasterUnitCore.Tests
{
    [TestClass()]
    public class CapeMaterialTemplateSystemTests
    {
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            CapeMaterialTemplateSystem.SetSimulationContext(new SimpleSimulationContext());
        }

        [TestMethod()]
        public void CreateMaterialTemplateTest()
        {
            string[] temps = CapeMaterialTemplateSystem.MaterialTemplates;
            foreach (var c in temps)
            {
                var instance=CapeMaterialTemplateSystem.CreateMaterialTemplate(c);
                Assert.IsInstanceOfType(instance, typeof (ICapeThermoMaterialTemplate));
            }

        }
    }
}