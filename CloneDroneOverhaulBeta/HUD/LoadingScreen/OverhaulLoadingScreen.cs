using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulLoadingScreen : OverhaulUI
    {
        public static OverhaulLoadingScreen Instance;

        private Text m_Header;
        private Slider m_Fill;

        public override void Initialize()
        {
            m_Header = MyModdedObject.GetObject<Text>(0);
            m_Header.text = string.Empty;
            m_Fill = MyModdedObject.GetObject<Slider>(1);
            m_Fill.value = 0f;
            Instance = this;
            SetScreenActive(false);
        }

        public void SetScreenActive(bool value)
        {
            if (IsDisposedOrDestroyed())
                return;

            GameObject gm = base.gameObject;
            if (gm.activeSelf == value)
                return;

            ShowCursor = value;
            gm.SetActive(value);
        }

        public void SetScreenText(string text)
        {
            if (IsDisposedOrDestroyed())
                return;

            m_Header.text = text;
        }

        public void SetScreenFill(float amount)
        {
            if (IsDisposedOrDestroyed())
                return;

            m_Fill.value = amount;
        }
    }
}