using UnityEngine;

namespace OverhaulAPI
{
    internal class PooledPrefabContainer : MonoBehaviour
    {
        public string ID
        {
            get;
            set;
        }

        public bool HasInitialized
        {
            get;
            set;
        }

        private PooledPrefabInstanceBase[] m_Prefabs;
        private bool[] m_PrefabStates;

        /// <summary>
        /// Populate recently created pooled prefab
        /// </summary>
        /// <param name="object"></param>
        /// <param name="count"></param>
        public void Populate<T>(in Transform @object, in int count) where T : PooledPrefabInstanceBase
        {
            m_Prefabs = new PooledPrefabInstanceBase[count];
            m_PrefabStates = new bool[count];
            for (int i = 0; i < count; i++)
            {
                T newObject = Instantiate(@object, base.transform).gameObject.AddComponent<T>();
                newObject.gameObject.SetActive(false);
                newObject.gameObject.name = "PooledPrefab, " + newObject.gameObject.name + " [" + i + "]";
                newObject.PrefabContainer = this;
                newObject.OnInitialize();
                m_Prefabs[i] = newObject;
            }
            HasInitialized = true;
        }

        /// <summary>
        /// Get unused pooled prefab
        /// </summary>
        /// <returns></returns>
        private PooledPrefabInstanceBase getAvailablePooledPrefab()
        {
            PooledPrefabInstanceBase result = null;
            for (int i = 0; i < m_PrefabStates.Length; i++)
            {
                bool val = m_PrefabStates[i];
                if (!val)
                {
                    result = m_Prefabs[i];
                    result.MyIndex = i;
                    SetPrefabActiveState(i, true);

                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Mark gameobject as used or not
        /// </summary>
        /// <param name="value"></param>
        public void SetPrefabActiveState(in int index, in bool value)
        {
            m_PrefabStates[index] = value;
        }

        /// <summary>
        /// Show pooled prefab at specifed position with rotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public T SpawnPooledPrefab<T>(in Vector3 position, in Vector3 rotation) where T : PooledPrefabInstanceBase
        {
            T result = (T)getAvailablePooledPrefab();
            if (result)
                result.UsePrefab(position, rotation);

            return null;
        }
    }
}