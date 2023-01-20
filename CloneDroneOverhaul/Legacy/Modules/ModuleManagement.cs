using System;
using System.Collections.Generic;
using System.Linq;
using CloneDroneOverhaul.V3.Notifications;

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
            V3.Base.V3_MainModController.CallEvent(funcName, args);
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
            V3.Base.V3_MainModController.CallEvent(funcName, args);
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
            V3.Base.V3_MainModController.CallEvent(funcName, new object[] { obj });
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
            V3.Base.V3_MainModController.SendSettingWasRefreshed(id, value);
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                mBase.OnSettingRefreshed(id, value, isOnStart);
            }
        }

        public static void ShowError(string message)
        {
            SNotification notif = new SNotification("ERROR", message, 20f, UINotifications.NotificationSize_Default, null);
            notif.Send();
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
