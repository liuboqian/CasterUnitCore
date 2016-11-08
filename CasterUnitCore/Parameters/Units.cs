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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace CasterUnitCore
{
    /// <summary>
    /// UnitOp relate, get unit list, convert between units
    /// </summary>
    public static class Units
    {
        static readonly XmlNodeList unitNodeList;
        static readonly XmlNodeList unitCategoryList;

        /// <summary>
        /// load XMLDocument contains units, use GetManifestResourceStream("CasterUnitCore.Parameters.unit.xml")
        /// </summary>
        static Units()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            //load unit.xml
            XmlDocument unitDoc = new XmlDocument();
            Assembly assembly = Assembly.GetAssembly(typeof(Units));
            Stream s = assembly.GetManifestResourceStream("CasterUnitCore.Parameters.unit.xml");
            unitDoc.Load(s);
            unitNodeList = unitDoc.SelectNodes("Units/CurrentUnit_Specs");
            //load unitCategory
            XmlDocument unitCategoryDoc = new XmlDocument();
            s = assembly.GetManifestResourceStream("CasterUnitCore.Parameters.unitCategory.xml");
            unitCategoryDoc.Load(s);
            unitCategoryList = unitCategoryDoc.SelectNodes("CategorySpecifications/Category_Spec");
        }
        /// <summary>
        /// Get unit list of specified category
        /// </summary>
        public static List<string> GetUnitList(UnitCategoryEnum unitCategory)
        {
            return (from XmlNode node in unitNodeList
                    where node.SelectSingleNode("Category").InnerText == unitCategory.ToString()
                    select node.SelectSingleNode("CurrentUnit").InnerText).ToList();
        }
        /// <summary>
        /// Convert from origin unit to destination unit
        /// </summary>
        public static double UnitConvert(string destinationUnit, double value, string originUnit, UnitCategoryEnum unitCategory)
        {
            double destinationConvTimes = 0;
            double destinationConvPlus = 0;
            double originConvTimes = 0;
            double originConvPlus = 0;
            string destUnitType = null;
            string originUnitType = null;
            foreach (XmlNode node in unitNodeList)
            {
                if (destinationUnit == node.SelectSingleNode("CurrentUnit").InnerText && node.SelectSingleNode("Category").InnerText == unitCategory.ToString())
                {
                    destinationConvTimes = Convert.ToDouble(node.SelectSingleNode("ConversionTimes").InnerText);
                    destinationConvPlus = Convert.ToDouble(node.SelectSingleNode("ConversionPlus").InnerText);
                    destUnitType = node.SelectSingleNode("Category").InnerText;
                }
                if (originUnit == node.SelectSingleNode("CurrentUnit").InnerText && node.SelectSingleNode("Category").InnerText == unitCategory.ToString())
                {
                    originConvTimes = Convert.ToDouble(node.SelectSingleNode("ConversionTimes").InnerText);
                    originConvPlus = Convert.ToDouble(node.SelectSingleNode("ConversionPlus").InnerText);
                    originUnitType = node.SelectSingleNode("Category").InnerText;
                }
            }
            if (destUnitType != originUnitType) return Double.NaN;
            return (value * originConvTimes + originConvPlus) / destinationConvTimes - destinationConvPlus;
        }
        /// <summary>
        /// Convert a value from its origin unit to SI unit
        /// </summary>
        public static double ConvertToSI(double value, string originUnit, UnitCategoryEnum unitCategory)
        {
            return UnitConvert(GetSIUnit(unitCategory), value, originUnit, unitCategory);
        }
        /// <summary>
        /// Convert a value from its SI unit to destination unit
        /// </summary>
        public static double ConvertFromSI(string destinationUnit, double value, UnitCategoryEnum unitCategory)
        {
            return UnitConvert(destinationUnit, value, GetSIUnit(unitCategory), unitCategory);
        }
        /// <summary>
        /// Get SI unit
        /// </summary>
        public static string GetSIUnit(UnitCategoryEnum unitCategory)
        {
            foreach (XmlNode node in unitCategoryList)
            {
                if (unitCategory.ToString() == node.SelectSingleNode("Category").InnerText)
                    return node.SelectSingleNode("SI_Unit").InnerText;
            }
            return null;
        }
        /// <summary>
        /// Get physical dimensionality, contains 8 number, represent in order Length|Mass|Time|ElectricalCurrent|Temperature|AmountOfSubstance|Luminous|Currency
        /// </summary>
        public static IEnumerable<double> GetDimensionality(UnitCategoryEnum unitCategory)
        {
            double[] dimension = new double[8];
            foreach (XmlNode node in unitCategoryList)
            {
                if (unitCategory.ToString() == node.SelectSingleNode("Category").InnerText)
                {
                    dimension[0] = Convert.ToDouble(node.SelectSingleNode("Length").InnerText);
                    dimension[1] = Convert.ToDouble(node.SelectSingleNode("Mass").InnerText);
                    dimension[2] = Convert.ToDouble(node.SelectSingleNode("Time").InnerText);
                    dimension[3] = Convert.ToDouble(node.SelectSingleNode("ElectricalCurrent").InnerText);
                    dimension[4] = Convert.ToDouble(node.SelectSingleNode("Temperature").InnerText);
                    dimension[5] = Convert.ToDouble(node.SelectSingleNode("AmountOfSubstance").InnerText);
                    dimension[6] = Convert.ToDouble(node.SelectSingleNode("Luminous").InnerText);
                    dimension[7] = Convert.ToDouble(node.SelectSingleNode("Currency").InnerText);
                    return dimension;
                }
            }
            return null;
        }
        /// <summary>
        /// Get a unit category through name, not practical for now
        /// </summary>
        public static UnitCategoryEnum GetUnitCategory(string categoryName)
        {
            switch (categoryName.ToLower())
            {
                case "temperature":
                case "温度":
                    return UnitCategoryEnum.Temperature;
                case "pressure":
                case "压力":
                case "压降":
                    return UnitCategoryEnum.Pressure;
                case "heatduty":
                case "power":
                case "热负荷":
                    return UnitCategoryEnum.Power;
            }
            return UnitCategoryEnum.Dimensionless;
        }
    }
}
