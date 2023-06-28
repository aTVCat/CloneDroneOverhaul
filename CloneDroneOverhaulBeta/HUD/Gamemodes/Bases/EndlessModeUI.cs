using UnityEngine;

namespace CDOverhaul.HUD.Gamemodes
{
    public class EndlessModeUI : OverhaulGamemodeUIBase
    {
        protected override void OnInitialize()
        {
            _ = base.GetComponent<ModdedObject>();
        }

        protected override void OnShow()
        {
            GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/chapter" + "5" + "Preview.jpeg");
        }

        private void goBackToGamemodeSelection()
        {
            Hide();
            GameUIRoot.Instance.TitleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(true);
        }

        public override void Update()
        {
            base.Update();

            if (GamemodesUI.FullscreenWindow.IsActive)
                return;

            if (Input.GetKeyDown(KeyCode.Backspace))
                goBackToGamemodeSelection();
        }
    }
}
