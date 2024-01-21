using System;
using System.Collections.Generic;

namespace CDOverhaul
{
    public static class OverhaulEvents
    {
        /// <summary>
        /// This prefix is automatically added to any of mod events
        /// </summary>
        public const string Prefix = "Overhaul_";

        private static GlobalEventManager s_GlobalEventManager;
        private static GlobalEventManager globalEventManager
        {
            get
            {
                if (!s_GlobalEventManager)
                {
                    s_GlobalEventManager = GlobalEventManager.Instance;
                }
                return s_GlobalEventManager;
            }
        }

        internal static void Initialize()
        {
            AddEventListener(OverhaulMod.ModDeactivatedEventString, Deconstruct);
        }

        internal static void Deconstruct()
        {
            if (!globalEventManager)
                return;

            List<string> list = new List<string>();
            foreach (string eventName in globalEventManager._eventListeners.Keys)
            {
                if (!eventName.Contains(Prefix))
                    continue;

                list.Add(eventName);
            }

            foreach (string key in list)
            {
                _ = globalEventManager._eventListeners.Remove(key);
            }
        }

        internal static string GetString(in string eventName, in bool prefix) => !prefix ? Prefix + eventName : eventName;

        public static void AddEventListener(in string name, in Action callback, in bool dontAddPrefix = false)
        {
            if (!globalEventManager)
                return;

            globalEventManager.AddEventListener(GetString(name, dontAddPrefix), callback);
        }

        public static void AddEventListener<T>(in string name, in Action<T> callback, in bool dontAddPrefix = false)
        {
            if (!globalEventManager)
                return;

            globalEventManager.AddEventListener(GetString(name, dontAddPrefix), callback);
        }

        public static void RemoveEventListener(in string name, in Action callback, in bool dontAddPrefix = false)
        {
            if (!globalEventManager)
                return;

            globalEventManager.RemoveEventListener(GetString(name, dontAddPrefix), callback);
        }

        public static void RemoveEventListener<T>(in string name, in Action<T> callback, in bool dontAddPrefix = false)
        {
            if (!globalEventManager)
                return;

            globalEventManager.RemoveEventListener(GetString(name, dontAddPrefix), callback);
        }

        public static void DispatchEvent(in string name, in bool dontAddPrefix = false)
        {
            if (!globalEventManager)
                return;

            globalEventManager.Dispatch(GetString(name, dontAddPrefix));
        }

        public static void DispatchEvent<T>(in string name, in T argument, in bool dontAddPrefix = false)
        {
            if (!globalEventManager)
                return;

            globalEventManager.Dispatch(GetString(name, dontAddPrefix), argument);
        }
    }
}
