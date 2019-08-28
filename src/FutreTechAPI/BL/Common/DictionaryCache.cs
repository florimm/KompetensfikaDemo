using System.Collections.Generic;

namespace FutreTechAPI.BL
{
    public interface ICache
    {
        T Get<T>(string key);

        void Add<T>(string key, T obj);
    }

    public class DictionaryCache : ICache
    {
        private Dictionary<string, object> cache;

        public DictionaryCache()
        {
            cache = new Dictionary<string, object>();
        }

        public void Add<T>(string key, T obj)
        {
            if(cache.ContainsKey(key))
            {
                cache[key] = obj;
            }
            else
            {
                cache.Add(key, obj);
            }
        }

        public T Get<T>(string key)
        {
            return cache.ContainsKey(key) ? (T)cache[key] : default;
        }
    }
}
