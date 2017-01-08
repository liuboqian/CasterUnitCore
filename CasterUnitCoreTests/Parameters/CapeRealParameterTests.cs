using Microsoft.VisualStudio.TestTools.UnitTesting;
using CasterUnitCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAPEOPEN;

namespace CasterUnitCore.Tests
{
    [TestClass()]
    public class CapeRealParameterTests
    {
        private static CapeRealParameter param;

        [TestInitialize()]
        public void Initilize()
        {
            param = new CapeRealParameter(
                "real", UnitCategoryEnum.Temperature,
                CapeParamMode.CAPE_INPUT,
                0, 3000, 273.15);
        }

        [TestMethod()]
        public void CloneTest()
        {
            CapeRealParameter newParam = param.Clone() as CapeRealParameter;
            Assert.AreEqual("real", newParam.ComponentName);
            Assert.AreEqual(273.15, (double)newParam.value);
        }

        [TestMethod()]
        public void ResetTest()
        {
            param.value = 220;
            Assert.AreEqual(220, param.value);
            param.Reset();
            Assert.AreEqual(273.15, param.value);
        }

        [TestMethod()]
        public void ValidateTest()
        {
            Assert.AreEqual(true, param.Validate());
            param.value = double.NaN;
            Assert.AreEqual(false, param.Validate());
        }

        [TestMethod()]
        public void ValidateTest1()
        {
            string err = "";
            param.Validate(ref err);
            Assert.AreEqual("", err);
            param.value = -1;
            param.Validate(ref err);
            Assert.AreEqual("value is out of range", err);
        }

        [TestMethod()]
        public void ValidateTest2()
        {
            string err = "";
            param.Validate(0, ref err);
            Assert.AreEqual("", err);
            param.Validate(-1, ref err);
            Assert.AreEqual("value is out of range", err);
        }

        [TestMethod()]
        public void CompareToTest()
        {
            Assert.AreEqual(0, param.CompareTo(273.15));
            Assert.IsTrue(param.CompareTo(-273.15) > 0);
            Assert.IsTrue(param.CompareTo(400) < 0);
            Assert.IsTrue(param.CompareTo(double.NaN) != 0);
        }

        [TestMethod()]
        public void CompareToTest1()
        {
            CapeRealParameter real2 = new CapeRealParameter();
            real2.value = 300;
            Assert.IsTrue(param.CompareTo(real2 as ICapeRealParameterSpec) < 0);
            real2.value = 273.15;
            Assert.IsTrue(param.CompareTo(real2 as ICapeRealParameterSpec) == 0);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.IsTrue(param.Equals(273.15));
            Assert.IsFalse(param.Equals(300));
            CapeRealParameter real2 = new CapeRealParameter();
            real2.value = -300;
            Assert.IsFalse(param.Equals(real2));
            real2.value = 273.15;
            Assert.IsFalse(param.Equals(real2));
            Assert.IsTrue(param.Equals(param));
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            CapeRealParameter real2 = new CapeRealParameter();
            Assert.AreNotEqual(real2.GetHashCode(), param.GetHashCode());
        }
    }
}