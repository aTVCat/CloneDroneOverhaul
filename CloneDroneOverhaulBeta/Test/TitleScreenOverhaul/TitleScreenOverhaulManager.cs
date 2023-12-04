using CDOverhaul.HUD;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class TitleScreenOverhaulManager : OverhaulManager<TitleScreenOverhaulManager>
    {
        private UITitleScreenRework m_TitleScreen;

        public TitleScreenCustomizationSystem customization
        {
            get;
            private set;
        }

        public bool updateCamera
        {
            get;
            set;
        } = true;

        public override void Initialize()
        {
            base.Initialize();
            if (!customization)
                customization = base.gameObject.AddComponent<TitleScreenCustomizationSystem>();
        }

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            if (customization)
                customization.Dispose(true);
        }

        public void DoTitleScreenOverhaul()
        {
            UIConstants.ShowNewTitleScreen();
            if (customization)
                customization.SpawnLevel(out _);

            m_TitleScreen = OverhaulUIManager.reference.GetUI<UITitleScreenRework>(UIConstants.UI_NEW_TITLE_SCREEN);
        }

        private void LateUpdate()
        {
            if (!GameModeManager.IsOnTitleScreen() || !updateCamera || !m_TitleScreen)
                return;

            Camera logoCamera = ArenaCameraManager.Instance.TitleScreenLogoCamera;
            if (!logoCamera)
                return;

            RectTransform rootButtonsContainer = m_TitleScreen.GetButtonsContainerTransform();
            float num = rootButtonsContainer.anchoredPosition.x + rootButtonsContainer.rect.width / 2f;
            float width = TitleScreenCustomizationSystem.UIAlignment == 1 ? 1f : 2f * num / UIManager.Instance.UIRoot.rect.width;
            logoCamera.rect = new Rect(0f, logoCamera.rect.y, width, 1f);
        }
    }
}
