using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CAPEOPEN;

namespace WPFUnit.Parameter
{
    [Serializable]
    [ComVisible(true)]
    [Guid("BD386D8B-BE91-4548-96C9-38C2EF8F21B8")]
    [ComDefaultInterface(typeof(ICapeParameter))]
    public class CapeNumArrayParameter
        : CapeParameterBase, ICapeArrayParameterSpec
    {
        List<double> _array;

        #region Constructor

        public CapeNumArrayParameter()
            : this("arrayParameter")
        {
        }

        public CapeNumArrayParameter(string name)
            : base(name, CapeParamType.CAPE_ARRAY, null)
        {
            _array = new List<double>();
        }

        #endregion

        #region CapeParameterBase

        public override bool Validate(ref string message)
        {
            bool[] isAvailable;
            object messages = null;
            if (this.Type == CapeParamType.CAPE_ARRAY)
                isAvailable = (bool[])Validate(this._array, ref messages);
            else
                return false; //未知参数类型当然不能用

            for (int i = 0; i < isAvailable.Rank; i++)
            {
                if (isAvailable[i] == false)
                {
                    ValStatus = CapeValidationStatus.CAPE_INVALID;
                    return false;
                }
            }
            ValStatus = CapeValidationStatus.CAPE_VALID;
            return true;
        }

        public override void Reset()
        {
            _array.Clear();
            ValStatus = CapeValidationStatus.CAPE_NOT_VALIDATED;
            _dirty = true;
        }

        public override object value
        {
            get { return ItemsSpecifications; }
            set
            {
                var v = value as IEnumerable<double>;
                if (v == null)
                    return; //throw new COMException("变量不是CapeArrayParameter");
                _array.Clear();
                _array.Capacity = v.Count();
                foreach (var element in v)
                {
                    _array.Add(element);
                }
            }
        }

        #endregion

        #region ICapeArrayParameterSpec

        public object Validate(object value, ref object message)
        {
            //这里的value应该是内部指定的存储类型
            var v = value as IEnumerable<double>;
            //这里能正常转换吗？？？可以用单元测试试一下
            //这样写能不能获取到二维数组？？？？?????????????????
            if (v == null)
            {
                message = "变量不是CapeArrayParameter";
                return false;
            }

            return true;
        }

        public int NumDimensions
        {
            get { return 1; }
        }

        public object Size
        {
            get
            {
                int[] sizeList = new int[1];
                sizeList[0] = _array.Count;
                return sizeList;
            }
        }

        public object ItemsSpecifications
        {
            get
            {
                object[] spec = new object[1];
                spec[0] = _array.ToArray();
                return spec;
            }
        }

        #endregion
    }
}

