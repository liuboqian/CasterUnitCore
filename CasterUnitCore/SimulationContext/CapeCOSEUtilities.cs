using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAPEOPEN;

namespace CasterUnitCore
{
    public static class CapeCOSEUtilities
    {
        static ICapeCOSEUtilities _utilities;

        public static void SetSimulationContext(ICapeCOSEUtilities utilities)
        {
            //if utilities is null, just ignore it
            _utilities = utilities;
        }

        public static string[] NamedValueList
        {
            get { return (_utilities.NamedValueList as IEnumerable<string>).ToArray(); }
        }

        public static object NamedValue(string name)
        {
            return _utilities.get_NamedValue(name);
        }
    }
}
