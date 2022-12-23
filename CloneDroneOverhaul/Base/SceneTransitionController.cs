using System;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.RemovedOrOld
{
    public static class SceneTransitionController
    {
        public class SceneTransitionUI : MonoBehaviour
        {
            private ModdedObject ModdedObj;
            private Text _header;
            private Text _details;
            private Image _bg;
            private float _unscaledTimeToStopUpdatingColor;
            private bool _isAnimatingBG;
            private bool _isShowingBG;
            private bool _manualControl;
            private readonly Color InitialColor = new Color(1, 1, 1, 0);
            private readonly Color EndColor = new Color(1, 1, 1, 1);
            private Action _currentAction;

            private void Awake()
            {
                ModdedObj = base.GetComponent<ModdedObject>();
            }

            private void Update()
            {
                if (Time.unscaledTime < _unscaledTimeToStopUpdatingColor)
                {
                    if (_isAnimatingBG && _isShowingBG)
                    {
                        BaseUtils.SmoothChangeImageColor(_bg, InitialColor, EndColor, 1f);
                    }
                    else if (_isAnimatingBG && !_isShowingBG)
                    {
                        BaseUtils.SmoothChangeImageColor(_bg, EndColor, InitialColor, 1f);
                    }
                }
                else
                {
                    if (!_manualControl)
                    {
                        if (_isAnimatingBG && _isShowingBG && _currentAction != null)
                        {
                            _currentAction();
                            _currentAction = null;
                            _isShowingBG = false;
                            _isAnimatingBG = true;
                            _unscaledTimeToStopUpdatingColor = Time.unscaledTime + 1f;
                            return;
                        }
                        else if (_isAnimatingBG && !_isShowingBG)
                        {
                            HideAndDestroy();
                        }
                    }
                    else
                    {
                        if (_currentAction != null)
                        {
                            _currentAction();
                            _currentAction = null;
                        }
                    }
                    _isAnimatingBG = false;
                }
            }

            internal void ShowAndDo(Action action, string header, string details, bool manualControl = false)
            {
                _header = ModdedObj.GetObjectFromList<Text>(0);
                _details = ModdedObj.GetObjectFromList<Text>(1);
                _bg = ModdedObj.GetObjectFromList<Image>(2);

                _manualControl = manualControl;

                _bg.color = InitialColor;
                _isAnimatingBG = true;
                _isShowingBG = true;
                _unscaledTimeToStopUpdatingColor = Time.unscaledTime + 1f;

                _header.text = header;
                _details.text = details;

                _currentAction = action;

                base.gameObject.SetActive(true);
            }

            public void HideAndDestroy()
            {
                Destroy(base.gameObject);
            }
        }


        public static SceneTransitionUI SpawnTransitionScreen(Action action, string header, string details, bool manualControl = false)
        {
            SceneTransitionUI ui = UnityEngine.Object.Instantiate(OverhaulCacheManager.GetCached<GameObject>("SceneTransition_LevelEditorStyle")).AddComponent<SceneTransitionUI>();
            UnityEngine.Object.DontDestroyOnLoad(ui.gameObject);
            ui.ShowAndDo(action, header, details, manualControl);
            return ui;
        }
    }
}
