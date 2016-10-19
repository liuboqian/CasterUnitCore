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
using System.Linq;
using System.Runtime.InteropServices;
using CAPEOPEN;

namespace CasterUnitCore
{
    /// <summary>
    /// matrix Parameters, can have multi dimensions, can not work for any simulator, if you have any clue, please contact me
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [Guid("274D3807-36E0-475B-8754-1F20274DEA57")]
    [ComDefaultInterface(typeof(ICapeParameter))]
    public class CapeMatrixParameter
        : CapeParameterBase, ICapeArrayParameterSpec
    {
        //Matrix会导致没有注册类？？
        private List<List<ICapeParameter>> _matrix;

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public CapeMatrixParameter()
            : this("matrixParameter", CapeParamMode.CAPE_INPUT_OUTPUT)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="name"></paramCollection>
        /// <paramCollection name="mode"></paramCollection>
        public CapeMatrixParameter(string name, CapeParamMode mode)
            : base(name, CapeParamType.CAPE_ARRAY, mode, UnitCategoryEnum.Dimensionless)
        {
            _matrix = new List<List<ICapeParameter>>();
        }
        #endregion

        #region CapeParameterBase
        /// <summary>
        /// Check for valid status for each item
        /// </summary>
        public override bool Validate(ref string message)
        {
            bool[,] isAvailable;
            object messages = null;
            if (Type == CapeParamType.CAPE_ARRAY)
                isAvailable = (bool[,])Validate(_matrix, ref messages);
            else
                return false;  //未知参数类型当然不能用

            for (int i = 0; i < isAvailable.Rank; i++)
            {
                for (int j = 0; j < isAvailable.Length / isAvailable.Rank; j++)
                {
                    if (isAvailable[i, j] == false)
                    {
                        ValStatus = CapeValidationStatus.CAPE_INVALID;
                        return false;
                    }
                }
            }
            ValStatus = CapeValidationStatus.CAPE_VALID;
            return true;
        }
        /// <summary>
        /// Check for valid status for each item
        /// </summary>
        public override bool Validate()
        {
            string message = null;
            return Validate(ref message);
        }
        /// <summary>
        /// Clear All
        /// </summary>
        public override void Reset()
        {
            _matrix.Clear();
            ValStatus = CapeValidationStatus.CAPE_NOT_VALIDATED;
            Dirty = true;
        }
        /// <summary>
        /// return value, cannot work
        /// </summary>
        public override object value
        {
            get { return ItemsSpecifications; }
            set
            {
                var v = value as List<List<CapeParameterBase>>;
                if (v == null)
                    throw new COMException("value is not List<List<CapeParameterBase>>");
                _matrix.Clear();
                foreach (var sublist in v)
                {
                    var newlist = new List<ICapeParameter>();
                    foreach (var element in sublist)
                    {
                        newlist.Add(element);
                    }
                    _matrix.Add(newlist);
                }
            }
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICapeArrayParameterSpec
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="value"></paramCollection>
        /// <paramCollection name="message"></paramCollection>
        /// <returns></returns>
        public object Validate(object value, ref object message)
        {
            //这里的value应该是内部指定的存储类型
            var v = value as IEnumerable<IEnumerable<CapeParameterBase>>;
            //这里能正常转换吗？？？可以用单元测试试一下
            //这样写能不能获取到二维数组？？？？?????????????????
            if (v == null)
            {
                message = "Parameter is not CapeArrayParameter";
                return false;
            }
            int x = v.Count();
            int y = 0;   //最大的维数
            foreach (var sublist in v)
            {
                if (sublist.Count() > y)
                    y = sublist.Count();
            }
            bool[,] validList = new bool[x, y];
            message = new string[x, y];
            string[,] messageRef = message as string[,];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                    validList[i, j] = v.ElementAtOrDefault(i).ElementAtOrDefault(j).Validate(ref messageRef[i, j]);
            }
            return validList;
        }
        /// <summary>
        /// return numbers of dimensions
        /// </summary>
        public int NumDimensions { get { return _matrix.Count; } }
        /// <summary>
        /// return an array contains size of each dimension
        /// </summary>
        public object Size
        {
            get
            {
                int[] sizeList = new int[_matrix.Count];
                for (int i = 0; i < _matrix.Count; i++)
                {
                    sizeList[i] = _matrix[i].Count;
                }
                return sizeList;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public object ItemsSpecifications
        {
            get
            {
                if (NumDimensions == 0) return null;
                int maxDimension = ((int[])Size).Max();
                object[,] specList = new object[NumDimensions, maxDimension];
                for (int i = 0; i < _matrix.Count; i++)
                {
                    for (int j = 0; j < _matrix[i].Count; j++)
                        specList[i, j] = _matrix[i][j].Specification;
                }
                return specList;
            }
        }
        #endregion
    }
}