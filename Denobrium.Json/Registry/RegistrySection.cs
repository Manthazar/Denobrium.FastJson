using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Denobrium.Json.Registry
{
    /// <remarks>
    /// http://www.codeproject.com/Articles/159450/fastJSON
    /// The version over there (2.0.9) could not be taken directly, as its serializer is taking all public properties, disregarding any attribute policy. This is
    /// not good for our case, as we want to return (portions of) data objects as well.
    /// </remarks>
    internal sealed class RegistrySection<TKey, TValue>
    {
        private readonly object _padlock = new object();
        private readonly Dictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="capacity"></param>
        public RegistrySection()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Returns true, if the given key is present in the registry.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Tries to return the value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_padlock)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        /// <summary>
        /// Returns the value for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                lock (_padlock)
                {
                    return _dictionary[key];
                }
            }
            set
            {
                lock (_padlock)
                {
                    _dictionary[key] = value;
                }
            }
        }

        /// <summary>
        /// Adds the value to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            lock (_padlock)
            {
                _dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// Tries to adds the value to the given key and returns true, if it was possible.
        /// </summary>
        /// <remarks>
        /// Fails usually only when the key is already in the dictionary.
        /// </remarks>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool TryAdd(TKey key, TValue value)
        {
            lock (_padlock)
            {
                try
                {
                    _dictionary.Add(key, value);
                    return true;
                }
                catch (ArgumentException)
                {
                    // "An item with the same key has already been added"
                    Debug.WriteLine("An item with the same key has already been added: " + key.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// Removes all values from the dictionary.
        /// </summary>
        public void Clear()
        {
            lock (_padlock)
            {
                _dictionary.Clear();
            }
        }

        /// <summary>
        /// Gets the values... not thread safe.
        /// </summary>
        public IEnumerable<TValue> Values
        {
            get { return _dictionary.Values; }
        }
    }
}
