using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MultiSet
{
    public class MultiSet<T> : IMultiSet<T>
    {
        private Dictionary<T, int> _multiSet;

        private IEqualityComparer<T> comparer;

        public MultiSet()
        {
            _multiSet = new Dictionary<T, int>();
        }

        public MultiSet(IEqualityComparer<T> comparer) : this()
        {
            this.comparer = comparer;
        }

        public MultiSet(IEnumerable<T> sequance) : this()
        {
            foreach (var data in sequance)
            {
                _multiSet.Add(data, 1);
            }
        }

        public MultiSet(IEnumerable<T> sequence, IEqualityComparer<T> comparer) : this(sequence)
        {
            this.comparer = comparer;
        }

        public static MultiSet<T> operator +(MultiSet<T> first, MultiSet<T> second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            MultiSet<T> newMultiSet = new MultiSet<T>();

            foreach (var item in first)
                newMultiSet.Add(item, first[item]);

            foreach (var item in second)
                newMultiSet.Add(item, second[item]);

            return newMultiSet;
        }

        public static MultiSet<T> operator -(MultiSet<T> first, MultiSet<T> second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            MultiSet<T> newMultiSet = new MultiSet<T>();

            foreach (var item in first)
                newMultiSet.Add(item, first[item]);

            foreach (var item in second)
                newMultiSet.Remove(item, second[item]);

            return newMultiSet;
        }

        public static MultiSet<T> operator *(MultiSet<T> first, MultiSet<T> second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            MultiSet<T> newMultiSet = new MultiSet<T>();

            foreach (var fItem in first)
                foreach (var sItem in second)
                    if (second.Contains(fItem))
                        newMultiSet.Add(fItem, first[fItem] >= second[sItem] ? second[sItem] : first[fItem]);

            return newMultiSet;
        }

        public static IMultiSet<T> Empty => new MultiSet<T>();

        public int this[T item]
        {
            get
            {
                try { return _multiSet[item]; }
                catch(KeyNotFoundException)
                {
                    Console.WriteLine("MultiSet does not contains that element");
                    return 0;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                try { return _multiSet.ElementAt(index).Key; }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("The entry at specified index does not exists");
                    return default;
                }
            }
        }

        public bool IsEmpty => Count == 0;

        public IEqualityComparer<T> Comparer => comparer;

        public int Count => _multiSet.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public MultiSet<T> Add(T item, int numberOfItems = 1)
        {
            if (numberOfItems < 1)
                return this;
            else
            {
                if (_multiSet.ContainsKey(item))
                    _multiSet[item] += numberOfItems;
                else
                    _multiSet.Add(item, numberOfItems);

                return this;
            }
        }

        public void Add(T item)
        {
            Add(item, 1);
        }

        public IReadOnlyDictionary<T, int> AsDictionary()
        {
            Dictionary<T, int> newMultiSet = new Dictionary<T, int>();

            foreach (var item in _multiSet)
                newMultiSet.Add(item.Key, item.Value);

            return newMultiSet;
        }

        public IReadOnlySet<T> AsSet()
        {
            ISet<T> set = new HashSet<T>();

            foreach (var item in _multiSet)
                set.Add(item.Key);

            return (IReadOnlySet<T>)set;
        }



        public void Clear()
        {
            _multiSet.Clear();
        }

        public bool Contains(T item)
        {
            return _multiSet.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex > Count || arrayIndex < 0)
                return;

            array = new T[Count - arrayIndex];
            for (int i = arrayIndex; i < Count; i++)
            {
                array[arrayIndex] = this[arrayIndex];
            }
        }

        public MultiSet<T> ExceptWith(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            foreach (var item in other)
                Remove(item);

            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MultiSetEnumerator(AsDictionary());
        }

        public MultiSet<T> IntersectWith(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            foreach (var item in _multiSet)
                if (!other.Contains(item.Key))
                    Remove(item.Key);

            return this;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            foreach (var item in _multiSet)
                if (!other.Contains(item.Key))
                    return false;

            foreach (var item in other)
                if (!_multiSet.ContainsKey(item))
                    return true;

            return false;
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            foreach (var item in other)
                if (!_multiSet.ContainsKey(item))
                    return false;

            foreach (var item in _multiSet)
                if (!other.Contains(item.Key))
                    return true;

            return false;
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            foreach (var item in _multiSet)
                if (!other.Contains(item.Key))
                    return false;

            return true;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            foreach (var item in other)
                if (!_multiSet.ContainsKey(item))
                    return false;

            return true;
        }

        public bool MultiSetEquals(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            foreach (var item in _multiSet)
                if (!other.Contains(item.Key))
                    return false;

            foreach (var item in other)
                if (!_multiSet.ContainsKey(item))
                    return false;

            return true;
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            foreach (var item in other)
                if (_multiSet.ContainsKey(item))
                    return true;

            return false;
        }

        public MultiSet<T> Remove(T item, int numberOfItems = 1)
        {
            if (_multiSet.ContainsKey(item) && numberOfItems >= 1)
            {
                if (_multiSet[item] <= numberOfItems)
                    _multiSet.Remove(item);
                else
                    _multiSet[item] -= numberOfItems;
            }
            return this;
        }

        public bool Remove(T item)
        {
            return _multiSet.Remove(item);
        }

        public MultiSet<T> RemoveAll(T item)
        {
            if (_multiSet.ContainsKey(item))
                _multiSet.Remove(item);

            return this;
        }

        public MultiSet<T> SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            IEnumerable<T> union = new List<T>();

            foreach (var item in other)
                if (_multiSet.ContainsKey(item))
                    union.Append(item);

            foreach (var item in other)
                if (!_multiSet.ContainsKey(item))
                    Add(item);

            foreach (var item in union)
                Remove(item);

            return this;
        }

        public MultiSet<T> UnionWith(IEnumerable<T> other)
        {
            if (other is null)
                throw new ArgumentNullException();

            foreach (var item in other)
                if (!_multiSet.ContainsKey(item))
                    Add(item);

            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class MultiSetEnumerator : IEnumerator<T>
        {
            public int index = 0;

            private IReadOnlyDictionary<T, int> _multiSet;

            public MultiSetEnumerator(IReadOnlyDictionary<T, int> multiSet)
            {
                _multiSet = multiSet;
            }

            public T Current => _multiSet.ElementAt(index).Key;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                if (index < _multiSet.Count)
                {
                    index++;
                    return true;
                }

                return false;
                
            }

            public void Reset()
            {
                index = 0;
            }
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            foreach (var item in _multiSet)
            {
                for (int i = 0; i < item.Value; i++)
                    output.Append($"{item.Key} ");
                output.Append('\n');
            }
            return output.ToString();
        }

        public string ToString(string format)
        {
            if (format == "Q")
            {
                StringBuilder output = new StringBuilder();
                foreach (var item in _multiSet)
                    output.Append($"{item.Key} : {item.Value}\n");
                return output.ToString();
            }

            return "";
        }
    }
}
