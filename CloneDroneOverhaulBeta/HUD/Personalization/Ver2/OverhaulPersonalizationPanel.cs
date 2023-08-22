using CDOverhaul.Gameplay.Multiplayer;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    /// <summary>
    /// An updated version of <see cref="PersonalizationMenu"/> to be implemented in 0.3 updates/0.4
    /// </summary>
    public abstract class OverhaulPersonalizationPanel : OverhaulUIController
    {
        protected Vector2 TargetAnchorPosition;

        public bool IsPopulatingItems
        {
            get;
            protected set;
        }


        [ObjectReference("BG")]
        protected GameObject PanelBG;

        [ObjectReference("Button_Done")]
        protected Button DoneButton;

        [ObjectReference("ScrollRect")]
        protected CanvasGroup ScrollViewCanvasGroup;

        protected PrefabAndContainer MainItemContainer;

        public override void Initialize()
        {
            base.Initialize();

            OverhaulUIAnchoredPanelSlider slider = PanelBG.AddComponent<OverhaulUIAnchoredPanelSlider>();
            slider.StartPosition = new Vector3(-300f, 0f, 0f);
            slider.TargetPosition = (PanelBG.transform as RectTransform).anchoredPosition;

            DoneButton.onClick.AddListener(OnDoneButtonClicked);
            Hide();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnDoneButtonClicked();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            ShowCursor = true;

            PopulateItems();
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            ShowCursor = false;

            OverhaulPlayerInfo info = OverhaulPlayerInfo.LocalOverhaulPlayerInfo;
            if (info != null && info.HasReceivedData)
                info.RefreshData();
        }

        #region Item related

        protected abstract void PopulateItems();
        protected abstract IEnumerator PopulateItemsCoroutine();

        protected virtual IEnumerator PlayFadeAnimation(bool isOut)
        {
            if (!ScrollViewCanvasGroup)
                yield break;

            ScrollViewCanvasGroup.alpha = isOut ? 0f : 1f;
            for (int i = 0; i < 4; i++)
            {
                ScrollViewCanvasGroup.alpha += isOut ? 0.25f : -0.25f;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            yield break;
        }

        protected virtual void SearchItems(string text, bool onlyExclusive, bool byAuthor) { }

        #endregion

        #region Actions

        protected virtual void OnDoneButtonClicked()
        {
            Hide();
        }

        #endregion
    }
}
