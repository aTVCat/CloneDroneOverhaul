namespace CloneDroneOverhaul.V3.HUD
{
    public class UITitleScreenPatch : V3_ModHUDBase
    {
        private TitleScreenUI _titleScreenUI;

        private bool _hasInitialized;

        private void Start()
        {
            _titleScreenUI = GameUIRoot.Instance.TitleScreenUI;
            if (_titleScreenUI == null)
            {
                return;
            }

            _titleScreenUI.VersionLabel.gameObject.SetActive(false);

            _hasInitialized = true;
        }
    }
}
