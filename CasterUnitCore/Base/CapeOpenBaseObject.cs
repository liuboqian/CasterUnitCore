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
using CAPEOPEN;

namespace CasterUnitCore
{
    /// <summary>
    /// This class is the base class of all CO component, default setting is canRename and name is "COComponent"
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    public abstract class CapeOpenBaseObject
        : ICapeIdentification, ICloneable
        , ECapeRoot, ECapeUnknown, ECapeUser
    {
        private string _name; //ComponentName
        /// <summary>
        /// whether the ComponentName can be modified, most simulator will change the name of unit, so dont set to false for unit operation
        /// </summary>
        protected bool CanRename;
        string _description;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        protected CapeOpenBaseObject()
            : this("COComponent", null, true)
        {// must have a non-paramCollection constructor to compatible with COM
        }
        /// <summary>
        /// 
        /// </summary>
        protected CapeOpenBaseObject(string name = "COComponent", string description = null, bool canRename = true)
        {
            _name = name;
            _description = description;
            CanRename = canRename;
            Dirty = false;
        }

        #endregion

        #region ICapeIdentification

        /// <summary>
        /// Description of this component
        /// </summary>
        public string ComponentDescription
        {
            get { return _description; }
            set { _description = value; Dirty = true; }
        }

        /// <summary>
        /// Component Name, if canRename is false in the constructor, will raise an error if facing changes
        /// </summary>
        public string ComponentName
        {
            get { return _name; }
            set
            {
                if (!CanRename)
                    throw new ECapeUnknownException(this,"Rename Failed:"+ComponentName + " cannot be renamed.","ICapeIdentification");
                _name = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// whether the component has been modified
        /// </summary>
        public bool Dirty { get; set; }

        #endregion

        #region ICloneable

        /// <summary>
        /// Not Implemented, each derived class should override this
        /// </summary>
        public virtual object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Error Handle

        string _errorDesc;
        string _errorIface;
        string _errorScope;

        #region ECapeRoot

        public string name { get { return _errorDesc; } }

        #endregion

        #region ECapeUser

        public int code { get { return 0; } }
        public string description { get { return _errorDesc; } }
        public string interfaceName { get { return _errorIface; } }
        public string moreInfo { get { return "Visit Caster"; } }
        public string operation { get { return "N/A"; } }
        public string scope { get { return _errorScope; } }

        public void SetError(string errorDesc, string errorIface, string errorScope)
        {
            this._errorDesc = errorDesc;
            this._errorIface = errorIface;
            this._errorScope = errorScope;
        }

        #endregion

        #endregion
    }//class
}
