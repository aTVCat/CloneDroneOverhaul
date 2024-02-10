using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPauseMenuRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("LegacyUIButton")]
        private readonly Button m_legacyUIButton;

        public override bool enableCursor
        {
            get
            {
                return true;
            }
        }

        public static bool disableOverhauledVersion { get; set; }

        public override void Show()
        {
            base.Show();
            TimeManager.Instance.OnGamePaused();
        }

        public override void Hide()
        {
            base.Hide();
            TimeManager.Instance.OnGameUnPaused();
        }

        /*
        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }*/

        public void OnLegacyUIButtonClicked()
        {
            Hide();
            ModUIUtils.ShowVanillaEscMenu();
        }
    }
}
