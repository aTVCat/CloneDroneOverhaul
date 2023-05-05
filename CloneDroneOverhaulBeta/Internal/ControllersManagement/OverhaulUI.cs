using Bolt;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of any HUD in the mod
    /// </summary>
    [RequireComponent(typeof(ModdedObject), typeof(RectTransform))]
    public class OverhaulUI : OverhaulController
    {
        private ModdedObject m_ModdedObject;
        /// <summary>
        /// The instance of <see cref="ModdedObject"/>
        /// <b>Note: gameobject with this script must have <see cref="ModdedObject"/></b>
        /// </summary>
        public ModdedObject MyModdedObject
        {
            get
            {
                if (IsDisposedOrDestroyed())
                {
                    return null;
                }
                if (m_ModdedObject == null)
                {
                    m_ModdedObject = base.GetComponent<ModdedObject>();
                }
                return m_ModdedObject;
            }
        }

        private byte _enableCursorConditionID = 0;
        /// <summary>
        /// Make the game to force show system cursor
        /// </summary>
        protected bool ShowCursor
        {
            set
            {
                if (value && !IsDisposedOrDestroyed())
                {
                    if (_enableCursorConditionID != 0)
                    {
                        return;
                    }
                    _enableCursorConditionID = EnableCursorController.AddCondition();
                }
                else
                {
                    if (_enableCursorConditionID != 0)
                    {
                        EnableCursorController.RemoveCondition(_enableCursorConditionID);
                        _enableCursorConditionID = 0;
                    }
                }
            }
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }

        public override void Initialize()
        {
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            m_ModdedObject = null;
            ShowCursor = false;
        }

        public class LoadingIndicator
        {
            public CanvasGroup CanvasGr;
            public Transform Indicator;
            public Image FillImage;

            public bool UseCanvas() => CanvasGr != null;

            private float m_Alpha;
            private float m_Progress;

            public LoadingIndicator(ModdedObject moddedObject, int indexOfIndicator, int indexOfFillImage)
            {
                if (moddedObject == null)
                {
                    throw new System.ArgumentNullException();
                }

                setUp(moddedObject.GetObject<Transform>(indexOfIndicator), moddedObject.GetObject<Image>(indexOfFillImage));
            }

            private void setUp(Transform indicator, Image fillImage)
            {
                if (indicator == null || fillImage == null)
                {
                    throw new System.ArgumentNullException();
                }

                CanvasGr = indicator.GetComponent<CanvasGroup>();
                Indicator = indicator;
                Indicator.gameObject.SetActive(UseCanvas());
                FillImage = fillImage;
                FillImage.fillAmount = 0f;
                m_Progress = 0f;
                m_Alpha = 0f;
            }

            public void UpdateIndicator(OverhaulRequestProgressInfo progress)
            {
                if(progress == null)
                {
                    if (UseCanvas())
                    {
                        m_Alpha += (0f - m_Alpha) * Time.unscaledDeltaTime * 10f;
                        CanvasGr.alpha = m_Alpha;
                    }
                    else
                    {
                        Indicator.gameObject.SetActive(false);
                    }
                    return;
                }

                if (UseCanvas())
                {
                    m_Alpha += (1f - m_Alpha) * Time.unscaledDeltaTime * 10f;
                    CanvasGr.alpha = m_Alpha;
                }
                else
                {
                    Indicator.gameObject.SetActive(true);
                }
                m_Progress += (progress.Progress - m_Progress) * Time.unscaledDeltaTime * 10f;
                FillImage.fillAmount = m_Progress;
            }

            public static void UpdateIndicator(LoadingIndicator indicator, OverhaulRequestProgressInfo progress)
            {
                if(indicator == null)
                {
                    return;
                }

                indicator.UpdateIndicator(progress);
            }

            public static void ResetIndicator(LoadingIndicator indicator)
            {
                if (indicator == null)
                {
                    return;
                }

                indicator.m_Progress = 0f;
                indicator.FillImage.fillAmount = 0f;
            }
        }

        public class PrefabAndContainer
        {
            public ModdedObject Prefab;
            public Transform Container;

            public PrefabAndContainer(ModdedObject moddedObject, int indexOfPrefab, int indexOfContainer)
            {
                if (moddedObject == null)
                {
                    throw new System.ArgumentNullException();
                }

                setUp(moddedObject.GetObject(indexOfPrefab), moddedObject.GetObject<Transform>(indexOfContainer));
            }

            public PrefabAndContainer(Object prefab, Transform container)
            {
                setUp(prefab, container);
            }

            private void setUp(Object prefab, Transform container)
            {
                if (prefab == null || container == null)
                {
                    throw new System.ArgumentNullException();
                }

                if (!(prefab is GameObject))
                {
                    throw new System.ArgumentException("UnityEngine.Object must be GameObject");
                }

                if (!(prefab as GameObject).GetComponent<ModdedObject>())
                {
                    throw new System.NullReferenceException("ModdedObject component not found");
                }

                Prefab = (prefab as GameObject).GetComponent<ModdedObject>();
                Prefab.gameObject.SetActive(false);
                Container = container;
            }

            public ModdedObject CreateNew(bool startActive = true)
            {
                if(Prefab == null || Container == null)
                {
                    return null;
                }

                ModdedObject result = UnityEngine.Object.Instantiate(Prefab, Container);
                result.gameObject.SetActive(startActive);
                return result;
            }

            public void ClearContainer()
            {
                if (Container == null)
                {
                    return;
                }

                TransformUtils.DestroyAllChildren(Container);
            }
        }
    }
}