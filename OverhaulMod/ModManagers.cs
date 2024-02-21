using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;

namespace OverhaulMod
{
    public class ModManagers : Singleton<ModManagers>
    {
        public override void Awake()
        {
            base.Awake();

            ModCore.GameInitialized += onGameInitialized;

            _ = base.gameObject.AddComponent<UIDeveloperMenu>();
        }

        private void FixedUpdate()
        {
            ModTime.hasFixedUpdated = true;
            ModTime.fixedFrameCount++;
        }

        private void LateUpdate()
        {
            ModTime.hasFixedUpdated = false;
        }

        private void OnDestroy()
        {
            ModCore.GameInitialized -= onGameInitialized;
        }

        private void onGameInitialized()
        {
            TriggerGameLoadedEvent();
            _ = ModActionUtils.RunCoroutine(showUIDelay());
        }

        private IEnumerator showUIDelay()
        {
            yield return null;
            yield return null;
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.TitleScreenRework))
                ModUIConstants.ShowTitleScreenRework();
            yield break;
        }

        public void TriggerModLoadedEvent()
        {
            foreach (MonoBehaviour behaviour in base.GetComponents<MonoBehaviour>())
            {
                if (behaviour is IModLoadListener modLoadListener)
                    modLoadListener.OnModLoaded();
            }
        }

        public void TriggerModContentLoadedEvent(string errorString)
        {
            GlobalEventManager.Instance.Dispatch(Content.ContentManager.CONTENT_DOWNLOAD_DONE_EVENT, errorString);
        }

        public void TriggerGameLoadedEvent()
        {
            foreach (MonoBehaviour behaviour in base.GetComponents<MonoBehaviour>())
            {
                if (behaviour is IGameLoadListener gameLoadListener)
                    gameLoadListener.OnGameLoaded();
            }
        }

        public static T New<T>() where T : Singleton<T>
        {
            return Instance.gameObject.AddComponent<T>();
        }
    }
}
