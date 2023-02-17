using System.Collections.Generic;
using UnityEngine;

namespace OverhaulAPI
{
    public static class PooledPrefabController
    {
        private static readonly Dictionary<string, PooledPrefabContainer> _containers = new Dictionary<string, PooledPrefabContainer>();

        public static void Reset()
        {
            _containers.Clear();
        }

        /// <summary>
        /// Dublicates one object
        /// </summary>
        /// <param name="object"></param>
        /// <param name="count"></param>
        public static void TurnObjectIntoPooledPrefab<T>(in Transform @object, in int count, in string name) where T : PooledPrefabInstanceBase
        {
            if (_containers.ContainsKey(name))
            {
                return;
            }

            GameObject newGO = new GameObject("PooledPrefab_" + @object.gameObject.name);
            PooledPrefabContainer container = newGO.AddComponent<PooledPrefabContainer>();
            container.ContainerName = name;
            container.Populate<T>(@object, count);
            _containers.Add(name, container);
            Object.DontDestroyOnLoad(container.gameObject);
        }

        public static T SpawnObject<T>(in string containerName, in Vector3 position, in Vector3 rotation) where T : PooledPrefabInstanceBase
        {
            PooledPrefabContainer container = _containers[containerName];
            T result = container.SpawnPooledPrefab<T>(position, rotation);
            return result;
        }
    }
}