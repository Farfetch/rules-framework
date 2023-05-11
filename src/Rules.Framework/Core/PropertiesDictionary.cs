namespace Rules.Framework.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A dictionary to hold the properties of a <see cref="IConditionNode{TConditionType}"/>.
    /// </summary>
    /// <seealso cref="IDictionary{TKey, TValue}"/>
    public class PropertiesDictionary : IDictionary<string, object>
    {
        private const uint DictionarySlimLimit = 5;
        private static readonly Type dictionarySlimType = typeof(DictionarySlim<string, object>);
        private IDictionary<string, object> underlyingDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesDictionary"/> class with a fixed size.
        /// </summary>
        /// <param name="size">The size.</param>
        public PropertiesDictionary(int size)
        {
            if (size > DictionarySlimLimit)
            {
                underlyingDictionary = new Dictionary<string, object>(size, StringComparer.Ordinal);
            }
            else
            {
                this.underlyingDictionary = new DictionarySlim<string, object>(size);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesDictionary"/> class source from
        /// another dictionary.
        /// </summary>
        /// <param name="source">The source dictionary.</param>
        /// <exception cref="ArgumentNullException">when the given source dictionary is null.</exception>
        public PropertiesDictionary(IDictionary<string, object> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Count > DictionarySlimLimit)
            {
                underlyingDictionary = new Dictionary<string, object>(source.Count, StringComparer.Ordinal);
            }
            else
            {
                this.underlyingDictionary = source.Count > 0
                    ? new DictionarySlim<string, object>(source)
                    : new DictionarySlim<string, object>(Constants.DefaultPropertiesDictionarySize);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public int Count => this.underlyingDictionary.Count;

        /// <summary>
        /// Gets a value indicating whether the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        public bool IsReadOnly => this.underlyingDictionary.IsReadOnly;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of
        /// the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        public ICollection<string> Keys => this.underlyingDictionary.Keys;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values
        /// in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        public ICollection<object> Values => this.underlyingDictionary.Values;

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>The <see cref="System.Object"/>.</value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object this[string key]
        {
            get => this.underlyingDictionary[key];
            set
            {
                if (object.Equals(this.underlyingDictionary.GetType(), dictionarySlimType))
                {
                    this.SwitchToDictionaryFullImplementationIfLimitReached();
                }

                this.underlyingDictionary[key] = value;
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, object value)
        {
            if (object.Equals(this.underlyingDictionary.GetType(), dictionarySlimType))
            {
                this.SwitchToDictionaryFullImplementationIfLimitReached();
            }

            this.underlyingDictionary.Add(key, value);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public void Add(KeyValuePair<string, object> item)
        {
            if (object.Equals(this.underlyingDictionary.GetType(), dictionarySlimType))
            {
                this.SwitchToDictionaryFullImplementationIfLimitReached();
            }
            this.underlyingDictionary.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public void Clear()
            => this.underlyingDictionary.Clear();

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="item"/> is found in the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Contains(KeyValuePair<string, object> item)
            => this.underlyingDictionary.Contains(item);

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains
        /// an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="T:System.Collections.Generic.IDictionary`2"/>
        /// contains an element with the key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(string key)
            => this.underlyingDictionary.ContainsKey(key);

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to
        /// an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements
        /// copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see
        /// cref="T:System.Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            => this.underlyingDictionary.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => this.underlyingDictionary.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate
        /// through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
            => this.underlyingDictionary.GetEnumerator();

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// <see langword="true"/> if the element is successfully removed; otherwise, <see
        /// langword="false"/>. This method also returns <see langword="false"/> if <paramref
        /// name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public bool Remove(string key)
            => this.underlyingDictionary.Remove(key);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="item"/> was successfully removed from the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, <see langword="false"/>.
        /// This method also returns <see langword="false"/> if <paramref name="item"/> is not found
        /// in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public bool Remove(KeyValuePair<string, object> item)
            => this.underlyingDictionary.Remove(item);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is
        /// found; otherwise, the default value for the type of the <paramref name="value"/>
        /// parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the object that implements <see
        /// cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the
        /// specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetValue(string key, out object value)
            => this.underlyingDictionary.TryGetValue(key, out value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SwitchToDictionaryFullImplementationIfLimitReached()
        {
            if (this.underlyingDictionary.Count >= DictionarySlimLimit)
            {
                var oldDictionary = this.underlyingDictionary;
                this.underlyingDictionary = new Dictionary<string, object>(oldDictionary, StringComparer.Ordinal);
            }
        }
    }
}