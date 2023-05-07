namespace Rules.Framework.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal sealed class DictionarySlim<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IEqualityComparer<TKey> keysComparer;
        private readonly IEqualityComparer<TValue> valuesComparer;
        private KeyValuePair<TKey, TValue>[] entries;
        private int lastUsedIndex;

        public DictionarySlim(int size)
        {
            if (typeof(TKey) == typeof(string))
            {
                this.keysComparer = (IEqualityComparer<TKey>)StringComparer.Ordinal;
            }
            else
            {
                this.keysComparer = EqualityComparer<TKey>.Default;
            }
            this.valuesComparer = EqualityComparer<TValue>.Default;
            this.lastUsedIndex = -1;
            this.entries = new KeyValuePair<TKey, TValue>[size];
        }

        public DictionarySlim(IDictionary<TKey, TValue> source)
            : this(source.Count)
        {
            this.AddRange(source);
        }

        public int Count => this.lastUsedIndex + 1;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => GetProjectedArrayPortion(this.entries, 0, this.Count, e => e.Key);

        public ICollection<TValue> Values => GetProjectedArrayPortion(this.entries, 0, this.Count, e => e.Value)!;

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                var existentItemIndex = this.FindIndex(key);
                if (existentItemIndex < 0)
                {
                    throw new KeyNotFoundException($"Key with value '{key}' was not found.");
                }

                return this.entries[existentItemIndex].Value;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                var index = this.FindIndex(key);
                if (index >= 0)
                {
                    this.entries[index] = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }

                if (this.lastUsedIndex >= this.Count - 1)
                {
                    this.GrowEntriesArray();
                }

                this.entries[++this.lastUsedIndex] = new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var existentItemIndex = this.FindIndex(key);

            if (existentItemIndex >= 0)
            {
                throw new ArgumentException("An element with the same key already exists in the System.Collections.Generic.IDictionary`2.", nameof(key));
            }

            if (this.lastUsedIndex >= this.Count - 1)
            {
                this.GrowEntriesArray();
            }

            this.entries[++this.lastUsedIndex] = new KeyValuePair<TKey, TValue>(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentException("The item key is null.", nameof(item));
            }

            var existentItemIndex = this.FindIndex(item.Key);

            if (existentItemIndex >= 0)
            {
                throw new ArgumentException("An element with the same key already exists in the System.Collections.Generic.IDictionary`2.", nameof(item));
            }

            if (this.lastUsedIndex >= this.Count - 1)
            {
                this.GrowEntriesArray();
            }

            this.entries[++this.lastUsedIndex] = item;
        }

        public void Clear()
        {
            for (int i = 0; i < this.lastUsedIndex; i++)
            {
                this.entries[i] = default;
            }

            this.lastUsedIndex = -1;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            int i = this.Count;
            while (--i >= 0)
            {
                var entry = this.entries[i];
                if (this.AreKeysEqual(item.Key, entry.Key) && this.AreValuesEqual(item.Value, entry.Value))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key) => this.FindIndex(key) >= 0;

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var count = this.Count;
            for (int i = 0; i < count; i++)
            {
                array[arrayIndex++] = this.entries[i];
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this.entries, this.Count);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this.entries, this.Count);

        public bool Remove(TKey key)
        {
            int initialCount = this.Count;
            int keyAtIndex = this.FindIndex(key);

            if (keyAtIndex < 0)
            {
                return false;
            }

            for (int i = keyAtIndex; i < initialCount - 1; i++)
            {
                this.entries[i] = this.entries[i + 1];
            }

            this.entries[this.lastUsedIndex--] = default;
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            int initialCount = this.Count;
            int keyAtIndex = -1;
            for (int i = 0; i < initialCount; i++)
            {
                var entry = this.entries[i];
                if (this.AreKeysEqual(item.Key, entry.Key) && this.AreValuesEqual(item.Value, entry.Value))
                {
                    keyAtIndex = i;
                    break;
                }
            }

            if (keyAtIndex < 0)
            {
                return false;
            }

            for (int i = keyAtIndex; i < initialCount - 1; i++)
            {
                this.entries[i] = this.entries[i + 1];
            }

            this.entries[this.lastUsedIndex--] = default;
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var existentItemIndex = this.FindIndex(key);
            if (existentItemIndex >= 0)
            {
                value = this.entries[existentItemIndex].Value;
                return true;
            }

            value = default!;
            return false;
        }

        private static P[] GetProjectedArrayPortion<T, P>(T[] array, int startIndex, int length, Func<T, P> projectionFunc)
        {
            P[] newArray = new P[length];
            for (int i = 0; i < length; i++)
            {
                newArray[i] = projectionFunc.Invoke(array[startIndex + i]);
            }

            return newArray;
        }

        private void AddRange(IDictionary<TKey, TValue> source)
        {
            if (source is DictionarySlim<TKey, TValue> dictionarySlim)
            {
                for (int i = 0; (i < dictionarySlim.Count); i++)
                {
                    this.entries[i] = dictionarySlim.entries[i];
                }

                return;
            }

            foreach (var item in source)
            {
                this.entries[++this.lastUsedIndex] = item;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AreKeysEqual(TKey left, TKey right)
        {
            if (typeof(TKey) == typeof(string))
            {
                return string.Equals(left as string, right as string, StringComparison.Ordinal);
            }

            return this.keysComparer.Equals(left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AreValuesEqual(TValue left, TValue right)
            => this.valuesComparer.Equals(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindIndex(TKey key)
        {
            var i = this.Count;
            while (--i >= 0)
            {
                if (this.AreKeysEqual(this.entries[i].Key, key))
                {
                    return i;
                }
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GrowEntriesArray()
        {
            var newSize = this.entries.Length * 2;
            var newEntries = new KeyValuePair<TKey, TValue>[newSize];
            var i = this.Count;
            while (--i >= 0)
            {
                newEntries[i] = this.entries[i];
            }

            this.entries = newEntries;
        }

        private sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly int count;
            private readonly KeyValuePair<TKey, TValue>[] entries;
            private int currentIndex;

            public Enumerator(KeyValuePair<TKey, TValue>[] entries, int count)
            {
                this.entries = entries;
                this.count = count;
                this.currentIndex = -1;
            }

            public KeyValuePair<TKey, TValue> Current => this.GetKeyValuePair();

            object IEnumerator.Current => this.GetKeyValuePair();

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this.currentIndex < this.count - 1)
                {
                    this.currentIndex++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                this.currentIndex = 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private KeyValuePair<TKey, TValue> GetKeyValuePair()
            {
                return this.entries[this.currentIndex];
            }
        }
    }
}