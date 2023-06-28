using CDOverhaul.NetworkAssets;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulUIImageViewer : OverhaulUI
    {
        private static OverhaulUIImageViewer s_Viewer;
        public static bool IsNull => s_Viewer == null || s_Viewer.IsDisposedOrDestroyed();
        public static bool IsActive => !IsNull && s_Viewer.gameObject.activeInHierarchy;

        public RawImage Image;
        private float m_TimeToReceiveInput;

        public override void Initialize()
        {
            s_Viewer = this;
            Image = MyModdedObject.GetObject<RawImage>(0);
            SetActive(false, null);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            s_Viewer = null;
        }

        public static void SetActive(bool value, Texture texture)
        {
            if (IsNull)
                return;

            if (GameModeManager.Is(GameMode.None))
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);

            s_Viewer.gameObject.SetActive(false);
            if (!value || !texture)
                return;

            s_Viewer.gameObject.SetActive(value);
            s_Viewer.Image.texture = texture;
            s_Viewer.m_TimeToReceiveInput = Time.unscaledTime + 0.1f;

            if (GameModeManager.Is(GameMode.None))
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);

            float num = (float)texture.width / (float)texture.height;
            s_Viewer.Image.rectTransform.sizeDelta = new Vector2(s_Viewer.Image.rectTransform.rect.height * num, s_Viewer.Image.rectTransform.sizeDelta.y);
        }

        private void Update()
        {
            if (Time.unscaledTime >= m_TimeToReceiveInput && Input.anyKeyDown)
                SetActive(false, null);
        }
    }
}