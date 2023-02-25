using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// MonoBehaviour class that will be used in mod
    /// </summary>
    public class OverhaulMonoBehaviour : MonoBehaviour, IDisposable
    {
        public bool IsDisposed
        {
            get;
            private set;
        }

        public bool IsDestroyed
        {
            get;
            private set;
        }

        public bool IsDisposedOrDestroyed() => IsDestroyed || IsDestroyed;

        public virtual void Awake()
        {
        }
        public virtual void Start()
        {
        }
        public virtual void OnEnable()
        {
        }
        public virtual void OnDisable()
        {
        }
        protected virtual void OnDisposed()
        {
        }

        public void DestroyGameObject()
        {
            Destroy(this.gameObject);
        }

        public void DestroyBehaviour()
        {
            Destroy(this);
        }

        public void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            OnDisposed();
            IsDisposed = true;
            GC.SuppressFinalize(this);
            if (!this.IsDestroyed)
            {
                this.IsDestroyed = true;
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            if (this.IsDestroyed)
            {
                return;
            }
            this.IsDestroyed = true;
            this.Dispose();
        }


        #region Static

        protected static System.Random m_Random = new System.Random();

        public static OverhaulMonoBehaviour AddBehaviour<T>(GameObject gameObject) where T : OverhaulMonoBehaviour
        {
            if(gameObject == null)
            {
                return null;
            }

            return (T)gameObject.AddComponent(typeof(T));
        }

        #endregion
    }
}