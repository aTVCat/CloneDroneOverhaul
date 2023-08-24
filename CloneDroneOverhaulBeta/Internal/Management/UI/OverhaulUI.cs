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
                    return null;

                if (m_ModdedObject == null)
                    m_ModdedObject = base.GetComponent<ModdedObject>();

                return m_ModdedObject;
            }
        }

        private byte m_EnableCursorConditionID = 0;
        protected bool ShowCursor
        {
            get
            {
                return m_EnableCursorConditionID != 0;
            }
            set
            {
                if (value && !IsDisposedOrDestroyed())
                {
                    if (m_EnableCursorConditionID != 0)
                        return;

                    m_EnableCursorConditionID = EnableCursorController.DisableCursor();
                }
                else
                {
                    if (m_EnableCursorConditionID == 0)
                        return;

                    EnableCursorController.TryEnableCursor(m_EnableCursorConditionID);
                    m_EnableCursorConditionID = 0;
                }
            }
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            m_ModdedObject = null;
            ShowCursor = false;
        }

        public override void Initialize() { }

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
                    throw new System.ArgumentNullException();

                setUp(moddedObject.GetObject<Transform>(indexOfIndicator), moddedObject.GetObject<Image>(indexOfFillImage));
            }

            private void setUp(Transform indicator, Image fillImage)
            {
                if (indicator == null || fillImage == null)
                    throw new System.ArgumentNullException();

                CanvasGr = indicator.GetComponent<CanvasGroup>();
                Indicator = indicator;
                Indicator.gameObject.SetActive(UseCanvas());
                FillImage = fillImage;
                FillImage.fillAmount = 0f;
                m_Progress = 0f;
                m_Alpha = 0f;
            }

            public void UpdateIndicator(ProgressInformation progress, bool hideIf0or1)
            {
                if (progress == null || ((progress.Progress == 1f || progress.Progress == 0f) && hideIf0or1))
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

            public static void UpdateIndicator(LoadingIndicator indicator, ProgressInformation progress, bool hideIf0or1 = false)
            {
                if (indicator == null)
                    return;

                indicator.UpdateIndicator(progress, hideIf0or1);
            }

            public static void ResetIndicator(LoadingIndicator indicator)
            {
                if (indicator == null)
                    return;

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
                    throw new System.ArgumentNullException();

                setUp(moddedObject.GetObject(indexOfPrefab), moddedObject.GetObject<Transform>(indexOfContainer));
            }

            public PrefabAndContainer(Object prefab, Transform container)
            {
                setUp(prefab, container);
            }

            private void setUp(Object prefab, Transform container)
            {
                if (prefab == null || container == null)
                    throw new System.ArgumentNullException();

                if (!(prefab is GameObject))
                    throw new System.ArgumentException("UnityEngine.Object must be GameObject");

                if (!(prefab as GameObject).GetComponent<ModdedObject>())
                    throw new System.NullReferenceException("ModdedObject component not found");

                Prefab = (prefab as GameObject).GetComponent<ModdedObject>();
                Prefab.gameObject.SetActive(false);
                Container = container;
            }

            public ModdedObject CreateNew(bool startActive = true)
            {
                if (Prefab == null || Container == null)
                    return null;

                ModdedObject result = UnityEngine.Object.Instantiate(Prefab, Container);
                result.gameObject.SetActive(startActive);
                result.gameObject.name = result.gameObject.name.Replace("(Clone)", string.Empty);
                return result;
            }

            public void ClearContainer()
            {
                if (Container == null)
                    return;

                TransformUtils.DestroyAllChildren(Container);
            }
        }
    }
}