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

        public int ID
        {
            get;
            private set;
        }

        public virtual void Awake()
        {
            ID = GetID();
            m_Behaviours.Add(ID, this);
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
                m_Behaviours.Remove(ID);
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
            m_Behaviours.Remove(ID);
            this.IsDestroyed = true;
            this.Dispose();
        }


        #region Static

        private static System.Random m_Random = new System.Random();

        private static Dictionary<int, OverhaulMonoBehaviour> m_Behaviours = new Dictionary<int, OverhaulMonoBehaviour>();

        public static OverhaulMonoBehaviour GetBehaviour(int id)
        {
            m_Behaviours.TryGetValue(id, out OverhaulMonoBehaviour result);
            return result;
        }

        public static OverhaulMonoBehaviour AddBehaviour<T>(GameObject gameObject) where T : OverhaulMonoBehaviour
        {
            if(gameObject == null)
            {
                return null;
            }

            return (T)gameObject.AddComponent(typeof(T));
        }

        private static int GetID()
        {
            return m_Random.Next();
        }

        #endregion
    }
}