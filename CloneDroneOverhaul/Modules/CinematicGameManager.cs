namespace CloneDroneOverhaul.Modules
{
    public class CinematicGameManager : ModuleBase
    {
        public static bool IsUIHidden;
        public override bool ShouldWork()
        {
            return true;
        }
        public override void OnActivated()
        {
            IsUIHidden = false;
        }

        public static void SwitchHud()
        {
            IsUIHidden = !IsUIHidden;
            if (GameModeManager.IsBattleRoyale())
            {
                GameUIRoot.Instance.BattleRoyaleUI.gameObject.SetActive(!IsUIHidden);
            }

            bool shouldHide = CutSceneManager.Instance.IsInCutscene() || Utilities.PlayerUtilities.GetPlayerRobotInfo().IsNull || IsUIHidden;
            if (shouldHide)
            {
                GameUIRoot.Instance.SetPlayerHUDVisible(false);
                return;
            }
            GameUIRoot.Instance.SetPlayerHUDVisible(!IsUIHidden);
        }
    }
}
