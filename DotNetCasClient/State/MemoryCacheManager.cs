/*
 * Licensed to Apereo under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Apereo licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

#if NET40 || NET45
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace DotNetCasClient.State
{
    public class MemoryCacheManager
    {
        private static readonly Lazy<MemoryCacheManager> lazy = new Lazy<MemoryCacheManager>(() => new MemoryCacheManager());

        public static MemoryCacheManager Instance { get { return lazy.Value; } }

        private MemoryCache _cache;

        /// <summary>
        /// Used for locking the cache object against other threads while an item in the cache is refreshed.
        /// </summary>
        private object _cacheLock;

        private MemoryCacheManager()
        {
            // Inititalize lock object.
            _cacheLock = new object();

            if (_cache == null)
            {
                _cache = new MemoryCache("apereo-cas-client");
            }
        }

        /// <summary>
        /// Returns the number of items in the cache.
        /// </summary>
        /// <returns>Returns an integer representing the number of items in the cache.</returns>
        public int Count()
        {
            return _cache.Count();
        }

        /// <summary>
        /// Removes all items from the cache.
        /// </summary>
        public void Clear()
        {
            var cacheItemKeys = _cache.ToList();
            foreach (var cacheItemKey in cacheItemKeys)
            {
                Remove(cacheItemKey.Key);
            }
        }

        /// <summary>
        /// Removes an item from the cache with the specified key.
        /// </summary>
        /// <param name="key">Key of the item to remove from the cache.</param>
        public void Remove(string key)
        {
            if (Contains(key))
                _cache.Remove(key);
        }

        /// <summary>
        /// Checks if an item exists in the cache.
        /// </summary>
        /// <param name="key">Key of the object to check.</param>
        /// <returns>Returns true if the object exists in the cache; otherwise it returns false.</returns>
        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        /// <summary>
        /// Retrieves a copy of the requested item from cache using the provided key.  If the item is available, it returns it as the type specified; otherwise, it returns the default of the type specified.
        /// </summary>
        /// <param name="key">Key of the item to store in the cache.</param>
        public T Get<T>(string key)
        {
            if (!Contains(key))
            {
                return default(T);
            }

            return (T)_cache[key];
        }

        /// <summary>
        /// Retrieves a copy of the requested item from cache using the provided key.  If the item is available, it returns it; otherwise, it returns null.
        /// </summary>
        /// <param name="key">Key of the item to store in the cache.</param>
        public object Get(string key)
        {
            if (!Contains(key))
            {
                return null;
            }

            return _cache[key];
        }

        /// <summary>
        /// Retrieves all items from the cache.
        /// </summary>
        /// <returns>A IEnumerable KeyValuePair collection.</returns>
        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _cache.ToList().AsEnumerable();
        }

        /// <summary>
        /// Inserts a copy of the requested item into cache using the provided key.  The item is cached with a CacheItemPolicy that has an absolute expiration set to the expiration timespan provided.
        /// </summary>
        /// <param name="key">Key of the item to store in the cache.</param>
        /// <param name="value">Value of the item to store in the cache.</param>
        /// <param name="expiration">TimeSpan from DateTime.Now to expire the item from cache.</param>
        public void Set(string key, object value, TimeSpan expiration)
        {
            // If the item is not in the cache, lock _cacheLock to prevent other calls to the cache while we re-insert the item.
            lock (_cacheLock)
            {
                // Check if the item is still not in the cache.
                if (!Contains(key))
                {
                    // Insert the item into cache.
                    _cache.Set(key, value, DateTimeOffset.Now.Add(expiration));
                }
            }
        }

        /// <summary>
        /// Inserts a copy of the requested item into cache using the provided key.  The item is cached with a CacheItemPolicy that has an absolute expiration set to the expiration timespan provided.
        /// </summary>
        /// <param name="key">Key of the item to store in the cache.</param>
        /// <param name="value">Value of the item to store in the cache.</param>
        /// <param name="expiration">Specific DateTime to expire the item from cache.</param>
        public void Set(string key, object value, DateTime expiration)
        {
            // If the item is not in the cache, lock _cacheLock to prevent other calls to the cache while we re-insert the item.
            lock (_cacheLock)
            {
                // Check if the item is still not in the cache.
                if (!Contains(key))
                {
                    // Insert the item into cache.
                    _cache.Set(key, value, new DateTimeOffset(expiration));
                }
            }
        }
    }
}
#endif