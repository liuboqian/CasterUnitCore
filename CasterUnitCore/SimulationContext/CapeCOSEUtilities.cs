using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            get
            {
                if (_utilities == null)
                {
                    Debug.WriteLine("Get NamedValueList Failed.");
                    return new string[0];
                }
                return (_utilities.NamedValueList as IEnumerable<string>).ToArray();
            }
        }

        public static object NamedValue(string name)
        {
            if (_utilities == null)
            {
                Debug.WriteLine("Get NamedValue Failed.");
                return null;
            }
            return _utilities.get_NamedValue(name);
        }
    }
}
