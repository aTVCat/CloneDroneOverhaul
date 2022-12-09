using CloneDroneOverhaul.Modules;
using System.Collections.Generic;

namespace CloneDroneOverhaul.UI
{
    public class GUIManagement : ModuleBase
    {
        // Token: 0x06000103 RID: 259 RVA: 0x000082A4 File Offset: 0x000064A4
        public override void Start()
        {
            GUIManagement.Instance = this;
        }

        // Token: 0x06000104 RID: 260 RVA: 0x000082AC File Offset: 0x000064AC
        public T GetGUI<T>() where T : ModGUIBase
        {
            for (int i = 0; i < this.guis.Count; i++)
            {
                if (this.guis[i] is T)
                {
                    return this.guis[i] as T;
                }
            }
            return default(T);
        }

        // Token: 0x06000105 RID: 261 RVA: 0x00008302 File Offset: 0x00006502
        public void AddGUI(ModGUIBase gui)
        {
            if (!this.guis.Contains(gui))
            {
                gui.OnInstanceStart();
                gui.GUIModule = this;
                this.guis.Add(gui);
            }
        }

        // Token: 0x06000106 RID: 262 RVA: 0x0000832C File Offset: 0x0000652C
        public override void OnManagedUpdate()
        {
            for (int i = 0; i < this.guis.Count; i++)
            {
                if (this.guis[i].gameObject.activeInHierarchy)
                {
                    ModGUIBase modGUIBase = this.guis[i];
                    modGUIBase.OnManagedUpdate();
                }
            }
        }

        // Token: 0x06000107 RID: 263 RVA: 0x0000837C File Offset: 0x0000657C
        public override void OnNewFrame()
        {
            for (int i = 0; i < this.guis.Count; i++)
            {
                if (this.guis[i].gameObject.activeSelf)
                {
                    ModGUIBase modGUIBase = this.guis[i];
                    modGUIBase.OnNewFrame();
                }
            }
        }

        // Token: 0x06000108 RID: 264 RVA: 0x000083CC File Offset: 0x000065CC
        public override void OnFixedUpdate()
        {
            for (int i = 0; i < this.guis.Count; i++)
            {
                if (this.guis[i].gameObject.activeInHierarchy)
                {
                    ModGUIBase modGUIBase = this.guis[i];
                    modGUIBase.OnFixedUpdate();
                }
            }
        }

        // Token: 0x06000109 RID: 265 RVA: 0x0000841A File Offset: 0x0000661A
        protected override bool ExectuteFunctionAnyway()
        {
            return true;
        }

        // Token: 0x0600010A RID: 266 RVA: 0x00008420 File Offset: 0x00006620
        public override void RunFunction(string name, object[] arguments)
        {
            foreach (ModGUIBase modGUIBase in this.guis)
            {
                modGUIBase.RunFunction(name, arguments);
            }
        }

        // Token: 0x0600010B RID: 267 RVA: 0x00008474 File Offset: 0x00006674
        public override void RunFunction<T>(string name, T obj)
        {
            foreach (ModGUIBase modGUIBase in this.guis)
            {
                modGUIBase.RunFunction<T>(name, obj);
            }
        }

        // Token: 0x0600010C RID: 268 RVA: 0x000084C8 File Offset: 0x000066C8
        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            foreach (ModGUIBase modGUIBase in this.guis)
            {
                modGUIBase.OnSettingRefreshed(ID, value, isRefreshedOnStart);
            }
        }

        // Token: 0x040000AE RID: 174
        private List<ModGUIBase> guis = new List<ModGUIBase>();

        // Token: 0x040000AF RID: 175
        public static GUIManagement Instance;
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
