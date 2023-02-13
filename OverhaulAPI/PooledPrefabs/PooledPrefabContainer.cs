using UnityEngine;

namespace OverhaulAPI
{
    public class PooledPrefabContainer : MonoBehaviour
    {
        public string ContainerName { get; set; }
        public bool HasPopulatedTransform { get; set; }

        private PooledPrefabInstanceBase[] _prefabs;
        private bool[] _activatedPrefabs;

        /// <summary>
        /// Populate recently created pooled prefab
        /// </summary>
        /// <param name="object"></param>
        /// <param name="count"></param>
        public void Populate<T>(in Transform @object, in int count) where T : PooledPrefabInstanceBase
        {
            _prefabs = new PooledPrefabInstanceBase[count];
            _activatedPrefabs = new bool[count];
            for (int i = 0; i < count; i++)
            {
                T newObject = Instantiate<Transform>(@object, base.transform).gameObject.AddComponent<T>();
                newObject.gameObject.SetActive(false);
                newObject.gameObject.name = "PooledPrefab(" + i + ")_" + newObject.gameObject.name;
                newObject.transform.position = Vector3.zero;
                newObject.transform.eulerAngles = Vector3.zero;
                newObject.PrefabContainer = this;
                newObject.PreparePrefab();
                _prefabs[i] = newObject;
            }
            HasPopulatedTransform = true;
        }

        /// <summary>
        /// Get unused pooled prefab
        /// </summary>
        /// <returns></returns>
        private PooledPrefabInstanceBase getAvailablePooledPrefab(in bool markAsUsed = false)
        {
            PooledPrefabInstanceBase result = null;
            for (int i = 0; i < _activatedPrefabs.Length; i++)
            {
                bool val = _activatedPrefabs[i];
                if (!val)
                {
                    result = _prefabs[i];
                    result.MyIndex = i;
                    if (markAsUsed)
                    {
                        SetPrefabState(i, true);
                    }

                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Mark gameobject as used or not
        /// </summary>
        /// <param name="value"></param>
        public void SetPrefabState(in int index, in bool value)
        {
            _activatedPrefabs[index] = value;
        }

        /// <summary>
        /// Show pooled prefab at specifed position with rotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public T SpawnPooledPrefab<T>(in Vector3 position, in Vector3 rotation) where T : PooledPrefabInstanceBase
        {
            T result = null;
            result = (T)getAvailablePooledPrefab(true);

            if (result != null)
            {
                result.UsePrefab(position, rotation);
            }

            return result;
        }
    }
}