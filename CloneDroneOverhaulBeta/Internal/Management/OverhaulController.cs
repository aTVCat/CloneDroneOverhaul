using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of any controller in this mod.
    /// </summary>
    public abstract class OverhaulController : OverhaulBehaviour
    {
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
            OverhaulEventsController.RemoveEventListener(OverhaulMod.ModDeactivatedEventString, OnModDeactivated);
            RemoveController(this);

            OverhaulDisposable.AssignNullToAllVars(this);
        }

        internal void InitializeInternal()
        {
            _ = OverhaulEventsController.AddEventListener(OverhaulMod.ModDeactivatedEventString, OnModDeactivated);
            Initialize();
        }

        #region Static

        private static readonly BindingFlags s_BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private static GameObject s_ControllersGameObject;
        private static readonly List<OverhaulController> s_ControllersList = new List<OverhaulController>();

        /// <summary>
        /// Initialize static fields
        /// </summary>
        /// <param name="controllersGO"></param>
        internal static void InitializeStatic(in GameObject controllersGO)
        {
            s_ControllersGameObject = controllersGO;
            s_ControllersList.Clear();
        }

        /// <summary>
        /// Create and initialize new controller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transformOverride"></param>
        /// <returns></returns>
        public static T AddController<T>(in Transform transformOverride = null) where T : OverhaulController
        {
            Transform transform = transformOverride ?? s_ControllersGameObject.transform;
            T component = transform.gameObject.AddComponent<T>();
            s_ControllersList.Add(component);
            component.InitializeInternal();
            return component;
        }

        /// <summary>
        /// Get controller instance by its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetController<T>() where T : OverhaulController
        {
            foreach (OverhaulController controllerr in s_ControllersList)
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

        public static T[] GetControllers<T>() where T : OverhaulController
        {
            List<T> result = new List<T>();
            foreach (OverhaulController controllerr in s_ControllersList)
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
        internal static void RemoveController(OverhaulController controllerInstance)
        {
            if (controllerInstance == null)
            {
                throw new ArgumentNullException("Cannot remove controller instance because it is null.");
            }
            _ = s_ControllersList.Remove(controllerInstance);
        }

        #endregion;
    }
}
