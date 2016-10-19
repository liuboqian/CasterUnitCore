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
using System.Runtime.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.Linq;

namespace CasterUnitCore
{
    /// <summary>
    /// Option Parameters, can be initialize by enum, can compare to string or enum
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [Guid("8EB0F647-618C-4fcc-A16F-39A9D57EA72E")]
    [ComDefaultInterface(typeof(ICapeParameter))]
    public class CapeOptionParameter
        : CapeParameterBase, ICapeOptionParameterSpec,
        IComparable<string>, IComparable<ICapeOptionParameterSpec>, IComparable<Enum>
    {
        private string _curOption;
        private List<string> _optionList;
        [NonSerialized]
        public Type enumType;
        /// <summary>
        /// This string is used to deserialize enumType, only used when serialize and deserialize, no need to concern in other time.
        /// </summary>
        private string enumTypeName;
        /// <summary>
        /// used to retrive assembly and get type
        /// </summary>
        private string assemblyName;

        #region Constructor
        /// <summary>
        /// default name is "optionParameter" without any option
        /// </summary>
        public CapeOptionParameter()
            : this("optionParameter", (IEnumerable<string>)null, CapeParamMode.CAPE_INPUT_OUTPUT)
        { }
        /// <summary>
        /// use string array to initialize, better use enum
        /// </summary>
        [Obsolete("Use enum to initialize")]
        public CapeOptionParameter(string name, IEnumerable<string> optionList,
            CapeParamMode mode, string defOption = null, bool restrictedToList = true)
            : base(name, CapeParamType.CAPE_OPTION, mode, UnitCategoryEnum.Dimensionless)
        {
            this.enumType = null;
            _optionList = new List<string>(optionList);
            RestrictedToList = restrictedToList;
            if (defOption == null) defOption = _optionList[0];
            DefaultValue = defOption;
            _curOption = DefaultValue;
        }
        /// <summary>
        /// Highly Recommended! Use a enum type to initialize this parameter
        /// </summary>
        public CapeOptionParameter(string name, Type enumType, CapeParamMode mode,
            Enum defOption = null, bool restrictedToList = true)
            : base(name, CapeParamType.CAPE_OPTION, mode, UnitCategoryEnum.Dimensionless)
        {
            this.enumType = enumType;
            _optionList = new List<string>(enumType.GetEnumNames());
            RestrictedToList = restrictedToList;
            if (defOption == null) DefaultValue = _optionList[0];
            else DefaultValue = enumType.GetEnumName(defOption);
            _curOption = DefaultValue;
        }
        #endregion

        #region CapeParameterBase
        /// <summary>
        /// Check whether the value must be restricted to list and the option is not in the list
        /// </summary>
        /// <paramCollection name="message"></paramCollection>
        /// <returns></returns>
        public override bool Validate(ref string message)
        {
            bool isAvailable;
            if (Type == CapeParamType.CAPE_OPTION)
                isAvailable = Validate(_curOption, ref message);
            else
                isAvailable = false;  //未知参数类型当然不能用

            if (isAvailable)
                ValStatus = CapeValidationStatus.CAPE_VALID;
            else
                ValStatus = CapeValidationStatus.CAPE_INVALID;
            return isAvailable;
        }
        /// <summary>
        /// Check whether the value must be restricted to list and the option is not in the list
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            string message = "";
            return Validate(ref message);
        }
        /// <summary>
        /// reset value to default value
        /// </summary>
        public override void Reset()
        {
            _curOption = DefaultValue;
            ValStatus = CapeValidationStatus.CAPE_NOT_VALIDATED;
            Dirty = true;
        }
        /// <summary>
        /// accept string or enum or ICapeOptionParameterSpec
        /// </summary>
        public override object value
        {
            get { return _curOption; }
            set
            {
                string v;
                if (value is string)
                {
                    v = Convert.ToString(value);
                }
                else if (value is ICapeOptionParameterSpec)
                {
                    v = (string)((ICapeParameter)value).value;
                }
                else if (value is Enum && value.GetType() == enumType)
                {
                    v = ((Enum)value).ToString();
                }
                else
                {
                    throw new ECapeBadArgumentException("value is not string or specified enum or ICapeOptionParameterSpec", 0);
                }
                if (v == _curOption) return;
                _curOption = v;
                Dirty = true;
            }
        }

