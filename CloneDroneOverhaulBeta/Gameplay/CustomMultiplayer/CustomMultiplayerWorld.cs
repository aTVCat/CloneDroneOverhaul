using ModLibrary;
using UnityEngine;

namespace CDOverhaul.CustomMultiplayer
{
    public class CustomMultiplayerWorld
    {
        public void StartGameInUsualWorld()
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

        public void SpawnMainPlayer(bool sendPacket = true)
        {
            Camera camera = Camera.main;
            if (camera && camera.name == "Main Camera")
            {
                UnityEngine.Object.Destroy(camera.gameObject);
            }

            GameObject spawnPoint = new GameObject();
            spawnPoint.transform.position = new Vector3(0, 10f, 0);
            _ = GameFlowManager.Instance.SpawnPlayer(spawnPoint.transform, true, true, null);
            /*PlayerSync sync = firstPersonMover.gameObject.AddComponent<PlayerSync>();
            sync.IsOwner = true;
            sync.OwnerSteamID = SteamUser.GetSteamID();*/
        }
    }
}
