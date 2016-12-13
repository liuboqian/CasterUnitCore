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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using CAPEOPEN;

namespace CasterUnitCore
{
    /// <summary>
    /// CO1.0 material wrapper
    /// </summary>
    [Serializable]
    public class MaterialObject10 : MaterialObject
    {
        #region Interface of Material

        private ICapeThermoMaterialObject _capeThermoMaterialObject;
        //private ICapeThermoPropertyPackage _capeThermoPropertyPackage;

        #endregion

        #region Constructor
        /// <summary>
        /// create a MaterialObject10
        /// </summary>
        public MaterialObject10()
            : this(null)
        {
        }
        /// <summary>
        /// create a MaterialObject10 connected to object, should only be invoked by CapeUnitPortBase
        /// </summary>
        public MaterialObject10(object objectToConnect)
        {
            SetMaterial(objectToConnect);
        }

        #endregion

        #region Material Object Manipulate

        public override object CapeThermoMaterialObject
        {
            get { return _capeThermoMaterialObject; }
        }

        public override void ClearAllProperties()
        {
            Debug.Assert(IsValid());
            _capeThermoMaterialObject.RemoveResults("");
        }

        public override MaterialObject Duplicate()
        {
            Debug.Assert(IsValid());
            if (_capeThermoMaterialObject == null) return null;
            MaterialObject newMaterial = new MaterialObject10();
            newMaterial.SetMaterial(_capeThermoMaterialObject.Duplicate());
            return newMaterial;
        }

        public override bool IsValid()
        {
            return _capeThermoMaterialObject != null;
        }

        public override bool SetMaterial(object materialObject)
        {
            if (materialObject == null) return false;

            if (materialObject is ICapeThermoMaterialObject)
            {
                //_alreadyFlashed = true;
                MaterialObjectVersion = 10;
                _capeThermoMaterialObject = materialObject as ICapeThermoMaterialObject;
                //_capeThermoPropertyPackage = materialObject as ICapeThermoPropertyPackage;
                //if (_capeThermoPropertyPackage == null)
                //    Debug.WriteLine("Aspen dont support CapeThermoPropertyPackage interface!");
            }
            else if (materialObject is MaterialObject10)
            {
                SetMaterial(((MaterialObject10)materialObject).CapeThermoMaterialObject);
            }
            else
                throw new ArgumentException("parameter is not a CO1.0 material object");

            //Set Proper Phase name
            if (AllowedPhases.FirstOrDefault(phase => phase.Value.Contains("vap")) != null)
                Phases.Vapor = AllowedPhases.First(phase => phase.Value.Contains("vap"));
            if (AllowedPhases.FirstOrDefault(phase => phase.Value.Contains("liq")) != null)
                Phases.Liquid = AllowedPhases.First(phase => phase.Value.Contains("liq"));
            if (AllowedPhases.FirstOrDefault(phase => phase.Value.Contains("solid")) != null)
                Phases.Liquid = AllowedPhases.First(phase => phase.Value.Contains("solid"));
            UpdateCompoundList();
            return true;
        }

        public override void Dispose()
        {
            if (_capeThermoMaterialObject != null && _capeThermoMaterialObject.GetType().IsCOMObject)
                Marshal.FinalReleaseComObject(_capeThermoMaterialObject);
            //if (_capeThermoPropertyPackage != null && _capeThermoPropertyPackage.GetType().IsCOMObject)
            //    Marshal.FinalReleaseComObject(_capeThermoPropertyPackage);
            _capeThermoMaterialObject = null;
        }

        #endregion

        #region DoFlash

        protected override bool CheckEquilibriumSpec(string[] flashSpec1, string[] flashSpec2, string solutionType)
        {
            return true;
        }

        public override bool DoFlash(string[] flashSpec1, string[] flashSpec2, string solutionType, bool showWarning = false)
        {
            try
            {
                _capeThermoMaterialObject.CalcEquilibrium(flashSpec1.First(), null);
                //_alreadyFlashed = true;
            }
            catch (Exception e)
            {
                if (showWarning)
                    MessageBox.Show("Flash fails. " + e.Message);
                Debug.WriteLine("Flash fails. {0}", e.Message);
                return false;
            }
            return true;
        }

