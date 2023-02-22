using System;
using System.Collections;
using System.Collections.Generic;

namespace CDOverhaul
{
    public static class OverhaulExtentions
    {
        public static bool IsNullOrEmpty(this ICollection list)
        {
            return list == null || list.Count == 0;
        }

        public static bool TryRemove<T>(this ICollection<T> list, T item)
        {
            if (list.Contains(item))
            {
                _ = list.Remove(item);
                return true;
            }
            return false;
        }

        public static bool TryAdd<T>(this ICollection<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        public static bool IsNullOrEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }
    }
}
