﻿using System;
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

        public bool IsDisposedOrDestroyed() => IsDestroyed || IsDestroyed || !OverhaulMod.IsCoreCreated;

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
            Destroy(gameObject);
        }

        public void DestroyBehaviour()
        {
            Destroy(this);
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            OnDisposed();
            IsDisposed = true;
            GC.SuppressFinalize(this);
            if (!IsDestroyed)
            {
                IsDestroyed = true;
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (IsDestroyed)
            {
                return;
            }
            IsDestroyed = true;
            Dispose();
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