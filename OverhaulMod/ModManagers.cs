using OverhaulMod.UI;
using UnityEngine;

namespace OverhaulMod
{
    public class ModManagers : Singleton<ModManagers>
    {
        public override void Awake()
        {
            base.Awake();
            base.gameObject.AddComponent<UIDeveloperMenu>();
        }

        public void DispatchModLoadedEvent()
        {
            foreach (MonoBehaviour behaviour in base.GetComponents<MonoBehaviour>())
            {
                IModLoadListener modLoadListener = behaviour as IModLoadListener;
                if (modLoadListener != null)
                    modLoadListener.OnModLoaded();
            }
        }

        public static T New<T>() where T : Singleton<T>
        {
            return Instance.gameObject.AddComponent<T>();
        }
    }
}
