namespace CDOverhaul.Misc
{
    public class OverhaulPhotoModeControls : OverhaulUI
    {
        public override void Initialize()
        {
            Hide();
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
        }
    }
}