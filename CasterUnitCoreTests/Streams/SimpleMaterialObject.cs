using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAPEOPEN;

namespace CasterUnitCoreTests.Streams
{
    //CO1.0 Material 
    public class SimpleMaterialObject10:ICapeThermoMaterialObject
    {
        private Dictionary<string, double> universalConstant;

        public SimpleMaterialObject10()
        {
            universalConstant = new Dictionary<string, double>();
            universalConstant.Add("standardAccelerationOfGravity", 9.80665);
            universalConstant.Add("avogadroConstant", 6.0221419947e23);
            universalConstant.Add("boltzmannConstant", 1.380650324e-23);
            universalConstant.Add("molarGasConstant", 8.31447215e15);
        }

        public object GetUniversalConstant(object props)
        {
            throw new NotImplementedException();
        }

        public object GetComponentConstant(object props, object compIds)
        {
            throw new NotImplementedException();
        }

        public void CalcProp(object props, object phases, string calcType)
        {
            throw new NotImplementedException();
        }

        public object GetProp(string property, string phase, object compIds, string calcType, string basis)
        {
            throw new NotImplementedException();
        }

        public void SetProp(string property, string phase, object compIds, string calcType, string basis, object values)
        {
            throw new NotImplementedException();
        }

        public void CalcEquilibrium(string flashType, object props)
        {
            throw new NotImplementedException();
        }

        public void SetIndependentVar(object indVars, object values)
        {
            throw new NotImplementedException();
        }

        public object GetIndependentVar(object indVars)
        {
            throw new NotImplementedException();
        }

        public object PropCheck(object props)
        {
            throw new NotImplementedException();
        }

        public object AvailableProps()
        {
            throw new NotImplementedException();
        }

        public void RemoveResults(object props)
        {
            throw new NotImplementedException();
        }

        public object CreateMaterialObject()
        {
            throw new NotImplementedException();
        }

        public object Duplicate()
        {
            throw new NotImplementedException();
        }

        public object ValidityCheck(object props)
        {
            throw new NotImplementedException();
        }

        public object GetPropList()
        {
            throw new NotImplementedException();
        }

        public int GetNumComponents()
        {
            throw new NotImplementedException();
        }

        public object ComponentIds { get; }
        public object PhaseIds { get; }
    }
}
