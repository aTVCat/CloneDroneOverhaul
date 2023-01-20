using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    public static class ModControllerManager
    {
        /// <summary>
        /// The gameobject controllers will be connected to
        /// </summary>
        internal static GameObject ControllersGameObject;

        private static Dictionary<Type, ModController> _controllers;

        internal static void Initialize(in GameObject controllersGO)
        {
            ControllersGameObject = controllersGO;

            if (_controllers == null)
            {
                _controllers = new Dictionary<Type, ModController>();
            }
            _controllers.Clear();
        }

        /// <summary>
        /// Create and initialize new controller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transformOverride"></param>
        /// <returns></returns>
        public static T NewController<T>(in Transform transformOverride = null) where T : ModController
        {
            Transform transform = transformOverride != null ? transformOverride : ControllersGameObject.transform;

            T component = transform.gameObject.AddComponent<T>();
            component.Initialize();
            _controllers.Add(typeof(T), component);

            return component;
        }

        public static T GetController<T>() where T : ModController
        {
            return (T)_controllers[typeof(T)];
        }
    }
}