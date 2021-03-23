using System;
using System.Collections.Generic;
using Magician.Common.Threading.Synchronize;

namespace Magician.Common.Collections
{
    /// <summary>
    ///     有序的数组，SortedArray 中的元素是不允许重复的。如果添加数组中已经存在的元素，将会被忽略。
    ///     该实现是线程安全的。
    /// </summary>
    [Serializable]
    public class SortedArray<T> : SortedArray2<T>, IComparer<T> where T : IComparable
    {
        public SortedArray()
        {
            _comparer4Key = this;
        }

        public SortedArray(ICollection<T> collection)
        {
            _comparer4Key = this;
            Rebuild(collection);
        }

        #region IComparer<TKey> 成员

        public int Compare(T x, T y)
        {
            return x == null ? -1 : x.CompareTo(y);
        }

        #endregion
    }

    /// <summary>
    ///     有序的数组，SortedArray 中的元素是不允许重复的。如果添加数组中已经存在的元素，将会被忽略。
    ///     该实现是线程安全的。
    /// </summary>
    [Serializable]
    public class SortedArray2<T>
    {
        private T[] _array = new T[32];
        protected IComparer<T> _comparer4Key;
        private List<T> _lazyCopy;
        private int _minCapacityForShrink = 32;

        #region Index

        public T this[int index]
        {
            get
            {
                using (SmartRwLocker.Lock(AccessMode.Read))
                {
                    if (index < 0 || index >= Count) throw new Exception("Index out of the range !");

                    return _array[index];
                }
            }
        }

        #endregion

        #region Contains

        public bool Contains(T t)
        {
            return IndexOf(t) >= 0;
        }

        #endregion

        #region IndexOf

        public int IndexOf(T t)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) return -1;

                var index = Array.BinarySearch(_array, 0, Count, t, _comparer4Key);

