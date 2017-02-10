using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CasterUnitCoreTests.Streams;
using CAPEOPEN;

namespace CasterUnitCoreTests.SimulationContext
{
    public class SimpleThermoMaterialTemplate:ICapeThermoMaterialTemplate
    {
        public object CreateMaterialObject()
        {
            return new Streams.SimpleMaterialObject10();
        }

        public void SetProp(string property, object values)
        {
            Console.WriteLine($"Property {property} is set to {values}");
        }
    }
}
