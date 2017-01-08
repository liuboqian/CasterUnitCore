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
    public class CapeBooleanParameterTests
    {
        private CapeBooleanParameter param ;

        [TestInitialize()]
        public void Initialize()
        {
            param = new CapeBooleanParameter("bool", false, CAPEOPEN.CapeParamMode.CAPE_INPUT);
        }

        [TestMethod()]
        public void ValidateTest()
        {
            Assert.IsTrue(param.Validate());
        }

        [TestMethod()]
        public void ValidateTest1()
        {
            string err = "";
            param.Validate(ref err);
            Assert.AreEqual("", err);
        }

        [TestMethod()]
        public void ResetTest()
        {
            param.value = true;
            param.Reset();
            Assert.IsFalse((bool)param.value);
        }

        [TestMethod()]
        public void CloneTest()
        {
            CapeBooleanParameter newParam = param.Clone() as CapeBooleanParameter;
            Assert.AreEqual("bool", newParam.ComponentName);
            Assert.AreEqual(false, (bool)newParam.value);
            param.value = true;
            Assert.AreEqual(false, (bool)newParam.value);
        }

        [TestMethod()]
        public void ValidateTest2()
        {
            string err = "";
            param.Validate(false, ref err);
            Assert.AreEqual("", err);
        }

        [TestMethod()]
        public void CompareToTest()
        {
            Assert.AreEqual(0, param.CompareTo(false));
            Assert.IsTrue(param.CompareTo(true) != 0);
        }

        [TestMethod()]
        public void CompareToTest1()
        {
            CapeBooleanParameter bool2 = new CapeBooleanParameter();
            bool2.value = true;
            Assert.IsTrue(param.CompareTo(bool2) != 0);
        }
    }
}