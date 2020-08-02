using System;

namespace MonolithicNetCore.Common.Caching
{
    public interface ICacheManager : IDisposable
    {
        /// <summary>
        /// Get or sets the value associated with the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// Adds the specified key and object to the cached
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime">Cache time in minutes</param>
        void Set(string key, object data, int cacheTime);

        /// <summary>
        /// Get aa value indicating whether the value associated with specified key is cached
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsSet(string key);

        /// <summary>
        /// Removes items by key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        void Clear();
    }
}
