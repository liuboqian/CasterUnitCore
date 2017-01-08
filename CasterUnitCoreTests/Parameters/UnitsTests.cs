using Microsoft.VisualStudio.TestTools.UnitTesting;
using CasterUnitCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasterUnitCore.Tests
{
    [TestClass()]
    public class UnitsTests
    {
        [TestMethod()]
        public void GetUnitListTest()
        {
            var list = Units.GetUnitList(UnitCategoryEnum.Temperature);
            Assert.IsTrue(list.Contains("C"));
            Assert.IsTrue(list.Contains("K"));
            Assert.IsTrue(list.Contains("F"));
            Assert.IsFalse(list.Contains("L"));
        }

        [TestMethod()]
        public void UnitConvertTest()
        {
            Assert.AreEqual(0, Units.UnitConvert("C", 273.15, "K", UnitCategoryEnum.Temperature));
            Assert.AreEqual(0, Units.UnitConvert("kPag", 101.325, "kPa", UnitCategoryEnum.Pressure));
        }

        [TestMethod()]
        public void ConvertToSITest()
        {
            Assert.AreEqual(0, Units.ConvertToSI(0, "C"));
            Assert.AreEqual(1, Units.ConvertToSI(1000, "Pa"));
        }

        [TestMethod()]
        public void ConvertFromSITest()
        {
            Assert.AreEqual(273.15, Units.ConvertFromSI("K", 0));
            Assert.AreEqual(1000000, Units.ConvertFromSI("Pa", 1000));
        }

        [TestMethod()]
        public void GetSIUnitTest()
        {
            Assert.AreEqual("C", Units.GetSIUnit(UnitCategoryEnum.Temperature));
        }

        [TestMethod()]
        public void GetDimensionalityTest()
        {
            var res=Units.GetDimensionality(UnitCategoryEnum.Pressure);
            foreach (var i in res)
                Console.Write(i);
        }

        [TestMethod()]
        public void GetUnitCategoryTest()
        {
            Assert.AreEqual(UnitCategoryEnum.Pressure, Units.GetUnitCategory("pressure"));
        }

        [TestMethod()]
        public void GetUnitCategoryByUnitNameTest()
        {
            Assert.AreEqual(UnitCategoryEnum.Temperature, Units.SearchUnitCategoryByUnitName("F"));
            Assert.AreEqual(UnitCategoryEnum.AmountOfSubstance, Units.SearchUnitCategoryByUnitName("mol"));
        }
    }
}