using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of any controller in this mod.
    /// </summary>
    public abstract class OverhaulController : OverhaulMonoBehaviour, IConsoleCommandReceiver
    {
        private bool m_HadBadStart;
        /// <summary>
        /// Check if an exception occured while initializng the controller.
        /// </summary>
        public bool HadBadStart => m_HadBadStart;

        /// <summary>
        /// Called at the same time when controller is created.
        /// It is better to set up thing here.
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Called when mod was deactivated.
        /// </summary>
        public virtual void OnModDeactivated() { }

        /// <summary>
        /// The array of available commands you may type in debug console.
        /// </summary>
        /// <returns></returns>
        public abstract string[] Commands();
        /// <summary>
        /// Called when any command ran.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract string OnCommandRan(string[] command);

        protected override void OnDisposed()
        {
            OverhaulConsoleController.RemoveListener(this);
            RemoveController(this);
        }

        internal void InitializeInternal()
        {
            _ = OverhaulEventManager.AddEventListener(OverhaulMod.ModDeactivatedEventString, OnModDeactivated);
            try
            {
                Initialize();
            }
            catch(Exception exc)
            {
                Debug.LogError("Caught error while initializing Overhaul ModController [" + GetType() + "]: " + exc);
                m_HadBadStart = true;
                base.enabled = false;
            }
        }

        #region Static

        private static GameObject m_ControllersGameObject;
        private static readonly Dictionary<Type, OverhaulController> m_ControllersDictionary = new Dictionary<Type, OverhaulController>();

        /// <summary>
        /// Initialize static fields
        /// </summary>
        /// <param name="controllersGO"></param>
        internal static void InitializeStatic(in GameObject controllersGO)
        {
            m_ControllersGameObject = controllersGO;
            m_ControllersDictionary.Clear();
        }

        /// <summary>
        /// Create and initialize new controller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transformOverride"></param>
        /// <returns></returns>
        public static T AddController<T>(in Transform transformOverride = null) where T : OverhaulController
        {
            Transform transform = transformOverride != null ? transformOverride : m_ControllersGameObject.transform;
            T component = transform.gameObject.AddComponent<T>();
            component.InitializeInternal();
            m_ControllersDictionary.Add(typeof(T), component);
            return component;
        }

        /// <summary>
        /// Get controller instance by its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetController<T>() where T : OverhaulController
        {
            if (!m_ControllersDictionary.ContainsKey(typeof(T)))
            {
                throw new NullReferenceException("Controller with type " + typeof(T) + " does not exist");
            }
            OverhaulController controller = m_ControllersDictionary[typeof(T)];
            if (controller.HadBadStart)
            {
                throw new Exception("Using incorrectly started controller is not allowed");
            }
            return (T)controller;
        }

        /// <summary>
        /// Remove controller from existing controllers dictionary
        /// </summary>
        /// <param name="controllerInstance"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void RemoveController(OverhaulController controllerInstance)
        {
            if(controllerInstance == null)
            {
                throw new ArgumentNullException("Cannot remove controller instance because it is null.");
            }
            _ = m_ControllersDictionary.Remove(controllerInstance.GetType());
        }

        #endregion;
    }
}
