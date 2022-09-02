using CloneDroneOverhaul.Modules;
using System;
using System.Collections.Generic;

namespace CloneDroneOverhaul.UI
{
    public class GUIManagement : ModuleBase
    {
        List<ModGUIBase> guis = new List<ModGUIBase>();

        public override bool ShouldWork()
        {
            return true;
        }

        public T GetGUI<T>() where T : ModGUIBase
        {
            for (int i = 0; i < guis.Count; i++)
            {
                if (guis[i] is T)
                {
                    return guis[i] as T;
                }
            }
            return null;
        }

        public void AddGUI(ModGUIBase gui)
        {
            if (!guis.Contains(gui))
            {
                gui.OnAdded();
                gui.GUIModule = this;
                guis.Add(gui);
            }
        }

        public override void OnManagedUpdate()
        {
            for (int i = 0; i < guis.Count; i++)
            {
                if (guis[i].gameObject.activeInHierarchy)
                {
                    ModGUIBase mBase = guis[i];
                    mBase.OnManagedUpdate();
                }
            }
        }

        public override void OnNewFrame()
        {
            for (int i = 0; i < guis.Count; i++)
            {
                if (guis[i].gameObject.activeInHierarchy)
                {
                    ModGUIBase mBase = guis[i];
                    mBase.OnNewFrame();
                }
            }
        }
    }

    public class ModGUIBase : UnityEngine.MonoBehaviour
    {
        public ModdedObject MyModdedObject { get; protected set; }
        public GUIManagement GUIModule { get; set; }

        public virtual void OnAdded() { }
        public virtual void OnSettingRefreshed(string ID, object value) { throw new NotImplementedException(); }
        public virtual void RunFunction(string name, object[] arguments) { throw new NotImplementedException(); }
        public virtual void OnManagedUpdate() { }
        public virtual void OnNewFrame() { }
    }
}
