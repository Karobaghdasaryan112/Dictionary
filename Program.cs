using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyDictionary<int, string> keyValuePairs = new MyDictionary<int, string>();
            keyValuePairs.Add(1, "a");
            keyValuePairs.Add(2, "b");
            keyValuePairs.Add(3, "c");

            foreach (var item in keyValuePairs)
            {
                Console.WriteLine(item.Key);
                Console.WriteLine(item.Value);
            }

            Console.ReadLine();
        }
    }

    public class MyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private List<KeyValuePair<TKey, TValue>>[] list;

        private int Backets;

        private IEqualityComparer<TKey> _comparer;

        private int _count;

        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public int Count => _count;
        public void SetBacket()
        {
            Backets = (Backets == 0) ? 4 : Backets;
        }

        public int GetBacketIndex(TKey key)
        {
            return Math.Abs(_comparer.GetHashCode(key) % Backets);
        }

        public void Initialize()
        {
            SetBacket();
            if (Backets / (_count+1) < 0.75)
            {
                Backets *= 2;
            }
            list = new List<KeyValuePair<TKey, TValue>>[Backets];
            for (int i = 0; i < Backets; i++)
            {
                list[i] = new List<KeyValuePair<TKey, TValue>>();
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                int index = GetBacketIndex(Key);
                foreach (var item in list[index])
                {
                    if (_comparer.Equals(item.Key, key)) return item.Value;
                }
                return default;
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");
            int index = GetBacketIndex(key);

            var item = new KeyValuePair<TKey, TValue>(key, value);
            foreach (var pair in list[index])
            {
                if (_comparer.Equals(pair.Key, key))
                {
                    throw new ArgumentException();
                }
            }
            list[index].Add(item);
            _count++;
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException();
            int index = GetBacketIndex(key);
            if (index >= list.Length) throw new ArgumentException("key");
            else
            {
                foreach (var item in list[index])
                {
                    if (_comparer.Equals(item.Key, key))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default;
            if (key == null) return false;
            int index = GetBacketIndex(key);
            if (index >= list.Length) return false;
            else
            {
                foreach (var item in list[index])
                {
                    if (_comparer.Equals(item.Key, key))
                    {
                        value = item.Value;
                        return true;
                    }
                }
                return false;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                foreach (var item in list)
                {
                    foreach (var keyValuePair in item)
                    {
                        Keys.Add(keyValuePair.Key);
                    }
                }
                return Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                foreach (var item in list)
                {
                    foreach (var keyValuePair in item)
                    {
                        Values.Add(keyValuePair.Value);
                    }
                }
                return Values;
            }
        }

        public bool Remove(TKey key)
        {
            if (key == null) return false;
            int index = GetBacketIndex(key);
            if (index >= list.Length) return false;
            else
            {
                var pair = list[index];
                foreach (var item in list[index])
                {
                    if (_comparer.Equals(item.Key, key))
                    {
                        pair.Remove(item);
                        return true;
                    }
                }
                return false;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (item.Value == null || item.Key == null) throw new ArgumentNullException("item");
            int index = GetBacketIndex(item.Key);
            if (index >= list.Length) throw new ArgumentException();
            else
            {
                foreach (var pair in list[index])
                {
                    if (_comparer.Equals(pair.Key, item.Key))
                    {
                        throw new ArgumentException();
                    }
                }
                list[index].Add(item);
            }
        }

        public void Clear()
        {
            Backets = 0;
            list = new List<KeyValuePair<TKey, TValue>>[Backets];
        }

        public bool IsReadOnly => false;

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (ContainsKey(item.Key) && TryGetValue(item.Key, out TValue value))
            {
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        //Constructors
        /// </summary>
        public MyDictionary()
        {
            _comparer = EqualityComparer<TKey>.Default;
            Initialize();
        }

        public MyDictionary(int Capacity, IEqualityComparer<TKey> comparer)
        {
            if (Capacity < 0) throw new ArgumentOutOfRangeException();
            Backets = Capacity;
            _comparer = comparer;
            Initialize();
        }

        public MyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> Comparer)
        {
            _comparer = Comparer;
        }

        public MyDictionary(IDictionary<TKey, TValue> dictionary)
        {

            _comparer =  EqualityComparer<TKey>.Default;
        }

        public MyDictionary(IEqualityComparer<TKey> Comparer)
        {
            _comparer = Comparer;
        }

        public MyDictionary(int Capacity)
        {
            _comparer = EqualityComparer<TKey>.Default;
            Backets = Capacity;
            Initialize();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new MyEnumerator<TKey, TValue>(list,Backets);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class MyEnumerator<TKey,TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private List<KeyValuePair<TKey, TValue>>[] _list;

        private int Backets;

        private int _pairListIndex = 0;

        private int _pairIndex = 0;

        public MyEnumerator(List<KeyValuePair<TKey, TValue>>[] keyValuePairs,int Backets)
        {
            _list = keyValuePairs;
            this.Backets = Backets;
        }

        public void Dispose() { }

        public bool MoveNext()
        {
            while (_pairListIndex < _list.Length)
            {
                if (_pairIndex < _list[_pairListIndex].Count)
                {
                    current = _list[_pairListIndex][_pairIndex];
                    _pairIndex++;
                    return true;
                }
                _pairListIndex++;
                _pairIndex = 0;
            }
            return false;
        }

        public KeyValuePair<TKey, TValue> current;

        public KeyValuePair<TKey, TValue> Current => current;

        object IEnumerator.Current => current;

        public void Reset()
        {
            _list = null;
            Backets = 0;
        }

    }
}
