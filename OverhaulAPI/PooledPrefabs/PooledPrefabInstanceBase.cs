using UnityEngine;

namespace OverhaulAPI
{
    public class PooledPrefabInstanceBase : MonoBehaviour
    {
        internal PooledPrefabContainer PrefabContainer { get; set; }

        internal int MyIndex
        {
            get;
            set;
        }

        public float TimeLeft
        {
            get;
            private set;
        }

        public bool IsActivated
        {
            get;
            private set;
        }

        /// <summary>
        /// Enable object and set position and rotation up
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        internal void UsePrefab(in Vector3 pos, in Vector3 rot)
        {
            TimeLeft = GetLifeTime();
            IsActivated = true;

            base.transform.position = pos;
            base.transform.eulerAngles = rot;
            base.gameObject.SetActive(true);

            OnSpawn();
        }

        /// <summary>
        /// Hide object from scene and mark it as unused
        /// </summary>
        protected void ReturnToPool()
        {
            IsActivated = false;
            PrefabContainer.SetPrefabActiveState(MyIndex, false);
            base.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!IsActivated)
                return;

            TimeLeft -= Time.deltaTime;
            if (TimeLeft <= 0)
                ReturnToPool();
        }

        /// <summary>
        /// Get required references if there's need
        /// </summary>
        public virtual void OnInitialize() { }

        /// <summary>
        /// Override this method to expand prefab functionality when it is used
        /// </summary>
        protected virtual void OnSpawn() { }

        /// <summary>
        /// Get time to wait to return the object to pool
        /// </summary>
        /// <returns></returns>
        protected virtual float GetLifeTime() => 1f;
    }
}
