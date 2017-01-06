/*Copyright 2016 Caster

* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
*     http://www.apache.org/licenses/LICENSE-2.0
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.*/

using System;
using System.Runtime.InteropServices;
using CasterUnitCore;
using CAPEOPEN;

namespace CasterUnitSample
{
    [Serializable]
    [ComVisible(true)]
    [Guid("7CF9589B-19D6-4B1F-B259-0E0413A88968")]
    [CapeName("CasterUnitSample")]
    [CapeDescription("This unit is a test to the CasterUnitCore.")]
    [CapeAbout("Caster")]
    [CapeHelpURL("liuboqian2015@outlook.com")]
    [CapeVendorURL("")]
    [CapeVersion("1.1")]
    [ComDefaultInterface(typeof(ICapeUnit))]
    public class CasterUnitSample:CasterUnitOperationBase,ICapeUnit
    {
        public CasterUnitSample()
            : base(new TestCalculator(), "CasterUnitSample", "This unit is a test to the CasterUnitCore.")
        { }

        public override void InitParameters()
        {
            Parameters = new CapeCollection("Parameters", "this is the parameter collection of test unit.");
            CapeRealParameter ParamT = new CapeRealParameter("T", UnitCategoryEnum.Temperature, CapeParamMode.CAPE_INPUT, defaultVal: 500);
            ParamT.ComponentDescription = "temperature of outlet material";
            Parameters.Add(ParamT);
            CapeRealParameter ParamP = new CapeRealParameter("P", UnitCategoryEnum.Pressure, CapeParamMode.CAPE_INPUT, defaultVal: 200000);
            ParamP.ComponentDescription = "pressure of outlet material";
            Parameters.Add(ParamP);

            Parameters.Add(new CapeIntParameter("intParam", CapeParamMode.CAPE_INPUT));
            Parameters.Add(new CapeBooleanParameter("boolParam", true, CapeParamMode.CAPE_INPUT));
            Parameters.Add(new CapeOptionParameter("optionParam", typeof(Options), CapeParamMode.CAPE_INPUT));
        }

        public override void InitPorts()
        {
            Ports = new CapeCollection("Ports", "this is the port collection of test unit.");
            Ports.Add(new CapeMaterialPort("feed", CapePortDirection.CAPE_INLET, "Inlet Material"));
            Ports.Add(new CapeMaterialPort("product", CapePortDirection.CAPE_OUTLET, "Outlet Material"));
            //Ports.Add(new CapeEnergyPort("energy", CapePortDirection.CAPE_INLET));
            //Ports.Add(new CapeInformationPort("info", CapePortDirection.CAPE_INLET));
        }

        public override void InitResults()
        {
            Results = new CapeCollection("Results", "this is the result collection of test unit.");
            CapeRealParameter ParamTout = new CapeRealParameter("Tout", UnitCategoryEnum.Temperature, CapeParamMode.CAPE_OUTPUT);
            ParamTout.ComponentDescription = "temperature of actual outlet material";
            Results.Add(ParamTout);
            CapeRealParameter ParamPout = new CapeRealParameter("Pout", UnitCategoryEnum.Pressure, CapeParamMode.CAPE_OUTPUT);
            ParamPout.ComponentDescription = "pressure of actual outlet material";
            Results.Add(ParamPout);
        }
    }
}
