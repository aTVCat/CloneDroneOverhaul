using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class TransitionManager : Singleton<TransitionManager>, IGameLoadListener
    {
        public const float WAIT_BEFORE_FADING_TRANSITION = 0.3f;

        [ModSetting(ModSettingsConstants.OVERHAUL_SCENE_TRANSITIONS, true)]
        public static bool OverhaulSceneTransitions;

        [ModSetting(ModSettingsConstants.TRANSITION_ON_STARTUP, true)]
        public static bool TransitionOnStartup;

        [ModSetting(ModSettingsConstants.TRANSITION_SOUND, true)]
        public static bool TransitionSound;

        private TransitionBehaviour _transitionBehaviour;

        private Transform _crossSceneCanvas;

        private bool _hasGameReloaded;

        public override void Awake()
        {
            base.Awake();
            if (TransitionOnStartup)
            {
                DoTransition(new TransitionArgs(null, Color.white, false, true)
                {
                    DeltaTimeMultiplier = 3f
                });
            }
        }

        public void OnGameLoaded()
        {
            _hasGameReloaded = true;
        }

        public void DoSceneTransition()
        {
            DoTransition(new TransitionArgs(sceneTransitionCoroutine(), getBackgroundColor(), true, false, true));
        }

        public void DoInGameTransition(IEnumerator coroutine)
        {
            DoTransition(new TransitionArgs(coroutine, getBackgroundColor(), true, false));
        }

        public void DoTransition(TransitionArgs transitionArgs)
        {
            if (transitionArgs == null)
                throw new ArgumentNullException(nameof(transitionArgs));

            if (_transitionBehaviour)
                return;

            Transform parent = null;
            if (transitionArgs.CrossScene)
            {
                if (!_crossSceneCanvas)
                {
                    GameObject crossSceneCanvasObject = new GameObject("Cross Scene Canvas");
                    DontDestroyOnLoad(crossSceneCanvasObject);

                    Canvas canvas = crossSceneCanvasObject.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.sortingOrder = 100;
                    canvas.scaleFactor = ModCache.UIRootCanvas.scaleFactor;
                    CanvasScaler scaler = crossSceneCanvasObject.AddComponent<CanvasScaler>();
                    scaler.screenMatchMode = ModCache.UIRootCanvasScaler.screenMatchMode;
                    scaler.referenceResolution = ModCache.UIRootCanvasScaler.referenceResolution;
                    scaler.matchWidthOrHeight = ModCache.UIRootCanvasScaler.matchWidthOrHeight;

                    _crossSceneCanvas = crossSceneCanvasObject.transform;
                }

                parent = _crossSceneCanvas;
                parent.gameObject.SetActive(true);
            }
            else
            {
                parent = ModCache.UIRoot.transform;
            }

            GameObject gameObject = Instantiate(ModResources.Prefab(AssetBundleConstants.UI, "UI_Transition"), parent, false);
            RectTransform transform = gameObject.transform as RectTransform;
            transform.anchoredPosition = Vector2.zero;
            transform.localEulerAngles = Vector2.zero;
            transform.localScale = Vector2.one;
            if (!transitionArgs.CrossScene) transform.SetSiblingIndex(ModUIManager.Instance.GetSiblingIndex(ModUIManager.UILayer.BeforeCrashScreen));
            TransitionBehaviour transitionBehaviour = gameObject.AddComponent<TransitionBehaviour>();
            transitionBehaviour.FadeOut = transitionArgs.FadeOut;
            transitionBehaviour.DeltaTimeMultiplier = transitionArgs.DeltaTimeMultiplier;
            transitionBehaviour.WaitBeforeFadeOut = transitionArgs.WaitBeforeFadeOut;
            transitionBehaviour.IsCrossScene = transitionArgs.CrossScene;
            transitionBehaviour.SetColor(transitionArgs.BGColor);
            transitionBehaviour.SetLoadingIndicatorActive(transitionArgs.ShowIndicator);
            transitionBehaviour.RunCoroutine(transitionArgs.Coroutine);
            transitionBehaviour.StartFading();
            _transitionBehaviour = transitionBehaviour;
        }

        public void EndTransition()
        {
            if (_transitionBehaviour) _transitionBehaviour.FadeOut = true;
        }

        private void fadeOutMusic(float duration)
        {
            AudioManager audioManager = AudioManager.Instance;
            audioManager._musicFadeOutStartTime = Time.unscaledTime;
            audioManager._musicFadeOutDuration = duration;
            audioManager._musicFadeInStartTime = -1f;
        }

        public bool IsDoingTransition() => _transitionBehaviour;

        private Color getBackgroundColor() => ModParseUtils.TryParseColor("#050D1A", Color.black);

        private IEnumerator sceneTransitionCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            SceneTransitionManager sceneTransitionManager = SceneTransitionManager.Instance;
            sceneTransitionManager._isExitingToMainMenu = true;
            GlobalEventManager.Instance.Dispatch("ExitingToMainMenu");
            sceneTransitionManager._isDisconnecting = true;
            SceneTransitionManager.LastDisconnectTime = Time.realtimeSinceStartup;
            SceneTransitionManager.LastDisconnectHadBoltRunning = false;

            bool waitForPlaytestLevelToLoad = WorkshopLevelManager.Instance.HasAfterSceneLoadPlaytest();
            bool hasToReloadTheScene = true;
            if (BoltNetwork.IsConnected || BoltNetwork.IsRunning)
            {
                fadeOutMusic(1.5f);
                DelegateScheduler.Instance.Schedule(delegate
                {
                    if (AudioManager.Instance) AudioManager.Instance.StopMusic();
                }, 2f);

                SceneTransitionManager.LastDisconnectHadBoltRunning = true;

                bool wasDisconnecting = true;
                object obj = SceneTransitionManager.mutex;
                lock (obj)
                {
                    if (!sceneTransitionManager._isBoltDisconnectInProgress)
                    {
                        wasDisconnecting = false;
                        sceneTransitionManager._isBoltDisconnectInProgress = true;
                    }
                }

                if (wasDisconnecting) yield break;

                _hasGameReloaded = false;
                bool isWaitingForSceneToSwitch = false;

                Scene currentScene = SceneManager.GetActiveScene();
                try
                {
                    BoltLauncher.Shutdown();
                    isWaitingForSceneToSwitch = true;
                    hasToReloadTheScene = false;
                }
                catch (Exception)
                {
                    isWaitingForSceneToSwitch = false;
                }

                if (isWaitingForSceneToSwitch)
                {
                    float timeOut = Time.realtimeSinceStartup + 10f;
                    while (!_hasGameReloaded && Time.realtimeSinceStartup < timeOut) yield return null;
                }
            }

            if (sceneTransitionManager) sceneTransitionManager._isDisconnecting = false;
            if (hasToReloadTheScene)
            {
                SceneManager.LoadScene("Gameplay");
                yield return null;
                yield return null;
            }

            yield return new WaitForSecondsRealtime(WAIT_BEFORE_FADING_TRANSITION);

            if (waitForPlaytestLevelToLoad)
            {
                GameUIRoot uiRoot = ModCache.UIRoot;
                if (uiRoot && uiRoot.MultiplayerConnectingScreen && !uiRoot.MultiplayerConnectingScreen.isActiveAndEnabled)
                {
                    LevelManager levelManager = LevelManager.Instance;
                    if (levelManager)
                    {
                        if (_transitionBehaviour) _transitionBehaviour.ChangeLoadingText("loading_level");
                        while (levelManager.IsSpawningCurrentLevel()) yield return null;
                    }
                }
            }

            EndTransition();

            yield break;
        }

        public class TransitionArgs
        {
            public IEnumerator Coroutine;

            public Color BGColor;

            public bool ShowIndicator;

            public bool FadeOut;

            public bool CrossScene;

            public float DeltaTimeMultiplier = 15f;

            public float WaitBeforeFadeOut = 0.25f;

            public TransitionArgs() { }

            public TransitionArgs(IEnumerator coroutine, Color bgColor, bool showIndicator, bool fadeOut, bool crossScene = false)
            {
                Coroutine = coroutine;
                BGColor = bgColor;
                ShowIndicator = showIndicator;
                FadeOut = fadeOut;
                CrossScene = crossScene;
            }
        }
    }
}
