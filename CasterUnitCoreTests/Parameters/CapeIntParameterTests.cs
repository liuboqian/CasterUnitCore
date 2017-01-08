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
    public class CapeIntParameterTests
    {
        private CapeIntParameter param;

        [TestInitialize]
        public void Initialize()
        {
            param = new CapeIntParameter("int", CapeParamMode.CAPE_INPUT, 0, 1000, 10);
        }

        [TestMethod()]
        public void ValidateTest()
        {
            Assert.AreEqual(true, param.Validate());
            param.value = -10;
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
        public void ResetTest()
        {
            param.value = 220;
            Assert.AreEqual(220, param.value);
            param.Reset();
            Assert.AreEqual(10, param.value);
        }

        [TestMethod()]
        public void CloneTest()
        {
            param.value = 50;
            CapeIntParameter newParam = param.Clone() as CapeIntParameter;
            Assert.AreEqual("int", newParam.ComponentName);
            Assert.AreEqual(50, (double)newParam.value);
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
            Assert.AreEqual(0, param.CompareTo(10));
            Assert.IsTrue(param.CompareTo(-273) > 0);
            Assert.IsTrue(param.CompareTo(400) < 0);
            Assert.IsTrue(param.CompareTo(int.MaxValue) != 0);
        }

        [TestMethod()]
        public void CompareToTest1()
        {
            CapeIntParameter int2 = new CapeIntParameter();
            int2.value = 300;
            Assert.IsTrue(param.CompareTo(int2 as CapeIntParameter) < 0);
            int2.value = 10;
            Assert.IsTrue(param.CompareTo(int2 as CapeIntParameter) == 0);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.IsTrue(param.Equals(10));
            Assert.IsFalse(param.Equals(300));
            CapeIntParameter int2 = new CapeIntParameter();
            int2.value = -300;
            Assert.IsFalse(param.Equals(int2));
            int2.value = 10;
            Assert.IsFalse(param.Equals(int2));
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