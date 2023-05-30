using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of any controller in this mod.
    /// </summary>
    public abstract class OverhaulController : OverhaulBehaviour, IConsoleCommandReceiver
    {
        /// <summary>
        /// Check if an exception occurred while initializing the controller.
        /// </summary>
        public bool HadBadStart { get; private set; }

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
            OverhaulEventsController.RemoveEventListener(OverhaulMod.ModDeactivatedEventString, OnModDeactivated);
            OverhaulConsoleController.RemoveListener(this);
            RemoveController(this);

            foreach (FieldInfo field in this.GetType().GetFields(s_BindingFlags))
            {
                Type fieldType = field.FieldType;
                if (fieldType != typeof(bool) && fieldType != typeof(int) && fieldType != typeof(float))
                {
                    field.SetValue(this, null);
                }
            }
        }

        internal void InitializeInternal()
        {
            _ = OverhaulEventsController.AddEventListener(OverhaulMod.ModDeactivatedEventString, OnModDeactivated);
            try
            {
                Initialize();
            }
            catch (Exception exc)
            {
                Debug.LogError("Caught error while initializing Overhaul ModController [" + GetType() + "]: " + exc);
                HadBadStart = true;
                base.enabled = false;
            }
        }

        #region Static

        private static BindingFlags s_BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
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
            component.InitializeInternal();
            s_ControllersList.Add(component);
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
                if (controllerr.GetType() == typeof(T))
                {
                    return controllerr.HadBadStart ? throw new Exception("Using incorrectly started controller is not allowed") : (T)controllerr;
                }
            }
            return null;
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