        public override bool DoTPFlash(bool showWarning = false)
        {
            if (_capeThermoMaterialObject == null) return false;
            try
            {
                _capeThermoMaterialObject.CalcEquilibrium("TP", null);
                //_alreadyFlashed = true;
            }
            catch (Exception e)
            {
                if (showWarning)
                    MessageBox.Show("Flash fails. " + e.Message);
                Debug.WriteLine("Flash fails. {0}", e.Message);
                return false;
            }
            return true;
        }

        public override bool DoPHFlash(bool showWarning = false)
        {
            if (_capeThermoMaterialObject == null) return false;
            try
            {
                _capeThermoMaterialObject.CalcEquilibrium("PH", null);
                //_alreadyFlashed = true;
            }
            catch (Exception e)
            {
                if (showWarning)
                    MessageBox.Show("Flash fails. " + e.Message);
                Debug.WriteLine("Flash fails. {0}", e.Message);
                return false;
            }
            return true;
        }

        public override bool DoTHFlash(bool showWarning = false)
        {
            if (_capeThermoMaterialObject == null) return false;
            try
            {
                _capeThermoMaterialObject.CalcEquilibrium("TH", null);
                //_alreadyFlashed = true;
            }
            catch (Exception e)
            {
                if (showWarning)
                    MessageBox.Show("Flash fails. " + e.Message);
                Debug.WriteLine("Flash fails. {0}", e.Message);
                return false;
            }
            return true;
        }

        public override bool DoTVFFlash(bool showWarning = false)
        {
            if (_capeThermoMaterialObject == null) return false;
            try
            {
                _capeThermoMaterialObject.CalcEquilibrium("TVF", null);
                //_alreadyFlashed = true;
            }
            catch (Exception e)
            {
                if (showWarning)
                    MessageBox.Show("Flash fails. " + e.Message);
                Debug.WriteLine("Flash fails. {0}", e.Message);
                return false;
            }
            return true;
        }

        public override bool DoPVFlash(bool showWarning = false)
        {
            if (_capeThermoMaterialObject == null) return false;
            try
            {
                _capeThermoMaterialObject.CalcEquilibrium("PVF", null);
                //_alreadyFlashed = true;
            }
            catch (Exception e)
            {
                if (showWarning)
                    MessageBox.Show("Flash fails. " + e.Message);
                Debug.WriteLine("Flash fails. {0}", e.Message);
                return false;
            }
            return true;
        }

        #endregion

        #region Phase

        public override Phases[] GetListOfAllowedPhase(out string[] phaseAggregationList, out string keyCompoundId)
        {
            object allowedPhaseObject = null;
            phaseAggregationList = null;  //CO1.0 Not support
            keyCompoundId = null;         //CO1.0 Not support
            if (_capeThermoMaterialObject == null) return null;
            //try
            //{
            //    if (_capeThermoPropertyPackage == null)
            //        allowedPhaseObject = _capeThermoMaterialObject.PhaseIds;
            //    else
            //        allowedPhaseObject = _capeThermoPropertyPackage.GetPhaseList();

            //}
            //catch (Exception e)
            //{
            Debug.WriteLine("Allowed phase not found, use Vapor and Liquid.");
            allowedPhaseObject = new[] { "Vapor", "Liquid" };
            //}

            string[] phaseStringList = allowedPhaseObject as string[];
            Phases[] phaseList = (from phaseString in phaseStringList select new Phases(phaseString)).ToArray();
            return phaseList;
        }

        public override Phases[] GetListOfPresentPhases(out eCapePhaseStatus[] presentPhaseStatus)
        {
            object phaseLabel;
            presentPhaseStatus = null;   //CO1.0 Not support
            if (_capeThermoMaterialObject == null) return null;
            try
            {
                phaseLabel = _capeThermoMaterialObject.PhaseIds;
            }
            catch (Exception e)
            {
                Debug.WriteLine("No present phase. {0}", e.Message);
                phaseLabel = new string[0];
            }

            string[] phaseStringList = phaseLabel as string[];
            Phases[] phaseList = (from phaseString in phaseStringList
                                  select new Phases(phaseString)).ToArray();
            return phaseList;
        }

