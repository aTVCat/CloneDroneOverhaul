using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Workshop
{
    public class OverhaulWorkshopBrowserUI : OverhaulUI
    {
        [OverhaulSetting("Game interface.Network.New Workshop browser design", true)]
        public static bool UseThisUI;

        public static bool BrowserIsNull => BrowserUIInstance == null;
        public static OverhaulWorkshopBrowserUI BrowserUIInstance;

        #region UI elements

        private Button m_ExitButton;

        #endregion

        public override void Initialize()
        {
            BrowserUIInstance = this;

            m_ExitButton = MyModdedObject.GetObject<Button>(0);
            m_ExitButton.onClick.AddListener(Hide);

            Hide(true);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            BrowserUIInstance = null;
        }

        public bool TryShow()
        {
            if (!UseThisUI)
            {
                return false;
            }

            Show();
            return true;
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
        }

        public void Hide(bool hiddenBecauseLoading = false)
        {
            base.gameObject.SetActive(false);
            if(!hiddenBecauseLoading) GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
        }

        public void Hide()
        {
            Hide(false);
        }
    }
}
