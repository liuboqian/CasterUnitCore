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
using System.Runtime.InteropServices;

namespace CasterCore
{
    /// <summary>
    /// CapeVersion, will register at 
    /// \ClassRoot\CLSID\{Guid}\CapeDescription\CapeVersion
    /// There is another version called ComponentVersion which is the version of this project
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Class)]
    public class CapeVersionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Version { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <paramCollection name="version"></paramCollection>
        public CapeVersionAttribute(string version)
        {
            Version = version;
        }
    }
}
