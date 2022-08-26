using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneDroneOverhaul.Modules
{
    public class ModuleManagement
    {
        List<ModuleBase> modules = new List<ModuleBase>();

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
                    mBase.OnNewFrame();
            }
        }
        public void OnFixedUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                if (mBase.ShouldWork())
                    mBase.OnFixedUpdate();
            }
        }
        public void OnTime(float time)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                if (mBase.ShouldWork())
                    mBase.OnSecond(time);
            }
        }
        public void OnManagedUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                ModuleBase mBase = modules[i];
                if(mBase.ShouldWork())
                   mBase.OnManagedUpdate();
            }
        }
    }

    public class ModuleBase // Not Implemented Fully Yet
    {
        public virtual void OnActivated() { }
        public virtual void OnModDeactivated() { throw new NotImplementedException(); }
        public virtual bool ShouldWork() { throw new NotImplementedException(); return false; }
        public virtual void OnSettingRefreshed(string ID, object value) { throw new NotImplementedException(); }
        public virtual void RunFunction(string name, object[] arguments) { throw new NotImplementedException(); }
        public virtual void OnNewFrame() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnSecond(float time) { }
        public virtual void OnManagedUpdate() { }
    }
}
