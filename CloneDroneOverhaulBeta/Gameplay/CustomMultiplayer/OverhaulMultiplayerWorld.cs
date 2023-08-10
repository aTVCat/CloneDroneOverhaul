using ModLibrary;
using UnityEngine;

namespace CDOverhaul.CustomMultiplayer
{
    public class OverhaulMultiplayerWorld : OverhaulBehaviour
    {
        public void InitializeWorld()
        {
            LevelManager.Instance.CleanUpLevelThisFrame();
            GameUIRoot.Instance.TitleScreenUI.Hide();
            EmperorArrivalManager.Instance.SetBattleCruiserVisible(true);
            GameUIRoot.Instance.SetPlayerHUDVisible(true);
            GameUIRoot.Instance.RefreshCursorEnabled();
            CacheManager.Instance.CreateOrClearInstance();

            ArenaCameraManager.Instance.ArenaCamera.gameObject.SetActive(false);
            GameFlowManager.Instance.SetPrivateField("_gameMode", GameMode.Story);
            LevelManager.Instance.SetPrivateField("_currentLevelHidesTheArena", false);
            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {

            });
        }
    }
}
