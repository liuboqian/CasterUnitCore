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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace CasterCore
{
    /// <summary>
    /// This class provide functions to register and unregister component from system registry.
    /// The information came from Attributes of the target type
    /// </summary>
    public static class CapeOpenCOMRegister
    {
        /// <summary>
        /// register function, information came from Attributes of type t
        /// </summary>
        public static void RegisterFunction(Type t)
        {
            string progId = "";
            string className = "";
            string description = "";
            string progIdVersionIndenpent = "";
            string capeVersion = "";
            string about = "";
            string vendorURL = "";
            string helpURL = "";
            string componentVersion = "";
            Guid typeLibID = Guid.Empty;
            List<Guid> categoryGUIDs = new List<Guid>();
            //Get attribute like CapeName
            Assembly assembly = t.Assembly;
            foreach (var attribute in t.GetCustomAttributes(true))
            {
                progIdVersionIndenpent = attribute is CapeNameAttribute ? ((CapeNameAttribute)attribute).Name : progIdVersionIndenpent;
                description = attribute is CapeDescriptionAttribute ? ((CapeDescriptionAttribute)attribute).Description : description;
                capeVersion = attribute is CapeVersionAttribute ? ((CapeVersionAttribute)attribute).Version : capeVersion;
                about = attribute is CapeAboutAttribute ? ((CapeAboutAttribute)attribute).About : about;
                vendorURL = attribute is CapeVendorURLAttribute ? ((CapeVendorURLAttribute)attribute).VendorURL : vendorURL;
                helpURL = attribute is CapeHelpURLAttribute ? ((CapeHelpURLAttribute)attribute).HelpURL : helpURL;
                typeLibID = attribute is TypeLibIDAttribute ? ((TypeLibIDAttribute)attribute).GUID : typeLibID;
                if (attribute is CapeCategoryAttribute)
                    categoryGUIDs.Add(((CapeCategoryAttribute)attribute).GUID);
            }
            className = t.ToString();
            if (progIdVersionIndenpent == "") progIdVersionIndenpent = t.Namespace;
            componentVersion = t.Assembly.GetName().Version.ToString();
            progId = progIdVersionIndenpent + capeVersion;

            //progID
            RegistryKey keyProgID = Registry.ClassesRoot.CreateSubKey(progId);
            try
            {
                keyProgID.SetValue(null, className, RegistryValueKind.String);
                RegistryKey subKey = keyProgID.CreateSubKey("CLSID");
                subKey.SetValue(null, t.GUID.ToString("B"));
                subKey.Close();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
            keyProgID.Close();

            //versionIndenpentProgID
            RegistryKey keyProgIDVersionIndenpent = Registry.ClassesRoot.CreateSubKey(progIdVersionIndenpent);
            try
            {
                keyProgIDVersionIndenpent.SetValue(null, className, RegistryValueKind.String);
                RegistryKey subKey = keyProgIDVersionIndenpent.CreateSubKey("CLSID");
                subKey.SetValue(null, t.GUID.ToString("B"));
                subKey.Close();
                subKey = keyProgIDVersionIndenpent.CreateSubKey("CurVer");
                subKey.SetValue(null, progId);
                subKey.Close();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
            keyProgIDVersionIndenpent.Close();

            //CLSID
            RegistryKey keyCLSID = Registry.ClassesRoot.OpenSubKey("CLSID", true);
            try
            {
                keyCLSID = keyCLSID.CreateSubKey(t.GUID.ToString("B"));
                keyCLSID.SetValue(null, className);

                RegistryKey subKey;

                subKey = keyCLSID.CreateSubKey("ProgID");
                subKey.SetValue(null, progId);
                subKey.Close();

                subKey = keyCLSID.CreateSubKey("VersionIndependentProgID");
                subKey.SetValue(null, progIdVersionIndenpent);
                subKey.Close();

                subKey = keyCLSID.CreateSubKey("AppID");
                subKey.SetValue(null, progId);
                subKey.Close();

                if (typeLibID != Guid.Empty)
                {
                    subKey = keyCLSID.CreateSubKey("TypeLib");
                    subKey.SetValue(null, typeLibID.ToString("B"));
                    subKey.Close();
                }

                subKey = keyCLSID.CreateSubKey("Implemented Categories");
                foreach (var guid in categoryGUIDs)
                    if (guid != Guid.Empty)
                        subKey.CreateSubKey(guid.ToString("B"));
                subKey.Close();

                subKey = keyCLSID.CreateSubKey("CapeDescription");
                subKey.SetValue("Name", progIdVersionIndenpent);
                subKey.SetValue("Description", description);
                subKey.SetValue("CapeVersion", capeVersion);
                subKey.SetValue("ComponentVersion", componentVersion);
                subKey.SetValue("VendorURL", vendorURL);
                subKey.SetValue("HelpURL", helpURL);
                subKey.SetValue("About", about);
                subKey.Close();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
            Console.WriteLine($"Complete write to {keyCLSID.ToString()}");
            keyCLSID.Close();
        }

        /// <summary>
        /// Unregister function
        /// </summary>
        public static void UnRegisterFunction(Type t)
        {
            string progId = "";
            string progIdVersionIndenpent = "";
            string capeVersion = "";
            Assembly assembly = t.Assembly;
            foreach (var attribute in t.GetCustomAttributes(false))
            {
                progIdVersionIndenpent = attribute is CapeNameAttribute ? ((CapeNameAttribute)attribute).Name : progIdVersionIndenpent;
                capeVersion = attribute is CapeVersionAttribute ? ((CapeVersionAttribute)attribute).Version : capeVersion;
            }
            progId = progIdVersionIndenpent + capeVersion;
            Registry.ClassesRoot.DeleteSubKeyTree(progId, false);
            Registry.ClassesRoot.DeleteSubKeyTree(progIdVersionIndenpent, false);
            Registry.ClassesRoot.OpenSubKey("CLSID", true).DeleteSubKeyTree(t.GUID.ToString("B"), false);
        }


    }
}
