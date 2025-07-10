using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class PooledPrefabInfo
    {
        private List<PooledPrefabBehaviour> m_instantiatedObjects;

        public GameObject prefab
        {
            get;
            set;
        }

        public int limit
        {
            get;
            set;
        }

        public float lifeTime
        {
            get;
            set;
        }

        private PooledPrefabBehaviour getInstantiatedNonActiveObject()
        {
            List<PooledPrefabBehaviour> list = m_instantiatedObjects;
            if (list == null)
            {
                list = new List<PooledPrefabBehaviour>();
                m_instantiatedObjects = list;
            }

            if (list.Count == 0)
                return null;

            int i = 0;
            do
            {
                PooledPrefabBehaviour behaviour = list[i];
                if (behaviour && !behaviour.objectReference.activeSelf)
                {
                    return behaviour;
                }
                i++;
            } while (i < list.Count);
            return null;
        }

        private PooledPrefabBehaviour instantiateNewObject()
        {
            GameObject obj = prefab;
            if (!obj)
                return null;

            int l = limit;
            if (l != -1 && m_instantiatedObjects.Count >= l)
                return null;

            GameObject gameObject = Object.Instantiate(obj);
            PooledPrefabBehaviour behaviour = gameObject.AddComponent<PooledPrefabBehaviour>();
            m_instantiatedObjects.Add(behaviour);
            return behaviour;
        }

        public Transform SpawnObject(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            PooledPrefabBehaviour behaviour = getInstantiatedNonActiveObject();
            if (!behaviour)
            {
                behaviour = instantiateNewObject();
                if (!behaviour)
                    return null;
            }

            Transform transform = behaviour.transform;
            transform.position = position;
            transform.eulerAngles = rotation;
            transform.localScale = scale;
            behaviour.Activate(lifeTime);

            return transform;
        }
    }
}
