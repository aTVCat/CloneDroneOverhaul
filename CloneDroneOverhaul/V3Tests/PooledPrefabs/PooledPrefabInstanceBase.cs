using UnityEngine;
using System.Collections.Generic;
using CloneDroneOverhaul.V3Tests.Base;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class PooledPrefabInstanceBase : MonoBehaviour
    {
        public PooledPrefabContainer PrefabContainer { get; internal set; }
        public int MyIndex { get; set; }
        public float TimeLeft { get; private set; }
        public bool IsActivated { get; private set; }

        /// <summary>
        /// Enable object and set position and rotation up
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        internal void UsePrefab(in Vector3 pos, in Vector3 rot)
        {
            base.gameObject.SetActive(true);
            base.transform.position = pos;
            base.transform.eulerAngles = rot;
            TimeLeft = LifeTime();
            IsActivated = true;
            OnPrefabUsed();
        }

        /// <summary>
        /// Hide object from scene and mark it as unused
        /// </summary>
        protected void ReturnToPool()
        {
            IsActivated = false;
            TimeLeft = 0;
            PrefabContainer.SetPrefabState(MyIndex, false);
            base.gameObject.SetActive(false);
        }

        void FixedUpdate()
        {
            if (IsActivated)
            {
                TimeLeft -= Time.fixedDeltaTime;
                if(TimeLeft <= 0)
                {
                    ReturnToPool();
                }
            }
        }

        /// <summary>
        /// Override this method to expand prefab functionality when it is used
        /// </summary>
        protected virtual void OnPrefabUsed()
        {

        }

        /// <summary>
        /// Get time to wait to return the object to pool
        /// </summary>
        /// <returns></returns>
        protected virtual float LifeTime()
        {
            return 1f;
        }
    }
}
