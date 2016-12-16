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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CAPEOPEN;

namespace CasterUnitCore
{
    /// <summary>
    /// This class contains a collection of ICapeIdentification, with a key default set to its name, and can also use index to locate item
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [Guid("7D9B4745-1510-4BCD-B7CF-87DE6796AD23")]
    [ComDefaultInterface(typeof(ICapeCollection))]
    public class CapeCollection :
        CapeOpenBaseObject, ICapeCollection, ICollection<CapeCollectionPair>
    {
        /// <summary>
        /// contains items with a key
        /// </summary>
        protected Dictionary<string, ICapeIdentification> _items = new Dictionary<string, ICapeIdentification>();
        /// <summary>
        /// contains keys in the order when it was added
        /// </summary>
        protected List<string> _keys = new List<string>();
        //按照保存的顺序将值保存在keys中，这样既可以使用数字也可以使用字符串访问

        #region Constructor
        /// <summary>
        /// IsReadOnly is default set to false
        /// </summary>
        public CapeCollection( string name = null, string description = null,bool canRename = false)
            : base( name, description,canRename)
        {
            IsReadOnly = false;
        }

        #endregion

        #region ICapeCollection

        object ICapeCollection.Item(object id)
        {
            try
            {
                if (id is string)
                    return this[(string) id];
                else if (id is int)
                    return this[(int) id];
                else
                    throw new ECapeUnknownException(this,"index can only be int or string");
            }
            catch (Exception e)
            {
                throw new ECapeUnknownException(this,
                    "Index not avaliable. Count:" + Count.ToString() + " Wanted" + id.ToString());
            }
        }

        /// <summary>
        /// return value by index, index starts from one!!! According to CO standard
        /// </summary>
        /// <paramCollection name="index">Start from one!!! int, not long</paramCollection>
        public ICapeIdentification this[int index]
        {
            get { return this._items[_keys[index - 1]]; }
            set { this._items[_keys[index-1]] = value; }
        }

        /// <summary>
        /// return value by name
        /// </summary>
        public ICapeIdentification this[string key]
        {
            get { return this._items[key]; }
            set { this._items[key] = value; }
        }

        /// <summary>
        /// return CapeCollectionPair by index
        /// </summary>
        public CapeCollectionPair GetItem(int index)
        {
            return new CapeCollectionPair(_keys[index], _items[_keys[index]]);
        }

        int ICapeCollection.Count()
        {
            return _keys.Count;
        }

        #endregion

        #region IEnumerable

        public IEnumerator<CapeCollectionPair> GetEnumerator()
        {
            var sortedList = new List<CapeCollectionPair>(Count);
            for (int i = 0; i < Count; i++)
                sortedList.Add(new CapeCollectionPair(_keys[i], this[_keys[i]]));
            return sortedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection<CapeCollectionPair>

        public void Add(CapeCollectionPair item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Add an item, the default key is ComponentName
        /// </summary>
        public void Add(CapeOpenBaseObject item)
        {
            string autokey = item.ComponentName;
            Add(autokey, item);
        }
        /// <summary>
        /// Add an item with a key
        /// </summary>
        public void Add(string key, ICapeIdentification item)
        {
            if (_items.ContainsValue(item))
                throw new ECapeUnknownException(this,"This Collection already has the same item.");
            if (_items.ContainsKey(key))
                throw new ECapeUnknownException(this,"This collection already has an item with the same key.");
            _keys.Add(key);
            _items.Add(key, item);
        }

        public void Clear()
        {
            _keys.Clear();
            _items.Clear();
        }

        public bool Contains(CapeCollectionPair item)
        {
            return Contains(item.Key) && Contains(item.Value);
        }
        /// <summary>
        /// whether CapeCollection contains this item
        /// </summary>
        public bool Contains(ICapeIdentification item)
        {
            return _items.ContainsValue(item);
        }
        /// <summary>
        /// whether CapeCollection contains item with this key
        /// </summary>
        public bool Contains(string key)
        {
            return _keys.Contains(key);
        }

        public void CopyTo(CapeCollectionPair[] array, int arrayIndex)
        {
            var sortedList = new CapeCollectionPair[Count];
            for (int i = 0; i < Count; i++)
                sortedList[i] = new CapeCollectionPair(_keys[i], this[_keys[i]]);
            sortedList.CopyTo(array, arrayIndex);
        }

        public bool Remove(CapeCollectionPair item)
        {
            if (!Contains(item)) return false;
            return Remove(item.Key);
        }
        /// <summary>
        /// Remove item with this key
        /// </summary>
        public bool Remove(string key)
        {
            if (!_keys.Contains(key)) return false;
            _keys.Remove(key);
            _items.Remove(key);
            return true;
        }
        /// <summary>
        /// Remove the same item, should be reference equal
        /// </summary>
        public bool Remove(ICapeIdentification item)
        {
            string key = (from p in _items
                          where p.Value.Equals(item)
                          select p.Key).First();
            if (key == null) return false;
            _keys.Remove(key);
            _items.Remove(key);
            return true;
        }

        public int Count
        { get { return _keys.Count; } }

        public bool IsReadOnly { get; protected set; }

        #endregion

        /// <summary>
        /// Return item array, without key
        /// </summary>
        public ICapeIdentification[] GetValueArray()
        {
            ICapeIdentification[] array = new ICapeIdentification[Count];
            for (int i = 0; i < Count; i++)
            {
                array[i] = _items[_keys[i]];
            }
            return array;
        }

        #region ICloneable

        /// <summary>
        /// if the item inside the CapeCollection is not CapeOpenBaseObject or never implement Clone(), the method will fail
        /// </summary>
        public override object Clone()
        {
            CapeCollection newCollection = new CapeCollection( this.ComponentName,
                this.ComponentDescription,this.CanRename);
            foreach (var key in _keys)
            {
                newCollection._keys.Add(key);
                newCollection._items.Add(key,(ICapeIdentification)((CapeOpenBaseObject)this._items[key]).Clone());
            }
            return newCollection;
        }

        #endregion
    }
}
