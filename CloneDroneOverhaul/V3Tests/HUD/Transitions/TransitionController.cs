namespace CloneDroneOverhaul.V3Tests.HUD
{
    /// <summary>
    /// Make transitions better!
    /// </summary>
    public static class SceneTransitionController
    {
        public static TransitionUI CurrentTransition;

        /// <summary>
        /// Spawn a transition screen with specified titl texts
        /// </summary>
        public static TransitionUI StartTranstion(TransitionAction transitionAction, string title, string details, bool showProgress)
        {
            CurrentTransition = TransitionUI.Spawn(title, details, transitionAction, showProgress);
            return CurrentTransition;
        }

        public static void GoToMainMenu()
        {
            bool willLoadLBS = MultiplayerMatchmakingManager.ShouldStartBattleRoyaleGameOnLoad;

            string details = string.Empty;
            string header = OverhaulMain.GetTranslatedString("TransitionScreen_GeneralHeader");
            if (willLoadLBS)
            {
                details = OverhaulMain.GetTranslatedString("TransitionScreen_NextMatchHeader");
                header = OverhaulMain.GetTranslatedString("TransitionScreen_NextMatchDescription");
            }

            TransitionAction action = new TransitionAction
            {
                Type = ETranstionType.SceneSwitch,
                SceneName = "Gameplay",
                UseAsyncSceneLoading = true,
                HideOnComplete = true
            };
            StartTranstion(action, header, details, true);
        }

        public static void EndTripToMainMenu()
        {
            if (CurrentTransition != null)
            {
                CurrentTransition.IsDone = true;
            }
        }
    }
}