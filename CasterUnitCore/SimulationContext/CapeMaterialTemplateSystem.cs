using System;
using System.Collections.Generic;
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
            get { return (_materialTemplateSystem.MaterialTemplates as IEnumerable<string>).ToArray(); }
        }

        public static ICapeThermoMaterialTemplate CreateMaterialTemplate(string materialTemplateName)
        {
            return _materialTemplateSystem.CreateMaterialTemplate(materialTemplateName);
        }
    }
}