        public override object Clone()
        {
            if (enumType == null)
                return new CapeOptionParameter(this.ComponentName, (IEnumerable<string>)this.OptionList,
                this.Mode, this.DefaultValue, this.RestrictedToList)
                {
                    ComponentDescription = this.ComponentDescription,
                    value = this.value,
                    Dirty = true
                };
            else
                return new CapeOptionParameter(this.ComponentName, this.enumType, this.Mode, null, this.RestrictedToList)
                {
                    ComponentDescription = this.ComponentDescription,
                    value = this.value,
                    Dirty = true
                };
        }

        #endregion

        #region ICapeOptionParameterSpec
        /// <summary>
        /// Check whether the value must be restricted to list and the option is not in the list
        /// </summary>
        public bool Validate(string value, ref string message)
        {
            if (RestrictedToList)
            {
                if (_optionList.Contains(value))
                    return true;
                else
                {
                    message = "OptionList doesn't contain current value, and it must be restricted to list";
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// get and set option list
        /// </summary>
        public object OptionList
        {
            get { return _optionList.ToArray(); }
            set
            {
                if (value is IEnumerable<string>)
                {
                    _optionList = new List<string>(value as IEnumerable<string>);
                }
                else if (value is Type)
                {
                    enumType = (Type)value;
                    _optionList = new List<string>(((Type)value).GetEnumNames());
                }
                else
                    throw new COMException("value must be a enum or IEnumerable<string>");
            }
        }
        /// <summary>
        /// default value
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// whether the value can be outside of option list
        /// </summary>
        public bool RestrictedToList { get; set; }

        #endregion

        #region IComparable

        public int CompareTo(string other)
        {
            return _curOption.CompareTo(other);
        }

        public int CompareTo(ICapeOptionParameterSpec other)
        {
            return _curOption.CompareTo((string)((ICapeParameter)other).value);
        }

        public int CompareTo(Enum other)
        {
            return CompareTo(other.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator ==(CapeOptionParameter thisParameter, string other)
        {
            return (object)thisParameter != null && thisParameter.CompareTo(other) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator !=(CapeOptionParameter thisParameter, string other)
        {
            return !(thisParameter == other);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator ==(CapeOptionParameter thisParameter, ICapeOptionParameterSpec other)
        {
            return (object)thisParameter != null && thisParameter.CompareTo(other) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator !=(CapeOptionParameter thisParameter, ICapeOptionParameterSpec other)
        {
            return !(thisParameter == other);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator ==(CapeOptionParameter thisParameter, Enum other)
        {
            return (object)thisParameter != null && thisParameter.CompareTo(other) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="thisParameter"></paramCollection>
        /// <paramCollection name="other"></paramCollection>
        /// <returns></returns>
        public static bool operator !=(CapeOptionParameter thisParameter, Enum other)
        {
            return !(thisParameter == other);
        }

        public override bool Equals(object obj)
        {
            if (obj is string || obj is ICapeOptionParameterSpec || obj is Enum)
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
        /// convert to string
        /// </summary>
        /// <paramCollection name="optionParameter"></paramCollection>
        /// <returns></returns>
        public static implicit operator string(CapeOptionParameter optionParameter)
        {
            return optionParameter.value as string;
        }
        /// <summary>
        /// convert to Enum
        /// </summary>
        /// <paramCollection name="optionParameter"></paramCollection>
        /// <returns></returns>
        public static implicit operator Enum(CapeOptionParameter optionParameter)
        {
            foreach (var e in Enum.GetValues(optionParameter.enumType))
                if ((string)optionParameter.value == e.ToString())
                    return (Enum)e;
            throw new ECapeBadArgumentException("No enum is equal to the current value.", 0);
        }

        #region IPersist and Serialize
        /// <summary>
        /// Called on Serializing, save the enumType as string, don't call this method 
        /// </summary>
        [OnSerializing]
        internal void ConvertEnumType2String(StreamingContext context)
        {
            assemblyName = enumType.Assembly.FullName;
            enumTypeName = enumType.FullName;
        }
        /// <summary>
        /// Called on DeSerializing, restore the enumType by string, don't call this method 
        /// </summary>
        [OnDeserialized]
        internal void ConvertString2EnumType(StreamingContext context)
        {
            try
            {
                Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(x => x.FullName == assemblyName);
                enumType = assembly.GetType(enumTypeName);
            }
            catch
            {
                Debug.WriteLine("Unable to load option parameter, possible reason is the path of assembly is changed.");
                throw new ECapePersistenceNotFoundException(this.GetType().Name, "Unable to load option parameter, possible reason is the path of assembly is changed.", null);
            }
        }

        #endregion
    }
}
