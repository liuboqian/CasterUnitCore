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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using CAPEOPEN;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasterUnitCore
{
    /// <summary>
    /// Base class of unit operation, must be inherit
    /// Parameter initialize and port initialize and calculation should be in this class
    /// Genericity is not allowed, because of IPersist
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [ComDefaultInterface(typeof(ICapeUnit))]
    [Guid("E78BFCC3-4865-4AF5-8BEE-276C49DE506F")]
    public abstract class CasterUnitOperationBase
        : CapeOpenBaseObject, ICapeUnit, ICapeUtilities, IPersistStream  //The origin CAPE-OPEN IPersistStream has a problem, so overload it
    {
        #region fields

        /// <summary>
        /// is unit valid
        /// </summary>
        protected CapeValidationStatus Status = CapeValidationStatus.CAPE_NOT_VALIDATED;
        /// <summary>
        /// reference to simulationContext
        /// </summary>
        protected ICapeSimulationContext _simulationContext;
        /// <summary>
        /// whether the unit is loaded from file
        /// </summary>
        protected bool Isloaded;
        /// <summary>
        /// new thread to show GUI, important for aspen
        /// </summary>
        protected Thread GuiThread;
        /// <summary>
        /// calculate instance, contains derived instance of SpecCalculator
        /// </summary>
        public Calculator SpecCalculator { get; set; }
        /// <summary>
        /// Collection of ports connected to this unit
        /// </summary>
        public CapeCollection Ports;
        /// <summary>
        /// Collection of Parameters, should be CAPE_INPUT or CAPE_INPUT_OUTPUT
        /// </summary>
        public CapeCollection Parameters;
        /// <summary>
        /// Collection of results, should be CAPE_OUTPUT Parameters, used to display results after calculation
        /// </summary>
        public CapeCollection Results;
        /// <summary>
        /// Unique id of an instance, to identify the unit in CasterUnitLocator, default using Guid
        /// </summary>
        public readonly string unitId;

        #endregion

        #region Events

        /// <summary>
        /// Called during Initialize
        /// </summary>
        public event Action OnInitialize;
        /// <summary>
        /// Called during Terminate
        /// </summary>
        public event Action OnTerminate;
        /// <summary>
        /// Called when COSE ask for Parameters interface
        /// </summary>
        public event Action<CapeCollection> OnGetParameters;
        /// <summary>
        /// Called when COSE ask for ports interface
        /// </summary>
        public event Action<CapeCollection> OnGetPorts;

        #endregion

        #region Constructor
        /// <summary>
        /// This constructor assign SpecCalculator, create Ports, Parameters and Results and call their initialize method
        /// </summary>
        /// <paramCollection name="specCalculator">the specification calculate class</paramCollection>
        /// <paramCollection name="className">name of this unit operation</paramCollection>
        /// <paramCollection name="description">description of this unit operation</paramCollection>
        public CasterUnitOperationBase(Calculator specCalculator, string className, string description)
            : base(className, description, true)
        {
            this.SpecCalculator = specCalculator;
            SpecCalculator.UnitOp = this;
            Ports = new CapeCollection("Ports", "In and Out ports of this unit.");
            Parameters = new CapeCollection("Parameters", "User input Parameters of this unit.");
            Results = new CapeCollection("Results", "Calculate Results, output Parameters.");
            InitParameters();
            InitPorts();
            InitResults();
            Isloaded = false;
            unitId = Guid.NewGuid().ToString("B");
        }

        #endregion

        #region Get Parameters And Ports
        /// <summary>
        /// Add parameter to Parameters.
        /// if you use some variable point to the variable in Parameters, they might change before BeforeCalculate
        /// </summary>
        public abstract void InitParameters();
        /// <summary>
        /// Add port to Ports
        /// </summary>
        public abstract void InitPorts();
        /// <summary>
        /// Add result parameter to Results
        /// </summary>
        public abstract void InitResults();
        /// <summary> 
        /// Default action is to check whether all Parameters is valid, override to customize
        /// </summary>
        protected virtual bool ParamtersValidate(out string message)
        {
            bool valid = true;
            message = "";
            foreach (var parameter in Parameters)
            {
                string temp = "";
                if (!((CapeParameterBase)parameter.Value).Validate(ref temp))
                {
                    message += "\n" + temp;
                    valid = false;
                }
            }
            if (valid) return true;
            else return false;
        }
        /// <summary>
        /// Default action is to check if an inlet and a outlet material is connected, override to customize
        /// </summary>
        protected virtual bool PortsValidate(out string message)
        {
            bool hasInlet = false;
            bool hasOutlet = false;
            message = "";
            foreach (var port in Ports)
            {
                CapeMaterialPort p = port.Value as CapeMaterialPort;
                if (p == null) continue;
                if (p.direction == CapePortDirection.CAPE_INLET && p.IsConnected())
                    hasInlet = true;
                else if (p.direction == CapePortDirection.CAPE_OUTLET && p.IsConnected())
                    hasOutlet = true;
            }
            if (!hasInlet)
                message = "Inlet material is not connected.";
            else if (!hasOutlet)
                message = "Outlet material is not connected.";
            return hasInlet && hasOutlet;
        }

        /// <summary>
        /// Called after the compound list is changed
        /// </summary>
        protected void UpdateCompoundList()
        {
            foreach (var item in Ports)
            {
                CapeMaterialPort p = item.Value as CapeMaterialPort;
                if (p != null)
                    p.Material.UpdateCompoundList();
            }
        }

        #endregion

        #region ICapeUnit
        /// <summary>
        /// raw Calculate interface called by simulator, NO need to modify this
        /// </summary>
        public virtual void Calculate()
        {
            UpdateCompoundList();
            SpecCalculator.BeforeCalculate();
            CapeDiagnostic.LogMessage("{0} : Calculate Start.",ComponentName);
            SpecCalculator.Calculate();
            CapeDiagnostic.LogMessage("{0} : Calculate Complete.", ComponentName);
            SpecCalculator.OutputResult();
        }
        /// <summary>
        /// This method will call ParameterValidate and PortValidate, in most case, no need to override this, just override ParameterValidate and PortValidate
        /// </summary>
        /// <paramCollection name="message">return the combined error message</paramCollection>
        public virtual bool Validate(ref string message)
        {
            string parameterMessage = null;
            string portMessage = null;

            Status = CapeValidationStatus.CAPE_VALID;

            if (!ParamtersValidate(out parameterMessage))
                Status = CapeValidationStatus.CAPE_INVALID;

            if (!PortsValidate(out portMessage))
                Status = CapeValidationStatus.CAPE_INVALID;

            message = "" + parameterMessage + '\n' + portMessage;
            if (Status == CapeValidationStatus.CAPE_VALID)
                return true;
            else
            {
                Debug.WriteLine("Validate fails. {0}", message);
                return false;
            }
        }
        /// <summary>
        /// raw ports interface called by simulator
        /// </summary>
        public virtual object ports
        {
            get
            {
                if (OnGetPorts != null)
                    OnGetPorts(Ports);
                return Ports;
            }
        }
        /// <summary>
        /// raw ValStatus interface called by simulator
        /// </summary>
        public CapeValidationStatus ValStatus
        {
            get { return Status; }
        }

        #endregion

        #region ICapeUtilities

        /// <summary>
        /// raw Initialize interface called by simulator, do nothing but write a debug message
        /// </summary>
        public virtual void Initialize()
        {
            Debug.WriteLine("Initialize");

            CasterUnitLocator.Register(unitId, this);

            if (OnInitialize != null) OnInitialize();

            Debug.WriteLine("Initialize Done.");
        }

        /// <summary>
        /// raw Terminate interface called by simulator, release COM object write a debug message
        /// </summary>
        public virtual void Terminate()
        {
            Debug.WriteLine("Terminate");
            if (OnTerminate != null) OnTerminate();

            if (simulationContext != null && simulationContext.GetType().IsCOMObject)
                Marshal.FinalReleaseComObject(simulationContext);

            foreach (var port in Ports)
            {
                ((CapeUnitPort)port.Value).Disconnect();
            }

            CasterUnitLocator.UnRegister(unitId);

            Debug.WriteLine("Terminate Done.");
        }

        /// <summary>
        /// raw Edit interface called by simulator, create a new thread to display GUI
        /// </summary>
        public int Edit()
        {
            Debug.WriteLine("Edit");
            if (GuiThread != null && GuiThread.IsAlive)
                return 0;     //if not abort, return 0
            GuiThread = new Thread(OpenEditWindow);
            GuiThread.SetApartmentState(ApartmentState.STA);
            GuiThread.Start();
            GuiThread.Join();
            return 1;
        }

        /// <summary>
        /// This method will open the edit window in STA mode
        /// if you want to open your custom window, override this method
        /// After your window closed, MUST abort GuiThread!!!
        /// </summary>
        [STAThread]
        protected virtual void OpenEditWindow()
        {
            ParameterWindow paramWindow = new ParameterWindow(this, (CapeCollection)Parameters, (CapeCollection)Results);
            if (paramWindow.ShowDialog() == true)
            {
                //foreach (var param in paramWindow.ViewModel.Parameters)
                //{
                //    ((ICapeParameter)Parameters[pair.Key]).value = ((ICapeParameter)pair.Value).value;
                //}
            }
            GuiThread.DisableComObjectEagerCleanup();
            GuiThread.Abort();
        }
        /// <summary>
        /// raw Parameters interface called by simulator
        /// </summary>
        public object parameters
        {
            get
            {
                if (OnGetParameters != null)
                    OnGetParameters(Parameters);
                return Parameters;
            }
        }
        /// <summary>
        /// set and get simulationContext interface passed by simulator
        /// </summary>
        public object simulationContext
        {
            get { return _simulationContext; }
            set
            {
                _simulationContext = value as ICapeSimulationContext;
                CapeDiagnostic.SetSimulationContext(value as ICapeDiagnostic);
                CapeMaterialTemplateSystem.SetSimulationContext(value as ICapeMaterialTemplateSystem);
                CapeCOSEUtilities.SetSimulationContext(value as ICapeCOSEUtilities);
            }
        }

        #endregion

        #region IPersist
        /// <summary>
        /// return CLSID
        /// </summary>
        /// <paramCollection name="pClassID"></paramCollection>
        public void GetClassID(out Guid pClassID)
        {
            pClassID = this.GetType().GUID;
        }

        #endregion

        #region IPersistStream
        /// <summary>
        /// whether the unit has been modified, raw interface
        /// </summary>
        public virtual int IsDirty()
        {
            return Convert.ToInt16(Dirty);
        }
        /// <summary>
        /// Get Total size of saving
        /// </summary>
        public virtual void GetSizeMax(out long pcbSize)
        {
            object[] objArray = new object[5];
            objArray[0] = this.ComponentName;
            objArray[1] = this.ComponentDescription;
            var paramArray = new CapeCollectionPair[this.Parameters.Count];
            this.Parameters.CopyTo(paramArray, 0);
            objArray[2] = paramArray;
            var resultArray = new CapeCollectionPair[this.Results.Count];
            this.Results.CopyTo(resultArray, 0);
            objArray[3] = resultArray;
            var portArray = new CapeCollectionPair[this.Ports.Count];
            this.Ports.CopyTo(portArray, 0);
            objArray[4] = portArray;

            MemoryStream memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, objArray);
            pcbSize = memoryStream.Length;
        }
        /// <summary>
        /// Save ComponentName,ComponentDescription,Parameters,Results,Ports
        /// </summary>
        public virtual void Save(IStream pStm, [MarshalAs(UnmanagedType.U1)]bool fClearDirty)
        {
            byte[] pv1 = new byte[2];
            object[] objArray = new object[5];
            objArray[0] = this.ComponentName;
            objArray[1] = this.ComponentDescription;
            var paramArray = new CapeCollectionPair[this.Parameters.Count];
            this.Parameters.CopyTo(paramArray, 0);
            objArray[2] = paramArray;
            var resultArray = new CapeCollectionPair[this.Results.Count];
            this.Results.CopyTo(resultArray, 0);
            objArray[3] = resultArray;
            var portArray = new CapeCollectionPair[this.Ports.Count];
            this.Ports.CopyTo(portArray, 0);
            objArray[4] = portArray;

            MemoryStream memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, objArray);
            byte[] numArray = memoryStream.ToArray();
            memoryStream.Close();
            pv1[0] = (byte)(numArray.Length % 256);
            pv1[1] = (byte)(numArray.Length / 256);
            try
            {
                pStm.Write(pv1, 2, IntPtr.Zero);
                IStream stream = pStm;
                byte[] pv2 = numArray;
                int length = pv2.Length;
                IntPtr pcbWritten = IntPtr.Zero;
                stream.Write(pv2, length, pcbWritten);
                Marshal.ReleaseComObject(pStm);
                if (!fClearDirty)
                    return;
                this.Dirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Marshal.ReleaseComObject(pStm);
            }
        }
        /// <summary>
        /// Load ComponentName,ComponentDescription,Parameters,Results,Ports
        /// </summary>
        public virtual void Load(IStream pStm)
        {
            this.Isloaded = true;
            byte[] pv = new byte[2];
            IntPtr pcbRead = IntPtr.Zero;
            pStm.Read(pv, 2, IntPtr.Zero);
            int cb = pv[1] * 256 + pv[0];
            byte[] numArray = new byte[cb];
            pStm.Read(numArray, cb, pcbRead);
            Marshal.ReleaseComObject(pStm);
            MemoryStream memoryStream = new MemoryStream(numArray);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Binder = new UBinder();
            try
            {
                object[] objArray = binaryFormatter.Deserialize(memoryStream) as object[];
                this.ComponentName = objArray[0].ToString();
                this.ComponentDescription = objArray[1].ToString();

                Parameters.Clear();
                foreach (var pair in objArray[2] as CapeCollectionPair[])
                {
                    Parameters.Add(pair);
                }
                Results.Clear();
                foreach (var pair in objArray[3] as CapeCollectionPair[])
                {
                    Results.Add(pair);
                }
                Ports.Clear();
                foreach (var pair in objArray[4] as CapeCollectionPair[])
                {
                    Ports.Add(pair);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            memoryStream.Close();
        }

        #endregion

        #region Register
        /// <summary>
        /// register function, no need to modify, the information will get through the Attribute of UnitOp class
        /// </summary>
        /// <paramCollection name="t"></paramCollection>
        [ComRegisterFunction]
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
            List<Guid> categoryGUIDs = new List<Guid>();
            //Get attribute like CapeName
            Assembly assembly = t.Assembly;
            foreach (var attribute in t.GetCustomAttributes(false))
            {
                progIdVersionIndenpent = attribute is CapeNameAttribute ? ((CapeNameAttribute)attribute).Name : progIdVersionIndenpent;
                description = attribute is CapeDescriptionAttribute ? ((CapeDescriptionAttribute)attribute).Description : description;
                capeVersion = attribute is CapeVersionAttribute ? ((CapeVersionAttribute)attribute).Version : capeVersion;
                about = attribute is CapeAboutAttribute ? ((CapeAboutAttribute)attribute).About : about;
                vendorURL = attribute is CapeVendorURLAttribute ? ((CapeVendorURLAttribute)attribute).VendorURL : vendorURL;
                helpURL = attribute is CapeHelpURLAttribute ? ((CapeHelpURLAttribute)attribute).HelpURL : helpURL;
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

                subKey = keyCLSID.CreateSubKey("TypeLib");
                subKey.SetValue(null, "{7f43b3e5-bf70-4a59-97fc-4ff7641066ae}");
                subKey.Close();

                subKey = keyCLSID.CreateSubKey("Implemented Categories");
                subKey.CreateSubKey("{678C09A5-7D66-11D2-A67D-00105A42887F}");
                subKey.CreateSubKey("{678C09A1-7D66-11D2-A67D-00105A42887F}");
                foreach (var guid in categoryGUIDs)
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

                keyCLSID.Close();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
            keyCLSID.Close();
        }
        /// <summary>
        /// Unregister function
        /// </summary>
        /// <paramCollection name="t"></paramCollection>
        [ComUnregisterFunction]
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
            //MessageBox.Show("Delete");
            Registry.ClassesRoot.DeleteSubKeyTree(progId, false);
            Registry.ClassesRoot.DeleteSubKeyTree(progIdVersionIndenpent, false);
            Registry.ClassesRoot.OpenSubKey("CLSID", true).DeleteSubKeyTree(t.GUID.ToString("B"), false);
        }

        #endregion

        //#region IClonable

        //public override object Clone()
        //{
        //    return new CasterUnitOperationBase((SpecCalculator)this.SpecCalculator.Clone(),this.ComponentName,this.ComponentDescription)
        //    {
        //        _canRename = this._canRename,
        //        _dirty = this._dirty,
        //        _isloaded = this._isloaded
        //    };
        //}

        //#endregion
    }
}
