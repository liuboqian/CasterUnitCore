using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasterUnitCore
{
    /// <summary>
    /// This class is used to locate a CasterUnit
    /// </summary>
    public static class CasterUnitLocator
    {
        static Dictionary<string, CasterUnitOperationBase> unitcache=new Dictionary<string,CasterUnitOperationBase>();

        public static void Register(string id, CasterUnitOperationBase unit)
        {
            if (unitcache.ContainsKey(id))
            {
                if (unitcache[id] == unit)
                    return;
                else
                    throw new ECapeUnknownException(unit,
                        "Register Failed:Already create a different unit with the same id.");
            }
            else
                unitcache.Add(id, unit);
        }

        public static void UnRegister(string id)
        {
            if (!unitcache.ContainsKey(id))
                throw new Exception("UnRegister Failed:Key is not registered.");
            else
                unitcache.Remove(id);
        }

        public static CasterUnitOperationBase GetInstance(string id)
        {
            if (unitcache.ContainsKey(id))
                return unitcache[id];
            else
                throw new Exception("Load Failed:Key is not registered.");
        }
    }
}
