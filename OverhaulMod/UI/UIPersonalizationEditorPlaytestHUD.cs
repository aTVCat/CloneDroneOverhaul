using OverhaulMod.Content.Personalization;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorPlaytestHUD : OverhaulUIBehaviour
    {
        public override void Hide()
        {
            base.Hide();
            PersonalizationEditorManager.Instance.ExitPlaytestMode();
        }
    }
}
