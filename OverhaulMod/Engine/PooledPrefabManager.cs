using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class PooledPrefabManager : Singleton<PooledPrefabManager>
    {
        private Dictionary<string, PooledPrefabInfo> m_pooledPrefabs;

        private Transform m_container;

        public override void Awake()
        {
            base.Awake();

            Transform container = new GameObject("Pooled prefabs").transform;
            container.SetParent(base.transform, false);
            m_container = container;
        }

        private void Start()
        {
            m_pooledPrefabs = new Dictionary<string, PooledPrefabInfo>();
        }

        public void MakePooledPrefab(string id, GameObject prefab, float lifeTime, int limit)
        {
            if (m_pooledPrefabs == null || m_pooledPrefabs.ContainsKey(id))
                return;

            Transform container = new GameObject(id).transform;
            container.SetParent(m_container, false);

            PooledPrefabInfo pooledPrefabInfo = new PooledPrefabInfo()
            {
                container = container,
                prefab = prefab,
                lifeTime = lifeTime,
                limit = limit
            };
            m_pooledPrefabs.Add(id, pooledPrefabInfo);
        }

        public void MakePooledPrefab(string id, string bundle, string asset, float lifeTime, int limit)
        {
            if (m_pooledPrefabs == null || m_pooledPrefabs.ContainsKey(id))
                return;

            MakePooledPrefab(id, ModResources.Prefab(bundle, asset), lifeTime, limit);
        }

        public void MakePooledPrefab(string id, string bundle, string asset, string startPath, float lifeTime, int limit)
        {
            if (m_pooledPrefabs == null || m_pooledPrefabs.ContainsKey(id))
                return;

            MakePooledPrefab(id, ModResources.Prefab(bundle, asset, startPath), lifeTime, limit);
        }

        public Transform SpawnObject(string id, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (m_pooledPrefabs.IsNullOrEmpty() || !m_pooledPrefabs.TryGetValue(id, out PooledPrefabInfo pooledPrefabInfo))
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
