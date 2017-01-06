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
using CAPEOPEN;

namespace CasterUnitCore
{
    /// <summary>
    /// Uniform wrapper of material object
    /// </summary>
    [Serializable]
    public abstract class MaterialObject
        : IDisposable
    {
        /// <summary>
        /// get material object version, can be 11 or 10
        /// </summary>
        public int MaterialObjectVersion;
    
        /// <summary>
        /// whether the material has been flashed
        /// </summary>
        //protected bool _alreadyFlashed;
   
        /// <summary>
        /// A string array used to make CompoundList property faster, avoid calling Thermo System, contains the aliasName of compound
        /// </summary>
        protected string[] aliasName;

        #region Material Object Manipulate
     
        /// <summary>
        /// return COM raw interface
        /// </summary>
        public abstract object CapeThermoMaterialObject { get; }
   
        /// <summary>
        /// Clear all property in material, set to initial state
        /// </summary>
        public abstract void ClearAllProperties();

        /// <summary>
        /// Disconnect COM component, release the COM component, this method will call IDispose.
        /// </summary>
        public void Destroy()
        {
            Dispose();
        }
   
        /// <summary>
        /// Duplicate this material, it is the only way to create a new material object
        /// </summary>
        /// <returns>return MaterialObject10 or MaterialObject11</returns>
        public abstract MaterialObject Duplicate();
   
        /// <summary>
        /// Whether the COM material object is connected to this class
        /// </summary>
        public abstract bool IsValid();
  
        /// <summary>
        /// Set material object raw COM interface or MaterialObject to this class
        /// </summary>
        /// <paramCollection name="material">raw COM interface like ICapeThermoMaterial</paramCollection>
        /// <returns></returns>
        public abstract bool SetMaterial(object material);
  
        /// <summary>
        /// Set MaterialObject to this class
        /// </summary>
        /// <paramCollection name="material">MaterialObject</paramCollection>
        /// <returns></returns>
        public bool SetMaterial(MaterialObject material)
        {
            return SetMaterial((object)material);
        }
  
        /// <summary>
        /// Release resources, do nothing for now
        /// </summary>
        public abstract void Dispose();

        #endregion

        #region DoFlash
    
        /// <summary>
        /// Check if the specified condition is available by thermo system, CO1.0 will always return true
        /// </summary>
        protected abstract bool CheckEquilibriumSpec(string[] flashSpec1, string[] flashSpec2, string solutionType);
     
        /// <summary>
        /// Do Flash by flashSpec, in most case you don't need this, will check the spec first
        /// </summary>
        /// <paramCollection name="flashSpec1">
        /// 1.1: the first flash specification, eg. string[3]{name，basis，phase}
        /// 1.0: flash specification, eg. string[1]{specification}
        /// </paramCollection>
        /// <paramCollection name="flashSpec2">
        /// 1.1: the second flash specification，eg. string[3]{name，basis，phase}
        /// 1.0: null
        /// </paramCollection>
        /// <paramCollection name="showWarning">whether show a MessageBox to warn flash failure</paramCollection>
        /// <paramCollection name="solutionType"></paramCollection>
        /// <returns>calculate state</returns>
        public abstract bool DoFlash(string[] flashSpec1, string[] flashSpec2, string solutionType, bool showWarning = false);
       
        /// <summary>
        /// Temperature-Pressure flash, must set overall T,P,TotalFlow,Composition first, if fails, show message on Debug
        /// </summary>
        /// <paramCollection name="showWarning">whether show a MessageBox to warn flash failure</paramCollection>
        public abstract bool DoTPFlash(bool showWarning = false);
       
        /// <summary>
        /// Pressure-Enthalpy flash, must set overall P,H,TotalFlow,Composition first, if fails, show message on Debug
        /// </summary>
        /// <paramCollection name="showWarning">whether show a MessageBox to warn flash failure</paramCollection>
        public abstract bool DoPHFlash(bool showWarning = false);
     
        /// <summary>
        /// Temperature-Enthalpy flash, must set overall T,H,TotalFlow,Composition first, if fails, show message on Debug
        /// </summary>
        /// <paramCollection name="showWarning">whether show a MessageBox to warn flash failure</paramCollection>
        public abstract bool DoTHFlash(bool showWarning = false);
     
        /// <summary>
        /// Temperature-VaporFraction flash, must set overall T,VaporFraction,TotalFlow,Composition first, if fails, show message on Debug
        /// </summary>
        /// <paramCollection name="showWarning">whether show a MessageBox to warn flash failure</paramCollection>
        public abstract bool DoTVFFlash(bool showWarning = false);
     
        /// <summary>
        /// Pressure-VaporFraction flash, must set overall P,VaporFraction,TotalFlow,Composition first, if fails, show message on Debug
        /// </summary>
        /// <paramCollection name="showWarning">whether show a MessageBox to warn flash failure</paramCollection>
        public abstract bool DoPVFlash(bool showWarning = false);

        #endregion

        #region Phase
     
        /// <summary>
        /// Get all allowed phase by thermo system
        /// </summary>
        /// <paramCollection name="phaseAggregationList">null for CO1.0</paramCollection>
        /// <paramCollection name="keyCompoundId">null for CO1.0</paramCollection>
        public abstract Phases[] GetListOfAllowedPhase(out string[] phaseAggregationList, out string keyCompoundId);
      
        /// <summary>
        /// Get present phases, not alway reliable, sometimes may have some 0 flow phase
        /// 获取当前存在的相态，未必可靠（有时会有多出来的相）
        /// </summary>
        /// <paramCollection name="presentPhaseStatus">the state of phase, null for CO1.0</paramCollection>
        public abstract Phases[] GetListOfPresentPhases(out eCapePhaseStatus[] presentPhaseStatus);
       
        /// <summary>
        /// set present phase, unavailable for CO1.0
        /// </summary>
        public abstract void SetListOfPresentPhases(IEnumerable<Phases> presentPhases, IEnumerable<eCapePhaseStatus> presentPhasesStatus);
      
        /// <summary>
        /// get or set PresentPhases, setter is unavailable for CO1.0
        /// </summary>
        public Phases[] PresentPhases
        {
            get
            {
                eCapePhaseStatus[] obsolete;
                return GetListOfPresentPhases(out obsolete);
            }
            set
            {
                eCapePhaseStatus[] status = new eCapePhaseStatus[value.Length];
                for (int i = 0; i < status.Length; i++)
                {
                    status[i] = eCapePhaseStatus.CAPE_UNKNOWNPHASESTATUS;
                }
                SetListOfPresentPhases(value, status);
            }
        }
     
        /// <summary>
        /// get alloed phases, always used to get actual phase name in diffent software
        /// </summary>
        public Phases[] AllowedPhases
        {
            get
            {
                string[] phaseAggregationList;
                string keyCompound;
                return GetListOfAllowedPhase(out phaseAggregationList, out keyCompound);
            }
        }

        /// <summary>
        /// Get the vapor part of current material, if this material is totally liquid, will return null
        /// </summary>
        public MaterialObject VaporMaterial
        {
            get
            {
                MaterialObject mat = this.Duplicate();
                if (mat.VaporFraction != 0)
                {
                    mat.SetOverallTPFlowCompositionAndFlash(
                        this.T,
                        this.P,
                        this.TotalFlow * this.VaporFraction,
                        this.GetSinglePhaseComposition(Phases.Vapor, PropertyBasis.Mole));
                    return mat;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Get the liquid part of current material, if this material is totally vapor, will return null
        /// </summary>
        public MaterialObject LiquidMaterial
        {
            get
            {
                MaterialObject mat = this.Duplicate();
                if (VaporFraction < 1)
                {
                    mat.SetOverallTPFlowCompositionAndFlash(
                        this.T,
                        this.P,
                        this.TotalFlow * (1 - this.VaporFraction),
                        this.GetSinglePhaseComposition(Phases.Liquid, PropertyBasis.Mole));
                    return mat;
                }
                else
                    return null;
            }
        }

        #endregion

        #region CompoundId
    
        /// <summary>
        /// get number of compound, will return the previous stored data
        /// </summary>
        public int CompoundNum
        {
            get { return aliasName.Length; }
        }

        /// <summary>
        /// get compound aliasName, only get once really when SetMaterial is invoked,
        /// other time it will return the previous stored data.
        /// If your need to get CompoundList in somewhere else than Calculate, call UpdateCompoundList first.
        /// </summary>
        public string[] Compounds
        {
            get
            {
                string[] temp = new string[CompoundNum];
                aliasName.CopyTo(temp, 0);
                return temp;
            }
        }

        /// <summary>
        /// get compound formulaName, unavailable for CO1.0
        /// </summary>
        public abstract string[] Formulas { get; }

        /// <summary>
        /// Force to call the Thermo System, to get a new CompoundList. 
        /// If your need to get CompoundList in somewhere else than Calculate, call this method first.
        /// </summary>
        /// <returns></returns>
        public abstract string[] UpdateCompoundList();

        /// <summary>
        /// Return the count of compounds which flow is not zero
        /// </summary>
        public int ExistingCompoundCount
        {
            get { return Compounds.Count(c => Composition[c] != 0); }
        }

        /// <summary>
        /// Return compounds which flow is not zero
        /// </summary>
        public string[] ExistingCompounds
        {
            get { return Compounds.Where(c => Composition[c] != 0).ToArray(); }
        }

        #endregion

        #region Overall Property

        /// <summary>
        /// get overall property, return a double number, if not present, return 0; if result is an array, throw an exception
        /// </summary>
        public double GetOverallPropDouble(string propName, PropertyBasis basis)
        {
            return GetOverallPropList(propName, basis).SingleOrDefault();
        }

        /// <summary>
        /// get overall property, return a double array, if result is a single number, will return a single element array
        /// </summary>
        public abstract double[] GetOverallPropList(string propName, PropertyBasis basis);

        /// <summary>
        /// set overall property
        /// </summary>
        public void SetOverallPropDouble(string propName, PropertyBasis basis, double value)
        {
            SetOverallPropList(propName, basis, new[] { value });
        }

        /// <summary>
        /// set overall property
        /// </summary>
        /// <paramCollection name="value">the value to be set, MUST be IEnumerable double, if you want to set other data structure, use the raw interface</paramCollection>
        public abstract void SetOverallPropList(string propName, PropertyBasis basis, IEnumerable<double> value);

        /// <summary>
        /// overall temperature
        /// </summary>
        public abstract double T { get; set; }

        /// <summary>
        /// overall pressure
        /// </summary>
        public abstract double P { get; set; }

        /// <summary>
        /// overall totalflow, mole basis
        /// </summary>
        public abstract double TotalFlow { get; set; }

        /// <summary>
        /// Overall composition, mole basis
        /// If you set a partial composition with some compounds is not in the dict, the method will take them as 0.
        /// </summary>
        public abstract Dictionary<string, double> Composition { get; set; }

        /// <summary>
        /// overall composition, mole basis
        /// </summary>
        public Dictionary<string, double> CompositionFlow
        {
            get
            {
                Dictionary<string, double> flow = Composition;
                foreach (var c in Compounds)
                    flow[c] *= TotalFlow;
                return flow;
            }
            set
            {
                Dictionary<string, double> flow = new Dictionary<string, double>(value);
                TotalFlow = 0;
                foreach (var c in flow)
                    TotalFlow += c.Value;
                foreach (var c in flow)
                    flow[c.Key] /= TotalFlow;
                Composition = flow;
            }
        }

        /// <summary>
        /// fraction of vapor phase, mole basis
        /// </summary>
        public abstract double VaporFraction { get; set; }

        /// <summary>
        /// overall enthalpy, unit is J/s
        /// </summary>
        public double Enthalpy
        {
            get
            {
                return PresentPhases.Sum(phase =>
                        GetSinglePhasePropDouble("enthalpy", phase, PropertyBasis.Mole)
                        * GetSinglePhaseFlow(phase, PropertyBasis.Mole));
            }
            set
            {
                SetOverallPropDouble("enthalpy", PropertyBasis.Mole, value / TotalFlow);
            }
        }

        /// <summary>
        /// overall entropy, unit is J/s
        /// </summary>
        public double Entropy
        {
            get
            {
                return PresentPhases.Sum(phase =>
                    GetSinglePhasePropDouble("entropy", phase, PropertyBasis.Mole)
                    * GetSinglePhaseFlow(phase, PropertyBasis.Mole));
            }
        }

        /// <summary>
        /// total volume, unit is m3/s
        /// </summary>
        public double VolumeFlow
        {
            get
            {
                return PresentPhases.Sum(phase =>
                    GetSinglePhaseFlow(phase, PropertyBasis.Mole)
                    / GetSinglePhasePropDouble("density", phase, PropertyBasis.Mole));
            }
        }

        /// <summary>
        /// overall gibbs free energy, unit is J/s
        /// </summary>
        public double GibbsEnergy
        {
            get
            {
                return PresentPhases.Sum(phase =>
                    GetSinglePhasePropDouble(SearchPropName(PropertyCategory.SinglePhaseProp, "gibbs", "gibbsEnergy"), phase, PropertyBasis.Mole)
                    * GetSinglePhaseFlow(phase, PropertyBasis.Mole));
            }
        }

        /// <summary>
        /// get K value between Vapor and Liquid phase
        /// </summary>
        public Dictionary<string, double> KValue
        {
            get
            {
                double[] Karray = GetTwoPhasePropList("kvalue", Phases.Vapor, Phases.Liquid, PropertyBasis.Undefined);
                Dictionary<string, double> K = new Dictionary<string, double>();
                for (int i = 0; i < CompoundNum; i++)
                    K[Compounds[i]] = Karray[i];
                return K;
            }
        }

        /// <summary>
        /// get overall T,P,totalFlow,Composition
        /// </summary>
        public bool GetOverallTPFlowComposition(out double T, out double P, out double totalFlow,
            out Dictionary<string, double> composition)
        {
            T = this.T;
            P = this.P;
            totalFlow = this.TotalFlow;
            composition = this.Composition;
            return true;
        }

        /// <summary>
        /// set overall T,P,totalFlow,Composition, and do a TP flash
        /// </summary>
        public bool SetOverallTPFlowCompositionAndFlash(double T, double P, double totalFlow,
            Dictionary<string, double> composition)
        {
            this.T = T;
            this.P = P;
            this.TotalFlow = totalFlow;
            this.Composition = composition;
            //_alreadyFlashed = false;
            return DoTPFlash();
        }

        #endregion

        #region Single Phase Property

        /// <summary>
        /// get all available single phase property, for CO1.0 return all property
        /// </summary>
        public abstract string[] AvailableSinglePhaseProp { get; }

        /// <summary>
        /// get single phase property, return a double array, will try to calculate property first, if phase is not present, return new double[CompoundNum], all 0
        /// </summary>
        public abstract double[] GetSinglePhasePropList(string propName, Phases phase, PropertyBasis basis, bool calculate = true);

        /// <summary>
        /// set single phase property
        /// </summary>
        /// <paramCollection name="value">the value to be set, MUST be IEnumerable double, if you want to set other data structure, use the raw interface</paramCollection>
        public abstract void SetSinglePhasePropList(string propName, Phases phase, PropertyBasis basis, IEnumerable<double> value);
        
        /// <summary>
        /// get single phase property, return a double number, will try to calculate property first, if not present, return 0; if property is an array, throw exception
        /// </summary>
        public double GetSinglePhasePropDouble(string propName, Phases phase, PropertyBasis basis, bool calculate = true)
        {
            if (PresentPhases.All(p => p.Value != phase.Value)) return 0;
            return GetSinglePhasePropList(propName, phase, basis, calculate).SingleOrDefault();
        }
   
        /// <summary>
        /// set single pahse property
        /// </summary>
        public void SetSinglePhasePropDouble(string propName, Phases phase, PropertyBasis basis, double value)
        {
            SetSinglePhasePropList(propName, phase, basis, new[] { value });
        }
    
        /// <summary>
        /// get single phase flow
        /// </summary>
        public double GetSinglePhaseFlow(Phases phase, PropertyBasis basis)
        {
            return GetSinglePhasePropDouble("phaseFraction", phase, basis, false) * TotalFlow;
        }
     
        /// <summary>
        /// get the vapor or liquid part composition of the material, must be flashed first!
        /// </summary>
        public Dictionary<string, double> GetSinglePhaseComposition(Phases phase, PropertyBasis basis)
        {
            var composition = new Dictionary<string, double>();
            var compoundList = Compounds;
            object value = null;
            double[] compositionList = GetSinglePhasePropList("fraction", phase, basis);
            for (int i = 0; i < compoundList.Length; i++)
            {
                composition.Add(compoundList[i], compositionList[i]);
            }
            return composition;

        }
     
        /// <summary>
        /// set single phase composition, eg.set V or L in RadFrac
        /// </summary>
        public void SetSinglePhaseComposition(Phases phase, PropertyBasis basis, Dictionary<string, double> composition)
        {
            double[] temp = new double[CompoundNum];
            for (int i = 0; i < CompoundNum; i++)
            {
                temp[i] = composition[Compounds[i]];
            }
            SetSinglePhaseComposition(phase, PropertyBasis.Mole, temp);
        }
     
        /// <summary>
        /// Set composition of single phase, the composition is ordered by Compounds
        /// </summary>
        public void SetSinglePhaseComposition(Phases phase, PropertyBasis basis, IEnumerable<double> composition)
        {
            var enumerable = composition as double[] ?? composition.ToArray();
            SetSinglePhasePropList("fraction", phase, PropertyBasis.Mole, enumerable);
        }

        #endregion

        #region Two Phase Property
  
        /// <summary>
        /// get all available two phase property, for CO1.0 return all property
        /// </summary>
        public abstract string[] AvailableTwoPhaseProp { get; }
      
        /// <summary>
        /// Get two phase property, if a phase is not present, will return new double[CompoundNum]
        /// </summary>
        public abstract double[] GetTwoPhasePropList(string propName, Phases phase1, Phases phase2, PropertyBasis basis, bool calculate = true);
    
        /// <summary>
        /// set two phase property, 
        /// </summary>
        /// <paramCollection name="value">the value to be set, MUST be IEnumerable double, if you want to set other data structure, use the raw interface</paramCollection>
        public abstract void SetTwoPhasePropList(string propName, Phases phase1, Phases phase2, PropertyBasis basis, IEnumerable<double> value);
   
        /// <summary>
        /// get two phase property, return a double number, will try to calculate property first, if not present, return 0; if property is an array, throw exception
        /// </summary>
        public double GetTwoPhasePropDouble(string propName, Phases phase1, Phases phase2, PropertyBasis basis, bool calculate = true)
        {
            if (PresentPhases.All(p => p.Value != phase1.Value)
                || PresentPhases.All(p => p.Value != phase2.Value))
                return 0;
            return GetTwoPhasePropList(propName, phase1, phase2, basis, calculate).SingleOrDefault();
        }
    
        /// <summary>
        /// set two phase property
        /// </summary>
        public void SetTwoPhasePropDouble(string propName, Phases phase1, Phases phase2, PropertyBasis basis, double value)
        {
            SetTwoPhasePropList(propName, phase1, phase2, basis, new[] { value });
        }

        #endregion

        #region Constant Property & T-Dependent Property & P-Dependent Property & Universal Constant
     
        /// <summary>
        /// get all available constant property, unavailable for CO1.0 
        /// </summary>
        public abstract string[] AvailableConstProp { get; }
      
        /// <summary>
        /// get all available T-Dependent constant property, unavailable for CO1.0 
        /// </summary>
        public abstract string[] AvailableTDependentProp { get; }
      
        /// <summary>
        /// get all available P-Dependent constant property, unavailable for CO1.0 
        /// </summary>
        public abstract string[] AvailablePDependentProp { get; }
      
        /// <summary>
        /// get all available universal constant property, unavailable for CO1.0 
        /// </summary>
        public abstract string[] AvailableUniversalConstProp { get; }
      
        /// <summary>
        /// get constant property, return a double number, if not present, return 0; if property is an array, throw exception
        /// </summary>
        public abstract double GetCompoundConstPropDouble(string propName, string compoundId);
     
        /// <summary>
        /// get T-Dependent property, unavailable for CO1.0, return a double number, if not present, return 0; if property is an array, throw exception
        /// </summary>
        public abstract double GetCompoundTDependentProp(string propName, string compoundId, double T);
    
        /// <summary>
        /// get P-Dependent property, unavailable for CO1.0, return a double number, if not present, return 0; if property is an array, throw exception
        /// </summary>
        public abstract double GetCompoundPDependentProp(string propName, string compoundId, double P);
     
        /// <summary>
        /// get universal constant property, return a double number, if not present, return 0; if property is an array, throw exception
        /// </summary>
        /// <paramCollection name="constantId"></paramCollection>
        /// <returns></returns>
        public abstract double GetUniversalConstProp(string constantId);
     
        /// <summary>
        /// get constant property for all Compounds
        /// </summary>
        /// <paramCollection name="propName"></paramCollection>
        /// <returns></returns>
        public double[] GetAllCompoundConstProp(string propName)
        {
            double[] value = new double[CompoundNum];
            for (int i = 0; i < CompoundNum; i++)
                value[i] = GetCompoundConstPropDouble(propName, Compounds[i]);
            return value;
        }

        #endregion

        #region Misc

        /// <summary>
        /// Search possible propertyName in Available Prop List, find the first one start will the possible name, if not found, return defaultName
        /// </summary>
        /// <paramCollection name="category">category of property</paramCollection>
        /// <paramCollection name="possibleName">possible name of the property, like "gibbs" for "gibbsEnergy" or "gibbsFreeEnergy"</paramCollection>
        /// <paramCollection name="defaultName">if not found, return defaultName, default defaultName is possibleName</paramCollection>
        /// <returns></returns>
        public string SearchPropName(PropertyCategory category, string possibleName, string defaultName = null)
        {
            string propName = null;
            try
            {
                if (category == PropertyCategory.SinglePhaseProp)
                    propName = AvailableSinglePhaseProp.FirstOrDefault(x => x.ToLower().Contains(possibleName));
                else if (category == PropertyCategory.TwoPhaseProp)
                    propName = AvailableTwoPhaseProp.FirstOrDefault(x => x.ToLower().Contains(possibleName));
                else if (category == PropertyCategory.ConstantProp)
                    propName = AvailableConstProp.FirstOrDefault(x => x.ToLower().Contains(possibleName));
                else if (category == PropertyCategory.TDependentProp)
                    propName = AvailableTDependentProp.FirstOrDefault(x => x.ToLower().Contains(possibleName));
                else if (category == PropertyCategory.PDependentProp)
                    propName = AvailablePDependentProp.FirstOrDefault(x => x.ToLower().Contains(possibleName));
                else if (category == PropertyCategory.UniversalConstantProp)
                    propName = AvailableUniversalConstProp.FirstOrDefault(x => x.ToLower().Contains(possibleName));
            }
            catch (Exception)
            {
                Debug.WriteLine("Property {0} not found, use {1}", possibleName, defaultName);
            }
            if (propName == null)
            {
                propName = defaultName ?? possibleName;
                Debug.WriteLine("Property {0} not found, use {1}", possibleName, propName);
            }
            return propName;
        }

        public override string ToString()
        {
            return string.Format("T:{0} P:{1} TotalFlow:{2}", T, P, TotalFlow);
        }

        #endregion

    }//类结尾
}
