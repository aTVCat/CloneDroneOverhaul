using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIGameLossWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnRestartButtonClicked))]
        [UIElement("RestartButton")]
        private readonly Button m_restartButton;

        [UIElementAction(nameof(OnSoftRestartButtonClicked))]
        [UIElement("SoftRestartButton")]
        private readonly Button m_softRestartButton;

        [UIElementAction(nameof(OnMainMenuClicked))]
        [UIElement("MainMenuButton")]
        private readonly Button m_mainMenuButton;

        public override bool enableCursor => true;

        public void OnRestartButtonClicked()
        {
            Hide();
            if (GameModeManager.ShowLoadingScreen())
                GameUIRoot.Instance.LoadingScreen.Show();

            ModActionUtils.RunCoroutine(waitThenResetGame());
        }

        public void OnSoftRestartButtonClicked()
        {
            Hide();
            SoftGameRestartManager.Instance.RestartGame();
        }

        public void OnMainMenuClicked()
        {
            Hide();
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        private IEnumerator waitThenResetGame()
        {
            yield return null;
            yield return null;
            yield return null;
            GameFlowManager.Instance.ResetGameAndSpawnHuman(true);
            yield break;
        }
    }
}
