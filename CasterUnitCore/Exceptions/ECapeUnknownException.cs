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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using CAPEOPEN;

namespace CasterUnitCore
{
    /// <summary>
    /// 
    /// </summary>
    [Description("CapeUnknownException")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [Guid("B550B2CA-6714-4e7f-813E-C93248142410")]
    [Serializable]
    public class ECapeUnknownException : ApplicationException, ECapeUser, ECapeRoot, ECapeUnknown
    {
        public CapeOpenBaseObject _errorObject;

        #region Constructor

        /// <summary>
        /// create a cape unknown exception by .net exception, obj should be the unit which throw the exception, in most case, it's UnitOp
        /// </summary>
        /// <paramCollection name="message"></paramCollection>
        /// <paramCollection name="inner"></paramCollection>
        public ECapeUnknownException(CapeOpenBaseObject obj, Exception inner, string interfaceName = null)
            : base(inner.Message, inner)
        {
            _errorObject = obj;
            this.HResult = (int)ECapeErrorHResult.ECapeUnknownHR;
            this.name = inner.Message;
            this.interfaceName = interfaceName;
            obj.SetError(inner.Message, interfaceName, scope);
        }

        /// <summary>
        /// create an cape unknown exception, obj should be the unit which throw the exception, in most case, it's UnitOp
        /// </summary>
        /// <paramCollection name="message"></paramCollection>
        public ECapeUnknownException(CapeOpenBaseObject obj, string message, string interfaceName = null)
            : base(message)
        {
            _errorObject = obj;
            this.HResult = (int)ECapeErrorHResult.ECapeUnknownHR;
            this.name = message;
            this.interfaceName = interfaceName;
            obj.SetError(message, interfaceName, scope);
        }

        /// <summary>
        /// throw an error, will create an empty CapeCollection to throw this exception
        /// </summary>
        public ECapeUnknownException(string message = null)
            :base(message)
        {
            _errorObject = new CapeCollection(name:"ErrorObj");
            this.HResult = (int)ECapeErrorHResult.ECapeUnknownHR;
            this.name = null;
            this.interfaceName = interfaceName;
            _errorObject.SetError(message, interfaceName, scope);
        }

        #endregion

        #region ECapeUser
        /// <summary>
        /// 
        /// </summary>
        public int code { get { return HResult; } }
        /// <summary>
        /// 
        /// </summary>
        public string description { get { return Message; } }
        /// <summary>
        /// 
        /// </summary>
        public string scope { get { return Source; } }
        /// <summary>
        /// 
        /// </summary>
        public string interfaceName { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public string operation { get { return StackTrace; } }
        /// <summary>
        /// 
        /// </summary>
        public string moreInfo { get { return this.HelpLink; } }

        #endregion

        #region ECapeRoot

        /// <summary>
        /// 
        /// </summary>
        public string name { get; private set; }

        #endregion

    }
}
