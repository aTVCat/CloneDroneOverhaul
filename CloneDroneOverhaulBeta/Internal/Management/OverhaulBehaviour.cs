using System;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// MonoBehaviour class that replaces usual <see cref="MonoBehaviour"/>
    /// </summary>
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

        public bool IsDisposedOrDestroyed() => IsDestroyed || IsDestroyed || !OverhaulMod.IsModInitialized;

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        protected virtual void OnDisposed() { }

        public void DestroyGameObject() => Destroy(gameObject);
        public void DestroyBehaviour() => Destroy(this);

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