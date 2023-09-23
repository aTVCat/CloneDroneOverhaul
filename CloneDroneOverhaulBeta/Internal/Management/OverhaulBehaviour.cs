﻿using System;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulBehaviour : MonoBehaviour, IDisposable
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

        public bool IsDisposedOrDestroyed() => IsDestroyed || IsDisposed || !OverhaulMod.IsModInitialized;

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        protected virtual void OnDisposed() { }

        public void DestroyGameObject()
        {
            if (IsDestroyed)
                return;

            Destroy(gameObject);
        }
        public void DestroyBehaviour()
        {
            if (IsDestroyed)
                return;

            Destroy(this);
        }

        public void Dispose(bool destroy)
        {
            Dispose();
            if (destroy && !IsDestroyed)
            {
                DestroyBehaviour();
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnDisposed();
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        private void OnDestroy()
        {
            if (IsDestroyed)
                return;

            IsDestroyed = true;
            Dispose();
        }


        #region Static

        protected static readonly System.Random m_Random = new System.Random();

        public static OverhaulBehaviour AddBehaviour<T>(GameObject gameObject) where T : OverhaulBehaviour
        {
            return gameObject == null ? null : (OverhaulBehaviour)(T)gameObject.AddComponent(typeof(T));
        }

        #endregion
    }
}