﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CloneDroneOverhaul.Modules
{
    public class ModuleManagement
    {
        private List<ModuleBase> modules = new List<ModuleBase>();

        public T AddModule<T>(bool activateManually = false) where T : ModuleBase
        {
            T module = Activator.CreateInstance<T>();
            modules.Add(module);
            if (!activateManually)
            {
                module.Start();
            }
            return module;
        }

        public T GetModule<T>() where T : ModuleBase
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i] is T)
                {
                    return modules[i] as T;
                }
            }
            return null;
        }

        public void OnFrame()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                mBase.OnNewFrame();
            }
        }
        public void OnFixedUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                mBase.OnFixedUpdate();
            }
        }
        public void OnTime(float time)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                mBase.OnSecond(time);
            }
        }
        public void OnManagedUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                mBase.OnManagedUpdate();
            }
        }
        public void ExecuteFunction(string funcName, object[] args)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                if (mBase.ExecutesFunction(funcName))
                {
                    mBase.RunFunction(funcName, args);
                }
            }
        }
        public void ExecuteFunction<T>(string funcName, object[] args)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                if (mBase.ExecutesFunction(funcName))
                {
                    mBase.RunFunction<T>(funcName, args);
                }
            }
        }
        public void ExecuteFunction<T>(string funcName, T obj)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                if (mBase.ExecutesFunction(funcName))
                {
                    mBase.RunFunction<T>(funcName, obj);
                }
            }
        }

        public void OnSettingRefreshed(string id, object value, bool isOnStart)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                mBase.OnSettingRefreshed(id, value, isOnStart);
            }
        }

        public static void ShowError(string message)
        {
            CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
            notif.SetUp(message, "", 20, new UnityEngine.Vector2(900, 52), new UnityEngine.Color(0.5f, 0.1559941f, 0.1792453f, 0.6f), new UI.Notifications.Notification.NotificationButton[] { });
        }

        public static void ShowError_Type2(string title, string message)
        {
            CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
            notif.SetUp(title, message, 20, new UnityEngine.Vector2(600, 300), new UnityEngine.Color(0.5f, 0, 0, 0.9f), new UI.Notifications.Notification.NotificationButton[] { new UI.Notifications.Notification.NotificationButton { Action = new UnityEngine.Events.UnityAction(notif.HideThis), Text = "OK" } });
        }
    }

    public class ModuleBase
    {
        protected string[] Functions;
        public virtual void Start() { }
        public virtual void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false) { }
        public virtual void RunFunction(string name, object[] arguments) { }
        public virtual void RunFunction<T>(string name, object[] arguments) { }
        public virtual void RunFunction<T>(string name, T obj) { }
        protected virtual bool ExectuteFunctionAnyway() { return false; }
        public bool ExecutesFunction(string name)
        {
            if (ExectuteFunctionAnyway())
            {
                return true;
            }
            string[] list = GetExecutingFunctions();
            if (list == null || list.Length < 1)
            {
                return false;
            }
            return list.Contains(name);
        }
        public string[] GetExecutingFunctions()
        {
            return Functions;
        }
        public virtual void OnNewFrame() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnSecond(float time) { }
        public virtual void OnManagedUpdate() { }
    }
}
