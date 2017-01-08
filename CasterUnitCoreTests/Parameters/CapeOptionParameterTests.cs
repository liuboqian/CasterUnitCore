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
    public class CapeOptionParameterTests
    {
        private CapeOptionParameter param;
        private CapeOptionParameter param2;

        public enum Options
        {
            option1, option2
        }

        public string[] option2List = { "o1", "o2", "o3" };

        [TestInitialize()]
        public void Initialize()
        {
            param = new CapeOptionParameter(
                "option", typeof(Options), CAPEOPEN.CapeParamMode.CAPE_INPUT, defOption: Options.option1);
            param2 = new CapeOptionParameter(
                "option2", option2List, CAPEOPEN.CapeParamMode.CAPE_INPUT_OUTPUT,
                restrictedToList: false);
        }

        [TestMethod()]
        public void ValidateTest()
        {
            Assert.AreEqual(true, param.Validate());
            param.value = "option1";
            Assert.AreEqual(true, param.Validate());
            param.value = "option4";
            Assert.AreEqual(false, param.Validate());
            param2.value = "option4";
            Assert.AreEqual(true, param2.Validate());
        }

        [TestMethod()]
        public void ValidateTest1()
        {
            string err = "";
            param.Validate(ref err);
            Assert.AreEqual("", err);
            param.value = "option4";
            param.Validate(ref err);
            Assert.AreEqual("OptionList doesn't contain current value, and it must be restricted to list", err);
        }

        [TestMethod()]
        public void ResetTest()
        {
            Assert.AreEqual("option1", param.value);
            param.value = "option2";
            Assert.AreEqual("option2", param.value);
            param.Reset();
            Assert.AreEqual("option1", param.value);
        }

        [TestMethod()]
        public void CloneTest()
        {
            CapeOptionParameter newParam = param.Clone() as CapeOptionParameter;
            Assert.AreEqual("option", newParam.ComponentName);
            newParam.value = Options.option2;
            Assert.AreEqual(Options.option1, (Options)param);
        }

        [TestMethod()]
        public void ValidateTest2()
        {
            param2 = param.Clone() as CapeOptionParameter;
            param.value = Options.option2;
            Assert.AreEqual(Options.option2, (Options)param);
            Assert.AreEqual(Options.option1, (Options)param2);
        }

        [TestMethod()]
        public void CompareToTest()
        {
            Assert.AreEqual(0, param.CompareTo(Options.option1));
            Assert.IsTrue(param.CompareTo(Options.option2) != 0);
        }

        [TestMethod()]
        public void CompareToTest1()
        {
            Assert.AreEqual(0, param.CompareTo("option1"));
            Assert.AreEqual(0, param2.CompareTo("o1"));
            Assert.IsTrue(param.CompareTo("o2") != 0);
        }

        [TestMethod()]
        public void CompareToTest2()
        {
            CapeOptionParameter newParam = new CapeOptionParameter();
            newParam.value = Options.option1;
            Assert.AreEqual(0, param.CompareTo(newParam));
            newParam.value = "o3";
            Assert.IsTrue(param.CompareTo(newParam) != 0);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.IsTrue(param.Equals("option1"));
            Assert.IsTrue(param.Equals(Options.option1));
            CapeOptionParameter option2 = new CapeOptionParameter();
            Assert.IsFalse(param.Equals(option2));
            option2.value = "option1";
            Assert.IsFalse(param.Equals(option2));
            Assert.IsTrue(param.Equals(param));
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            CapeOptionParameter option2 = new CapeOptionParameter();
            Assert.AreNotEqual(option2.GetHashCode(), param.GetHashCode());
        }
    }
}