                return index < 0 ? -1 : index;
            }
        }

        #endregion

        #region GetBetween

        public T[] GetBetween(int minIndex, int maxIndex)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                minIndex = minIndex < 0 ? 0 : minIndex;
                maxIndex = maxIndex >= Count ? Count - 1 : maxIndex;

                if (maxIndex < minIndex) return new T[0];

                var count = maxIndex - minIndex - 1;
                var result = new T[count];

                Array.Copy(_array, minIndex, result, 0, count);
                return result;
            }
        }

        #endregion

        #region Shrink

        /// <summary>
        ///     Shrink 将内部数组收缩到最小，释放内存。
        /// </summary>
        public void Shrink()
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                if (_array.Length == Count) return;


                var len = Count >= _minCapacityForShrink ? Count : _minCapacityForShrink;

                var newAry = new T[len];

                Array.Copy(_array, 0, newAry, 0, Count);
                _array = newAry;
            }
        }

        #endregion

        #region AdjustCapacity

        private void AdjustCapacity(int newCount)
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                var totalCount = Count + newCount;
                if (_array.Length >= totalCount) return;

                var newCapacity = _array.Length;
                while (newCapacity < totalCount) newCapacity *= 2;

                var newAry = new T[newCapacity];
                Array.Copy(_array, 0, newAry, 0, Count);
                _array = newAry;
            }
        }

        #endregion

        #region GetMax

        public T GetMax()
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) throw new Exception("SortedArray is Empty !");

                return _array[Count - 1];
            }
        }

        #endregion

        #region GetMin

        public T GetMin()
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) throw new Exception("SortedArray is Empty !");

                return _array[0];
            }
        }

        #endregion

        #region GetAll

        public List<T> GetAll()
        {
            var list = new List<T>();
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                for (var i = 0; i < Count; i++) list.Add(_array[i]);
            }

            return list;
        }

        #endregion

        #region GetAllReadonly

        /// <summary>
        ///     注意，内部使用了Lazy缓存，返回的集合不可被修改。
        /// </summary>
        public List<T> GetAllReadonly()
        {
            if (_lazyCopy != null) return _lazyCopy;

            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                var list = new List<T>();
                for (var i = 0; i < Count; i++) list.Add(_array[i]);

                _lazyCopy = list;
                return _lazyCopy;
            }
        }

        #endregion

        #region Clear

        public void Clear()
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                _array = new T[_minCapacityForShrink];
                Count = 0;
            }

            _lazyCopy = null;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("Count:{0} ,Capacity:{1}", Count, _array.Length);
        }

        #endregion

        #region SmartRWLocker

        [NonSerialized] private SmartRwLocker _smartRwLocker = new SmartRwLocker();

        private SmartRwLocker SmartRwLocker => _smartRwLocker ?? (_smartRwLocker = new SmartRwLocker());

        #endregion

        #region Ctor

        protected SortedArray2()
        {
        }

        public SortedArray2(IComparer<T> comparer)
        {
            _comparer4Key = comparer;
        }

        public SortedArray2(IComparer<T> comparer, ICollection<T> collection)
        {
            _comparer4Key = comparer;
            Rebuild(collection);
        }

        protected void Rebuild(ICollection<T> collection)
        {
            if (collection == null || collection.Count == 0) return;
            _array = new T[collection.Count];
            collection.CopyTo(_array, 0);
            Array.Sort(_array, _comparer4Key);
            Count = collection.Count;
        }

        #endregion

        #region Property

        #region Count

        public int Count { get; private set; }

        #endregion

        #region Capacity

        public int Capacity => _array.Length;

        #endregion

        #region LastReadTime

        public DateTime LastReadTime => SmartRwLocker.LastRequireReadTime;

        #endregion

        #region LastWriteTime

        public DateTime LastWriteTime => SmartRwLocker.LastRequireWriteTime;

        #endregion

        #endregion

        #region Add

        public void Add(T t)
        {
            int posIndex;
            Add(t, out posIndex);
        }

        /// <summary>
        ///     Add 将一个元素添加到数组中。如果数组中已存在目标元素，则忽略。无论哪种情况，posIndex都会被赋予正确的值。
        /// </summary>
        public void Add(T t, out int posIndex)
        {
            if (t == null) throw new Exception("Target can't be null !");

            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                var index = Array.BinarySearch(_array, 0, Count, t, _comparer4Key);
                if (index >= 0)
                {
                    posIndex = index;
                    return;
                }

                AdjustCapacity(1);
                posIndex = ~index;
                Array.Copy(_array, posIndex, _array, posIndex + 1, Count - posIndex);
                _array[posIndex] = t;

                ++Count;
            }

            _lazyCopy = null;
        }

        public void Add(ICollection<T> collection)
        {
            Add(collection, true);
        }

        /// <summary>
        ///     Add 如果能保证collection中的元素不会与现有的元素重复，则checkRepeat可以传入false。
        /// </summary>
        public void Add(ICollection<T> collection, bool checkRepeat)
        {
            if (collection == null || collection.Count == 0) return;

            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                var resultCollection = collection;

                #region checkRepeat

                if (checkRepeat)
                {
                    var dic = new Dictionary<T, T>();
                    foreach (var t in collection)
                    {
                        if (dic.ContainsKey(t) || Contains(t)) continue;

                        dic.Add(t, t);
                    }

                    resultCollection = dic.Keys;
                }

                #endregion

                if (resultCollection.Count == 0) return;

                AdjustCapacity(resultCollection.Count);

                foreach (var t in resultCollection)
                {
                    _array[Count] = t;
                    ++Count;
                }

                Array.Sort(_array, 0, Count, _comparer4Key);
            }

            _lazyCopy = null;
        }

        #endregion

        #region Remove

        #region Remove

        /// <summary>
        ///     Remove 删除数组中所有值为t的元素。
        /// </summary>
        public void Remove(T t)
        {
            if (t == null) return;

            int index;
            do
            {
                index = IndexOf(t);
                if (index >= 0) RemoveAt(index);
            } while (index >= 0);

            _lazyCopy = null;
        }

        #endregion

        #region RemoveAt

        public void RemoveAt(int index)
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                if (index < 0 || index >= Count) return;

                if (index == Count - 1)
                    _array[index] = default(T);
                else
                    Array.Copy(_array, index + 1, _array, index, Count - index - 1);
                --Count;
            }

            _lazyCopy = null;
        }

        #endregion

        #region RemoveBetween

        public void RemoveBetween(int minIndex, int maxIndex)
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                minIndex = minIndex < 0 ? 0 : minIndex;
                maxIndex = maxIndex >= Count ? Count - 1 : maxIndex;

                if (maxIndex < minIndex) return;

                Array.Copy(_array, maxIndex + 1, _array, minIndex, Count - maxIndex - 1);

                Count -= maxIndex - minIndex + 1;
            }

            _lazyCopy = null;
        }

        #endregion

        #endregion
    }

    /// <summary>
    ///     SortedArray 有序的数组，其中Key是不允许重复的。如果单个添加重复的key，则将覆盖旧数据。如果是批添加出现重复，则批添加将全部失败。
    ///     该实现是线程安全的。
    /// </summary>
    [Serializable]
    public class SortedArray<TKey, TVal> : SortedArray2<TKey, TVal>, IComparer<TKey> where TKey : IComparable
    {
        public SortedArray()
        {
            _comparer4Key = this;
        }

        public SortedArray(IDictionary<TKey, TVal> dictionary)
        {
            _comparer4Key = this;
            Rebuild(dictionary);
        }

        #region IComparer<TKey> 成员

        public int Compare(TKey x, TKey y)
        {
            return x == null ? -1 : x.CompareTo(y);
        }

        #endregion
    }

    /// <summary>
    ///     SortedArray 有序的数组，其中Key是不允许重复的。如果单个添加重复的key，则将覆盖旧数据。如果是批添加出现重复，则批添加将全部失败。
    ///     该实现是线程安全的。
    /// </summary>
    [Serializable]
    public class SortedArray2<TKey, TVal>
    {
        protected IComparer<TKey> _comparer4Key;
        private TKey[] _keyArray = new TKey[32];
        private List<TVal> _lazyCopy;
        private int _minCapacityForShrink = 32;
        private TVal[] _valArray = new TVal[32];

        #region Index

        public TVal this[TKey key]
        {
            get
            {
                using (SmartRwLocker.Lock(AccessMode.Read))
                {
                    var index = IndexOfKey(key);
                    if (index < 0) throw new Exception(string.Format("SortedArray doesn't contain the key [{0}]", key));

                    return _valArray[index];
                }
            }
        }

        #endregion

        #region ContainsKey

        public bool ContainsKey(TKey t)
        {
            return IndexOfKey(t) >= 0;
        }

        #endregion

        #region TryGet

        public bool TryGet(TKey key, out TVal val)
        {
            val = default(TVal);
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                var index = IndexOfKey(key);
                if (index < 0) return false;

                val = _valArray[index];
                return true;
            }
        }

        #endregion

        #region IndexOfKey

        public int IndexOfKey(TKey t)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) return -1;

                var index = Array.BinarySearch(_keyArray, 0, Count, t, _comparer4Key);

                return index < 0 ? -1 : index;
            }
        }

        #endregion

        #region AddSafely

        /// <summary>
        ///     安全添加。如果已经存在相同的key，则直接返回。
        /// </summary>
        public void AddSafely(TKey key, TVal val)
        {
            if (key == null) throw new Exception("Target Key can't be null !");

            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                if (ContainsKey(key)) return;

                Add(key, val);
            }
        }

        #endregion

        #region GetBetween

        public void GetBetween(int minIndex, int maxIndex, out TKey[] resultKey, out TVal[] resultVal)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                minIndex = minIndex < 0 ? 0 : minIndex;
                maxIndex = maxIndex >= Count ? Count - 1 : maxIndex;

                if (maxIndex < minIndex)
                {
                    resultKey = new TKey[0];
                    resultVal = new TVal[0];
                    return;
                }

                var count = maxIndex - minIndex + 1;
                resultKey = new TKey[count];
                resultVal = new TVal[count];

                Array.Copy(_keyArray, minIndex, resultKey, 0, count);
                Array.Copy(_valArray, minIndex, resultVal, 0, count);
            }
        }

        #endregion

        #region HitKey

        private bool HitKey(TKey key, out int posIndex)
        {
            posIndex = Array.BinarySearch(_keyArray, 0, Count, key, _comparer4Key);
            if (posIndex >= 0) return true;

            posIndex = ~posIndex;
            return false;
        }

        #endregion

        #region KeyValuePair

        public KeyValuePair<TKey, TVal> GetAt(int index)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (index < 0 || index >= Count) throw new Exception("Index out of the range !");

                return new KeyValuePair<TKey, TVal>(_keyArray[index], _valArray[index]);
            }
        }

        #endregion

        #region GetValNotInKeyScope

        public TVal[] GetValNotInKeyScope(TKey minKey, TKey maxKey)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (_comparer4Key.Compare(minKey, maxKey) > 0) return new TVal[0];

                int minIndex, maxIndex;

                var hitMin = HitKey(minKey, out minIndex);
                var hitMax = HitKey(maxKey, out maxIndex);

                minIndex = hitMin ? minIndex - 1 : minIndex;
                maxIndex = hitMax ? maxIndex + 1 : maxIndex;

                //(-$,minIndex) + [maxIndex ,+$)

                var count = minIndex + Count - maxIndex;

                var result = new TVal[count];

                Array.Copy(_valArray, result, minIndex);
                Array.Copy(_valArray, maxIndex, result, minIndex, Count - maxIndex);
                return result;
            }
        }

        #endregion

        #region GetByKeys

        public List<TVal> GetByKeys(IEnumerable<TKey> collection)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (collection == null) return new List<TVal>();

                var list = new List<TVal>();

                foreach (var key in collection)
                {
                    var index = IndexOfKey(key);
                    if (index >= 0) list.Add(_valArray[index]);
                }

                return list;
            }
        }

        #endregion

        #region GetValNotInKeys

        public List<TVal> GetValNotInKeys(IEnumerable<TKey> collection)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (collection == null) return new List<TVal>();

                var keyList = new List<TKey>(collection);
                keyList.Sort();
                var ary = keyList.ToArray();

                var resultList = new List<TVal>();
                for (var i = 0; i < Count; i++)
                {
                    var index = Array.BinarySearch(ary, _keyArray[i], _comparer4Key);
                    if (index < 0) resultList.Add(_valArray[i]);
                }

                return resultList;
            }
        }

        #endregion

        #region Shrink

        /// <summary>
        ///     Shrink 将内部数组收缩到最小，释放内存。
        /// </summary>
        public void Shrink()
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                if (_keyArray.Length == Count) return;


                var len = Count >= _minCapacityForShrink ? Count : _minCapacityForShrink;

                var newkeyAry = new TKey[len];
                var newValAry = new TVal[len];

                Array.Copy(_keyArray, 0, newkeyAry, 0, Count);
                Array.Copy(_valArray, 0, newValAry, 0, Count);
                _keyArray = newkeyAry;
                _valArray = newValAry;
            }
        }

        #endregion

        #region AdjustCapacity

        private void AdjustCapacity(int newCount)
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                var totalCount = Count + newCount;
                if (_keyArray.Length >= totalCount) return;

                var newCapacity = _keyArray.Length;
                while (newCapacity < totalCount) newCapacity *= 2;

                var newKeyAry = new TKey[newCapacity];
                var newValAry = new TVal[newCapacity];
                Array.Copy(_keyArray, 0, newKeyAry, 0, Count);
                Array.Copy(_valArray, 0, newValAry, 0, Count);
                _keyArray = newKeyAry;
                _valArray = newValAry;
            }
        }

        #endregion

        #region GetAllReadonly

        public List<TVal> GetAllReadonly()
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (_lazyCopy == null) _lazyCopy = GetAllValueList();
                return _lazyCopy;
            }
        }

        #endregion

        #region GetAll

        public List<TVal> GetAll()
        {
            return GetAllValueList();
        }

        #endregion

        #region GetAllValueList

        public List<TVal> GetAllValueList()
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                var list = new List<TVal>();
                for (var i = 0; i < Count; i++) list.Add(_valArray[i]);

                return list;
            }
        }

        #endregion

        #region GetAllKeyList

        public List<TKey> GetAllKeyList()
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                var list = new List<TKey>();
                for (var i = 0; i < Count; i++) list.Add(_keyArray[i]);

                return list;
            }
        }

        #endregion

        #region GetMaxKey

        public TKey GetMaxKey()
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) throw new Exception("SortedArray is Empty !");

                return _keyArray[Count - 1];
            }
        }

        #endregion

        #region Get

        /// <summary>
        ///     如果key不存在，则返回default(TVal)。
        /// </summary>
        public TVal Get(TKey key)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                var index = IndexOfKey(key);
                if (index < 0) return default(TVal);

                return _valArray[index];
            }
        }

        #endregion

        #region GetMinKey

        public TKey GetMinKey()
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) throw new Exception("SortedArray is Empty !");

                return _keyArray[0];
            }
        }

        #endregion

        #region Clear

        public void Clear()
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                _lazyCopy = null;
                _keyArray = new TKey[_minCapacityForShrink];
                _valArray = new TVal[_minCapacityForShrink];
                Count = 0;
            }
        }

        #endregion

        #region ToDictionary

        public Dictionary<TKey, TVal> ToDictionary()
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                var dic = new Dictionary<TKey, TVal>(Count);
                for (var i = 0; i < Count; i++) dic.Add(_keyArray[i], _valArray[i]);

                return dic;
            }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("Count:{0} ,Capacity:{1}", Count, _keyArray.Length);
        }

        #endregion

        #region SmartRWLocker

        [NonSerialized] private SmartRwLocker _smartRwLocker = new SmartRwLocker();

        private SmartRwLocker SmartRwLocker => _smartRwLocker ?? (_smartRwLocker = new SmartRwLocker());

        #endregion

        #region Ctor

        public SortedArray2()
        {
        }

        public SortedArray2(IComparer<TKey> comparer4Key) : this(comparer4Key, null)
        {
        }

        public SortedArray2(IComparer<TKey> comparer4Key, IDictionary<TKey, TVal> dictionary)
        {
            this._comparer4Key = comparer4Key;
            Rebuild(dictionary);
        }

        public void Rebuild(IDictionary<TKey, TVal> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0) return;

            _keyArray = new TKey[dictionary.Count];
            _valArray = new TVal[dictionary.Count];

            dictionary.Keys.CopyTo(_keyArray, 0);
            dictionary.Values.CopyTo(_valArray, 0);
            Array.Sort(_keyArray, _valArray, _comparer4Key);
            Count = dictionary.Count;
        }

        #endregion

        #region Property

        #region Count

        public int Count { get; private set; }

        #endregion

        #region Capacity

        public int Capacity => _keyArray.Length;

        #endregion

        #region LastReadTime

        public DateTime LastReadTime => SmartRwLocker.LastRequireReadTime;

        #endregion

        #region LastWriteTime

        public DateTime LastWriteTime => SmartRwLocker.LastRequireWriteTime;

        #endregion

        #endregion

        #region Add

        public void Add(TKey key, TVal val)
        {
            if (key == null) throw new Exception("Target Key can't be null !");

            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                _lazyCopy = null;
                var index = Array.BinarySearch(_keyArray, 0, Count, key, _comparer4Key);
                if (index >= 0)
                {
                    _valArray[index] = val;
                    return;
                }

                AdjustCapacity(1);

                var posIndex = ~index;

                Array.Copy(_keyArray, posIndex, _keyArray, posIndex + 1, Count - posIndex);
                Array.Copy(_valArray, posIndex, _valArray, posIndex + 1, Count - posIndex);
                _keyArray[posIndex] = key;
                _valArray[posIndex] = val;

                ++Count;
            }
        }

        public void Add(IDictionary<TKey, TVal> dic)
        {
            if (dic == null || dic.Count == 0) return;

            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                _lazyCopy = null;
                if (Count == 0)
                {
                    Rebuild(dic);
                    return;
                }

                foreach (var key in dic.Keys)
                    if (ContainsKey(key))
                        throw new Exception(string.Format("The Key [{0}] has existed in SortedArray !", key));

                AdjustCapacity(dic.Count);

                foreach (var key in dic.Keys)
                {
                    _keyArray[Count] = key;
                    _valArray[Count] = dic[key];
                    ++Count;
                }

                Array.Sort(_keyArray, _valArray, 0, Count, _comparer4Key);
            }
        }

        #endregion

        #region Remove

        public void Remove(TKey t)
        {
            RemoveByKey(t);
        }

        #region RemoveByKey

        public void RemoveByKey(TKey t)
        {
            if (t == null) return;

            var index = IndexOfKey(t);
            if (index >= 0) RemoveAt(index);
        }

        #endregion

        #region RemoveByKeys

        public void RemoveByKeys(ICollection<TKey> keyCollection)
        {
            if (keyCollection == null || keyCollection.Count == 0) return;

            _lazyCopy = null;
            var dic = new Dictionary<TKey, TKey>();
            foreach (var key in keyCollection) dic.Add(key, key);

            IDictionary<TKey, TVal> newDic = new Dictionary<TKey, TVal>();
            for (var i = 0; i < Count; i++)
                if (!dic.ContainsKey(_keyArray[i]))
                    newDic.Add(_keyArray[i], _valArray[i]);

            Clear();
            Add(newDic);
        }

        #endregion

        #region RemoveAt

        public void RemoveAt(int index)
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                if (index < 0 || index >= Count) return;

                _lazyCopy = null;
                Array.Copy(_keyArray, index + 1, _keyArray, index, Count - index - 1);
                Array.Copy(_valArray, index + 1, _valArray, index, Count - index - 1);
                --Count;
            }
        }

        #endregion

        #region RemoveBetween

        public void RemoveBetween(int minIndex, int maxIndex)
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                minIndex = minIndex < 0 ? 0 : minIndex;
                maxIndex = maxIndex >= Count ? Count - 1 : maxIndex;

                if (maxIndex < minIndex) return;

                _lazyCopy = null;
                Array.Copy(_keyArray, maxIndex + 1, _keyArray, minIndex, Count - maxIndex - 1);
                Array.Copy(_valArray, maxIndex + 1, _valArray, minIndex, Count - maxIndex - 1);

                Count -= maxIndex - minIndex + 1;
            }
        }

        #endregion

        #region RemoveBetween

        public void RemoveBetween(TKey minKey, bool minClosed, TKey maxKey, bool maxClosed)
        {
            using (SmartRwLocker.Lock(AccessMode.Write))
            {
                if (_comparer4Key.Compare(minKey, maxKey) > 0) return;

                _lazyCopy = null;
                int minIndex, maxIndex;

                var hitMin = HitKey(minKey, out minIndex);
                var hitMax = HitKey(maxKey, out maxIndex);

                minIndex = minClosed ? minIndex : hitMin ? minIndex + 1 : minIndex;
                maxIndex = maxClosed ? hitMax ? maxIndex : maxIndex - 1 : maxIndex - 1;

                RemoveBetween(minIndex, maxIndex);
            }
        }

        #endregion

        #endregion

        #region GetByKeyScope

        /// <summary>
        ///     GetByKeyScope 获取minKey-maxKey范围内的键和值的有序数组。
        /// </summary>
        public void GetByKeyScope(TKey minKey, bool minClosed, TKey maxKey, bool maxClosed, out TKey[] resultKey,
            out TVal[] resultVal)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (_comparer4Key.Compare(minKey, maxKey) > 0)
                {
                    resultKey = new TKey[0];
                    resultVal = new TVal[0];
                    return;
                }

                int minIndex, maxIndex;

                var hitMin = HitKey(minKey, out minIndex);
                var hitMax = HitKey(maxKey, out maxIndex);

                minIndex = minClosed ? minIndex : hitMin ? minIndex + 1 : minIndex;
                maxIndex = maxClosed ? hitMax ? maxIndex : maxIndex - 1 : maxIndex - 1;


                GetBetween(minIndex, maxIndex, out resultKey, out resultVal);
            }
        }

        /// <summary>
        ///     GetByKeyScope 获取minKey-maxKey范围内的值的有序数组。
        /// </summary>
        public TVal[] GetByKeyScope(TKey minKey, bool minClosed, TKey maxKey, bool maxClosed)
        {
            TKey[] resultKey;
            TVal[] resultVal;

            GetByKeyScope(minKey, minClosed, maxKey, maxClosed, out resultKey, out resultVal);

            return resultVal;
        }

        #endregion

        #region GetGreater

        public void GetGreater(TKey minKey, bool closed, out TKey[] resultKey, out TVal[] resultVal)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                var maxKey = GetMaxKey();
                if (_comparer4Key.Compare(minKey, maxKey) > 0)
                {
                    resultKey = new TKey[0];
                    resultVal = new TVal[0];
                    return;
                }

                int minIndex;

                var hitMin = HitKey(minKey, out minIndex);

                if (hitMin && !closed) minIndex += 1;

                GetBetween(minIndex, Count - 1, out resultKey, out resultVal);
            }
        }

        public TVal[] GetGreater(TKey minKey, bool includeEqual)
        {
            TKey[] resultKey;
            TVal[] resultVal;

            GetGreater(minKey, includeEqual, out resultKey, out resultVal);

            return resultVal;
        }

        #endregion

        #region GetSmaller

        public void GetSmaller(TKey maxKey, bool closed, out TKey[] resultKey, out TVal[] resultVal)
        {
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                var minKey = GetMinKey();
                if (_comparer4Key.Compare(minKey, maxKey) > 0)
                {
                    resultKey = new TKey[0];
                    resultVal = new TVal[0];
                    return;
                }

                int maxIndex;

                var hitMax = HitKey(maxKey, out maxIndex);

                if (hitMax && !closed) maxIndex -= 1;

                GetBetween(0, maxIndex, out resultKey, out resultVal);
            }
        }

        public TVal[] GetSmaller(TKey maxKey, bool includeEqual)
        {
            TKey[] resultKey;
            TVal[] resultVal;

            GetSmaller(maxKey, includeEqual, out resultKey, out resultVal);

            return resultVal;
        }

        #endregion

        #region TryGetMaxKey

        public bool TryGetMaxKey(out TKey key)
        {
            key = default(TKey);
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) return false;

                key = _keyArray[Count - 1];
                return true;
            }
        }

        public bool TryGetMaxKey(out TKey key, out TVal val)
        {
            key = default(TKey);
            val = default(TVal);
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) return false;

                key = _keyArray[Count - 1];
                val = _valArray[Count - 1];
                return true;
            }
        }

        #endregion

        #region TryGetMinKey

        public bool TryGetMinKey(out TKey key)
        {
            key = default(TKey);
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) return false;

                key = _keyArray[0];
                return true;
            }
        }

        public bool TryGetMinKey(out TKey key, out TVal val)
        {
            key = default(TKey);
            val = default(TVal);
            using (SmartRwLocker.Lock(AccessMode.Read))
            {
                if (Count == 0) return false;

                key = _keyArray[0];
                val = _valArray[0];
                return true;
            }
        }

        #endregion
    }
}