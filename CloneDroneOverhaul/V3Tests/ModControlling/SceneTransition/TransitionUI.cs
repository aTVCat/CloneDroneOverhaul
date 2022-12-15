using Bolt;
using ModLibrary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3Tests.Base
{
    public class TransitionUI : BoltGlobalEventListenerSingleton<TransitionUI>
    {
        public static bool IsLoadingScene;
        public static bool LastIngoreCrashesState;

        private object mutex = new object();
        public Animator MyAnimator;

        public TextMeshProUGUI Title;
        public TextMeshProUGUI Details;
        public TextMeshProUGUI StatusText;
        public Transform LoadingSpinner;
        public Image BG;

        private bool _hasProgress;
        private int _progressTextIndex;

        public List<string> StatusTexts = new List<string>();
        public TransitionAction Action;

        private bool _isDone;
        public bool IsDone
        {
            get { return _isDone; }
            set
            {
                _isDone = value;
                if (value && Action.HideOnComplete)
                {
                    SceneTransitionController.CurrentTransition = null;
                    Hide();
                }
            }
        }

        private bool _isWaitingToDestroy;
        private float _timeLeftToDestroy;

        public static TransitionUI Spawn(string title, string details, TransitionAction action, bool hasProgress = false)
        {
            ModdedObject mObj = Instantiate(OverhaulCacheManager.GetCached<GameObject>("UI_TransitionScreen")).GetComponent<ModdedObject>();
            return mObj.gameObject.AddComponent<TransitionUI>().Initialize(mObj, title, details, action, hasProgress);
        }

        /// <summary>
        /// Set up values
        /// </summary>
        /// <param name="mObj"></param>
        /// <param name="title"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public TransitionUI Initialize(ModdedObject mObj, string title, string details, TransitionAction action, bool showProgress = false)
        {
            LastIngoreCrashesState = InternalModBot.IgnoreCrashesManager.GetIsIgnoringCrashes();
            InternalModBot.IgnoreCrashesManager.SetIsIgnoringCrashes(true);
            DontDestroyOnLoad(this.gameObject);

            MyAnimator = base.GetComponent<Animator>();
            Title = mObj.GetObjectFromList<TextMeshProUGUI>(0);
            Details = mObj.GetObjectFromList<TextMeshProUGUI>(1);
            StatusText = mObj.GetObjectFromList<TextMeshProUGUI>(2);
            LoadingSpinner = mObj.GetObjectFromList<Transform>(3);
            BG = mObj.GetObjectFromList<Image>(4);

            Title.text = title;
            Details.text = details;

            _hasProgress = showProgress;
            if (showProgress)
            {
                _progressTextIndex = AddStatusString("0% Done");
            }

            Action = action;

            Show();

            return this;
        }

        /// <summary>
        /// Show transition UI
        /// </summary>
        public void Show()
        {
            if (this is null)
            {
                return;
            }

            MyAnimator.Play("Show");

            OverhaulMain.Timer.AddNoArgAction(delegate
            {

                if (Action.Type == TranstionType.Method)
                {
                    Action.Action();
                    OverhaulMain.Timer.AddNoArgAction(delegate
                    {
                        IsDone = true;
                    }, 0.3f, true);
                }
                else if (Action.Type == TranstionType.SceneSwitch)
                {
                    StartCoroutine(switchScene(Action.SceneName));
                }

            }, 0.3f, true);
        }

        IEnumerator switchScene(string id, bool onlyLoadScene = false)
        {
            if (!onlyLoadScene)
            {
                AddStatusString("Waiting for Bolt...");
                if (!SceneTransitionManager.Instance.GetPrivateField<bool>("_isDisconnecting"))
                {
                    SceneTransitionManager.Instance.SetPrivateField<bool>("_isExitingToMainMenu", true);
                    Singleton<GlobalEventManager>.Instance.Dispatch("ExitingToMainMenu");
                    SceneTransitionManager.Instance.SetPrivateField<bool>("_isDisconnecting", true);
                    SceneTransitionManager.LastDisconnectTime = Time.realtimeSinceStartup;
                    SceneTransitionManager.LastDisconnectHadBoltRunning = false;

                    if (BoltNetwork.IsConnected || BoltNetwork.IsRunning)
                    {
                        SceneTransitionManager.LastDisconnectHadBoltRunning = true;
                        bool flag = false;
                        object obj = mutex;
                        lock (obj)
                        {
                            if (!SceneTransitionManager.Instance.GetPrivateField<bool>("_isBoltDisconnectInProgress"))
                            {
                                flag = true;
                                SceneTransitionManager.Instance.SetPrivateField<bool>("_isBoltDisconnectInProgress", true);
                            }
                        }
                        if (flag)
                        {
                            try
                            {
                                BoltLauncher.Shutdown();
                                yield break;
                            }
                            catch (System.Exception arg)
                            {
                                SceneTransitionManager.Instance.SetPrivateField<bool>("_isDisconnecting", false);
                                AddStatusString("BoltLauncher.Shutdown failed!");
                                goto IL_0000;
                            }
                        }
                    }
                    SceneTransitionManager.Instance.SetPrivateField<bool>("_isDisconnecting", false);
                }
            }

            if (IsLoadingScene)
            {
                yield break;
            }

        IL_0000:
            if (!string.IsNullOrEmpty(id))
            {
                AddStatusString("Start loading scene...");

                IsLoadingScene = true;
                if (Action.UseAsyncSceneLoading)
                {
                    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(id);

                    while (!asyncLoad.isDone)
                    {
                        if (_hasProgress)
                        {
                            SetProgress(Mathf.RoundToInt(asyncLoad.progress * 100));
                        }
                        yield return null;
                    }
                }
                else
                {
                    SceneManager.LoadScene(id);
                }
                IsLoadingScene = false;
                AddStatusString("Scene loaded!");
            }
            else
            {
                AddStatusString("Waiting for Bolt...");
                yield break;
            }

            IsLoadingScene = false;
            yield return new WaitForSecondsRealtime(1f);

            IsDone = true;

            yield break;
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            object obj = mutex;
            lock (obj)
            {
                SceneTransitionManager.Instance.SetPrivateField<bool>("_isBoltDisconnectInProgress", true);
            }
            registerDoneCallback(new System.Action(this.onShutDownComplete));
        }

        private void onShutDownComplete()
        {
            SceneTransitionManager.Instance.SetPrivateField<bool>("_isBoltDisconnectInProgress", false);
            if (SceneTransitionManager.Instance.GetPrivateField<bool>("_isDisconnecting"))
            {
                SceneTransitionManager.Instance.SetPrivateField<bool>("_isDisconnecting", false);
                if (IsLoadingScene) return;
                AddStatusString("BoltLauncher.Shutdown done!");
                base.StartCoroutine(switchScene(Action.SceneName, true));
            }
        }

        /// <summary>
        /// Hide and destroy transition UI
        /// </summary>
        public void Hide()
        {
            if (this is null)
            {
                return;
            }

            if (!IsDone)
            {
                return;
            }

            _isWaitingToDestroy = true;
            _timeLeftToDestroy = 1f;
            MyAnimator.Play("Hide");
            InternalModBot.IgnoreCrashesManager.SetIsIgnoringCrashes(LastIngoreCrashesState);
        }

        void Update()
        {
            LoadingSpinner.eulerAngles += new Vector3(0, 0, 180 * Time.deltaTime);
            if (_isWaitingToDestroy)
            {
                _timeLeftToDestroy -= Time.deltaTime;
                BG.color = new Color(0.4f, 0.4f, 0.5f, _timeLeftToDestroy);

                if (_timeLeftToDestroy < 0)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        public int AddStatusString(string str)
        {
            StatusTexts.Add(str);
            RefreshStatus();
            return StatusTexts.Count - 1;
        }

        public void RefreshStatus()
        {
            StatusText.text = string.Empty;
            foreach (string str in StatusTexts)
            {
                StatusText.text += str + "\n";
            }
        }

        public void SetProgress(float val)
        {
            if (_hasProgress)
            {
                StatusTexts[_progressTextIndex] = val + "% Done";
                RefreshStatus();
            }
        }
    }
}