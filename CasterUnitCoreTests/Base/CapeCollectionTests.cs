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
        private static CapeRealParameter realParam;

        [ClassInitialize]
        public static void CapeCollectionTest(TestContext context)
        {
            collection = new CapeCollection("collection", "a collection used to test");

            Assert.AreEqual(collection.ComponentName, "collection");
            Assert.AreEqual(collection.ComponentDescription, "a collection used to test");
        }

        [TestInitialize]
        public void AddDefaultItem()
        {
            collection.Clear();
            realParam = new CapeRealParameter("real", UnitCategoryEnum.Area, CapeParamMode.CAPE_INPUT);
            collection.Add(realParam);
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
            Assert.AreEqual("int", collection[2].ComponentName);
        }

        [TestMethod()]
        public void AddTest1()
        {
            collection.Add("int2", new CapeIntParameter("int", CapeParamMode.CAPE_INPUT));
            Assert.AreEqual("int", collection["int2"].ComponentName);
        }

        [TestMethod()]
        [ExpectedException(typeof(ECapeUnknownException))]
        public void AddTest2()
        {
            collection.Add(new CapeCollectionPair("int3", new CapeIntParameter("int", CapeParamMode.CAPE_INPUT)));
            Assert.AreEqual("int", collection["int3"].ComponentName);
            var item = collection["int2"];
        }

        [TestMethod()]
        [ExpectedException(typeof(ECapeUnknownException))]
        public void ClearTest()
        {
            collection.Clear();
            Assert.AreEqual(0, collection.Count);
            var item = collection[0];
        }

        [TestMethod()]
        public void ContainsTest()
        {
            Assert.AreEqual(true, collection.Contains("real"));
            Assert.AreEqual(false, collection.Contains("real2"));
        }

        [TestMethod()]
        public void ContainsTest1()
        {
            Assert.AreEqual(true, collection.Contains(realParam));
            Assert.AreEqual(false, collection.Contains((CapeRealParameter)null));
        }

        [TestMethod()]
        public void ContainsTest2()
        {
            ;
        }

        [TestMethod()]
        public void CopyToTest()
        {
            CapeCollectionPair[] array = new CapeCollectionPair[5];
            collection.CopyTo(array, 1);
            Assert.AreEqual(null, array[0].Key);
            Assert.AreEqual("real", array[1].Value.ComponentName);
            Assert.AreEqual("material", array[2].Value.ComponentName);
            Assert.AreEqual(null, array[3].Key);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            CapeCollectionPair p = new CapeCollectionPair("real", realParam);
            collection.Remove(p);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual("material", collection[0].ComponentName);
        }

        [TestMethod()]
        public void RemoveTest1()
        {
            collection.Remove(realParam);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual("material", collection[0].ComponentName);
        }

        [TestMethod()]
        public void RemoveTest2()
        {
            collection.Remove("real");
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual("material", collection[0].ComponentName);
        }

        [TestMethod()]
        public void GetValueArrayTest()
        {
            ICapeIdentification[] obj=collection.Values;
            Assert.AreEqual("real", obj[0].ComponentName);
            Assert.AreEqual("material", obj[1].ComponentName);
            Assert.AreEqual(2, obj.Length);
        }

        [TestMethod()]
        public void CloneTest()
        {
            CapeCollection newCollection = collection.Clone() as CapeCollection;

            collection[0].ComponentDescription = "second";
            Assert.AreEqual("", newCollection[0].ComponentDescription);
            collection.Remove("real");
            Assert.AreEqual("real", newCollection[0].ComponentName);

        }
    }
}