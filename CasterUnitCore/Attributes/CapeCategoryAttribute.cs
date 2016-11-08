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
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CasterUnitCore
{
    /// <summary>
    /// Label on UnitOp Operation, will add a category GUID at \CLSID\CapeDescription\CapeCategory
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Class)]
    public class CapeCategoryAttribute : Attribute
    {
        public Guid GUID { get; private set; }

        public CapeCategoryAttribute(string GUID)
        {
            this.GUID = new Guid(GUID);
        }


    }
}
