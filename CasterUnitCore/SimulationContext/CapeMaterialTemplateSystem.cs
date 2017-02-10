using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAPEOPEN;

namespace CasterUnitCore
{
    public static class CapeMaterialTemplateSystem
    {
        static ICapeMaterialTemplateSystem _materialTemplateSystem;

        public static void SetSimulationContext(ICapeMaterialTemplateSystem materialTemplate)
        {
            //if materialTemplate is null, just ignore it
            _materialTemplateSystem = materialTemplate;
        }

        public static string[] MaterialTemplates
        {
            get
            {
                if (_materialTemplateSystem == null)
                {
                    Debug.WriteLine("Get MaterialTemplates Failed.");
                    return new string[0];
                }
                return (_materialTemplateSystem.MaterialTemplates as IEnumerable<string>)?.ToArray();
            }
        }

        public static ICapeThermoMaterialTemplate CreateMaterialTemplate(string materialTemplateName)
        {
            if (_materialTemplateSystem == null)
            {
                Debug.WriteLine("Create MaterialTemplate Failed.");
                return null;
            }
            return _materialTemplateSystem.CreateMaterialTemplate(materialTemplateName);
        }
    }
}
