namespace CloneDroneOverhaul.V3Tests.Base
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
            string header = "Loading...";
            if (willLoadLBS)
            {
                details = "Unloading last match...";
                header = "Going to next match...";
            }

            TransitionAction action = new TransitionAction();/*
            if (BoltNetwork.IsRunning || BoltNetwork.IsConnected)
            {
                action.Type = TranstionType.SceneSwitch;
                action.SceneName = string.Empty;
                action.HideOnComplete = true;
            }
            else
            {
                action.Type = TranstionType.SceneSwitch;
                action.SceneName = "Gameplay";
                action.UseAsyncSceneLoading = true;
                action.HideOnComplete = true;
            }*/
            action.Type = TranstionType.SceneSwitch;
            action.SceneName = "Gameplay";
            action.UseAsyncSceneLoading = true;
            action.HideOnComplete = true;
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