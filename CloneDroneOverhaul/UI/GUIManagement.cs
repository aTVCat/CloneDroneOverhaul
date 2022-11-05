﻿using CloneDroneOverhaul.Modules;
using System;
using System.Collections.Generic;

namespace CloneDroneOverhaul.UI
{
    public class GUIManagement : ModuleBase
    {
        private List<ModGUIBase> guis = new List<ModGUIBase>();

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
                gui.OnInstanceStart();
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

        public override void OnFixedUpdate()
        {
            for (int i = 0; i < guis.Count; i++)
            {
                if (guis[i].gameObject.activeInHierarchy)
                {
                    ModGUIBase mBase = guis[i];
                    mBase.OnFixedUpdate();
                }
            }
        }

        protected override bool ExectuteFunctionAnyway()
        {
            return true;
        }
        public override void RunFunction(string name, object[] arguments)
        {
            foreach(ModGUIBase ui in guis)
            {
                ui.RunFunction(name, arguments);
            }
        }
        public override void RunFunction<T>(string name, T obj)
        {
            foreach (ModGUIBase ui in guis)
            {
                ui.RunFunction<T>(name, obj);
            }
        }
        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            foreach (ModGUIBase ui in guis)
            {
                ui.OnSettingRefreshed(ID, value, isRefreshedOnStart);
            }
        }
    }

    public class ModGUIBase : UnityEngine.MonoBehaviour
    {
        public ModdedObject MyModdedObject { get; protected set; }
        public GUIManagement GUIModule { get; set; }

        public virtual void OnInstanceStart() { }
        public virtual void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false) { }
        public virtual void RunFunction(string name, object[] arguments) { }
        public virtual void RunFunction<T>(string name, T obj) { }
        public virtual void OnManagedUpdate() { }
        public virtual void OnNewFrame() { }
        public virtual void OnFixedUpdate() { }
    }
}
