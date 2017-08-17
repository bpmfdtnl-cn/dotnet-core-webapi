/**
 * Created by w on 2017/7/6.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace W.WebApi.Helper
{
    public class CacheModel
    {
        #region
        public string key { get; set; }
        public object data { get; set; }
        public DateTime updateTime { get; set; }
        #endregion
    }

    /// <summary>
    /// 缓存
    /// </summary>
    public class Cache
    {
        public const string CACHE_KEY_TOKEN = "token";

        private static Cache cacheInstance;
        private IList<CacheModel> caches;
        private Cache()
        {
            caches = new List<CacheModel>();
            caches.Add(new CacheModel() { key = CACHE_KEY_TOKEN, data = new List<Token>(), updateTime = DateTime.Now });
        }
        public static Cache instance()
        {
            if (cacheInstance == null)
                cacheInstance = new Cache();
            return cacheInstance;
        }

        public CacheModel get(string key)
        {
            if (caches.Where(a => a.key == key).Count() == 1)
            {
                return caches.Single(a => a.key == key);
            }
            return null;
        }

        public void update(string key)
        {
            caches.Single(a => a.key == key).updateTime = DateTime.Now;
        }

        public void update(string key, object data)
        {
            caches.Single(a => a.key == key).data = data;
            caches.Single(a => a.key == key).updateTime = DateTime.Now;
        }

    }
}
