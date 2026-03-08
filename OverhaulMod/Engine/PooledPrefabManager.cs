using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class PooledPrefabManager : Singleton<PooledPrefabManager>
    {
        private Dictionary<string, PooledPrefabInfo> _pooledPrefabs;

        private Transform _container;

        public override void Awake()
        {
            base.Awake();

            Transform container = new GameObject("Pooled prefabs").transform;
            container.SetParent(base.transform, false);
            _container = container;
        }

        private void Start()
        {
            _pooledPrefabs = new Dictionary<string, PooledPrefabInfo>();
        }

        public void MakePooledPrefab(string id, GameObject prefab, float lifeTime, int limit)
        {
            if (_pooledPrefabs == null || _pooledPrefabs.ContainsKey(id))
                return;

            Transform container = new GameObject(id).transform;
            container.SetParent(_container, false);

            PooledPrefabInfo pooledPrefabInfo = new PooledPrefabInfo()
            {
                container = container,
                prefab = prefab,
                lifeTime = lifeTime,
                limit = limit
            };
            _pooledPrefabs.Add(id, pooledPrefabInfo);
        }

        public void MakePooledPrefab(string id, string bundle, string asset, float lifeTime, int limit)
        {
            if (_pooledPrefabs == null || _pooledPrefabs.ContainsKey(id))
                return;

            MakePooledPrefab(id, ModResources.Prefab(bundle, asset), lifeTime, limit);
        }

        public void MakePooledPrefab(string id, string bundle, string asset, string startPath, float lifeTime, int limit)
        {
            if (_pooledPrefabs == null || _pooledPrefabs.ContainsKey(id))
                return;

            MakePooledPrefab(id, ModResources.Prefab(bundle, asset, startPath), lifeTime, limit);
        }

        public Transform SpawnObject(string id, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (_pooledPrefabs.IsNullOrEmpty() || !_pooledPrefabs.TryGetValue(id, out PooledPrefabInfo pooledPrefabInfo))
                return null;

            return pooledPrefabInfo.SpawnObject(position, rotation, scale);
        }

        public Transform SpawnObject(string id, Vector3 position, Vector3 rotation)
        {
            return SpawnObject(id, position, rotation, Vector3.one);
        }

        public Transform SpawnObject(string id, Vector3 position)
        {
            return SpawnObject(id, position, Vector3.zero, Vector3.one);
        }
    }
}
