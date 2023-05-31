using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulUIDescriptionTooltip : OverhaulUI
    {
        private static OverhaulUIDescriptionTooltip m_Tooltip;
        public static bool IsNull => m_Tooltip == null || m_Tooltip.IsDisposedOrDestroyed();

        public Text TitleText;
        public Text DescriptionText;

        public override void Initialize()
        {
            m_Tooltip = this;
            TitleText = MyModdedObject.GetObject<Text>(1);
            DescriptionText = MyModdedObject.GetObject<Text>(2);
            SetActive(false);
        }

        public static void SetActive(bool value)
        {
            if (IsNull)
                return;

            m_Tooltip.gameObject.SetActive(value);
        }

        public static void SetActive(bool value, string title, string description)
        {
            SetActive(value);
            SetTitleAndDescription(title, description);
        }

        public static void SetTitleAndDescription(string title, string description)
        {
            if (IsNull)
                return;

            m_Tooltip.TitleText.text = title;
            m_Tooltip.DescriptionText.text = description;
        }
    }
}