        public override void SetListOfPresentPhases(IEnumerable<Phases> presentPhases, IEnumerable<eCapePhaseStatus> presentPhasesStatus)
        {
            throw new Exception("CO1.0 cannot set Present Phases");
        }

        #endregion

        #region CompoundId

        public override string[] Formulas
        {
            get { throw new Exception("formula name is unavailable for CO1.0"); }
        }

        public override string[] UpdateCompoundList()
        {
            try
            {
                return aliasName = _capeThermoMaterialObject.ComponentIds as string[];
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to get compound list. Make sure to call UpdateComoundList after compound list changed. {0}", e.Message);
                return null;
            }
        }

        #endregion

        #region Overall Property

        public override double[] GetOverallPropList(string propName, PropertyBasis basis)
        {
            return _capeThermoMaterialObject.GetProp(propName, "Overall", null, "mixture", basis.ToString()) as double[];
        }

        public override void SetOverallPropList(string propName, PropertyBasis basis, IEnumerable<double> value)
        {
            double[] temp = value as double[] ?? value.ToArray();
            _capeThermoMaterialObject.SetProp(propName, "Overall", null, "mixture", basis.ToString(), temp);
            //_alreadyFlashed = false;
        }

        public override double T
        {
            get
            {
                object value = _capeThermoMaterialObject.GetProp("temperature", "Overall", null, "mixture", PropertyBasis.Undefined.ToString());
                return (value as double[]).SingleOrDefault();
            }
            set
            {
                SetOverallPropDouble("temperature", PropertyBasis.Undefined, value);
            }
        }

        public override double P
        {
            get
            {
                object value = _capeThermoMaterialObject.GetProp("pressure", "Overall", null, "mixture", PropertyBasis.Undefined.ToString());
                return (value as double[]).SingleOrDefault();
            }
            set
            {
                SetOverallPropDouble("pressure", PropertyBasis.Undefined, value);
            }
        }

        public override double TotalFlow
        {
            get
            {
                object value = _capeThermoMaterialObject.GetProp("totalFlow", "Overall", null, "mixture", PropertyBasis.Mole.ToString());
                return (value as double[]).SingleOrDefault();
            }
            set
            {
                SetOverallPropDouble("totalFlow", PropertyBasis.Mole, value);
            }
        }

        public override double VaporFraction
        {
            get
            { //这里的LINQ是否能工作？？？
                object value = _capeThermoMaterialObject.GetProp("phaseFraction", Phases.Vapor.Value, null, "mixture", PropertyBasis.Mole.ToString());
                if (value == null) return 0;
                return (value as double[]).SingleOrDefault();
            }
            set
            {
                //如果没有气相
                try
                {
                    SetSinglePhasePropDouble("phaseFraction", Phases.Vapor, PropertyBasis.Mole, value);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Set vapor fraction fails. {0}", e.Message);
                }
            }
        }

        public override Dictionary<string, double> Composition
        {
            get
            {
                var composition = new Dictionary<string, double>();
                var compoundList = Compounds;
                object value = _capeThermoMaterialObject.GetProp("fraction", "Overall", null, "mixture", PropertyBasis.Mole.ToString());
                double[] compositionList = value as double[];
                for (int i = 0; i < compoundList.Length; i++)
                {
                    composition.Add(compoundList[i], compositionList[i]);
                }
                return composition;
            }
            set
            {
                double[] composition = new double[CompoundNum];
                for (int i = 0; i < CompoundNum; i++)
                {
                    value.TryGetValue(Compounds[i], out composition[i]);
                }
                SetOverallPropList("fraction", PropertyBasis.Mole, composition);
            }
        }

        #endregion

        #region Single Phase Property

        public override string[] AvailableSinglePhaseProp
        {
            get { return _capeThermoMaterialObject.GetPropList(); }
        }

