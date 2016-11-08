using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAPEOPEN;

namespace WPFUnit.Parameter
{
    public class ParamTemperature:CapeRealParameter
    {
        public ParamTemperature(string name=null,double value=default(double),CapeParamMode mode=CapeParamMode.CAPE_INPUT_OUTPUT)
        {
            ComponentName = "ParamTemperature";
            ComponentDescription = "Parameter of Temperature";
            Dimensionality = Units.GetDimensionality("Temperature");
        }
    }
}
