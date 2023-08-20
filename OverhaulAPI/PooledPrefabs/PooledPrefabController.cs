using System.Collections.Generic;
using UnityEngine;

namespace OverhaulAPI
{
    public static class PooledPrefabController
    {
        private static readonly Dictionary<string, PooledPrefabContainer> s_Entries = new Dictionary<string, PooledPrefabContainer>();

        /// <summary>
        /// Dublicates one object
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="premadeCount"></param>
        public static void CreateNewEntry<T>(in Transform prefab, in int premadeCount, in string id) where T : PooledPrefabInstanceBase
        {
            if (s_Entries.ContainsKey(id))
                return;

            GameObject newPooledPrefabObject = new GameObject("PooledPrefab_" + prefab.gameObject.name);
            PooledPrefabContainer container = newPooledPrefabObject.AddComponent<PooledPrefabContainer>();
            container.ID = id;
            container.Populate<T>(prefab, premadeCount);
            s_Entries.Add(id, container);
            Object.DontDestroyOnLoad(container.gameObject);
        }

        public static bool HasCreatedEntry(in string id)
        {
            return s_Entries.ContainsKey(id);
        }

        public static T SpawnEntry<T>(in string id, in Vector3 position, in Vector3 eulerAngles) where T : PooledPrefabInstanceBase
        {
            if (!HasCreatedEntry(id))
            {
                Debug.LogWarning("[OVERHAUL] [Assets] " + id + " pooled prefab is not created!");
                return null;
            }

            PooledPrefabContainer container = s_Entries[id];
            T result = container.SpawnPooledPrefab<T>(position, eulerAngles);
            return result;
        }
    }
}