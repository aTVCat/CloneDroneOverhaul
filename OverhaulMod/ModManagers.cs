using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod
{
    public class ModManagers : Singleton<ModManagers>
    {
        [ModSetting(ModSettingsConstants.SHOW_MOD_SETUP_SCREEN_ON_START, true)]
        public static bool ShowModSetupScreenOnStart;

        private List<IGameLoadListener> _gameLoadListeners;

        private List<IModLoadListener> _modLoadListeners;

        public override void Awake()
        {
            base.Awake();

            _gameLoadListeners = new List<IGameLoadListener>();
            _modLoadListeners = new List<IModLoadListener>();

            _ = base.gameObject.AddComponent<UIDeveloperMenu>();
        }

        public void TriggerModLoadedEvent()
        {
            ModDebug.Log($"Triggering OnModLoaded event");
            for (int i = 0; i < _modLoadListeners.Count; i++)
            {
                _modLoadListeners[i].OnModLoaded();
            }
        }

        public void TriggerGameLoadedEvent()
        {
            ModDebug.Log($"Triggering OnGameLoaded event");
            for (int i = 0; i < _gameLoadListeners.Count; i++)
            {
                try
                {
                    _gameLoadListeners[i].OnGameLoaded();
                }
                catch (System.Exception exc)
                {
                    Debug.LogException(exc);
                }
            }
        }

        public void TriggerModContentLoadedEvent(string errorString)
        {
            GlobalEventManager.Instance.Dispatch(Content.AddonManager.ADDON_DOWNLOADED_EVENT, errorString);
        }

        public void AddSingleton<T>(GameObject gameObject) where T : Singleton<T>
        {
            GameObject newGameObject = new GameObject(typeof(T).Name);
            newGameObject.transform.SetParent(gameObject.transform, false);
            T singletonInstance = newGameObject.AddComponent<T>();

            if (singletonInstance is IGameLoadListener gameLoadListener) _gameLoadListeners.Add(gameLoadListener);
            if (singletonInstance is IModLoadListener modLoadListener) _modLoadListeners.Add(modLoadListener);
        }
    }
}