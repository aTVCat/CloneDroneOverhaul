using CDOverhaul.Patches;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulAchievementsMenu : UIController
    {
        public static bool IsEnabled() => OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsAchievementsMenuRedesignEnabled;

        [UIElementActionReference(nameof(OnBackButtonClicked))]
        [UIElementReferenceAttribute("BackButton")]
        private readonly Button m_BackButton;

        public override void Initialize()
        {
            base.Initialize();
            if (!IsEnabled())
                return;

            BaseFixes.ChangeButtonAction(TitleScreen.transform, "AchievementsButton", Show);
        }

        public override void Show()
        {
            base.Show();
            HideTitleScreenButtons();
        }

        public override void Hide()
        {
            base.Hide();
            ShowTitleScreenButtons();
        }

        public void OnBackButtonClicked()
        {
            Hide();
        }
    }
}
