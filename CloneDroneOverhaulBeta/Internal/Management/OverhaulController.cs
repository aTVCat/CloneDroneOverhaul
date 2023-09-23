using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of any controller in this mod.
    /// </summary>
    public abstract class OverhaulController : OverhaulAdvancedBehaviour
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static GameObject mainGameObject
        {
            get;
            set;
        }

        public static List<OverhaulController> allControllers
        {
            get;
            set;
        } = new List<OverhaulController>();

        /// <summary>
        /// Check if an exception occurred while initializing the controller.
        /// </summary>
        public bool Error
        {
            get;
            private set;
        }

        public string ErrorString
        {
            get;
            private set;
        }

        /// <summary>
        /// Called at the same time when controller is created.
        /// It is better to set up thing here.
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Called when mod was deactivated.
        /// </summary>
        public virtual void OnModDeactivated() { }

        protected override void OnDisposed()
        {
            OverhaulEvents.RemoveEventListener(OverhaulMod.MOD_DEACTIVATED_EVENT, OnModDeactivated);
            Remove(this);

            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public virtual void OnSceneReloaded() { }

        internal void InternalInitialize()
        {
            OverhaulEvents.AddEventListener(OverhaulMod.MOD_DEACTIVATED_EVENT, OnModDeactivated);
            Initialize();
        }

        #region Static

        /// <summary>
        /// Initialize static fields
        /// </summary>
        /// <param name="controllersGO"></param>
        internal static void InitializeStatic(in GameObject controllersGO)
        {
            mainGameObject = controllersGO;
            allControllers.Clear();
        }

        /// <summary>
        /// Create and initialize new controller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transformOverride"></param>
        /// <returns></returns>
        public static T Add<T>(in Transform transformOverride = null) where T : OverhaulController
        {
            Transform transform = transformOverride ?? mainGameObject.transform;
            T component = transform.gameObject.AddComponent<T>();
            allControllers.Add(component);
            component.InternalInitialize();
            return component;
        }

        /// <summary>
        /// Get controller instance by its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : OverhaulController
        {
            foreach (OverhaulController controllerr in allControllers)
            {
                if (controllerr is T)
                {
                    return OverhaulVersion.IsTestMode && controllerr.Error
                        ? throw new Exception("Using incorrectly started controller is not allowed")
                        : (T)controllerr;
                }
            }
            return null;
        }

        public static T[] GetMany<T>() where T : OverhaulController
        {
            List<T> result = new List<T>();
            foreach (OverhaulController controllerr in allControllers)
            {
                if (controllerr is T)
                {
                    if (OverhaulVersion.IsTestMode && controllerr.Error)
                        throw new Exception("Using incorrectly started controller is not allowed");

                    result.Add((T)controllerr);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Remove controller from existing controllers dictionary
        /// </summary>
        /// <param name="controllerInstance"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void Remove(OverhaulController controllerInstance)
        {
            if (controllerInstance == null)
            {
                throw new ArgumentNullException("Cannot remove controller instance because it is null.");
            }
            _ = allControllers.Remove(controllerInstance);
        }

        #endregion;
    }
}