        public override double[] GetSinglePhasePropList(string propName, Phases phase, PropertyBasis basis, bool calculate = true)
        {
            if (PresentPhases.All(p => p.Value != phase.Value))
                return new double[CompoundNum];     //default is 0 for every element

            object value = null;
            try
            {
                if (calculate)
                    _capeThermoMaterialObject.CalcProp(new[] { propName }, new[] { phase.Value }, "mixture");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Calculate single phase prop {0} fails. {1}", propName, e.Message);
            }
            value = _capeThermoMaterialObject.GetProp(propName, phase.Value, null, "mixture", basis.ToString());
            return value as double[];
        }

        public override void SetSinglePhasePropList(string propName, Phases phase, PropertyBasis basis, IEnumerable<double> value)
        {
            if (PresentPhases.All(p => p.Value != phase.Value))
                PresentPhases = AllowedPhases;
            double[] temp = value as double[] ?? value.ToArray();
            _capeThermoMaterialObject.SetProp(propName, phase.Value, null, null, basis.ToString(), temp);
            //_alreadyFlashed = false;
        }

        #endregion

        #region Two Phase Property

        public override string[] AvailableTwoPhaseProp
        {
            get { return _capeThermoMaterialObject.GetPropList(); }
        }

        public override double[] GetTwoPhasePropList(string propName, Phases phase1, Phases phase2, PropertyBasis basis, bool calculate = true)
        {
            string phaseName = null;
            string[] phaseList = { phase1.Value, phase2.Value };
            if (phaseList.Contains(Phases.Vapor.Value) && phaseList.Contains(Phases.Liquid.Value))
                phaseName = Phases.Vapor.Value + Phases.Liquid.Value;
            else
                phaseName = phase1.Value + phase2.Value;
            try
            {
                if (calculate)
                    _capeThermoMaterialObject.CalcProp(new[] { "kvalues" }, new[] { phaseName }, "mixture");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Calculate two phase prop {0} fails. {1}", propName, e.Message);
            }
            return _capeThermoMaterialObject.GetProp("kvalues", phaseName, null, "mixture", basis.ToString());
        }

        public override void SetTwoPhasePropList(string propName, Phases phase1, Phases phase2, PropertyBasis basis, IEnumerable<double> value)
        {
            string phaseName = null;
            string[] phaseList = { phase1.Value, phase2.Value };
            if (phaseList.Contains(Phases.Vapor.Value) && phaseList.Contains(Phases.Liquid.Value))
                phaseName = Phases.Vapor.Value + Phases.Liquid.Value;
            else
                phaseName = phase1.Value + phase2.Value;
            double[] temp = value as double[] ?? value.ToArray();
            _capeThermoMaterialObject.SetProp(propName, phaseName, null, "mixture", basis.ToString(), temp);
        }

        #endregion

        #region Constant Property & T-Dependent Property & P-Dependent Property

        public override string[] AvailableConstProp
        {
            get { throw new System.Exception("CO1.0 Not support AvailableConstProp."); }
        }

        public override string[] AvailableTDependentProp
        {
            get { throw new System.Exception("CO1.0 Not support AvailableTDependentProp."); }
        }

        public override string[] AvailablePDependentProp
        {
            get { throw new System.Exception("CO1.0 Not support AvailablePDependentProp."); }
        }

        public override string[] AvailableUniversalConstProp
        {
            get { throw new System.Exception("CO1.0 Not support AvailableUniversalConstProp."); }
        }

        public override double GetCompoundConstPropDouble(string propName, string compoundId)
        {
            object[] value = _capeThermoMaterialObject.GetComponentConstant(new[] { propName }, new[] { compoundId });
            return Convert.ToDouble(value.SingleOrDefault());
        }

        public override double GetCompoundTDependentProp(string propName, string compoundId, double T)
        {
            throw new System.Exception("CO1.0 Not support GetCompoundTDependentProp");
        }

        public override double GetCompoundPDependentProp(string propName, string compoundId, double P)
        {
            throw new System.Exception("CO1.0 Not support GetCompoundPDependentProp");
        }

        public override double GetUniversalConstProp(string constantId)
        {
            return (_capeThermoMaterialObject.GetUniversalConstant(new[] { constantId }) as double[]).SingleOrDefault();
        }

        #endregion
    }
}
