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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CasterUnitCore.Annotations;
using CAPEOPEN;

namespace CasterUnitCore
{
    /// <summary>
    /// Real parameter, have a unit, can compare to double
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [Guid("77E39C43-046B-4b1f-9EE0-AA9EFC55D2EE")]
    [ComDefaultInterface(typeof(ICapeParameter))]
    public class CapeRealParameter : CapeParameterBase,
        ICapeRealParameterSpec, IComparable<double>, IComparable<ICapeRealParameterSpec>
    {
        double _dblvalue;        //当前值
        /// <summary>
        /// represent unit category, like temperature
        /// </summary>
        public UnitCategoryEnum CurrentUnitCategory;
        /// <summary>
        /// In default GUI, when unit is changed and mode is CAPE_INPUT, the value of parameter will change
        /// if you dont want the actual value changes with unit, use CAPE_INPUT_OUTPUT to avoid actual value change
        /// </summary>
        public string CurrentUnit { get; set; }

        #region Constructor
        /// <summary>
        /// default name is "realParameter"
        /// </summary>
        public CapeRealParameter()
            : this("realParameter", UnitCategoryEnum.Dimensionless, CapeParamMode.CAPE_INPUT_OUTPUT)
        { }
        /// <summary>
        /// real parameter
        /// </summary>
        /// <paramCollection name="unitCategory">unit type</paramCollection>
        /// <paramCollection name="mode">if set to Input, the value won't change with unit in default window</paramCollection>
        public CapeRealParameter(string name, UnitCategoryEnum unitCategory,
            CapeParamMode mode, double minVal = double.MinValue,
            double maxVal = double.MaxValue, double defaultVal = Double.NaN)
            : base(name, CapeParamType.CAPE_REAL, mode, unitCategory)
        {
            this.LowerBound = minVal;
            this.UpperBound = maxVal;
            this.DefaultValue = defaultVal;
            _dblvalue = DefaultValue;
            CurrentUnitCategory = unitCategory;
            CurrentUnit = Units.GetSIUnit(unitCategory);
        }
        #endregion

        #region ICapeParameter

        /// <summary>
        /// SI value, can be set to double, string, ICapeRealParameterSpec
        /// </summary>
        public override dynamic value
        {
            get
            {
                return _dblvalue;
            }
            set
            {
                double v;
                if (value is double || value is string)
                {
                    v = Convert.ToDouble(value);   //应该会自动抛异常
                }
                else if (value is ICapeRealParameterSpec)
                {
                    v = (double)((ICapeParameter)value).value;
                }
                else
                {
                    throw new ECapeInvalidArgumentException("value is not double or string or ICapeRealParameterSpec", 0);
                }
                if (v == _dblvalue) return;     //相同则不变
                _dblvalue = v;
                Dirty = true;
            }
        }

        public override object Clone()
        {
            return new CapeRealParameter(this.ComponentName, this.CurrentUnitCategory, this.Mode,
                this.LowerBound, this.UpperBound, this.DefaultValue)
            {
                ComponentDescription = this.ComponentDescription,
                CurrentUnit = this.CurrentUnit,
                value = this.value,
                Dirty = true,
            };
        }

        public override void Reset()
        {
            _dblvalue = DefaultValue;
            ValStatus = CapeValidationStatus.CAPE_NOT_VALIDATED;
            Dirty = true;
        }

        public override bool Validate(ref string message)
        {
            bool isAvailable;
            if (this.Type == CapeParamType.CAPE_REAL)
                isAvailable = Validate(this._dblvalue, ref message);
            else
                isAvailable = false;  //未知参数类型当然不能用

            if (isAvailable)
                ValStatus = CapeValidationStatus.CAPE_VALID;
            else
                ValStatus = CapeValidationStatus.CAPE_INVALID;
            return isAvailable;
        }
        /// <summary>
        /// whether the value is a number and inside range
        /// </summary>
        public override bool Validate()
        {
            string message = "";
            return Validate(ref message);
        }

        #endregion

        #region ICapeRealParameterSpec
        /// <summary>
        /// default value
        /// </summary>
        public double DefaultValue { get; set; }
        /// <summary>
        /// lower boundary
        /// </summary>
        public double LowerBound { get; set; }
        /// <summary>
        /// upper boundary
        /// </summary>
        public double UpperBound { get; set; }
        /// <summary>
        /// whether the value is a number and inside range
        /// </summary>
        public bool Validate(double value, ref string message)
        {
            bool isAvailable = false;
            if (double.IsNaN(value))
            {
                message = "value is NaN";
                isAvailable = false;
            }
            else if (double.IsInfinity(this._dblvalue))
            {
                message = "value is Infinity";
                isAvailable = false;
            }
            else if (this._dblvalue > this.UpperBound || this._dblvalue < this.LowerBound)
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
        #endregion

        #region IComparable

        public int CompareTo(double other)
        {
            return _dblvalue.CompareTo(other);
        }

        public int CompareTo(ICapeRealParameterSpec other)
        {
            return _dblvalue.CompareTo((double)((ICapeParameter)other).value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator ==(CapeRealParameter thisParameter, ICapeRealParameterSpec other)
        {
            return (object)thisParameter != null && thisParameter.CompareTo(other) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator !=(CapeRealParameter thisParameter, ICapeRealParameterSpec other)
        {
            return !(thisParameter == other);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator ==(CapeRealParameter thisParameter, double other)
        {
            return (object)thisParameter != null && thisParameter.CompareTo(other) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator !=(CapeRealParameter thisParameter, double other)
        {
            return !(thisParameter == other);
        }

        public override bool Equals(object obj)
        {
            if (obj is double || obj is ICapeRealParameterSpec)
                return obj == this;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        /// <summary>
        /// can convert to double
        /// </summary>
        /// <paramCollection name="realParameter"></paramCollection>
        /// <returns></returns>
        public static implicit operator double(CapeRealParameter realParameter)
        {
            return realParameter.value;
        }
    }
}
