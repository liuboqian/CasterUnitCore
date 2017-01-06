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
    public class CapeCollectionTests
    {
        private static CapeCollection collection;

        [ClassInitialize]
        public static void CapeCollectionTest(TestContext context)
        {
            collection = new CapeCollection("collection", "a collection used to test");

            Assert.AreEqual(collection.ComponentName, "collection");
            Assert.AreEqual(collection.ComponentDescription, "a collection used to test");

            collection.Add(new CapeRealParameter("real", UnitCategoryEnum.Area, CapeParamMode.CAPE_INPUT));
            collection.Add(new CapeMaterialPort("material", CapePortDirection.CAPE_INLET));
        }

        [TestMethod()]
        public void GetItemTest()
        {
            Assert.AreEqual(collection.GetItem(0).Value.ComponentName, "real");
            Assert.AreEqual(collection.GetItem(0).Value.GetType(), typeof(CapeRealParameter));
            Assert.AreEqual(collection.GetItem(1).Value.ComponentName, "material");

            Assert.AreEqual(collection[0].ComponentName, "real");
            Assert.AreEqual(collection[0].GetType(), typeof(CapeRealParameter));
            Assert.AreEqual(collection[1].ComponentName, "material");
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            string name = "";
            foreach (var item in collection)
            {
                name += item.Key;
            }
            Assert.AreEqual(name, "realmaterial");
        }

        [TestMethod()]
        public void AddTest()
        {
            collection.Add(new CapeIntParameter("int", CapeParamMode.CAPE_INPUT));
            Assert.AreEqual( "int",collection[2].ComponentName);
        }

        [TestMethod()]
        public void AddTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ClearTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CopyToTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetValueArrayTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CloneTest()
        {
            Assert.Fail();
        }
    }
}