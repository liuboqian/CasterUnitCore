﻿/*Copyright 2016 Caster

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CAPEOPEN;

namespace CasterUnitCore
{
    /// <summary>
    /// UnitPort connect to material
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [Guid("C53E7E5D-CE57-4656-990A-7321D28BD396")]
    [ComDefaultInterface(typeof(ICapeUnitPort))]
    public class CapeMaterialPort : CapeUnitPort
    {
        [NonSerialized]
        protected MaterialObject _materialObject;

        /// <summary>
        /// Create CapeMaterialPort
        /// </summary>
        public CapeMaterialPort(string name, CapePortDirection portDirection, string description = null, bool canRename = false)
            : base(name, CapePortType.CAPE_MATERIAL, portDirection, description, canRename)
        { }

        #region Override of CapeUnitPortBase
        /// <summary>
        /// connect to stream, can be CO1.0 ICapeThermoMaterialObject or CO1.1 ICapeThermoMaterial
        /// </summary>
        public override void Connect(object objectToConnect)
        {
            Disconnect();
            if (objectToConnect is ICapeThermoMaterial)
            {
                _materialObject = new MaterialObject11(objectToConnect);
            }
            else if (objectToConnect is ICapeThermoMaterialObject)
            {
                _materialObject = new MaterialObject10(objectToConnect);
            }
            else
                throw new ECapeBadArgumentException("object connected to material port must be ICapeThermoMaterial or ICapeThermoMaterialObject",0);
        }

        public override void Disconnect()
        {
            if(_materialObject!=null)
                _materialObject.Dispose();
            _materialObject = null;
        }

        /// <summary>
        /// get the raw COM interface of material created by simulator
        /// </summary>
        public override object connectedObject
        {
            get
            {
                if (_materialObject is MaterialObject11 || _materialObject is MaterialObject10)
                    return _materialObject.CapeThermoMaterialObject;
                else
                    return null;
            }
        }

        public override bool IsConnected()
        {
            return _materialObject != null;
        }

        #endregion

        /// <summary>
        /// get material with wrapper
        /// </summary>
        /// <returns></returns>
        public MaterialObject GetMaterial()
        {
            return _materialObject;
        }

        /// <summary>
        /// set material to port
        /// </summary>
        public void SetMaterial(MaterialObject mo)
        {
            Connect(mo.CapeThermoMaterialObject);
        }

        public override object Clone()
        {
            return new CapeMaterialPort(ComponentName, _portDirection, ComponentDescription, CanRename)
            {
                _materialObject = _materialObject,
            };
        }
    }
}