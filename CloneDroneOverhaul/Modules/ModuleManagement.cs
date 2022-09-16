using System;
using System.Collections.Generic;

namespace CloneDroneOverhaul.Modules
{
    public class ModuleManagement
    {
        private List<ModuleBase> modules = new List<ModuleBase>();

        public T AddModule<T>() where T : ModuleBase
        {
            T module = Activator.CreateInstance<T>();
            modules.Add(module);
            module.OnActivated();
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
                if (mBase.ShouldWork())
                {
                    mBase.OnNewFrame();
                }
            }
        }
        public void OnFixedUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                if (mBase.ShouldWork())
                {
                    mBase.OnFixedUpdate();
                }
            }
        }
        public void OnTime(float time)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                if (mBase.ShouldWork())
                {
                    mBase.OnSecond(time);
                }
            }
        }
        public void OnManagedUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                if (mBase.ShouldWork())
                {
                    mBase.OnManagedUpdate();
                }
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

        public static void ShowError(string message)
        {
            CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
            notif.SetUp(message, "", 5, new UnityEngine.Vector2(500, 52), new UnityEngine.Color(0.5f, 0.1559941f, 0.1792453f, 0.6f), new UI.Notifications.Notification.NotificationButton[] { });
        }
    }

    public class ModuleBase // Not Implemented Fully Yet
    {
        protected List<string> Functions = new List<string>();
        public virtual void OnActivated() { }
        public virtual void OnModDeactivated() { throw new NotImplementedException(); }
        public virtual bool ShouldWork() { throw new NotImplementedException(); return false; }
        public virtual void OnSettingRefreshed(string ID, object value) { throw new NotImplementedException(); }
        public virtual void RunFunction(string name, object[] arguments) { }
        public bool ExecutesFunction(string name)
        {
            List<string> list = GetExecutingFunctions();
            if (list == null || list.Count < 1)
            {
                return false;
            }
            return list.Contains(name);
        }
        public List<string> GetExecutingFunctions()
        {
            return Functions;
        }
        public virtual void OnNewFrame() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnSecond(float time) { }
        public virtual void OnManagedUpdate() { }
    }
}
