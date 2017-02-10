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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CAPEOPEN;

namespace CasterUnitCore
{
    /// <summary>
    /// bool parameter, implemented ICapeBooleanParameterSpec, can compare to bool, can be implicit convert to bool
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [Guid("8B8BC504-EEB5-4a13-B016-9614543E4536")]
    [ComDefaultInterface(typeof(ICapeParameter))]
    public class CapeBooleanParameter
        : CapeParameterBase, ICapeBooleanParameterSpec,
          IComparable<bool>, IComparable<ICapeBooleanParameterSpec>
    {
        private bool _boolvalue;

        #region Constructor
        /// <summary>
        /// default value is false, default name is "booleanParameter"
        /// </summary>
        public CapeBooleanParameter()
            : this("booleanParameter", false, CapeParamMode.CAPE_INPUT_OUTPUT)
        { }
        /// <summary>
        /// 
        /// </summary>
        public CapeBooleanParameter(string name, bool defVal, CapeParamMode mode)
            : base(name, CapeParamType.CAPE_BOOLEAN, mode, UnitCategoryEnum.Dimensionless)
        {
            DefaultValue = defVal;
            _boolvalue = DefaultValue;
        }
        #endregion

        #region CapeParameterBase
        /// <summary>
        /// bool paramCollection is always valid
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            string message = "";
            return Validate(ref message);
        }
        /// <summary>
        /// bool paramCollection is always valid
        /// </summary>
        public override bool Validate(ref string message)
        {
            ValStatus = CapeValidationStatus.CAPE_VALID;
            message = "";
            return true;
        }
        /// <summary>
        /// reset to the default value, and status set to CAPE_NOT_VALIDATED
        /// </summary>
        public override void Reset()
        {
            _boolvalue = DefaultValue;
            ValStatus = CapeValidationStatus.CAPE_NOT_VALIDATED;
            Dirty = true;
        }
        /// <summary>
        /// return bool value, can be set with string or bool or ICapeBooleanParameterSpec
        /// </summary>
        public override object value
        {
            get { return _boolvalue; }
            set
            {
                bool v;
                try
                {
                    if (value is ICapeBooleanParameterSpec)
                    {
                        v = (bool)((ICapeParameter)value).value;
                    }
                    else
                    {
                        v = Convert.ToBoolean(value);
                    }
                }
                catch (Exception e)
                {
                    throw new ECapeUnknownException(this,
                        "value is not bool or string or ICapeBooleanParameterSpec");
                }
                
                if (v == _boolvalue) return;
                _boolvalue = v;
                Dirty = true;
            }
        }

        public override object Clone()
        {
            return new CapeBooleanParameter(ComponentName, DefaultValue, Mode)
            {
                ComponentDescription = ComponentDescription,
                value = value,
                Dirty = true
            };
        }

        #endregion

        #region ICapeBooleanParameterSpec
        /// <summary>
        /// bool paramCollection is always valid
        /// </summary>
        /// <returns></returns>
        public bool Validate(bool validateValue, ref string message)
        {
            message = "";
            return true;
        }
        /// <summary>
        /// Default value
        /// </summary>
        /// <returns></returns>
        public bool DefaultValue { get; set; }

        #endregion

        #region IComparable

        public int CompareTo(bool other)
        {
            return _boolvalue.CompareTo(other);
        }

        public int CompareTo(ICapeBooleanParameterSpec other)
        {
            return _boolvalue.CompareTo((bool)((ICapeParameter)other).value);
        }

        #endregion

        /// <summary>
        /// can be used as bool, I hope this will not be confusing
        /// </summary>
        public static explicit operator bool(CapeBooleanParameter boolParameter)
        {
            return (bool)boolParameter.value;
        }
        //explicit this method to clarify the usage, it should be used as a class, not a double

    }
}
