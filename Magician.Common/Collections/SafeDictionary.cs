using System;
using System.Collections.Generic;
using System.Linq;

namespace Magician.Common.Collections
{
    [Serializable]
    public class SafeDictionary<TPKey, TObject> : IObjectManager<TPKey, TObject>
    {
        protected IDictionary<TPKey, TObject> _objectDictionary = new Dictionary<TPKey, TObject>();
        [NonSerialized]
        private List<TObject> _readonlyCopy;

        #region Locker
        [NonSerialized]
        protected object _objLocker;
        protected object ObjLocker => this._objLocker ?? (this._objLocker = new object());

        #endregion

        /// <summary>
        /// 返回的列表不能被修改。【使用缓存】
        /// </summary>  
        public List<TObject> GetAllReadonly()
        {
            lock (this.ObjLocker)
            {
                if (this._readonlyCopy == null)
                {
                    this._readonlyCopy = new List<TObject>(this._objectDictionary.Values);
                }

                return this._readonlyCopy;
            }
        }

        #region IObjectManager<TObject,TPKey> 成员

        public int Count => this._objectDictionary.Count;

        public virtual void Add(TPKey key, TObject obj)
        {
            lock (this.ObjLocker)
            {
                if (this._objectDictionary.ContainsKey(key))
                {
                    this._objectDictionary.Remove(key);
                }

                this._objectDictionary.Add(key, obj);

                this._readonlyCopy = null;
            }
        }

        public virtual void Remove(TPKey id)
        {
            lock (this.ObjLocker)
            {
                if (this._objectDictionary.ContainsKey(id))
                {
                    this._objectDictionary.Remove(id);
                    this._readonlyCopy = null;
                }
            }
        }

        public virtual void RemoveByValue(TObject val)
        {
            lock (this.ObjLocker)
            {
                List<TPKey> keyList = new List<TPKey>(this._objectDictionary.Keys);
                foreach (TPKey key in keyList)
                {
                    if (this._objectDictionary[key].Equals(val))
                    {
                        this._objectDictionary.Remove(key);
                    }
                }
                this._readonlyCopy = null;
            }
        }

        public void RemoveByPredication(Predicate<TObject> predicate)
        {
            if (this._objectDictionary.Count == 0)
            {
                return;
            }

            lock (this.ObjLocker)
            {
                List<TPKey> keyList = new List<TPKey>(this._objectDictionary.Keys);
                foreach (TPKey key in keyList)
                {
                    if (predicate(this._objectDictionary[key]))
                    {
                        this._objectDictionary.Remove(key);
                    }
                }

                this._readonlyCopy = null;
            }
        }

        public virtual void Clear()
        {
            lock (this.ObjLocker)
            {
                this._objectDictionary.Clear();
                this._readonlyCopy = null;
            }
        }

        public TObject Get(TPKey id)
        {
            lock (this.ObjLocker)
            {
                if (this._objectDictionary.ContainsKey(id))
                {
                    return this._objectDictionary[id];
                }
            }

            return default(TObject);
        }

        public bool Contains(TPKey id)
        {
            lock (this.ObjLocker)
            {
                return this._objectDictionary.ContainsKey(id);
            }
        }

        public List<TObject> GetAll()
        {
            lock (this.ObjLocker)
            {
                return this._objectDictionary.Values.ToList();
            }
        }

        public List<TPKey> GetKeyList()
        {
            lock (this.ObjLocker)
            {
                return this._objectDictionary.Keys.ToList();
            }
        }

        public List<TPKey> GetKeyListByObj(TObject obj)
        {
            lock (this.ObjLocker)
            {
                List<TPKey> list = new List<TPKey>();
                foreach (TPKey key in this.GetKeyList())
                {
                    if (this._objectDictionary[key].Equals(obj))
                    {
                        list.Add(key);
                    }
                }

                return list;
            }
        }

        public Dictionary<TPKey, TObject> ToDictionary()
        {
            lock (this.ObjLocker)
            {
                return new Dictionary<TPKey, TObject>(this._objectDictionary);
            }
        }


        #endregion
    }
}