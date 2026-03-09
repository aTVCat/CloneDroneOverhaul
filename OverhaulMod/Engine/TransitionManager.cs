using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OverhaulMod.Engine
{
    public class TransitionManager : Singleton<TransitionManager>
    {
        [ModSetting(ModSettingsConstants.OVERHAUL_SCENE_TRANSITIONS, true)]
        public static bool OverhaulSceneTransitions;

        [ModSetting(ModSettingsConstants.OVERHAUL_NON_SCENE_TRANSITIONS, true)]
        public static bool OverhaulNonSceneTransitions;

        [ModSetting(ModSettingsConstants.TRANSITION_ON_STARTUP, true)]
        public static bool TransitionOnStartup;

        [ModSetting(ModSettingsConstants.TRANSITION_SOUND, true)]
        public static bool TransitionSound;

        private TransitionBehaviour _transitionBehaviour;

        public override void Awake()
        {
            base.Awake();
            if (TransitionOnStartup)
                DoTransition(TransitionArgs.StartupTransition());
        }

        public static Color GetBlackScreenColor()
        {
            return ModParseUtils.TryParseColor("#050D1A", Color.black);
        }

        public bool IsDoingTransition()
        {
            return _transitionBehaviour;
        }

        public void DoNonSceneTransition(IEnumerator coroutine)
        {
            DoTransition(TransitionArgs.NonSceneTransition(coroutine));
        }

        public void DoTransition(TransitionArgs transitionArgs)
        {
            if (transitionArgs == null)
                throw new ArgumentNullException(nameof(transitionArgs));

            if (_transitionBehaviour)
                return;

            GameObject gameObject = Instantiate(ModResources.Prefab(AssetBundleConstants.UI, "UI_Transition"), ModCache.gameUIRoot.transform, false);
            RectTransform transform = gameObject.transform as RectTransform;
            transform.anchoredPosition = Vector2.zero;
            transform.localEulerAngles = Vector2.zero;
            transform.localScale = Vector2.one;
            transform.SetSiblingIndex(ModUIManager.Instance.GetSiblingIndex(ModUIManager.UILayer.BeforeCrashScreen));
            TransitionBehaviour transitionBehaviour = gameObject.AddComponent<TransitionBehaviour>();
            transitionBehaviour.FadeOut = transitionArgs.FadeOut;
            transitionBehaviour.DeltaTimeMultiplier = transitionArgs.DeltaTimeMultiplier;
            transitionBehaviour.WaitBeforeFadeOut = transitionArgs.WaitBeforeFadeOut;
            transitionBehaviour.SetColor(transitionArgs.BGColor);
            transitionBehaviour.SetLoadingIndicatorActive(transitionArgs.ShowIndicator);
            transitionBehaviour.RunCoroutine(transitionArgs.Coroutine);
            transitionBehaviour.StartFading();
            _transitionBehaviour = transitionBehaviour;
        }

        public void EndTransition()
        {
            if (_transitionBehaviour)
                _transitionBehaviour.FadeOut = true;
        }

        public static IEnumerator SceneTransitionCoroutine(SceneTransitionManager sceneTransitionManager)
        {
            yield return new WaitForSecondsRealtime(0.4f);
            sceneTransitionManager._isExitingToMainMenu = true;
            GlobalEventManager.Instance.Dispatch("ExitingToMainMenu");
            sceneTransitionManager._isDisconnecting = true;
            SceneTransitionManager.LastDisconnectTime = Time.realtimeSinceStartup;
            SceneTransitionManager.LastDisconnectHadBoltRunning = false;
            if (BoltNetwork.IsConnected || BoltNetwork.IsRunning)
            {
                sceneTransitionManager._isDisconnecting = true;
                SceneTransitionManager.LastDisconnectHadBoltRunning = true;
                bool flag = false;
                object obj = SceneTransitionManager.mutex;
                lock (obj)
                {
                    if (!sceneTransitionManager._isBoltDisconnectInProgress)
                    {
                        flag = true;
                        sceneTransitionManager._isBoltDisconnectInProgress = true;
                    }
                }

                if (!flag)
                    yield break;
                try
                {
                    BoltLauncher.Shutdown();
                    yield break;
                }
                catch (Exception)
                {
                    sceneTransitionManager._isDisconnecting = false;
                    SceneManager.LoadScene("Gameplay");
                    yield break;
                }
            }
            sceneTransitionManager._isDisconnecting = false;
            SceneManager.LoadScene("Gameplay");
            yield break;
        }

        public class TransitionArgs
        {
            public IEnumerator Coroutine;

            public Color BGColor;

            public bool ShowIndicator;

            public bool FadeOut;

            public float DeltaTimeMultiplier = 15f;

            public float WaitBeforeFadeOut = 0.25f;

            public TransitionArgs()
            {

            }

            public TransitionArgs(IEnumerator coroutine, Color bgColor, bool showIndicator, bool fadeOut)
            {
                Coroutine = coroutine;
                BGColor = bgColor;
                ShowIndicator = showIndicator;
                FadeOut = fadeOut;
            }

            public static TransitionArgs StartupTransition()
            {
                return new TransitionArgs(null, Color.white, false, true)
                {
                    DeltaTimeMultiplier = 3f
                };
            }

            public static TransitionArgs NonSceneTransition(IEnumerator coroutine)
            {
                return new TransitionArgs(coroutine, GetBlackScreenColor(), true, false);
            }

            public static TransitionArgs SceneTransition(IEnumerator coroutine)
            {
                return new TransitionArgs(coroutine, GetBlackScreenColor(), true, false);
            }
        }
    }
}
