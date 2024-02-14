using System;
using System.Collections.Generic;

namespace OverhaulMod.Utils
{
    public static class ModAdvancedCache
    {
        private static readonly Dictionary<object, object> s_items = new Dictionary<object, object>();

        public static void Add(object key, object item)
        {
            if (s_items.ContainsKey(key))
                s_items[key] = item;
            else
                s_items.Add(key, item);
        }

        public static void Remove(object key)
        {
            s_items.Remove(key);
        }

        public static bool Has(object key)
        {
            return s_items.ContainsKey(key);
        }

        public static T Get<T>(object key)
        {
            return s_items.TryGetValue(key, out object obj) ? (T)obj : default;
        }

        public static bool TryGet<T>(object key, out T item)
        {
            item = default;
            bool result = s_items.TryGetValue(key, out object obj);
            if (!result)
                return false;

            if (!(obj is T))
                return false;

            item = (T)obj;
            return true;
        }

        public static T GetOrCreate<T>(object key, Func<T> createFunc)
        {
            if (s_items.TryGetValue(key, out object obj))
                return (T)obj;

            T item = createFunc();
            s_items.Add(key, item);
            return item;
        }

        public static void ClearCache()
        {
            if (s_items.Count == 0)
                return;

            s_items.Clear();
        }
    }
}
