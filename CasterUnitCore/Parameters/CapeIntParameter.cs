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
    /// Int parameter, don't have an unit, can compare to int
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [Guid("2C57DC9F-1368-42eb-888F-5BC6ED7DDFA7")]
    [ComDefaultInterface(typeof(ICapeParameter))]
    public class CapeIntParameter
        : CapeParameterBase, ICapeIntegerParameterSpec,
        IComparable<Int32>, IComparable<ICapeIntegerParameterSpec>
    {
        private int _intvalue;

        #region Constructor
        /// <summary>
        /// dafault name is "intParameter", and mode is CAPE_INPUT_OUTPUT
        /// </summary>
        public CapeIntParameter()
            : this("intParameter", CapeParamMode.CAPE_INPUT_OUTPUT)
        { }
        /// <summary>
        /// 
        /// </summary>
        public CapeIntParameter(string name,
            CapeParamMode mode, int minVal = int.MinValue,
            int maxVal = int.MaxValue, int defaultVal = default(int))
            : base(name, CapeParamType.CAPE_INT, mode, UnitCategoryEnum.Dimensionless)
        {
            LowerBound = minVal;
            UpperBound = maxVal;
            DefaultValue = defaultVal;
            _intvalue = DefaultValue;
        }

        #endregion

        #region CapeParameterBase
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="message"></paramCollection>
        /// <returns></returns>
        public override bool Validate(ref string message)
        {
            bool isAvailable;
            if (Type == CapeParamType.CAPE_INT)
                isAvailable = Validate(_intvalue, ref message);
            else
                isAvailable = false;

            if (isAvailable)
                ValStatus = CapeValidationStatus.CAPE_VALID;
            else
                ValStatus = CapeValidationStatus.CAPE_INVALID;
            return isAvailable;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            string message = "";
            return Validate(ref message);
        }
        /// <summary>
        /// set to default value
        /// </summary>
        public override void Reset()
        {
            _intvalue = DefaultValue;
            ValStatus = CapeValidationStatus.CAPE_NOT_VALIDATED;
            Dirty = true;
        }
        /// <summary>
        /// get or set, can be set with int, string, ICapeIntegerParameterSpec
        /// </summary>
        public override dynamic value
        {
            get { return _intvalue; }
            set
            {
                int v;
                if (value is int || value is string)
                {
                    v = Convert.ToInt32(value);
                }
                else if (value is ICapeIntegerParameterSpec)
                {
                    v = (int)((ICapeParameter)value).value;
                }
                else
                {
                    throw new COMException("value is not int or string or ICapeIntegerParameterSpec");
                }
                if (v == _intvalue) return;
                _intvalue = v;
                Dirty = true;
            }
        }

        public override object Clone()
        {
            return new CapeIntParameter(ComponentName, Mode,
                LowerBound, UpperBound, DefaultValue)
            {
                ComponentDescription = ComponentDescription,
                value = value,
                Dirty = true,
            };
        }

        #endregion

        #region ICapeIntegerParameterSpec
        /// <summary>
        /// Check if the value is in LowerBound and UpperBound
        /// </summary>
        /// <paramCollection name="value"></paramCollection>
        /// <paramCollection name="message"></paramCollection>
        /// <returns></returns>
        public bool Validate(int value, ref string message)
        {
            bool isAvailable;
            if (_intvalue > UpperBound ||
                _intvalue < LowerBound)
            {
                message = "value is out of range";
                isAvailable = false;
            }
            else
            {
                message = "";
                isAvailable = true;
            }
            return isAvailable;
        }
        /// <summary>
        /// default value
        /// </summary>
        public int DefaultValue { get; set; }
        /// <summary>
        /// lower boundary
        /// </summary>
        public int LowerBound { get; set; }
        /// <summary>
        /// upper boundary
        /// </summary>
        public int UpperBound { get; set; }
        #endregion

        #region IComparable

        public int CompareTo(int other)
        {
            return _intvalue.CompareTo(other);
        }

        public int CompareTo(ICapeIntegerParameterSpec other)
        {
            return _intvalue.CompareTo((int)((ICapeParameter)other).value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator ==(CapeIntParameter thisParameter, ICapeIntegerParameterSpec other)
        {
            return (object)thisParameter != null && thisParameter.CompareTo(other) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator !=(CapeIntParameter thisParameter, ICapeIntegerParameterSpec other)
        {
            return !(thisParameter == other);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator ==(CapeIntParameter thisParameter, int other)
        {
            return (object)thisParameter != null && thisParameter.CompareTo(other) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator !=(CapeIntParameter thisParameter, int other)
        {
            return !(thisParameter == other);
        }

        public override bool Equals(object obj)
        {
            if (obj is double || obj is ICapeIntegerParameterSpec)
                return obj == this;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        /// <summary>
        /// can be convert to int
        /// </summary>
        /// <paramCollection name="intParameter"></paramCollection>
        /// <returns></returns>
        public static implicit operator int(CapeIntParameter intParameter)
        {
            return intParameter.value;
        }
    }
}
